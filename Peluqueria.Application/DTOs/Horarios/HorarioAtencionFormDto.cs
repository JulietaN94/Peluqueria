using Peluqueria.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.DTOs.Horarios
{
    public class HorarioAtencionFormDto : IValidatableObject
    {
        public int? HorarioAtencionId { get; set; }
        public DiaSemana DiaSemana { get; set; } = DiaSemana.Lunes;
        public TimeOnly HoraApertura { get; set; } = new(9, 0);
        public TimeOnly HoraCierre { get; set; } = new(19, 0);
        public TimeOnly? HoraInicioDescanso { get; set; }
        public TimeOnly? HoraFinDescanso { get; set; }
        public bool Activo { get; set; } = true;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Enum.IsDefined(typeof(DiaSemana), DiaSemana))
                yield return new ValidationResult("Día inválido.", new[] { nameof(DiaSemana) });
            if (HoraApertura >= HoraCierre)
                yield return new ValidationResult("La apertura debe ser anterior al cierre.", new[] { nameof(HoraApertura), nameof(HoraCierre) });
            if (HoraInicioDescanso.HasValue != HoraFinDescanso.HasValue)
                yield return new ValidationResult("Informá ambas horas del descanso.", new[] { nameof(HoraInicioDescanso), nameof(HoraFinDescanso) });
            if (HoraInicioDescanso.HasValue && HoraFinDescanso.HasValue &&
                (HoraInicioDescanso.Value < HoraApertura || HoraFinDescanso.Value > HoraCierre ||
                 HoraInicioDescanso.Value >= HoraFinDescanso.Value))
                yield return new ValidationResult("El descanso debe estar dentro del horario de atención y tener duración positiva.", new[] { nameof(HoraInicioDescanso), nameof(HoraFinDescanso) });
        }
    }

}
