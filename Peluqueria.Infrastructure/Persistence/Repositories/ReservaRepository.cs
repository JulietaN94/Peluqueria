using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Peluqueria.Application.DTOs.Turnos;
using Peluqueria.Application.Interfaces;
using Peluqueria.Domain.Entities;
using Peluqueria.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Peluqueria.Infrastructure.Persistence.Repositories
{
    public sealed class ReservaRepository : IReservaRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;
        private readonly IReservaCodeGenerator _generator;
        private readonly TimeProvider _clock;

        public ReservaRepository(IDbContextFactory<ApplicationDbContext> factory,
            IReservaCodeGenerator generator, TimeProvider clock)
        {
            _factory = factory;
            _generator = generator;
            _clock = clock;
        }

        public async Task<TurnoDto> CrearAsync(CrearReservaDto solicitud, int profesionalId,
            CancellationToken cancellationToken = default)
        {
            if (solicitud.FechaHoraInicio is null)
                throw new ValidationException("Falta la fecha y hora del turno.");
            var inicio = solicitud.FechaHoraInicio.Value;

            await using var db = await _factory.CreateDbContextAsync(cancellationToken);
            await using var transaccion = await db.Database.BeginTransactionAsync(
                IsolationLevel.Serializable, cancellationToken);

            // Lock SQL Server por profesional y día. Distintos profesionales pueden reservar en paralelo.
            // Esta protección presupone que TODAS las escrituras de turnos usan este mismo protocolo.
            var recurso = $"Peluqueria:Reservas:{profesionalId}:{inicio:yyyyMMdd}";
            await BloquearAgendaAsync(db, transaccion, recurso, cancellationToken);

            var servicio = await db.Servicios.AsNoTracking().SingleOrDefaultAsync(
                s => s.ServicioId == solicitud.ServicioId && s.Activo, cancellationToken)
                ?? throw new ValidationException("El servicio no está disponible.");
            var profesional = await db.Profesionales.AsNoTracking().SingleOrDefaultAsync(
                p => p.ProfesionalId == profesionalId && p.Activo, cancellationToken)
                ?? throw new ValidationException("El profesional no está disponible.");

            if (servicio.DuracionMinutos <= 0)
                throw new ValidationException("La duración del servicio es inválida.");
            var fin = inicio.AddMinutes(servicio.DuracionMinutos);
            var ahora = _clock.GetLocalNow().DateTime;
            if (inicio <= ahora)
                throw new ValidationException("No se pueden reservar fechas u horas pasadas.");
            if (inicio.Second != 0 || inicio.Millisecond != 0 || inicio.Minute % 15 != 0)
                throw new ValidationException("Los turnos deben empezar en intervalos de 15 minutos.");

            var dia = inicio.DayOfWeek == DayOfWeek.Sunday
                ? DiaSemana.Domingo : (DiaSemana)(int)inicio.DayOfWeek;
            var horario = await db.HorariosAtencion.AsNoTracking().SingleOrDefaultAsync(
                h => h.DiaSemana == dia && h.Activo, cancellationToken)
                ?? throw new ValidationException("El salón está cerrado en esa fecha.");

            var apertura = inicio.Date.Add(horario.HoraApertura.ToTimeSpan());
            var cierre = inicio.Date.Add(horario.HoraCierre.ToTimeSpan());
            if (inicio < apertura || fin > cierre || fin.Date != inicio.Date)
                throw new ValidationException("La reserva está fuera del horario de atención.");
            if (horario.HoraInicioDescanso.HasValue && horario.HoraFinDescanso.HasValue)
            {
                var descansoInicio = inicio.Date.Add(horario.HoraInicioDescanso.Value.ToTimeSpan());
                var descansoFin = inicio.Date.Add(horario.HoraFinDescanso.Value.ToTimeSpan());
                if (inicio < descansoFin && fin > descansoInicio)
                    throw new ValidationException("La reserva coincide con el descanso del salón.");
            }

            bool ocupado = await db.Turnos.AnyAsync(t => t.ProfesionalId == profesionalId &&
                (t.Estado == EstadoTurno.Pendiente || t.Estado == EstadoTurno.Confirmado) &&
                t.FechaHoraInicio < fin && t.FechaHoraFin > inicio, cancellationToken);
            if (ocupado)
                throw new ValidationException("Otro cliente acaba de reservar ese horario.");

            // Una reserva pública crea un Cliente nuevo; el teléfono NO se usa para identificarlo.
            var cliente = new Cliente
            {
                NombreCompleto = solicitud.NombreCompleto.Trim(),
                Telefono = solicitud.Telefono.Trim(),
                FechaAlta = DateTime.UtcNow,
                Activo = true
            };
            var turno = new Turno
            {
                Cliente = cliente,
                ProfesionalId = profesionalId,
                ServicioId = servicio.ServicioId,
                FechaHoraInicio = inicio,
                FechaHoraFin = fin,
                Precio = servicio.Precio,
                Estado = EstadoTurno.Pendiente,
                CodigoReserva = _generator.GenerarCodigo(),
                FechaCreacion = DateTime.UtcNow
            };
            db.Turnos.Add(turno);

            // El índice UX_Turno_CodigoReserva es la última garantía de unicidad.
            for (int intento = 1; intento <= 5; intento++)
            {
                try
                {
                    await db.SaveChangesAsync(cancellationToken);
                    break;
                }
                catch (DbUpdateException ex) when (
                    ex.InnerException is SqlException sql &&
                    (sql.Number == 2601 || sql.Number == 2627) && intento < 5)
                {
                    turno.CodigoReserva = _generator.GenerarCodigo();
                }
            }
            await transaccion.CommitAsync(cancellationToken);
            return new TurnoDto
            {
                TurnoId = turno.TurnoId,
                ClienteId = cliente.ClienteId,
                ClienteNombre = cliente.NombreCompleto,
                ProfesionalId = profesionalId,
                ProfesionalNombre = profesional.NombreCompleto,
                ServicioId = servicio.ServicioId,
                ServicioNombre = servicio.Nombre,
                FechaHoraInicio = inicio,
                FechaHoraFin = fin,
                Precio = turno.Precio,
                Estado = turno.Estado,
                CodigoReserva = turno.CodigoReserva,
                FechaCreacion = turno.FechaCreacion
            };
        }

        private static async Task BloquearAgendaAsync(ApplicationDbContext db,
            IDbContextTransaction transaction, string recurso, CancellationToken ct)
        {
            var conexion = db.Database.GetDbConnection();
            await using var comando = conexion.CreateCommand();
            comando.Transaction = transaction.GetDbTransaction();
            comando.CommandText = "DECLARE @r int; " +
                "EXEC @r = sys.sp_getapplock @Resource = @recurso, " +
                "@LockMode = 'Exclusive', @LockOwner = 'Transaction', @LockTimeout = 10000; " +
                "SELECT @r;";
            var parametro = comando.CreateParameter();
            parametro.ParameterName = "@recurso";
            parametro.Value = recurso;
            comando.Parameters.Add(parametro);
            var valor = await comando.ExecuteScalarAsync(ct);
            if (valor is null || Convert.ToInt32(valor) < 0)
                throw new InvalidOperationException("No fue posible bloquear la agenda. Intentá nuevamente.");
        }
    }

}
