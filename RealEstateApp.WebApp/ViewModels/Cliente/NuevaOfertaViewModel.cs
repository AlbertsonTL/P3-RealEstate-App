using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.WebApp.ViewModels.Cliente;

public class NuevaOfertaViewModel
{
    [Required]
    public int PropiedadId { get; set; }

    [Required, Range(1, double.MaxValue)]
    public decimal CifraOfertada { get; set; }
}
