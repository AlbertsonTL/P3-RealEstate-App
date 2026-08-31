using RealEstateApp.WebApp.ViewModels.Shared;

namespace RealEstateApp.WebApp.ViewModels.Agente;

public class ChatConClienteViewModel
{
    public int PropiedadId { get; set; }
    public string ClienteId { get; set; } = string.Empty;
    public List<ChatMensajeViewModel> Mensajes { get; set; } = [];
    public string Contenido { get; set; } = string.Empty;
}
