using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.OpenApi.Models;
using RealEstateApp.Infrastructure.Identity.Entidades;
using RealEstateApp.Core.Application.Extensiones;
using RealEstateApp.Core.Domain.Enumeraciones;
using RealEstateApp.Infrastructure.Data.Contexto;
using RealEstateApp.Infrastructure.Data.Extensiones;
using RealEstateApp.Infrastructure.Data.Semilla;
using RealEstateApp.Infrastructure.Identity.Contexto;
using RealEstateApp.Infrastructure.Identity.Extensiones;
using RealEstateApp.Infrastructure.Identity.Semilla;
using RealEstateApp.Infrastructure.Shared.Extensiones;
using RealEstateApp.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// REGISTRO DE CAPAS — Onion Architecture
builder.Services.AddInfraestructuraDatos(builder.Configuration);
builder.Services.AddInfraestructuraIdentidad(builder.Configuration);   // AddIdentity sets cookie defaults
builder.Services.AddInfraestructuraShared(builder.Configuration);
builder.Services.AddAplicacion();

// ✅ BUG FIX #1: Debe llamarse DESPUÉS de AddInfraestructuraIdentidad para que
// los defaults JWT sobreescriban los defaults de cookie que pone AddIdentity.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
});

// CONTROLLERS
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// SWAGGER
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RealEstateApp API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            new string[] {}
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("DesarrolloLocal", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseMiddleware<ManejadorExcepcionesGlobal>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RealEstateApp API v1");
        c.RoutePrefix = string.Empty;
    });
    app.UseCors("DesarrolloLocal");
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// INICIALIZACIÓN DE BASE DE DATOS Y SEMILLA
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
        await SemillaBD.InicializarAsync(servicios);

        var agentes  = await userManager.GetUsersInRoleAsync(TipoRol.Agente.ToString());
        var clientes = await userManager.GetUsersInRoleAsync(TipoRol.Cliente.ToString());
        var agenteIds  = agentes.Where(u => u.EstaActivo).Select(u => u.Id).ToList();
        var clienteIds = clientes.Where(u => u.EstaActivo).Select(u => u.Id).ToList();
        await SemillaBD.SembrarPropiedadesAsync(servicios, agenteIds, clienteIds);
        await SemillaBD.SembrarFavoritasYChatAsync(servicios, clienteIds);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error crítico al inicializar la base de datos.");
    }
}

app.Run();