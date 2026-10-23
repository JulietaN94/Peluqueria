using Peluqueria.Application.DTOs.Horarios;
using Peluqueria.Domain.Entities;
using Peluqueria.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.Interfaces
{
    public interface IHorarioRepository
    {
        Task<IReadOnlyList<HorarioAtencionDto>> ListarAsync(CancellationToken cancellationToken = default);
        Task<HorarioAtencionDto?> ObtenerPorDiaAsync(DiaSemana dia, CancellationToken cancellationToken = default);
        Task<HorarioAtencion?> ObtenerEntidadPorDiaAsync(DiaSemana dia, CancellationToken cancellationToken = default);
        Task GuardarAsync(HorarioAtencion horario, CancellationToken cancellationToken = default);
    }
}
