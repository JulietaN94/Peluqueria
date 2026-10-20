using Peluqueria.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.DTOs.Horarios
{
    public class HorarioAtencionDto
    {
        public int HorarioAtencionId { get; set; }
        public DiaSemana DiaSemana { get; set; }
        public TimeOnly HoraApertura { get; set; }
        public TimeOnly HoraCierre { get; set; }
        public TimeOnly? HoraInicioDescanso { get; set; }
        public TimeOnly? HoraFinDescanso { get; set; }
        public bool Activo { get; set; }
    }
}
