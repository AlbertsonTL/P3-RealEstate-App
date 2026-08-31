using RealEstateApp.WebApp.ViewModels.Publico;

namespace RealEstateApp.WebApp.ViewModels.Agente;

public class DetalleAgenteConPropiedadesViewModel
{
    public string AgenteId      { get; set; } = string.Empty;
    public string NombreAgente  { get; set; } = string.Empty;
    public string UrlFotoAgente { get; set; } = "/images/placeholder-agent.jpg";
    public List<PropiedadResumenViewModel> Propiedades { get; set; } = [];
    // FIX: filtros para cumplir requerimiento de filtros en todas las pantallas de listado
    public FiltrosPropiedadViewModel Filtros { get; set; } = new();
}
