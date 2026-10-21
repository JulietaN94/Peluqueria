using Peluqueria.Application.DTOs.Comun;
using Peluqueria.Application.DTOs.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.Interfaces
{
    public interface IServicioService
    {
        Task<PaginaResultadoDto<ServicioDto>> ListarAsync(
            string? busqueda = null, int pagina = 1, int tamanoPagina = 10,
            CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ServicioDto>> ListarActivosAsync(
            CancellationToken cancellationToken = default);
        Task<ServicioDto?> ObtenerPorIdAsync(int servicioId,
            CancellationToken cancellationToken = default);
        Task<int> CrearAsync(ServicioFormDto datos,
            CancellationToken cancellationToken = default);
        Task ActualizarAsync(int servicioId, ServicioFormDto datos,
            CancellationToken cancellationToken = default);
        Task DesactivarAsync(int servicioId,
            CancellationToken cancellationToken = default);
    }
}
