using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RealEstateApp.WebApp.ViewModels.Agente;

public class CrearPropiedadViewModel
{
    [Required] public int TipoPropiedadId { get; set; }
    [Required] public int TipoVentaId { get; set; }
    [Required, Range(1, double.MaxValue)] public decimal Precio { get; set; }
    [Required] public string Descripcion { get; set; } = string.Empty;
    [Required, Range(1, double.MaxValue)] public decimal TamañoMetros { get; set; }
    [Required, Range(0, 50)] public int CantidadHabitaciones { get; set; }
    [Required, Range(0, 20)] public int CantidadBanos { get; set; }
    [Required] public List<int> MejorasSeleccionadas { get; set; } = [];
    public List<IFormFile?> Imagenes { get; set; } = [];
    public List<SelectListItem> TiposPropiedad { get; set; } = [];
    public List<SelectListItem> TiposVenta { get; set; } = [];
    public List<SelectListItem> MejorasDisponibles { get; set; } = [];
}
