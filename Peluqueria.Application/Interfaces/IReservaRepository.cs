using Peluqueria.Application.DTOs.Turnos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.Interfaces
{
    public interface IReservaRepository
    {
        Task<TurnoDto> CrearAsync(CrearReservaDto solicitud, int profesionalId,
            CancellationToken cancellationToken = default);
    }
}
