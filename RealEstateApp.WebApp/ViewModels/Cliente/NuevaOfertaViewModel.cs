using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.WebApp.ViewModels.Cliente;

public class NuevaOfertaViewModel
{
    [Required(ErrorMessage = "La propiedad es obligatoria.")]
    public int PropiedadId { get; set; }

    [Required(ErrorMessage = "Debes indicar la cifra ofertada.")]
    [Range(1, double.MaxValue, ErrorMessage = "La cifra ofertada debe ser mayor a cero.")]
    public decimal CifraOfertada { get; set; }
}
