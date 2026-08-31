using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RealEstateApp.WebApp.ViewModels.Agente;

public class CrearPropiedadViewModel
{
    [Required(ErrorMessage = "Debes seleccionar un tipo de propiedad.")]
    public int TipoPropiedadId { get; set; }

    [Required(ErrorMessage = "Debes seleccionar un tipo de venta.")]
    public int TipoVentaId { get; set; }

    [Required(ErrorMessage = "El precio es obligatorio.")]
    [Range(1, double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero.")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tamaño en metros es obligatorio.")]
    [Range(1, double.MaxValue, ErrorMessage = "El tamaño debe ser mayor a cero.")]
    public decimal TamañoMetros { get; set; }

    [Required(ErrorMessage = "La cantidad de habitaciones es obligatoria.")]
    [Range(0, 50, ErrorMessage = "La cantidad de habitaciones debe estar entre 0 y 50.")]
    public int CantidadHabitaciones { get; set; }

    [Required(ErrorMessage = "La cantidad de baños es obligatoria.")]
    [Range(0, 20, ErrorMessage = "La cantidad de baños debe estar entre 0 y 20.")]
    public int CantidadBanos { get; set; }

    [Required(ErrorMessage = "Debes seleccionar al menos una mejora.")]
    public List<int> MejorasSeleccionadas { get; set; } = [];

    public List<IFormFile?> Imagenes { get; set; } = [];
    public List<SelectListItem> TiposPropiedad { get; set; } = [];
    public List<SelectListItem> TiposVenta { get; set; } = [];
    public List<SelectListItem> MejorasDisponibles { get; set; } = [];
}
