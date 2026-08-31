using RealEstateApp.WebApp.ViewModels.Shared;

namespace RealEstateApp.WebApp.ViewModels.Agente;

public class ChatClientesViewModel
{
    public int PropiedadId { get; set; }
    public List<ClienteResumenViewModel> Clientes { get; set; } = [];
}
