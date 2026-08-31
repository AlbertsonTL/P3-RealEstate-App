namespace RealEstateApp.Core.Application.DTOs.Cuenta
{
    public class RegistrarAdminDto
    {
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Cedula { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string NombreUsuario { get; set; } = null!;
        public string Contrasena { get; set; } = null!;
        public string ConfirmarContrasena { get; set; } = null!;
    }
}
