using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.DTOs.Turnos
{
    public class TurnoFormDto
    {
        public int? TurnoId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Seleccioná un cliente.")]
        public int ClienteId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Seleccioná un profesional.")]
        public int ProfesionalId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Seleccioná un servicio.")]
        public int ServicioId { get; set; }
        [Required(ErrorMessage = "Ingresá fecha y hora.")]
        public DateTime? FechaHoraInicio { get; set; }
        [StringLength(1000)]
        public string? Observaciones { get; set; }
    }
}
