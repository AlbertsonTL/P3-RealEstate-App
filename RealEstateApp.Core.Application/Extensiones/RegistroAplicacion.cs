using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace RealEstateApp.Core.Application.Extensiones
{
    public static class RegistroAplicacion
    {
        public static void AddAplicacion(this IServiceCollection servicios)
        {
            servicios.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
            servicios.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(Behaviors.ValidacionBehavior<,>));
            });
            servicios.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
