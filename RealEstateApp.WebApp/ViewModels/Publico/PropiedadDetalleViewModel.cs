using RealEstateApp.Core.Domain.Enumeraciones;
using RealEstateApp.WebApp.ViewModels.Shared;

namespace RealEstateApp.WebApp.ViewModels.Publico;

public class PropiedadDetalleViewModel
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string TipoPropiedad { get; set; } = string.Empty;
    public string TipoVenta { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int CantidadHabitaciones { get; set; }
    public int CantidadBanos { get; set; }
    public decimal TamañoMetros { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public List<string> UrlsImagenes { get; set; } = [];
    public List<string> NombresMejoras { get; set; } = [];
    public string NombreAgente { get; set; } = string.Empty;
    public string TelefonoAgente { get; set; } = string.Empty;
    public string EmailAgente { get; set; } = string.Empty;
    public string UrlFotoAgente { get; set; } = "/images/placeholder-agent.jpg";
    public string AgenteId { get; set; } = string.Empty;
    public EstadoPropiedad Estado { get; set; }
    public bool EsFavorita { get; set; }
    public bool PuedeHacerOferta { get; set; }
    public bool EsPropietarioAgente { get; set; }
    public List<OfertaViewModel> Ofertas { get; set; } = [];
    public List<ChatMensajeViewModel> Mensajes { get; set; } = [];
}
