using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.DTOs.Profesionales
{
    public class ProfesionalFormDto
    {
        public int? ProfesionalId { get; set; }
        [Required(ErrorMessage = "Ingresá el nombre completo.")]
        [StringLength(150)]
        public string NombreCompleto { get; set; } = string.Empty;
        [StringLength(30)]
        public string? Telefono { get; set; }
        [StringLength(150)]
        public string? Especialidad { get; set; }
        [StringLength(1000)]
        public string? Observaciones { get; set; }
        public bool Activo { get; set; } = true;
    }
}
