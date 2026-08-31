using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.WebApp.ViewModels.Cuenta;

public class LoginViewModel
{
    [Required(ErrorMessage = "Debes ingresar tu usuario o correo electrónico.")]
    public string UsuarioOCorreo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    public string Contrasena { get; set; } = string.Empty;

    public bool RecordarSesion { get; set; }
}
