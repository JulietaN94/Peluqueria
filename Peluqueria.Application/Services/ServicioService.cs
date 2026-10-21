using Peluqueria.Application.DTOs.Comun;
using Peluqueria.Application.DTOs.Servicios;
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
    public class ServicioService : IServicioService
    {
        private readonly IServicioRepository _repository;

        public ServicioService(IServicioRepository repository)
        {
            _repository = repository;
        }

        public Task<PaginaResultadoDto<ServicioDto>> ListarAsync(
            string? busqueda = null, int pagina = 1, int tamanoPagina = 10,
            CancellationToken cancellationToken = default)
        {
            if (pagina < 1) throw new ArgumentOutOfRangeException(nameof(pagina));
            if (tamanoPagina < 1 || tamanoPagina > 100)
                throw new ArgumentOutOfRangeException(nameof(tamanoPagina),
                    "El tamaño de página debe estar entre 1 y 100.");

            return _repository.ListarAsync(busqueda?.Trim(), pagina, tamanoPagina,
                cancellationToken);
        }

        public Task<IReadOnlyList<ServicioDto>> ListarActivosAsync(
            CancellationToken cancellationToken = default) =>
            _repository.ListarActivosAsync(cancellationToken);

        public Task<ServicioDto?> ObtenerPorIdAsync(int servicioId,
            CancellationToken cancellationToken = default)
        {
            if (servicioId <= 0) throw new ArgumentOutOfRangeException(nameof(servicioId));
            return _repository.ObtenerPorIdAsync(servicioId, cancellationToken);
        }

        public async Task<int> CrearAsync(ServicioFormDto datos,
            CancellationToken cancellationToken = default)
        {
            Validar(datos);
            var servicio = new Servicio
            {
                Nombre = datos.Nombre.Trim(),
                Descripcion = LimpiarOpcional(datos.Descripcion),
                Precio = datos.Precio,
                DuracionMinutos = datos.DuracionMinutos,
                ImagenUrl = LimpiarOpcional(datos.ImagenUrl),
                Activo = true
            };
            return await _repository.CrearAsync(servicio, cancellationToken);
        }

        public async Task ActualizarAsync(int servicioId, ServicioFormDto datos,
            CancellationToken cancellationToken = default)
        {
            if (servicioId <= 0) throw new ArgumentOutOfRangeException(nameof(servicioId));
            Validar(datos);
            var servicio = await _repository.ObtenerEntidadPorIdAsync(servicioId,
                cancellationToken) ?? throw new KeyNotFoundException(
                    $"No existe el servicio {servicioId}.");

            servicio.Nombre = datos.Nombre.Trim();
            servicio.Descripcion = LimpiarOpcional(datos.Descripcion);
            servicio.Precio = datos.Precio;
            servicio.DuracionMinutos = datos.DuracionMinutos;
            servicio.ImagenUrl = LimpiarOpcional(datos.ImagenUrl);
            servicio.Activo = datos.Activo;
            await _repository.ActualizarAsync(servicio, cancellationToken);
        }

        public async Task DesactivarAsync(int servicioId,
            CancellationToken cancellationToken = default)
        {
            if (servicioId <= 0) throw new ArgumentOutOfRangeException(nameof(servicioId));
            var servicio = await _repository.ObtenerEntidadPorIdAsync(servicioId,
                cancellationToken) ?? throw new KeyNotFoundException(
                    $"No existe el servicio {servicioId}.");
            if (!servicio.Activo) return;
            servicio.Activo = false;
            await _repository.ActualizarAsync(servicio, cancellationToken);
        }

        private static void Validar(ServicioFormDto? datos)
        {
            if (datos is null) throw new ArgumentNullException(nameof(datos));
            if (string.IsNullOrWhiteSpace(datos.Nombre))
                throw new ValidationException("El nombre del servicio es obligatorio.");

            var resultados = new List<ValidationResult>();
            if (!Validator.TryValidateObject(datos, new ValidationContext(datos),
                resultados, validateAllProperties: true))
                throw new ValidationException(string.Join("; ",
                    resultados.Select(x => x.ErrorMessage)));

            if (datos.Nombre.Trim().Length > 150)
                throw new ValidationException("El nombre supera los 150 caracteres.");
            if (datos.Precio < 0 || datos.Precio > 9999999999999999.99m)
                throw new ValidationException("El precio está fuera del rango permitido.");
            if (datos.DuracionMinutos <= 0)
                throw new ValidationException("La duración debe ser mayor que cero.");
            if (LimpiarOpcional(datos.Descripcion)?.Length > 500)
                throw new ValidationException("La descripción supera los 500 caracteres.");
            if (LimpiarOpcional(datos.ImagenUrl)?.Length > 500)
                throw new ValidationException("La imagen URL supera los 500 caracteres.");
        }

        private static string? LimpiarOpcional(string? texto) =>
            string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
    }

}
