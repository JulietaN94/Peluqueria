using Microsoft.EntityFrameworkCore;
using Peluqueria.Application.DTOs.Clientes;
using Peluqueria.Application.DTOs.Comun;
using Peluqueria.Application.Interfaces;
using Peluqueria.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Infrastructure.Persistence.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public ClienteRepository(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<PaginaResultadoDto<ClienteDto>> ListarAsync(
            string? busqueda, int pagina, int tamanoPagina,
            CancellationToken cancellationToken = default)
        {
            await using var db = await _factory.CreateDbContextAsync(cancellationToken);
            var query = db.Clientes.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                query = query.Where(c => c.NombreCompleto.Contains(busqueda)
                    || c.Telefono.Contains(busqueda));
            }

            var total = await query.CountAsync(cancellationToken);
            var items = await query.OrderBy(c => c.NombreCompleto)
                .ThenBy(c => c.ClienteId)
                .Skip((pagina - 1) * tamanoPagina).Take(tamanoPagina)
                .Select(c => new ClienteDto
                {
                    ClienteId = c.ClienteId,
                    NombreCompleto = c.NombreCompleto,
                    Telefono = c.Telefono,
                    FechaAlta = c.FechaAlta,
                    Activo = c.Activo,
                    Observaciones = c.Observaciones
                }).ToListAsync(cancellationToken);

            return new PaginaResultadoDto<ClienteDto>
            {
                Items = items,
                TotalRegistros = total,
                PaginaActual = pagina,
                TamanoPagina = tamanoPagina
            };
        }

        public async Task<ClienteDto?> ObtenerPorIdAsync(int clienteId,
            CancellationToken cancellationToken = default)
        {
            await using var db = await _factory.CreateDbContextAsync(cancellationToken);
            return await db.Clientes.AsNoTracking()
                .Where(c => c.ClienteId == clienteId)
                .Select(c => new ClienteDto
                {
                    ClienteId = c.ClienteId,
                    NombreCompleto = c.NombreCompleto,
                    Telefono = c.Telefono,
                    FechaAlta = c.FechaAlta,
                    Activo = c.Activo,
                    Observaciones = c.Observaciones
                }).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Cliente?> ObtenerEntidadPorIdAsync(int clienteId,
            CancellationToken cancellationToken = default)
        {
            await using var db = await _factory.CreateDbContextAsync(cancellationToken);
            return await db.Clientes.AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClienteId == clienteId, cancellationToken);
        }

        public async Task<int> CrearAsync(Cliente cliente,
            CancellationToken cancellationToken = default)
        {
            await using var db = await _factory.CreateDbContextAsync(cancellationToken);
            db.Clientes.Add(cliente);
            await db.SaveChangesAsync(cancellationToken);
            return cliente.ClienteId;
        }

        public async Task ActualizarAsync(Cliente cliente,
            CancellationToken cancellationToken = default)
        {
            await using var db = await _factory.CreateDbContextAsync(cancellationToken);
            db.Clientes.Update(cliente);
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
