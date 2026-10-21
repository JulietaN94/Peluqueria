using Peluqueria.Application.DTOs.Comun;
using Peluqueria.Application.DTOs.Servicios;
using Peluqueria.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.Interfaces
{
    public interface IServicioRepository
    {
        Task<PaginaResultadoDto<ServicioDto>> ListarAsync(string? busqueda, int pagina,
            int tamanoPagina, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ServicioDto>> ListarActivosAsync(
            CancellationToken cancellationToken = default);
        Task<ServicioDto?> ObtenerPorIdAsync(int servicioId,
            CancellationToken cancellationToken = default);
        Task<Servicio?> ObtenerEntidadPorIdAsync(int servicioId,
            CancellationToken cancellationToken = default);
        Task<int> CrearAsync(Servicio servicio,
            CancellationToken cancellationToken = default);
        Task ActualizarAsync(Servicio servicio,
            CancellationToken cancellationToken = default);
    }
}
