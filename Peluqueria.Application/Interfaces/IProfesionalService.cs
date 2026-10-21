using Peluqueria.Application.DTOs.Comun;
using Peluqueria.Application.DTOs.Profesionales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.Interfaces
{
    public interface IProfesionalService
    {
        Task<PaginaResultadoDto<ProfesionalDto>> ListarAsync(
            string? busqueda = null, int pagina = 1, int tamanoPagina = 10,
            CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ProfesionalDto>> ListarActivosAsync(
            CancellationToken cancellationToken = default);
        Task<ProfesionalDto?> ObtenerPorIdAsync(int profesionalId,
            CancellationToken cancellationToken = default);
        Task<int> CrearAsync(ProfesionalFormDto datos,
            CancellationToken cancellationToken = default);
        Task ActualizarAsync(int profesionalId, ProfesionalFormDto datos,
            CancellationToken cancellationToken = default);
        Task DesactivarAsync(int profesionalId,
            CancellationToken cancellationToken = default);
    }
}
