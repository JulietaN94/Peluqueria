using Peluqueria.Application.DTOs.Comun;
using Peluqueria.Application.DTOs.Profesionales;
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
    public class ProfesionalService : IProfesionalService
    {
        private readonly IProfesionalRepository _repository;

        public ProfesionalService(IProfesionalRepository repository)
        {
            _repository = repository;
        }

        public Task<PaginaResultadoDto<ProfesionalDto>> ListarAsync(
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

        public Task<IReadOnlyList<ProfesionalDto>> ListarActivosAsync(
            CancellationToken cancellationToken = default) =>
            _repository.ListarActivosAsync(cancellationToken);

        public Task<ProfesionalDto?> ObtenerPorIdAsync(int profesionalId,
            CancellationToken cancellationToken = default)
        {
            if (profesionalId <= 0) throw new ArgumentOutOfRangeException(nameof(profesionalId));
            return _repository.ObtenerPorIdAsync(profesionalId, cancellationToken);
        }

        public async Task<int> CrearAsync(ProfesionalFormDto datos,
            CancellationToken cancellationToken = default)
        {
            Validar(datos);
            var profesional = new Profesional
            {
                NombreCompleto = datos.NombreCompleto.Trim(),
                Telefono = LimpiarOpcional(datos.Telefono),
                Especialidad = LimpiarOpcional(datos.Especialidad),
                Observaciones = LimpiarOpcional(datos.Observaciones),
                Activo = true
            };
            return await _repository.CrearAsync(profesional, cancellationToken);
        }

        public async Task ActualizarAsync(int profesionalId, ProfesionalFormDto datos,
            CancellationToken cancellationToken = default)
        {
            if (profesionalId <= 0) throw new ArgumentOutOfRangeException(nameof(profesionalId));
            Validar(datos);
            var profesional = await _repository.ObtenerEntidadPorIdAsync(profesionalId,
                cancellationToken) ?? throw new KeyNotFoundException(
                    $"No existe el profesional {profesionalId}.");

            profesional.NombreCompleto = datos.NombreCompleto.Trim();
            profesional.Telefono = LimpiarOpcional(datos.Telefono);
            profesional.Especialidad = LimpiarOpcional(datos.Especialidad);
            profesional.Observaciones = LimpiarOpcional(datos.Observaciones);
            profesional.Activo = datos.Activo;
            await _repository.ActualizarAsync(profesional, cancellationToken);
        }

        public async Task DesactivarAsync(int profesionalId,
            CancellationToken cancellationToken = default)
        {
            if (profesionalId <= 0) throw new ArgumentOutOfRangeException(nameof(profesionalId));
            var profesional = await _repository.ObtenerEntidadPorIdAsync(profesionalId,
                cancellationToken) ?? throw new KeyNotFoundException(
                    $"No existe el profesional {profesionalId}.");
            if (!profesional.Activo) return;
            profesional.Activo = false;
            await _repository.ActualizarAsync(profesional, cancellationToken);
        }

        private static void Validar(ProfesionalFormDto? datos)
        {
            if (datos is null) throw new ArgumentNullException(nameof(datos));
            if (string.IsNullOrWhiteSpace(datos.NombreCompleto))
                throw new ValidationException("El nombre es obligatorio.");
            var resultados = new List<ValidationResult>();
            if (!Validator.TryValidateObject(datos, new ValidationContext(datos),
                resultados, validateAllProperties: true))
                throw new ValidationException(string.Join("; ",
                    resultados.Select(x => x.ErrorMessage)));
            if (datos.NombreCompleto.Trim().Length > 150)
                throw new ValidationException("El nombre supera los 150 caracteres.");
        }

        private static string? LimpiarOpcional(string? texto) =>
            string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
    }

}
