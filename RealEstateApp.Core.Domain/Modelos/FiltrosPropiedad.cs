namespace RealEstateApp.Core.Domain.Modelos
{
    public class FiltrosPropiedad
    {
        public string? CodigoBusqueda { get; set; }
        public int? TipoPropiedadId { get; set; }
        public decimal? PrecioMinimo { get; set; }
        public decimal? PrecioMaximo { get; set; }
        public int? CantidadHabitaciones { get; set; }
        public int? CantidadBanos { get; set; }
    }
}
