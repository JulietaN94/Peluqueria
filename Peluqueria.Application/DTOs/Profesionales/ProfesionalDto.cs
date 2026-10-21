using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.DTOs.Profesionales
{
    public class ProfesionalDto
    {
        public int ProfesionalId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Especialidad { get; set; }
        public bool Activo { get; set; }
        public string? Observaciones { get; set; }
    }
}
