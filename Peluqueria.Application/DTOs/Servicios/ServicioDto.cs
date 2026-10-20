using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.DTOs.Servicios
{
    public class ServicioDto
    {
        public int ServicioId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int DuracionMinutos { get; set; }
        public string? ImagenUrl { get; set; }
        public bool Activo { get; set; }
    }
}
