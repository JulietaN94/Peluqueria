using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Domain.Entities
{
    public class Profesional
    {
        public int ProfesionalId { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string? Telefono { get; set; }

        public string? Especialidad { get; set; }

        public bool Activo { get; set; } = true;

        public string? Observaciones { get; set; }
    }
}
