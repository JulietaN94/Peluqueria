using Peluqueria.Application.DTOs.Horarios;
using Peluqueria.Application.Interfaces;
using Peluqueria.Domain.Entities;
using Peluqueria.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.Services
{
    public class HorarioService : IHorarioService
    {
        private readonly IHorarioRepository _repository;

        public HorarioService(IHorarioRepository repository)
        {
            _repository = repository;
        }

        public Task<IReadOnlyList<HorarioAtencionDto>> ListarAsync(
            CancellationToken cancellationToken = default) =>
            _repository.ListarAsync(cancellationToken);

        public Task<HorarioAtencionDto?> ObtenerPorDiaAsync(
            DiaSemana dia, CancellationToken cancellationToken = default)
        {
            if (!Enum.IsDefined(typeof(DiaSemana), dia))
                throw new ValidationException("El día de la semana es inválido.");

            return _repository.ObtenerPorDiaAsync(dia, cancellationToken);
        }

        public async Task GuardarAsync(
            HorarioAtencionFormDto datos, CancellationToken cancellationToken = default)
        {
            if (datos is null) throw new ArgumentNullException(nameof(datos));

            var resultados = new List<ValidationResult>();
            if (!Validator.TryValidateObject(datos, new ValidationContext(datos),
                    resultados, validateAllProperties: true))
            {
                throw new ValidationException(string.Join("; ",
                    resultados.Select(x => x.ErrorMessage)));
            }

            // También validar aquí las reglas de la base de datos.
            if (datos.HoraApertura >= datos.HoraCierre)
                throw new ValidationException("La apertura debe ser anterior al cierre.");
            if (datos.HoraInicioDescanso.HasValue != datos.HoraFinDescanso.HasValue)
                throw new ValidationException("Debés informar ambas horas del descanso.");
            if (datos.HoraInicioDescanso is TimeOnly inicio &&
                datos.HoraFinDescanso is TimeOnly fin &&
                (inicio <= datos.HoraApertura || inicio >= fin || fin >= datos.HoraCierre))
            {
                throw new ValidationException("El descanso debe estar estrictamente dentro del horario de atención.");
            }

            var horario = await _repository.ObtenerEntidadPorDiaAsync(datos.DiaSemana, cancellationToken)
                ?? new HorarioAtencion { DiaSemana = datos.DiaSemana };

            horario.HoraApertura = datos.HoraApertura;
            horario.HoraCierre = datos.HoraCierre;
            horario.HoraInicioDescanso = datos.HoraInicioDescanso;
            horario.HoraFinDescanso = datos.HoraFinDescanso;
            horario.Activo = datos.Activo;

            await _repository.GuardarAsync(horario, cancellationToken);
        }
    }
}
