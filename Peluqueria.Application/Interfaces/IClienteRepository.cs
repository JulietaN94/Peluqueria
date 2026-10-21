using Peluqueria.Application.DTOs.Clientes;
using Peluqueria.Application.DTOs.Comun;
using Peluqueria.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.Interfaces
{
    public interface IClienteRepository
    {
        Task<PaginaResultadoDto<ClienteDto>> ListarAsync(string? busqueda, int pagina,
            int tamanoPagina, CancellationToken cancellationToken = default);
        Task<ClienteDto?> ObtenerPorIdAsync(int clienteId,
            CancellationToken cancellationToken = default);
        Task<Cliente?> ObtenerEntidadPorIdAsync(int clienteId,
            CancellationToken cancellationToken = default);
        Task<int> CrearAsync(Cliente cliente,
            CancellationToken cancellationToken = default);
        Task ActualizarAsync(Cliente cliente,
            CancellationToken cancellationToken = default);
    }
}
