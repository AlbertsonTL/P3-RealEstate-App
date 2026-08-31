using RealEstateApp.WebApp.ViewModels.Shared;

namespace RealEstateApp.WebApp.ViewModels.Agente;

public class OfertasDeClienteViewModel
{
    public int PropiedadId { get; set; }
    public string ClienteId { get; set; } = string.Empty;
    public List<OfertaViewModel> Ofertas { get; set; } = [];
}
