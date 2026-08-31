using System.ComponentModel.DataAnnotations;
using RealEstateApp.WebApp.ViewModels.Shared;

namespace RealEstateApp.WebApp.ViewModels.Cliente;

public class ChatClienteViewModel
{
    public int PropiedadId { get; set; }
    public string AgenteId { get; set; } = string.Empty;
    public List<ChatMensajeViewModel> Mensajes { get; set; } = [];

    [Required(ErrorMessage = "Debes escribir un mensaje.")]
    public string NuevoMensaje { get; set; } = string.Empty;
}
