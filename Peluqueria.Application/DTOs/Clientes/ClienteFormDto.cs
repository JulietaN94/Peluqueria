using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.DTOs.Clientes
{
    public class ClienteFormDto
    {
        public int? ClienteId { get; set; }
        [Required(ErrorMessage = "Ingresá el nombre completo.")]
        [StringLength(150, ErrorMessage = "Máximo 150 caracteres.")]
        public string NombreCompleto { get; set; } = string.Empty;
        [Required(ErrorMessage = "Ingresá el teléfono.")]
        [StringLength(30, ErrorMessage = "Máximo 30 caracteres.")]
        public string Telefono { get; set; } = string.Empty;
        [StringLength(1000, ErrorMessage = "Máximo 1000 caracteres.")]
        public string? Observaciones { get; set; }
        public bool Activo { get; set; } = true;
    }
}
