namespace RealEstateApp.WebApp.ViewModels.Publico;

public class HomeViewModel
{
    public List<PropiedadResumenViewModel> Propiedades { get; set; } = [];
    public FiltrosPropiedadViewModel Filtros { get; set; } = new();
}
