using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.WebApp.ViewModels.Cuenta;

public class RegistroViewModel
{
    [Required] public string Nombre { get; set; } = string.Empty;
    [Required] public string Apellido { get; set; } = string.Empty;
    [Required] public string Telefono { get; set; } = string.Empty;
    [Required] public string NombreUsuario { get; set; } = string.Empty;
    [Required, EmailAddress] public string Correo { get; set; } = string.Empty;
    [Required, DataType(DataType.Password)] public string Contrasena { get; set; } = string.Empty;
    [Required, DataType(DataType.Password), Compare(nameof(Contrasena))]
    public string ConfirmarContrasena { get; set; } = string.Empty;
    public IFormFile? FotoUsuario { get; set; }
    [Required] public string TipoUsuario { get; set; } = string.Empty;
}
