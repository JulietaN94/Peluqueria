using Microsoft.EntityFrameworkCore;
using Peluqueria.Application.DTOs.Comun;
using Peluqueria.Application.DTOs.Servicios;
using Peluqueria.Application.Interfaces;
using Peluqueria.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Infrastructure.Persistence.Repositories
{
    public class ServicioRepository : IServicioRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public ServicioRepository(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<PaginaResultadoDto<ServicioDto>> ListarAsync(
            string? busqueda, int pagina, int tamanoPagina,
            CancellationToken cancellationToken = default)
        {
            await using var db = await _factory.CreateDbContextAsync(cancellationToken);
            var query = db.Servicios.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(busqueda))
                query = query.Where(s => s.Nombre.Contains(busqueda) ||
                    (s.Descripcion != null && s.Descripcion.Contains(busqueda)));

            var total = await query.CountAsync(cancellationToken);
            var items = await query.OrderBy(s => s.Nombre)
                .ThenBy(s => s.ServicioId)
                .Skip((pagina - 1) * tamanoPagina).Take(tamanoPagina)
                .Select(s => new ServicioDto
                {
                    ServicioId = s.ServicioId,
                    Nombre = s.Nombre,
                    Descripcion = s.Descripcion,
                    Precio = s.Precio,
                    DuracionMinutos = s.DuracionMinutos,
                    ImagenUrl = s.ImagenUrl,
                    Activo = s.Activo
                }).ToListAsync(cancellationToken);

            return new PaginaResultadoDto<ServicioDto>
            {
                Items = items,
                TotalRegistros = total,
                PaginaActual = pagina,
                TamanoPagina = tamanoPagina
            };
        }

        public async Task<IReadOnlyList<ServicioDto>> ListarActivosAsync(
            CancellationToken cancellationToken = default)
        {
            await using var db = await _factory.CreateDbContextAsync(cancellationToken);
            return await db.Servicios.AsNoTracking()
                .Where(s => s.Activo)
                .OrderBy(s => s.Nombre)
                .ThenBy(s => s.ServicioId)
                .Select(s => new ServicioDto
                {
                    ServicioId = s.ServicioId,
                    Nombre = s.Nombre,
                    Descripcion = s.Descripcion,
                    Precio = s.Precio,
                    DuracionMinutos = s.DuracionMinutos,
                    ImagenUrl = s.ImagenUrl,
                    Activo = s.Activo
                }).ToListAsync(cancellationToken);
        }

        public async Task<ServicioDto?> ObtenerPorIdAsync(int servicioId,
            CancellationToken cancellationToken = default)
        {
            await using var db = await _factory.CreateDbContextAsync(cancellationToken);
            return await db.Servicios.AsNoTracking()
                .Where(s => s.ServicioId == servicioId)
                .Select(s => new ServicioDto
                {
                    ServicioId = s.ServicioId,
                    Nombre = s.Nombre,
                    Descripcion = s.Descripcion,
                    Precio = s.Precio,
                    DuracionMinutos = s.DuracionMinutos,
                    ImagenUrl = s.ImagenUrl,
                    Activo = s.Activo
                }).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Servicio?> ObtenerEntidadPorIdAsync(int servicioId,
            CancellationToken cancellationToken = default)
        {
            await using var db = await _factory.CreateDbContextAsync(cancellationToken);
            return await db.Servicios.AsNoTracking()
                .FirstOrDefaultAsync(s => s.ServicioId == servicioId,
                    cancellationToken);
        }

        public async Task<int> CrearAsync(Servicio servicio,
            CancellationToken cancellationToken = default)
        {
            await using var db = await _factory.CreateDbContextAsync(cancellationToken);
            db.Servicios.Add(servicio);
            await db.SaveChangesAsync(cancellationToken);
            return servicio.ServicioId;
        }

        public async Task ActualizarAsync(Servicio servicio,
            CancellationToken cancellationToken = default)
        {
            await using var db = await _factory.CreateDbContextAsync(cancellationToken);
            db.Servicios.Update(servicio);
            await db.SaveChangesAsync(cancellationToken);
        }
    }

}
