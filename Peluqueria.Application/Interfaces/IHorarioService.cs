using Peluqueria.Application.DTOs.Horarios;
using Peluqueria.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.Interfaces
{
    public interface IHorarioService
    {
        Task<IReadOnlyList<HorarioAtencionDto>> ListarAsync(
            CancellationToken cancellationToken = default);
        Task<HorarioAtencionDto?> ObtenerPorDiaAsync(DiaSemana dia,
            CancellationToken cancellationToken = default);
        Task GuardarAsync(HorarioAtencionFormDto datos,
            CancellationToken cancellationToken = default);
    }
}
