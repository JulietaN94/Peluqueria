
using Peluqueria.Web.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using Peluqueria.Infrastructure.Persistence;
using Peluqueria.Infrastructure.Identity;

// Servicios e interfaces de Application
using Peluqueria.Application.Interfaces;
using Peluqueria.Application.Services;

// Repositorios de Infrastructure
using Peluqueria.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. CONFIGURACIÓN DE BLAZOR
// ==========================================

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ==========================================
// 2. CONFIGURACIÓN DE SQL SERVER
// ==========================================

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "No se encontró la cadena de conexión 'DefaultConnection'.");

builder.Services.AddDbContextFactory<ApplicationDbContext>(
    options => options.UseSqlServer(connectionString));

// ==========================================
// 3. SERVICIOS GENERALES
// ==========================================

builder.Services.AddSingleton(TimeProvider.System);

// ==========================================
// 4. ASP.NET CORE IDENTITY
// ==========================================

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(
    IdentityConstants.ApplicationScheme)
    .AddIdentityCookies();

builder.Services.AddAuthorization();

// ==========================================
// 5. SERVICIOS DE APPLICATION
// ==========================================

// Gestión de clientes
builder.Services.AddScoped<IClienteService, ClienteService>();

// Gestión de profesionales
builder.Services.AddScoped<IProfesionalService, ProfesionalService>();

builder.Services.AddScoped<IServicioService, ServicioService>();

// Gestión de horarios
builder.Services.AddScoped<IHorarioService, HorarioService>();

// Generación de códigos de reserva
builder.Services.AddSingleton<
    IReservaCodeGenerator,
    ReservaCodeGenerator>();

// Disponibilidad de turnos
builder.Services.AddScoped<DisponibilidadService>();

// Creación de reservas
builder.Services.AddScoped<ReservaService>();

// ==========================================
// 6. REPOSITORIOS DE INFRASTRUCTURE
// ==========================================

// Persistencia de clientes
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();

// Persistencia de profesionales
builder.Services.AddScoped<IProfesionalRepository, ProfesionalRepository>();

builder.Services.AddScoped<IServicioRepository, ServicioRepository>();

// Persistencia de horarios
builder.Services.AddScoped<IHorarioRepository, HorarioRepository>();

// Persistencia de disponibilidad
builder.Services.AddScoped<
    IDisponibilidadRepository,
    DisponibilidadRepository>();

// Persistencia de reservas
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();

// ==========================================
// 7. CONSTRUCCIÓN DE LA APLICACIÓN  (donde testeo)
// ==========================================

var app = builder.Build();


// ==========================================
// 8. MIGRACIONES Y DATOS INICIALES
// ==========================================

// Solo se ejecuta en Development.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var contextFactory = scope.ServiceProvider
        .GetRequiredService<IDbContextFactory<ApplicationDbContext>>();

    await using var context =
        await contextFactory.CreateDbContextAsync();

    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<ApplicationUser>>();

    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    await DatabaseSeeder.SeedAsync(
        context,
        userManager,
        roleManager,
        app.Configuration["Seed:AdminPassword"]);
}

// ==========================================
// 9. MIDDLEWARE
// ==========================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Error",
        createScopeForErrors: true);

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseAntiforgery();

// ==========================================
// 10. COMPONENTES BLAZOR
// ==========================================

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
