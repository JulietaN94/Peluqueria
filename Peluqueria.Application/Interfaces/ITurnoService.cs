using Peluqueria.Application.DTOs.Comun;
using Peluqueria.Application.DTOs.Turnos;
using Peluqueria.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Application.Interfaces
{
    public interface ITurnoService
    {
        Task<IReadOnlyList<TurnoDisponibleDto>> ObtenerDisponibilidadAsync(
            int servicioId, DateOnly fecha, int? profesionalId = null,
            CancellationToken cancellationToken = default);

        Task<TurnoDto> CrearReservaAsync(CrearReservaDto datos,
            CancellationToken cancellationToken = default);
        Task<TurnoDto> CrearTurnoAdministrativoAsync(TurnoFormDto datos,
            CancellationToken cancellationToken = default);

        Task<TurnoDto?> ConsultarPorCodigoAsync(string codigoReserva,
            CancellationToken cancellationToken = default);
        Task<TurnoDto?> ObtenerPorIdAsync(int turnoId,
            CancellationToken cancellationToken = default);
        Task<PaginaResultadoDto<TurnoDto>> ListarAsync(
            DateOnly? fecha = null, EstadoTurno? estado = null,
            int? profesionalId = null, int pagina = 1, int tamanoPagina = 10,
            CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TurnoDto>> ObtenerHistorialClienteAsync(int clienteId,
            CancellationToken cancellationToken = default);

        // Una cancelación pública requiere el código de reserva.
        Task CancelarPorCodigoAsync(string codigoReserva,
            CancellationToken cancellationToken = default);
        Task CambiarEstadoAsync(int turnoId, EstadoTurno nuevoEstado,
            CancellationToken cancellationToken = default);
    }
}
