using RealEstateApp.Core.Domain.Enumeraciones;

namespace RealEstateApp.WebApp.ViewModels.Shared;

public class OfertaViewModel
{
    public int Id { get; set; }
    public decimal CifraOfertada { get; set; }
    public DateTime FechaOferta { get; set; }
    public EstadoOferta Estado { get; set; }
}
