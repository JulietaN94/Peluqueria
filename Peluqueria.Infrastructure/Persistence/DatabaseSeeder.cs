using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Peluqueria.Domain.Entities;
using Peluqueria.Domain.Enums;
using Peluqueria.Infrastructure.Identity;

namespace Peluqueria.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
       ApplicationDbContext context,
       UserManager<ApplicationUser> userManager,
       RoleManager<IdentityRole> roleManager,
       string? administratorPassword)
    {
        await context.Database.MigrateAsync();
        await SeedIdentityAsync(userManager, roleManager, administratorPassword);
        await SeedBusinessDataAsync(context);
    }

    private static async Task SeedIdentityAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        string? administratorPassword)
    {
        const string roleName = "Administrador";
        const string email = "admin@peluqueria.local";

        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            EnsureSucceeded(roleResult, "crear el rol Administrador");
        }

        var admin = await userManager.FindByEmailAsync(email);
        if (admin is null)
        {
            if (string.IsNullOrWhiteSpace(administratorPassword))
            {
                throw new InvalidOperationException(
                    "Falta configurar Seed:AdminPassword en User Secrets. " +
                    "Es necesario para crear el administrador inicial.");
            }

            admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var userResult = await userManager.CreateAsync(admin, administratorPassword);
            EnsureSucceeded(userResult, "crear el usuario administrador");
        }

        if (!await userManager.IsInRoleAsync(admin, roleName))
        {
            var addRoleResult = await userManager.AddToRoleAsync(admin, roleName);
            EnsureSucceeded(addRoleResult, "asignar el rol Administrador");
        }
    }

    private static void EnsureSucceeded(IdentityResult result, string action)
    {
        if (!result.Succeeded)
        {
            string details = string.Join(" | ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Error al {action}: {details}");
        }
    }

    private static async Task SeedBusinessDataAsync(ApplicationDbContext context)
    {
        if (!await context.Profesionales.AnyAsync())
        {
            context.Profesionales.AddRange(
                new Profesional { NombreCompleto = "María González", Telefono = "11-4000-1001", Especialidad = "Corte y peinado", Activo = true },
                new Profesional { NombreCompleto = "Juan Pérez", Telefono = "11-4000-1002", Especialidad = "Coloración", Activo = true },
                new Profesional { NombreCompleto = "Laura Rodríguez", Telefono = "11-4000-1003", Especialidad = "Corte y color", Activo = true });
        }

        if (!await context.Servicios.AnyAsync())
        {
            context.Servicios.AddRange(
                new Servicio { Nombre = "Corte", Descripcion = "Corte de cabello.", Precio = 10000m, DuracionMinutos = 30, Activo = true },
                new Servicio { Nombre = "Corte + Brushing", Descripcion = "Corte de cabello y brushing.", Precio = 15000m, DuracionMinutos = 60, Activo = true },
                new Servicio { Nombre = "Brushing", Descripcion = "Brushing de cabello.", Precio = 9000m, DuracionMinutos = 45, Activo = true },
                new Servicio { Nombre = "Coloración", Descripcion = "Servicio de coloración.", Precio = 30000m, DuracionMinutos = 120, Activo = true },
                new Servicio { Nombre = "Mechas", Descripcion = "Servicio de mechas.", Precio = 35000m, DuracionMinutos = 150, Activo = true },
                new Servicio { Nombre = "Peinado", Descripcion = "Peinado para eventos y ocasiones especiales.", Precio = 12000m, DuracionMinutos = 60, Activo = true },
                new Servicio { Nombre = "Alisado", Descripcion = "Servicio de alisado.", Precio = 28000m, DuracionMinutos = 120, Activo = true });
        }

        if (!await context.HorariosAtencion.AnyAsync())
        {
            foreach (var day in new[] {
                DiaSemana.Lunes, DiaSemana.Martes, DiaSemana.Miercoles,
                DiaSemana.Jueves, DiaSemana.Viernes })
            {
                context.HorariosAtencion.Add(CrearHorario(day, new TimeOnly(9, 0),
                    new TimeOnly(19, 0), new TimeOnly(13, 0), new TimeOnly(15, 0)));
            }

            context.HorariosAtencion.Add(CrearHorario(DiaSemana.Sabado,
                new TimeOnly(9, 0), new TimeOnly(13, 0)));
            // Un horario inactivo también debe cumplir Apertura < Cierre.
            context.HorariosAtencion.Add(CrearHorario(DiaSemana.Domingo,
                new TimeOnly(9, 0), new TimeOnly(13, 0), activo: false));
        }

        await context.SaveChangesAsync();
    }

    private static HorarioAtencion CrearHorario(
        DiaSemana dia, TimeOnly apertura, TimeOnly cierre,
        TimeOnly? inicioDescanso = null, TimeOnly? finDescanso = null,
        bool activo = true)
    {
        return new HorarioAtencion
        {
            DiaSemana = dia,
            HoraApertura = apertura,
            HoraCierre = cierre,
            HoraInicioDescanso = inicioDescanso,
            HoraFinDescanso = finDescanso,
            Activo = activo
        };
    }
}