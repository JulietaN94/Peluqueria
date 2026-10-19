using Peluqueria.Web.Components;
using Microsoft.EntityFrameworkCore;
using Peluqueria.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Peluqueria.Infrastructure.Identity;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "No se encontró la cadena de conexión 'DefaultConnection'.");

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies();
builder.Services.AddAuthorization();

var app = builder.Build();

// Para esta etapa académica, el seed y las migraciones se ejecutan
// automáticamente solo en Development; no ejecutar migraciones
// automáticas en producción sin un procedimiento de despliegue controlado.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var contextFactory = scope.ServiceProvider
        .GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    await using var context = await contextFactory.CreateDbContextAsync();
    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    await DatabaseSeeder.SeedAsync(
        context, userManager, roleManager,
        app.Configuration["Seed:AdminPassword"]);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();