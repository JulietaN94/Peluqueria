using Peluqueria.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.Interfaces
{
    public interface IDisponibilidadRepository
    {
        Task<Servicio?> ObtenerServicioAsync(int servicioId, CancellationToken ct = default);
        Task<IReadOnlyList<Profesional>> ObtenerProfesionalesAsync(int? profesionalId, CancellationToken ct = default);
        Task<HorarioAtencion?> ObtenerHorarioAsync(DayOfWeek dia, CancellationToken ct = default);
        Task<IReadOnlyList<Turno>> ObtenerTurnosOcupadosAsync(DateTime inicio, DateTime fin, CancellationToken ct = default);
    }
}
