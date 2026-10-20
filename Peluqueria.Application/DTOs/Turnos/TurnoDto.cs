using Peluqueria.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.DTOs.Turnos
{
    public class TurnoDto
    {
        public int TurnoId { get; set; }
        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public int ProfesionalId { get; set; }
        public string ProfesionalNombre { get; set; } = string.Empty;
        public int ServicioId { get; set; }
        public string ServicioNombre { get; set; } = string.Empty;
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }
        public decimal Precio { get; set; }
        public EstadoTurno Estado { get; set; }
        public string CodigoReserva { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
