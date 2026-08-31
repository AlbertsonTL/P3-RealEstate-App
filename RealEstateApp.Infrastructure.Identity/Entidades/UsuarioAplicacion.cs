using Microsoft.AspNetCore.Identity;

namespace RealEstateApp.Infrastructure.Identity.Entidades
{
    public class UsuarioAplicacion : IdentityUser
    {
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string? UrlFoto { get; set; }
        public string? Cedula { get; set; }
        public bool EstaActivo { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
