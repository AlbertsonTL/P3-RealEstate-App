using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Infrastructure.Shared.Configuracion;
using RealEstateApp.Infrastructure.Shared.Servicios;

namespace RealEstateApp.Infrastructure.Shared.Extensiones
{
    public static class RegistroShared
    {
        public static void AddInfraestructuraShared(this IServiceCollection servicios, IConfiguration config)
        {
            servicios.Configure<EmailSettings>(config.GetSection("EmailSettings"));
            servicios.AddTransient<IServicioEmail, ServicioEmail>();
            servicios.AddTransient<IServicioArchivo, ServicioArchivo>();
        }
    }
}
