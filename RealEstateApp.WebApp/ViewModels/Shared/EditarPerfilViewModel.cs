using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.WebApp.ViewModels.Shared;

/// <summary>
/// ViewModel compartido para editar el perfil de cualquier tipo de usuario
/// (Cliente, Agente, Administrador, Desarrollador).
/// </summary>
public class EditarPerfilViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El apellido no puede superar 100 caracteres.")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [Phone(ErrorMessage = "Formato de teléfono no válido.")]
    [MaxLength(20, ErrorMessage = "El teléfono no puede superar 20 caracteres.")]
    public string Telefono { get; set; } = string.Empty;

    /// <summary>Correo electrónico del usuario. Obligatorio y debe ser válido.</summary>
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    [MaxLength(256, ErrorMessage = "El correo no puede superar 256 caracteres.")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Archivo de imagen subido por el usuario (opcional).</summary>
    public IFormFile? FotoUsuario { get; set; }

    /// <summary>URL de la foto actual guardada en base de datos (solo lectura en el formulario).</summary>
    public string? UrlFotoActual { get; set; }
}
