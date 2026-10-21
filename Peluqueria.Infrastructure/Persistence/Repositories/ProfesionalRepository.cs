using Microsoft.EntityFrameworkCore;
using Peluqueria.Application.DTOs.Comun;
using Peluqueria.Application.DTOs.Profesionales;
using Peluqueria.Application.Interfaces;
using Peluqueria.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Infrastructure.Persistence.Repositories
{
    public class ProfesionalRepository : IProfesionalRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public ProfesionalRepository(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<PaginaResultadoDto<ProfesionalDto>> ListarAsync(
            string? busqueda, int pagina, int tamanoPagina,
            CancellationToken cancellationToken = default)
        {
            await using var db = await _factory.CreateDbContextAsync(cancellationToken);
            var query = db.Profesionales.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(busqueda))
                query = query.Where(p => p.NombreCompleto.Contains(busqueda) ||
                    (p.Especialidad != null && p.Especialidad.Contains(busqueda)));

            var total = await query.CountAsync(cancellationToken);
            var items = await query.OrderBy(p => p.NombreCompleto)
                .ThenBy(p => p.ProfesionalId)
                .Skip((pagina - 1) * tamanoPagina).Take(tamanoPagina)
                .Select(p => new ProfesionalDto
                {
                    ProfesionalId = p.ProfesionalId,
                    NombreCompleto = p.NombreCompleto,
                    Telefono = p.Telefono,
                    Especialidad = p.Especialidad,
                    Activo = p.Activo,
                    Observaciones = p.Observaciones
                }).ToListAsync(cancellationToken);

            return new PaginaResultadoDto<ProfesionalDto>
            {
                Items = items,
                TotalRegistros = total,
                PaginaActual = pagina,
                TamanoPagina = tamanoPagina
            };
        }

        public async Task<IReadOnlyList<ProfesionalDto>> ListarActivosAsync(
            CancellationToken cancellationToken = default)
        {
            await using var db = await _factory.CreateDbContextAsync(cancellationToken);
            return await db.Profesionales.AsNoTracking()
                .Where(p => p.Activo)
                .OrderBy(p => p.NombreCompleto)
                .ThenBy(p => p.ProfesionalId)
                .Select(p => new ProfesionalDto
                {
                    ProfesionalId = p.ProfesionalId,
                    NombreCompleto = p.NombreCompleto,
                    Telefono = p.Telefono,
                    Especialidad = p.Especialidad,
                    Activo = p.Activo,
                    Observaciones = p.Observaciones
                }).ToListAsync(cancellationToken);
        }

        public async Task<ProfesionalDto?> ObtenerPorIdAsync(int profesionalId,
            CancellationToken cancellationToken = default)
        {
            await using var db = await _factory.CreateDbContextAsync(cancellationToken);
            return await db.Profesionales.AsNoTracking()
                .Where(p => p.ProfesionalId == profesionalId)
                .Select(p => new ProfesionalDto
                {
                    ProfesionalId = p.ProfesionalId,
                    NombreCompleto = p.NombreCompleto,
                    Telefono = p.Telefono,
                    Especialidad = p.Especialidad,
                    Activo = p.Activo,
                    Observaciones = p.Observaciones
                }).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Profesional?> ObtenerEntidadPorIdAsync(int profesionalId,
            CancellationToken cancellationToken = default)
        {
            await using var db = await _factory.CreateDbContextAsync(cancellationToken);
            return await db.Profesionales.AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProfesionalId == profesionalId,
                    cancellationToken);
        }

        public async Task<int> CrearAsync(Profesional profesional,
            CancellationToken cancellationToken = default)
        {
            await using var db = await _factory.CreateDbContextAsync(cancellationToken);
            db.Profesionales.Add(profesional);
            await db.SaveChangesAsync(cancellationToken);
            return profesional.ProfesionalId;
        }

        public async Task ActualizarAsync(Profesional profesional,
            CancellationToken cancellationToken = default)
        {
            await using var db = await _factory.CreateDbContextAsync(cancellationToken);
            db.Profesionales.Update(profesional);
            await db.SaveChangesAsync(cancellationToken);
        }
    }

}
