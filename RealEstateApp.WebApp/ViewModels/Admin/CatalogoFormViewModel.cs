using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.WebApp.ViewModels.Admin;

public class CatalogoFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public string Descripcion { get; set; } = string.Empty;
}
