using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.WebApp.ViewModels.Admin;

public class CatalogoFormViewModel
{
    public int Id { get; set; }

    [Required]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public string Descripcion { get; set; } = string.Empty;
}
