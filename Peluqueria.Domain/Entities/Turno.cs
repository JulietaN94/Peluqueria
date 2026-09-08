using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Peluqueria.Domain.Enums;

namespace Peluqueria.Domain.Entities
{
    public class Turno
    {
        public int TurnoId { get; set; }

        public int ClienteId { get; set; }

        public int ProfesionalId { get; set; }

        public int ServicioId { get; set; }

        public DateTime FechaHoraInicio { get; set; }

        public DateTime FechaHoraFin { get; set; }

        public decimal Precio { get; set; }

        public EstadoTurno Estado { get; set; } = EstadoTurno.Pendiente;

        public string CodigoReserva { get; set; } = string.Empty;

        public string? Observaciones { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public Cliente Cliente { get; set; } = null!;

        public Profesional Profesional { get; set; } = null!;

        public Servicio Servicio { get; set; } = null!;
    }
}
