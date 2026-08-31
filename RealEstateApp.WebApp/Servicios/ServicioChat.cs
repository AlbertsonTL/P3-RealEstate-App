using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;
using RealEstateApp.WebApp.Interfaces.Servicios;
using RealEstateApp.WebApp.ViewModels.Shared;

namespace RealEstateApp.WebApp.Servicios;

public class ServicioChat : IServicioChat
{
    private readonly IRepositorioChatMensaje _repositorioChat;

    public ServicioChat(IRepositorioChatMensaje repositorioChat)
    {
        _repositorioChat = repositorioChat;
    }

    public async Task<List<ChatMensajeViewModel>> ObtenerConversacionAsync(int propiedadId, string clienteId, string agenteId, string usuarioActualId)
    {
        var mensajes = await _repositorioChat.ObtenerConversacionAsync(propiedadId, clienteId, agenteId);
        return mensajes.Select(m => new ChatMensajeViewModel
        {
            RemitenteId = m.RemitenteId,
            NombreRemitente = m.RemitenteId == agenteId ? "Agente" : "Cliente",
            Contenido = m.Contenido,
            FechaEnvio = m.FechaEnvio,
            EsMio = m.RemitenteId == usuarioActualId
        }).ToList();
    }

    public async Task EnviarMensajeAsync(int propiedadId, string remitenteId, string destinatarioId, string contenido)
    {
        await _repositorioChat.AgregarAsync(new ChatMensaje
        {
            PropiedadId = propiedadId,
            RemitenteId = remitenteId,
            DestinatarioId = destinatarioId,
            Contenido = contenido,
            FechaEnvio = DateTime.UtcNow
        });
    }

    public async Task<List<string>> ObtenerClientesConMensajesAsync(int propiedadId, string agenteId)
    {
        var clientes = await _repositorioChat.ObtenerClientesConMensajesAsync(propiedadId, agenteId);
        return clientes.ToList();
    }
}
