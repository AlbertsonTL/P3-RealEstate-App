using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Infrastructure.Identity.Configuracion;
using RealEstateApp.Infrastructure.Identity.Contexto;
using RealEstateApp.Infrastructure.Identity.Entidades;
using RealEstateApp.Infrastructure.Identity.Servicios;
using System.Reflection;
using System.Text;

namespace RealEstateApp.Infrastructure.Identity.Extensiones
{
    public static class RegistroIdentidad
    {
        public static void AddInfraestructuraIdentidad(this IServiceCollection servicios, IConfiguration config)
        {
            servicios.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
            servicios.Configure<JwtSettings>(config.GetSection("JwtSettings"));

            servicios.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"),
                m => m.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName)));

            servicios.AddIdentity<UsuarioAplicacion, IdentityRole>(options =>
            {
                // Impedir que se registren dos cuentas con el mismo correo
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            servicios.AddAuthentication()
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = config["JwtSettings:Issuer"],
                    ValidAudience = config["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:Key"]!))
                };
            });

            servicios.AddTransient<IServicioCuenta, ServicioCuenta>();
            servicios.AddTransient<ServicioJwt>();
        }
    }
}
