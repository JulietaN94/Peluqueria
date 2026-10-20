using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.DTOs.Turnos
{
    public class TurnoDisponibleDto
    {
        public int ProfesionalId { get; set; }
        public string ProfesionalNombre { get; set; } = string.Empty;
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }
    }
}
