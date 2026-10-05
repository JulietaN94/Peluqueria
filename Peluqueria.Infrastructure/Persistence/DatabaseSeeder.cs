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
        RoleManager<IdentityRole> roleManager)
    {
        await context.Database.MigrateAsync();

        await SeedIdentityAsync(userManager, roleManager);
        await SeedBusinessDataAsync(context);
    }

    private static async Task SeedIdentityAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        const string rolAdministrador = "Administrador";
        const string emailAdministrador = "admin@peluqueria.local";

        if (!await roleManager.RoleExistsAsync(rolAdministrador))
        {
            var roleResult = await roleManager.CreateAsync(
                new IdentityRole(rolAdministrador));

            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"No se pudo crear el rol '{rolAdministrador}'.");
            }
        }

        var usuario = await userManager.FindByEmailAsync(
            emailAdministrador);

        if (usuario is null)
        {
            usuario = new ApplicationUser
            {
                UserName = emailAdministrador,
                Email = emailAdministrador,
                EmailConfirmed = true
            };

            const string password = "Admin123!";

            var userResult = await userManager.CreateAsync(
                usuario,
                password);

            if (!userResult.Succeeded)
            {
                var errores = string.Join(
                    ", ",
                    userResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"No se pudo crear el usuario administrador: {errores}");
            }
        }

        if (!await userManager.IsInRoleAsync(
                usuario,
                rolAdministrador))
        {
            var roleResult = await userManager.AddToRoleAsync(
                usuario,
                rolAdministrador);

            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "No se pudo asignar el rol Administrador al usuario.");
            }
        }
    }

    private static async Task SeedBusinessDataAsync(
        ApplicationDbContext context)
    {
        if (!await context.Profesionales.AnyAsync())
        {
            context.Profesionales.AddRange(
                new Profesional
                {
                    NombreCompleto = "María González",
                    Telefono = "11-4000-1001",
                    Especialidad = "Corte y peinado",
                    Activo = true
                },
                new Profesional
                {
                    NombreCompleto = "Juan Pérez",
                    Telefono = "11-4000-1002",
                    Especialidad = "Coloración",
                    Activo = true
                },
                new Profesional
                {
                    NombreCompleto = "Laura Rodríguez",
                    Telefono = "11-4000-1003",
                    Especialidad = "Corte y color",
                    Activo = true
                });
        }

        if (!await context.Servicios.AnyAsync())
        {
            context.Servicios.AddRange(
                new Servicio
                {
                    Nombre = "Corte",
                    Descripcion = "Corte de cabello.",
                    Precio = 10000m,
                    DuracionMinutos = 30,
                    Activo = true
                },
                new Servicio
                {
                    Nombre = "Corte + Brushing",
                    Descripcion = "Corte de cabello y brushing.",
                    Precio = 15000m,
                    DuracionMinutos = 60,
                    Activo = true
                },
                new Servicio
                {
                    Nombre = "Brushing",
                    Descripcion = "Brushing de cabello.",
                    Precio = 9000m,
                    DuracionMinutos = 45,
                    Activo = true
                },
                new Servicio
                {
                    Nombre = "Coloración",
                    Descripcion = "Servicio de coloración.",
                    Precio = 30000m,
                    DuracionMinutos = 120,
                    Activo = true
                },
                new Servicio
                {
                    Nombre = "Mechas",
                    Descripcion = "Servicio de mechas.",
                    Precio = 35000m,
                    DuracionMinutos = 150,
                    Activo = true
                },
                new Servicio
                {
                    Nombre = "Peinado",
                    Descripcion = "Peinado para eventos y ocasiones especiales.",
                    Precio = 12000m,
                    DuracionMinutos = 60,
                    Activo = true
                },
                new Servicio
                {
                    Nombre = "Alisado",
                    Descripcion = "Servicio de alisado.",
                    Precio = 28000m,
                    DuracionMinutos = 120,
                    Activo = true
                });
        }

        if (!await context.HorariosAtencion.AnyAsync())
        {
            context.HorariosAtencion.AddRange(
                CrearHorario(
                    DiaSemana.Lunes,
                    new TimeOnly(9, 0),
                    new TimeOnly(19, 0),
                    new TimeOnly(13, 0),
                    new TimeOnly(15, 0)),

                CrearHorario(
                    DiaSemana.Martes,
                    new TimeOnly(9, 0),
                    new TimeOnly(19, 0),
                    new TimeOnly(13, 0),
                    new TimeOnly(15, 0)),

                CrearHorario(
                    DiaSemana.Miercoles,
                    new TimeOnly(9, 0),
                    new TimeOnly(19, 0),
                    new TimeOnly(13, 0),
                    new TimeOnly(15, 0)),

                CrearHorario(
                    DiaSemana.Jueves,
                    new TimeOnly(9, 0),
                    new TimeOnly(19, 0),
                    new TimeOnly(13, 0),
                    new TimeOnly(15, 0)),

                CrearHorario(
                    DiaSemana.Viernes,
                    new TimeOnly(9, 0),
                    new TimeOnly(19, 0),
                    new TimeOnly(13, 0),
                    new TimeOnly(15, 0)),

                CrearHorario(
                    DiaSemana.Sabado,
                    new TimeOnly(9, 0),
                    new TimeOnly(13, 0)),

                CrearHorario(
                    DiaSemana.Domingo,
                    new TimeOnly(0, 0),
                    new TimeOnly(0, 0),
                    activo: false));
        }

        await context.SaveChangesAsync();
    }

    private static HorarioAtencion CrearHorario(
        DiaSemana dia,
        TimeOnly apertura,
        TimeOnly cierre,
        TimeOnly? inicioDescanso = null,
        TimeOnly? finDescanso = null,
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