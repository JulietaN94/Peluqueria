using Peluqueria.Application.DTOs.Turnos;
using Peluqueria.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.Services
{
    public sealed class ReservaService
    {
        private readonly DisponibilidadService _disponibilidad;
        private readonly IReservaRepository _repository;

        public ReservaService(DisponibilidadService disponibilidad, IReservaRepository repository)
        {
            _disponibilidad = disponibilidad;
            _repository = repository;
        }

        public async Task<TurnoDto> CrearAsync(CrearReservaDto solicitud,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(solicitud);
            var errores = new List<ValidationResult>();
            if (!Validator.TryValidateObject(solicitud, new ValidationContext(solicitud),
                errores, validateAllProperties: true))
                throw new ValidationException(string.Join("; ", errores.Select(e => e.ErrorMessage)));

            if (string.IsNullOrWhiteSpace(solicitud.NombreCompleto) ||
                string.IsNullOrWhiteSpace(solicitud.Telefono))
                throw new ValidationException("Nombre y teléfono son obligatorios.");

            if (!solicitud.FechaHoraInicio.HasValue)
                throw new ValidationException("Seleccioná una fecha y hora.");

            var inicio = solicitud.FechaHoraInicio.Value;
            var fecha = DateOnly.FromDateTime(inicio);
            var opciones = await _disponibilidad.ObtenerDisponibilidadAsync(
                solicitud.ServicioId, fecha, solicitud.ProfesionalId, cancellationToken);

            var opcion = opciones.FirstOrDefault(o => o.FechaHoraInicio == inicio &&
                (!solicitud.ProfesionalId.HasValue || o.ProfesionalId == solicitud.ProfesionalId.Value));
            if (opcion is null)
                throw new ValidationException("El horario seleccionado ya no está disponible.");

            return await _repository.CrearAsync(solicitud, opcion.ProfesionalId, cancellationToken);
        }
    }
}
