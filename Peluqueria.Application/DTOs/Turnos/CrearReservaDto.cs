using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.DTOs.Turnos
{
    public class CrearReservaDto
    {
        [Required(ErrorMessage = "Ingresá tu nombre.")]
        [StringLength(150)]
        public string NombreCompleto { get; set; } = string.Empty;
        [Required(ErrorMessage = "Ingresá tu teléfono.")]
        [StringLength(30)]
        public string Telefono { get; set; } = string.Empty;
        [Range(1, int.MaxValue, ErrorMessage = "Seleccioná un servicio.")]
        public int ServicioId { get; set; }
        public int? ProfesionalId { get; set; } // null: cualquier profesional disponible
        [Required(ErrorMessage = "Seleccioná fecha y hora.")]
        public DateTime? FechaHoraInicio { get; set; }
    }
}
