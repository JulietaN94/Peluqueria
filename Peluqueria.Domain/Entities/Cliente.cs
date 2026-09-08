using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Domain.Entities
{
    public class Cliente
    {
        public int ClienteId { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        public DateTime FechaAlta { get; set; } = DateTime.UtcNow;

        public bool Activo { get; set; } = true;

        public string? Observaciones { get; set; }

        public ICollection<Turno> Turnos { get; set; } = new List<Turno>();
    }
}
