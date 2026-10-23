using Microsoft.EntityFrameworkCore;
using Peluqueria.Application.DTOs.Horarios;
using Peluqueria.Application.Interfaces;
using Peluqueria.Domain.Entities;
using Peluqueria.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Infrastructure.Persistence.Repositories
{
    public class HorarioRepository : IHorarioRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public HorarioRepository(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<IReadOnlyList<HorarioAtencionDto>> ListarAsync(
            CancellationToken cancellationToken = default)
        {
            await using var db = await _factory.CreateDbContextAsync(cancellationToken);
            return await db.HorariosAtencion.AsNoTracking()
                .OrderBy(h => h.DiaSemana)
                .Select(h => new HorarioAtencionDto
                {
                    HorarioAtencionId = h.HorarioAtencionId,
                    DiaSemana = h.DiaSemana,
                    HoraApertura = h.HoraApertura,
                    HoraCierre = h.HoraCierre,
                    HoraInicioDescanso = h.HoraInicioDescanso,
                    HoraFinDescanso = h.HoraFinDescanso,
                    Activo = h.Activo
                }).ToListAsync(cancellationToken);
        }

        public async Task<HorarioAtencionDto?> ObtenerPorDiaAsync(
            DiaSemana dia, CancellationToken cancellationToken = default)
        {
            await using var db = await _factory.CreateDbContextAsync(cancellationToken);
            return await db.HorariosAtencion.AsNoTracking()
                .Where(h => h.DiaSemana == dia)
                .Select(h => new HorarioAtencionDto
                {
                    HorarioAtencionId = h.HorarioAtencionId,
                    DiaSemana = h.DiaSemana,
                    HoraApertura = h.HoraApertura,
                    HoraCierre = h.HoraCierre,
                    HoraInicioDescanso = h.HoraInicioDescanso,
                    HoraFinDescanso = h.HoraFinDescanso,
                    Activo = h.Activo
                }).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<HorarioAtencion?> ObtenerEntidadPorDiaAsync(
            DiaSemana dia, CancellationToken cancellationToken = default)
        {
            await using var db = await _factory.CreateDbContextAsync(cancellationToken);
            return await db.HorariosAtencion.AsNoTracking()
                .FirstOrDefaultAsync(h => h.DiaSemana == dia, cancellationToken);
        }

        public async Task GuardarAsync(
            HorarioAtencion horario, CancellationToken cancellationToken = default)
        {
            await using var db = await _factory.CreateDbContextAsync(cancellationToken);
            if (horario.HorarioAtencionId == 0)
                db.HorariosAtencion.Add(horario);
            else
                db.HorariosAtencion.Update(horario);
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
