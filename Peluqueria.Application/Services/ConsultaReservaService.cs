using Peluqueria.Application.DTOs.Turnos;
using Peluqueria.Application.Interfaces;
using Peluqueria.Domain.Enums;

namespace Peluqueria.Application.Services;

public sealed class ConsultaReservaService
{
    private const string Caracteres = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private readonly IConsultaReservaRepository _repository;
    private readonly TimeProvider _clock;

    public ConsultaReservaService(IConsultaReservaRepository repository, TimeProvider clock)
    {
        _repository = repository;
        _clock = clock;
    }

    public async Task<ReservaPublicaDto?> ConsultarAsync(
        string? codigoReserva, CancellationToken cancellationToken = default)
    {
        var codigo = Normalizar(codigoReserva);
        if (!EsCodigoValido(codigo)) return null;
        return await _repository.ObtenerPorCodigoAsync(codigo, cancellationToken);
    }

    public async Task<bool> CancelarAsync(
        string? codigoReserva, CancellationToken cancellationToken = default)
    {
        var codigo = Normalizar(codigoReserva);
        if (!EsCodigoValido(codigo)) return false;

        var reserva = await _repository.ObtenerPorCodigoAsync(codigo, cancellationToken);
        if (reserva is null) return false;

        var ahora = _clock.GetLocalNow().DateTime;
        if ((reserva.Estado != EstadoTurno.Pendiente &&
             reserva.Estado != EstadoTurno.Confirmado) ||
            reserva.FechaHoraInicio <= ahora)
            return false;

        // El repositorio vuelve a comprobar estado y fecha en el UPDATE atómico.
        return await _repository.CancelarPorCodigoAsync(codigo, ahora, cancellationToken);
    }

    private static string Normalizar(string? codigo) =>
        (codigo ?? string.Empty).Trim().ToUpperInvariant();

    private static bool EsCodigoValido(string codigo) =>
        codigo.Length == 16 && codigo.StartsWith("PEL-", StringComparison.Ordinal) &&
        codigo[4..].All(c => Caracteres.Contains(c));
}
