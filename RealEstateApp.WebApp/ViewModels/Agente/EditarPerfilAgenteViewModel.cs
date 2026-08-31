using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.WebApp.ViewModels.Agente;

public class EditarPerfilAgenteViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    public string Telefono { get; set; } = string.Empty;

    /// <summary>Imagen de perfil opcional.</summary>
    public IFormFile? FotoUsuario { get; set; }
}
