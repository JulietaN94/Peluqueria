using Peluqueria.Application.DTOs.Clientes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Peluqueria.Application.DTOs.Comun;

namespace Peluqueria.Application.Interfaces
{
    public interface IClienteService
    {
        Task<PaginaResultadoDto<ClienteDto>> ListarAsync(
            string? busqueda = null, int pagina = 1, int tamanoPagina = 10,
            CancellationToken cancellationToken = default);
        Task<ClienteDto?> ObtenerPorIdAsync(int clienteId,
            CancellationToken cancellationToken = default);
        Task<int> CrearAsync(ClienteFormDto datos,
            CancellationToken cancellationToken = default);
        Task ActualizarAsync(int clienteId, ClienteFormDto datos,
            CancellationToken cancellationToken = default);
        Task DesactivarAsync(int clienteId,
            CancellationToken cancellationToken = default);
    }
}
