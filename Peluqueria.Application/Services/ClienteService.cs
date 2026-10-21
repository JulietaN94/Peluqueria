using Peluqueria.Application.DTOs.Clientes;
using Peluqueria.Application.DTOs.Comun;
using Peluqueria.Application.Interfaces;
using Peluqueria.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _repository;

        public ClienteService(IClienteRepository repository)
        {
            _repository = repository;
        }

        public Task<PaginaResultadoDto<ClienteDto>> ListarAsync(
            string? busqueda = null, int pagina = 1, int tamanoPagina = 10,
            CancellationToken cancellationToken = default)
        {
            if (pagina < 1) throw new ArgumentOutOfRangeException(nameof(pagina));
            if (tamanoPagina < 1 || tamanoPagina > 100)
                throw new ArgumentOutOfRangeException(nameof(tamanoPagina),
                    "El tamaño de página debe estar entre 1 y 100.");

            return _repository.ListarAsync(busqueda?.Trim(), pagina,
                tamanoPagina, cancellationToken);
        }

        public Task<ClienteDto?> ObtenerPorIdAsync(int clienteId,
            CancellationToken cancellationToken = default)
        {
            if (clienteId <= 0) throw new ArgumentOutOfRangeException(nameof(clienteId));
            return _repository.ObtenerPorIdAsync(clienteId, cancellationToken);
        }

        public async Task<int> CrearAsync(ClienteFormDto datos,
            CancellationToken cancellationToken = default)
        {
            Validar(datos);
            var cliente = new Cliente
            {
                NombreCompleto = datos.NombreCompleto.Trim(),
                Telefono = datos.Telefono.Trim(),
                Observaciones = LimpiarOpcional(datos.Observaciones),
                FechaAlta = DateTime.UtcNow,
                Activo = true
            };
            return await _repository.CrearAsync(cliente, cancellationToken);
        }

        public async Task ActualizarAsync(int clienteId, ClienteFormDto datos,
            CancellationToken cancellationToken = default)
        {
            if (clienteId <= 0) throw new ArgumentOutOfRangeException(nameof(clienteId));
            Validar(datos);
            var cliente = await _repository.ObtenerEntidadPorIdAsync(clienteId, cancellationToken)
                ?? throw new KeyNotFoundException($"No existe el cliente {clienteId}.");

            cliente.NombreCompleto = datos.NombreCompleto.Trim();
            cliente.Telefono = datos.Telefono.Trim();
            cliente.Observaciones = LimpiarOpcional(datos.Observaciones);
            cliente.Activo = datos.Activo;
            await _repository.ActualizarAsync(cliente, cancellationToken);
        }

        public async Task DesactivarAsync(int clienteId,
            CancellationToken cancellationToken = default)
        {
            if (clienteId <= 0) throw new ArgumentOutOfRangeException(nameof(clienteId));
            var cliente = await _repository.ObtenerEntidadPorIdAsync(clienteId, cancellationToken)
                ?? throw new KeyNotFoundException($"No existe el cliente {clienteId}.");
            if (!cliente.Activo) return;
            cliente.Activo = false;
            await _repository.ActualizarAsync(cliente, cancellationToken);
        }

        private static void Validar(ClienteFormDto? datos)
        {
            if (datos is null) throw new ArgumentNullException(nameof(datos));
            if (string.IsNullOrWhiteSpace(datos.NombreCompleto))
                throw new ValidationException("El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(datos.Telefono))
                throw new ValidationException("El teléfono es obligatorio.");

            var resultados = new List<ValidationResult>();
            if (!Validator.TryValidateObject(datos, new ValidationContext(datos),
                resultados, validateAllProperties: true))
                throw new ValidationException(string.Join("; ", resultados.Select(x => x.ErrorMessage)));
        }

        private static string? LimpiarOpcional(string? texto) =>
            string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
    }
}
