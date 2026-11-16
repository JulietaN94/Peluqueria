using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Peluqueria.Web.Models;

/// <summary>
/// Modelo de validación para el formulario administrativo de servicios.
/// Evita analizar límites decimales expresados como texto según la cultura regional.
/// </summary>
public sealed class ServicioAdminFormModel : IValidatableObject
{
    public int? ServicioId { get; set; }

    [Required(ErrorMessage = "Ingresá el nombre del servicio.")]
    [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    public string? Descripcion { get; set; }

    public decimal Precio { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La duración debe ser mayor que cero.")]
    public int DuracionMinutos { get; set; } = 30;

    [StringLength(500, ErrorMessage = "La URL no puede superar los 500 caracteres.")]
    public string? ImagenUrl { get; set; }

    public bool Activo { get; set; } = true;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Precio < 0m || Precio > 9999999999999999.99m)
        {
            yield return new ValidationResult(
                "El precio debe estar entre $0 y $9.999.999.999.999.999,99.",
                new[] { nameof(Precio) });
        }
    }
}

