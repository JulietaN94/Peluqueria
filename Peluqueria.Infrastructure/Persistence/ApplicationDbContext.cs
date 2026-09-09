using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Peluqueria.Domain.Entities;
using Peluqueria.Infrastructure.Identity;

namespace Peluqueria.Infrastructure.Persistence;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes { get; set; } = null!;

    public DbSet<Profesional> Profesionales { get; set; } = null!;

    public DbSet<Servicio> Servicios { get; set; } = null!;

    public DbSet<Turno> Turnos { get; set; } = null!;

    public DbSet<HorarioAtencion> HorariosAtencion { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}