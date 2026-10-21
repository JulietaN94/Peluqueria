using Peluqueria.Application.DTOs.Comun;
using Peluqueria.Application.DTOs.Profesionales;
using Peluqueria.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.Interfaces
{
    public interface IProfesionalRepository
    {
        Task<PaginaResultadoDto<ProfesionalDto>> ListarAsync(string? busqueda, int pagina,
            int tamanoPagina, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ProfesionalDto>> ListarActivosAsync(
            CancellationToken cancellationToken = default);
        Task<ProfesionalDto?> ObtenerPorIdAsync(int profesionalId,
            CancellationToken cancellationToken = default);
        Task<Profesional?> ObtenerEntidadPorIdAsync(int profesionalId,
            CancellationToken cancellationToken = default);
        Task<int> CrearAsync(Profesional profesional,
            CancellationToken cancellationToken = default);
        Task ActualizarAsync(Profesional profesional,
            CancellationToken cancellationToken = default);
    }

}
