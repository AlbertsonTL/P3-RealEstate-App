using RealEstateApp.WebApp.ViewModels.Shared;

namespace RealEstateApp.WebApp.Interfaces.Servicios;

public interface IServicioChat
{
    Task<List<ChatMensajeViewModel>> ObtenerConversacionAsync(int propiedadId, string clienteId, string agenteId, string usuarioActualId);
    Task EnviarMensajeAsync(int propiedadId, string remitenteId, string destinatarioId, string contenido);
    Task<List<string>> ObtenerClientesConMensajesAsync(int propiedadId, string agenteId);
}
