using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Peluqueria.Web.Models;

/// <summary>
/// Datos que completa el cliente en el paso 4 del asistente de reservas.
/// Se mantiene en Web para no alterar las entidades ni contratos del backend.
/// </summary>
public sealed class ReservaDatosFormModel
{
    [Required(ErrorMessage = "Ingresá tu nombre y apellido.")]
    [StringLength(150, MinimumLength = 3,
        ErrorMessage = "El nombre debe tener entre 3 y 150 caracteres.")]
    [NombreClienteValido]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresá tu teléfono de contacto.")]
    [StringLength(30, MinimumLength = 6,
        ErrorMessage = "El teléfono debe tener entre 6 y 30 caracteres.")]
    [TelefonoContactoValido]
    public string Telefono { get; set; } = string.Empty;
}

/// <summary>
/// Comprueba que el nombre tenga al menos tres caracteres útiles y contenga
/// exclusivamente letras Unicode (incluidas tildes y ñ) y espacios internos.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
internal sealed class NombreClienteValidoAttribute : ValidationAttribute
{
    public NombreClienteValidoAttribute()
    {
        ErrorMessage = "Ingresá un nombre de al menos 3 caracteres usando solo letras y espacios (se permiten tildes y ñ).";
    }

    public override bool IsValid(object? value)
    {
        // Required se encarga de los valores vacíos.
        if (value is not string nombre || string.IsNullOrWhiteSpace(nombre))
            return true;

        var nombreLimpio = nombre.Trim();
        return nombreLimpio.Length >= 3 &&
               Regex.IsMatch(nombreLimpio, @"^\p{L}+(?: +\p{L}+)*$", RegexOptions.CultureInvariant);
    }
}

/// <summary>
/// Acepta teléfonos con espacios, paréntesis, guiones y prefijo +,
/// pero exige al menos seis dígitos reales.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
internal sealed class TelefonoContactoValidoAttribute : ValidationAttribute
{
    public TelefonoContactoValidoAttribute()
    {
        ErrorMessage = "Ingresá un teléfono válido con al menos 6 números (podés usar +, espacios, guiones o paréntesis).";
    }

    public override bool IsValid(object? value)
    {
        // Required se encarga de los valores vacíos.
        if (value is not string telefono || string.IsNullOrWhiteSpace(telefono))
            return true;

        var cantidadDigitos = 0;
        foreach (var caracter in telefono)
        {
            if (caracter is >= '0' and <= '9')
            {
                cantidadDigitos++;
                continue;
            }

            if (caracter is not (' ' or '+' or '-' or '(' or ')'))
                return false;
        }

        return cantidadDigitos >= 6;
    }
}
