using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace RealEstateApp.Infrastructure.Data.Contexto
{
    /// <summary>
    /// Fábrica de DbContext en tiempo de diseño.
    ///
    /// FIX: sin esta fábrica, "dotnet ef" no puede crear el DbContext porque
    /// RealEstateApp.Infrastructure.Data es una librería de clases sin Program.cs
    /// ni contenedor de DI propio, por lo que no puede resolver
    /// DbContextOptions&lt;AplicacionDbContext&gt; (este era el error del log:
    /// "Unable to resolve service for type DbContextOptions...AplicacionDbContext").
    ///
    /// Con esta fábrica ya se puede ejecutar, desde la carpeta del proyecto Data:
    ///   dotnet ef migrations add NombreMigracion
    ///   dotnet ef database update
    ///
    /// Alternativamente, desde la raíz de la solución, siempre se puede indicar
    /// explícitamente el proyecto de arranque (recomendado si se agregan más
    /// migraciones), por ejemplo:
    ///   dotnet ef database update --project RealEstateApp.Infrastructure.Data --startup-project RealEstateApp.WebApp
    /// </summary>
    public class AplicacionDbContextFactory : IDesignTimeDbContextFactory<AplicacionDbContext>
    {
        public AplicacionDbContext CreateDbContext(string[] args)
        {
            // IMPORTANTE: NO usamos Directory.GetCurrentDirectory() como base.
            // "dotnet ef" puede invocarse desde la raíz de la solución, desde
            // este mismo proyecto, o con --project/--startup-project, y en cada
            // caso el directorio "actual" cambia. Si la ruta relativa no
            // encuentra el appsettings.Development.json real (con
            // "RealEstateApp-DB"), GetConnectionString devuelve null y el código
            // caía silenciosamente en la cadena de respaldo (Database=RealEstateApp,
            // SIN el prefijo "DB") -> esa era la causa del bug.
            //
            // En su lugar anclamos la búsqueda al directorio del ensamblado
            // compilado (AppContext.BaseDirectory, algo como
            // .../RealEstateApp.Infrastructure.Data/bin/Debug/net8.0/), que es
            // estable sin importar desde dónde se ejecute el comando, y subimos
            // hasta encontrar la carpeta del proyecto WebApp.
            var directorioBase = AppContext.BaseDirectory;
            var carpetaWebApp = BuscarCarpetaWebApp(directorioBase)
                ?? BuscarCarpetaWebApp(Directory.GetCurrentDirectory());

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true);

            if (carpetaWebApp != null)
            {
                builder
                    .AddJsonFile(Path.Combine(carpetaWebApp, "appsettings.json"), optional: true)
                    .AddJsonFile(Path.Combine(carpetaWebApp, "appsettings.Development.json"), optional: true);
            }

            var configuracion = builder.Build();

            var cadenaConexion = configuracion.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(cadenaConexion))
            {
                // Cadena de respaldo para poder generar migraciones sin una base real disponible.
                // OJO: si esta cadena se está usando en la práctica, significa que no se
                // encontró ningún appsettings.Development.json real -> revisar la ruta.
                cadenaConexion = "Server=(localdb)\\mssqllocaldb;Database=RealEstateApp-DB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";
            }

            var opciones = new DbContextOptionsBuilder<AplicacionDbContext>()
                .UseSqlServer(cadenaConexion, m => m.MigrationsAssembly(typeof(AplicacionDbContext).Assembly.FullName))
                .Options;

            return new AplicacionDbContext(opciones);
        }

        /// <summary>
        /// Sube desde <paramref name="inicio"/> hasta encontrar una carpeta hermana
        /// "RealEstateApp.WebApp" (busca hasta 6 niveles arriba, suficiente para
        /// llegar desde bin/Debug/netX.0 hasta la raíz de la solución).
        /// </summary>
        private static string? BuscarCarpetaWebApp(string inicio)
        {
            var directorio = new DirectoryInfo(inicio);

            for (int i = 0; i < 6 && directorio != null; i++)
            {
                var candidata = Path.Combine(directorio.FullName, "RealEstateApp.WebApp");
                if (Directory.Exists(candidata))
                {
                    return candidata;
                }

                directorio = directorio.Parent;
            }

            return null;
        }
    }
}