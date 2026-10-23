using Microsoft.EntityFrameworkCore;
using Peluqueria.Application.Interfaces;
using Peluqueria.Domain.Entities;
using Peluqueria.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Infrastructure.Persistence.Repositories
{
    public sealed class DisponibilidadRepository : IDisponibilidadRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public DisponibilidadRepository(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<Servicio?> ObtenerServicioAsync(int servicioId, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.Servicios.AsNoTracking()
                .FirstOrDefaultAsync(s => s.ServicioId == servicioId, ct);
        }

        public async Task<IReadOnlyList<Profesional>> ObtenerProfesionalesAsync(int? profesionalId, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            var query = db.Profesionales.AsNoTracking().Where(p => p.Activo);
            if (profesionalId.HasValue)
                query = query.Where(p => p.ProfesionalId == profesionalId.Value);
            return await query.OrderBy(p => p.NombreCompleto).ToListAsync(ct);
        }

        public async Task<HorarioAtencion?> ObtenerHorarioAsync(DayOfWeek dia, CancellationToken ct = default)
        {
            // DayOfWeek: domingo=0; DiaSemana del dominio: lunes=1 ... domingo=7.
            var diaSistema = dia == DayOfWeek.Sunday ? DiaSemana.Domingo : (DiaSemana)(int)dia;
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.HorariosAtencion.AsNoTracking()
                .FirstOrDefaultAsync(h => h.DiaSemana == diaSistema, ct);
        }

        public async Task<IReadOnlyList<Turno>> ObtenerTurnosOcupadosAsync(DateTime inicio, DateTime fin, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.Turnos.AsNoTracking()
                .Where(t => (t.Estado == EstadoTurno.Pendiente || t.Estado == EstadoTurno.Confirmado)
                    && t.FechaHoraInicio < fin && t.FechaHoraFin > inicio)
                .ToListAsync(ct);
        }
    }
}
