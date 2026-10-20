using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.DTOs.Servicios
{
    public class ServicioFormDto
    {
        public int? ServicioId { get; set; }
        [Required(ErrorMessage = "Ingresá el nombre del servicio.")]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;
        [StringLength(500)]
        public string? Descripcion { get; set; }
        [Range(typeof(decimal), "0", "9999999999999999.99", ErrorMessage = "El precio no puede ser negativo.")]
        public decimal Precio { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "La duración debe ser mayor que cero.")]
        public int DuracionMinutos { get; set; }
        [StringLength(500)]
        public string? ImagenUrl { get; set; }
        public bool Activo { get; set; } = true;
    }

}
