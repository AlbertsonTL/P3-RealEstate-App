namespace RealEstateApp.Core.Application.DTOs.Cuenta
{
    public class RespuestaAutenticacion
    {
        public string Id { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime Expiracion { get; set; }
        public List<string> Roles { get; set; } = new();
        public bool TieneError { get; set; }
        public string? MensajeError { get; set; }
    }
}
