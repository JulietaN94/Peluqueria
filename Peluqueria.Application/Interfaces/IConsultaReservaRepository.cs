using Peluqueria.Application.DTOs.Turnos;

namespace Peluqueria.Application.Interfaces;

public interface IConsultaReservaRepository
{
    Task<ReservaPublicaDto?> ObtenerPorCodigoAsync(
        string codigoReserva,
        CancellationToken cancellationToken = default);

    // Una actualización atómica evita cancelar turnos ya iniciados o finalizados.
    Task<bool> CancelarPorCodigoAsync(
        string codigoReserva,
        DateTime ahoraLocal,
        CancellationToken cancellationToken = default);
}
