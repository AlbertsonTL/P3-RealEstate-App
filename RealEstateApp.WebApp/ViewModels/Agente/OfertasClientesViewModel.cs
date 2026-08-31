using RealEstateApp.WebApp.ViewModels.Shared;

namespace RealEstateApp.WebApp.ViewModels.Agente;

public class OfertasClientesViewModel
{
    public int PropiedadId { get; set; }
    public List<ClienteResumenViewModel> Clientes { get; set; } = [];
}
