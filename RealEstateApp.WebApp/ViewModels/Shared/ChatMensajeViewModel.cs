using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.WebApp.ViewModels.Shared;

public class ChatMensajeViewModel
{
    public string RemitenteId { get; set; } = string.Empty;
    public string NombreRemitente { get; set; } = string.Empty;

    [Required]
    public string Contenido { get; set; } = string.Empty;

    public DateTime FechaEnvio { get; set; }
    public bool EsMio { get; set; }
}
