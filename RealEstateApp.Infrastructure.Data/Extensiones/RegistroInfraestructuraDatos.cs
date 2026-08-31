using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;
using RealEstateApp.Infrastructure.Data.Contexto;
using RealEstateApp.Infrastructure.Data.Repositorios;

namespace RealEstateApp.Infrastructure.Data.Extensiones
{
    public static class RegistroInfraestructuraDatos
    {
        public static void AddInfraestructuraDatos(this IServiceCollection servicios, IConfiguration config)
        {
            // Registro del DbContext con SQL Server
            servicios.AddDbContext<AplicacionDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"),
                    m => m.MigrationsAssembly(typeof(AplicacionDbContext).Assembly.FullName)));

            // Registro de Repositorios (Scoped)
            servicios.AddScoped(typeof(IRepositorioGenerico<>), typeof(RepositorioGenerico<>));
            servicios.AddScoped<IRepositorioPropiedad, RepositorioPropiedad>();
            servicios.AddScoped<IRepositorioOferta, RepositorioOferta>();
            servicios.AddScoped<IRepositorioChatMensaje, RepositorioChatMensaje>();
            servicios.AddScoped<IRepositorioPropiedadFavorita, RepositorioPropiedadFavorita>();
        }
    }
}
