using RealEstateApp.WebApp.ViewModels.Shared;

namespace RealEstateApp.WebApp.ViewModels.Agente;

public class ChatAgenteViewModel
{
    public int PropiedadId { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string ClienteId { get; set; } = string.Empty;
    public List<ChatMensajeViewModel> Mensajes { get; set; } = [];
}
