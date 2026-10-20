using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.DTOs.Clientes
{
    public class ClienteDto
    {
        public int ClienteId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public DateTime FechaAlta { get; set; }
        public bool Activo { get; set; }
        public string? Observaciones { get; set; }
    }
}
