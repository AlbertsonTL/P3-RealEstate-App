using RealEstateApp.Core.Domain.Enumeraciones;

namespace RealEstateApp.WebApp.ViewModels.Publico;

public class PropiedadResumenViewModel
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public int TipoPropiedadId { get; set; }
    public string TipoPropiedad { get; set; } = string.Empty;
    public string TipoVenta { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int CantidadHabitaciones { get; set; }
    public int CantidadBanos { get; set; }
    public decimal TamañoMetros { get; set; }
    public string UrlImagenPrincipal { get; set; } = "/images/placeholder-property.jpg";
    public EstadoPropiedad Estado { get; set; }
    public bool EsFavorita { get; set; }
}
