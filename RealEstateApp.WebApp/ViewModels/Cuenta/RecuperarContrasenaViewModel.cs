using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.WebApp.ViewModels.Cuenta;

public class RecuperarContrasenaViewModel
{
    [Required(ErrorMessage = "El correo es requerido.")]
    [EmailAddress(ErrorMessage = "Ingresa un correo válido.")]
    [Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = string.Empty;
}
