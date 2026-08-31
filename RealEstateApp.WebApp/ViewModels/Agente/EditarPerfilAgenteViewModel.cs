using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.WebApp.ViewModels.Agente;

public class EditarPerfilAgenteViewModel
{
    [Required] public string Nombre { get; set; } = string.Empty;
    [Required] public string Apellido { get; set; } = string.Empty;
    [Required] public string Telefono { get; set; } = string.Empty;
    public IFormFile? FotoUsuario { get; set; }
}
