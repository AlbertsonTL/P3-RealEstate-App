using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.WebApp.ViewModels.Cuenta;

public class LoginViewModel
{
    [Required]
    public string UsuarioOCorreo { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Contrasena { get; set; } = string.Empty;

    public bool RecordarSesion { get; set; }
}
