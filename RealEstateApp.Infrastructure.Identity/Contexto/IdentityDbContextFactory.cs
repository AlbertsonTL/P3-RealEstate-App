using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace RealEstateApp.Infrastructure.Identity.Contexto
{
    /// <summary>
    /// Fábrica de DbContext en tiempo de diseño para Identity.
    /// </summary>
    public class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
    {
        public IdentityDbContext CreateDbContext(string[] args)
        {
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
                cadenaConexion = "Server=(localdb)\\mssqllocaldb;Database=RealEstateApp-DB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";
            }

            var opciones = new DbContextOptionsBuilder<IdentityDbContext>()
                .UseSqlServer(cadenaConexion, m => m.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName))
                .Options;

            return new IdentityDbContext(opciones);
        }

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