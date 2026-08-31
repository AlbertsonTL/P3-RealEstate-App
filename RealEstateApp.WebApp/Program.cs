using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using RealEstateApp.Core.Application.Extensiones;
using RealEstateApp.Infrastructure.Data.Contexto;
using RealEstateApp.Infrastructure.Data.Extensiones;
using RealEstateApp.Infrastructure.Data.Semilla;
using RealEstateApp.Infrastructure.Identity.Contexto;
using RealEstateApp.Infrastructure.Identity.Entidades;
using RealEstateApp.Infrastructure.Identity.Extensiones;
using RealEstateApp.Infrastructure.Identity.Semilla;
using RealEstateApp.Infrastructure.Shared.Extensiones;
using RealEstateApp.WebApp.Filtros;
using RealEstateApp.WebApp.Interfaces.Servicios;
using RealEstateApp.WebApp.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfraestructuraDatos(builder.Configuration);
builder.Services.AddInfraestructuraIdentidad(builder.Configuration);
builder.Services.AddInfraestructuraShared(builder.Configuration);
builder.Services.AddAplicacion();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Cuenta/Login";
    options.AccessDeniedPath = "/Cuenta/AccesoDenegado";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

builder.Services.AddAuthorization();
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});
builder.Services.AddSession();
builder.Services.AddControllersWithViews().AddDataAnnotationsLocalization();

builder.Services.AddScoped<IServicioCuentaWebApp, ServicioCuentaWebApp>();
builder.Services.AddScoped<IServicioCatalogoAdmin, ServicioCatalogoAdmin>();
builder.Services.AddScoped<IServicioPropiedad, ServicioPropiedad>();
builder.Services.AddScoped<IServicioOferta, ServicioOferta>();
builder.Services.AddScoped<IServicioChat, ServicioChat>();
builder.Services.AddScoped<FiltroRolDesarrollador>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    // Maneja los errores (ErrorController -> Views/Error/Error500.cshtml).
    app.UseExceptionHandler("/Error/500");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Inicio}/{action=Index}/{id?}")
    .WithStaticAssets();

// Inicialización de BD y semilla
using (var scope = app.Services.CreateScope())
{
    var servicios = scope.ServiceProvider;
    var logger = servicios.GetRequiredService<ILogger<Program>>();
    try
    {
        var dbApp = servicios.GetRequiredService<AplicacionDbContext>();
        await dbApp.Database.EnsureCreatedAsync();

        var dbIdentity = servicios.GetRequiredService<IdentityDbContext>();
        await dbIdentity.Database.ExecuteSqlRawAsync(
            "IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'Identity') EXEC(N'CREATE SCHEMA [Identity]')");

        var conn = dbIdentity.Database.GetDbConnection();
        await conn.OpenAsync();
        int tablaIdentityExiste;
        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "SELECT COUNT(*) FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'Identity' AND t.name = 'Roles'";
            tablaIdentityExiste = (int)(await cmd.ExecuteScalarAsync() ?? 0);
        }
        await conn.CloseAsync();

        if (tablaIdentityExiste == 0)
        {
            var creator = (RelationalDatabaseCreator)dbIdentity.Database.GetService<IDatabaseCreator>();
            await creator.CreateTablesAsync();
        }

        var userManager = servicios.GetRequiredService<UserManager<UsuarioAplicacion>>();
        var roleManager = servicios.GetRequiredService<RoleManager<IdentityRole>>();
        await SemillaIdentidad.InicializarAsync(userManager, roleManager);

        await SemillaBD.SembrarCatalogosAsync(servicios);

        var agentes  = await userManager.GetUsersInRoleAsync("Agente");
        var clientes = await userManager.GetUsersInRoleAsync("Cliente");
        var agenteIds  = agentes.Where(u => u.EstaActivo).Select(u => u.Id).ToList();
        var clienteIds = clientes.Where(u => u.EstaActivo).Select(u => u.Id).ToList();

        await SemillaBD.SembrarPropiedadesAsync(servicios, agenteIds, clienteIds);
        await SemillaBD.SembrarFavoritasYChatAsync(servicios, clienteIds);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error al inicializar la base de datos.");
    }
}

app.Run();
