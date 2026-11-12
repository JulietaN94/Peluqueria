using Microsoft.EntityFrameworkCore;
using Peluqueria.Application.DTOs.Turnos;
using Peluqueria.Application.Interfaces;
using Peluqueria.Domain.Enums;

namespace Peluqueria.Infrastructure.Persistence.Repositories;

public sealed class ConsultaReservaRepository : IConsultaReservaRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public ConsultaReservaRepository(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<ReservaPublicaDto?> ObtenerPorCodigoAsync(
        string codigoReserva,
        CancellationToken cancellationToken = default)
    {
        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        return await db.Turnos.AsNoTracking()
            .Where(t => t.CodigoReserva == codigoReserva)
            .Select(t => new ReservaPublicaDto
            {
                CodigoReserva = t.CodigoReserva,
                ClienteNombre = t.Cliente.NombreCompleto,
                ServicioNombre = t.Servicio.Nombre,
                ProfesionalNombre = t.Profesional.NombreCompleto,
                FechaHoraInicio = t.FechaHoraInicio,
                FechaHoraFin = t.FechaHoraFin,
                Precio = t.Precio,
                Estado = t.Estado
            })
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> CancelarPorCodigoAsync(
        string codigoReserva,
        DateTime ahoraLocal,
        CancellationToken cancellationToken = default)
    {
        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        // Un único UPDATE condicional comprueba el estado inmediatamente
        // antes de modificarlo. No elimina al cliente ni el turno.
        int modificados = await db.Turnos
            .Where(t => t.CodigoReserva == codigoReserva &&
                t.FechaHoraInicio > ahoraLocal &&
                (t.Estado == EstadoTurno.Pendiente ||
                 t.Estado == EstadoTurno.Confirmado))
            .ExecuteUpdateAsync(actualizacion => actualizacion
                .SetProperty(t => t.Estado, EstadoTurno.Cancelado),
                cancellationToken);

        return modificados == 1;
    }
}
