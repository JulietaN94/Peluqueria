using Peluqueria.Application.DTOs.Turnos;
using Peluqueria.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.Services
{
    public sealed class DisponibilidadService
    {
        private const int IntervaloMinutos = 15;
        private readonly IDisponibilidadRepository _repository;
        private readonly TimeProvider _clock;

        public DisponibilidadService(IDisponibilidadRepository repository, TimeProvider clock)
        {
            _repository = repository;
            _clock = clock;
        }

        public async Task<IReadOnlyList<TurnoDisponibleDto>> ObtenerDisponibilidadAsync(
            int servicioId, DateOnly fecha, int? profesionalId = null,
            CancellationToken cancellationToken = default)
        {
            var hoy = DateOnly.FromDateTime(_clock.GetLocalNow().DateTime);
            if (fecha < hoy) return Array.Empty<TurnoDisponibleDto>();

            var servicio = await _repository.ObtenerServicioAsync(servicioId, cancellationToken);
            if (servicio is null || !servicio.Activo || servicio.DuracionMinutos <= 0)
                return Array.Empty<TurnoDisponibleDto>();

            var horario = await _repository.ObtenerHorarioAsync(fecha.DayOfWeek, cancellationToken);
            if (horario is null || !horario.Activo) return Array.Empty<TurnoDisponibleDto>();

            var profesionales = await _repository.ObtenerProfesionalesAsync(profesionalId, cancellationToken);
            if (profesionales.Count == 0) return Array.Empty<TurnoDisponibleDto>();

            var apertura = fecha.ToDateTime(horario.HoraApertura);
            var cierre = fecha.ToDateTime(horario.HoraCierre);
            if (cierre <= apertura) return Array.Empty<TurnoDisponibleDto>();

            DateTime? descansoInicio = horario.HoraInicioDescanso is TimeOnly di
                ? fecha.ToDateTime(di) : null;
            DateTime? descansoFin = horario.HoraFinDescanso is TimeOnly df
                ? fecha.ToDateTime(df) : null;

            var ocupados = await _repository.ObtenerTurnosOcupadosAsync(apertura, cierre, cancellationToken);
            var ahora = _clock.GetLocalNow().DateTime;
            var disponibles = new List<TurnoDisponibleDto>();

            foreach (var profesional in profesionales)
            {
                if (!profesional.Activo) continue;
                for (var inicio = apertura; inicio.AddMinutes(servicio.DuracionMinutos) <= cierre;
                     inicio = inicio.AddMinutes(IntervaloMinutos))
                {
                    var fin = inicio.AddMinutes(servicio.DuracionMinutos);
                    if (inicio <= ahora) continue;

                    if (descansoInicio.HasValue && descansoFin.HasValue &&
                        inicio < descansoFin.Value && fin > descansoInicio.Value)
                        continue;

                    if (ocupados.Any(t => t.ProfesionalId == profesional.ProfesionalId &&
                        inicio < t.FechaHoraFin && fin > t.FechaHoraInicio))
                        continue;

                    disponibles.Add(new TurnoDisponibleDto
                    {
                        ProfesionalId = profesional.ProfesionalId,
                        ProfesionalNombre = profesional.NombreCompleto,
                        FechaHoraInicio = inicio,
                        FechaHoraFin = fin
                    });
                }
            }

            return disponibles.OrderBy(d => d.FechaHoraInicio)
                .ThenBy(d => d.ProfesionalNombre).ToList();
        }
    }
}
