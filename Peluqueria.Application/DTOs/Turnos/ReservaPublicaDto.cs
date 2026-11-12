using Peluqueria.Domain.Enums;

namespace Peluqueria.Application.DTOs.Turnos;

// El DTO público solo incluye datos necesarios para consultar una reserva.
public sealed class ReservaPublicaDto
{
    public string CodigoReserva { get; set; } = string.Empty;
    public string ClienteNombre { get; set; } = string.Empty;
    public string ServicioNombre { get; set; } = string.Empty;
    public string ProfesionalNombre { get; set; } = string.Empty;
    public DateTime FechaHoraInicio { get; set; }
    public DateTime FechaHoraFin { get; set; }
    public decimal Precio { get; set; }
    public EstadoTurno Estado { get; set; }
}

