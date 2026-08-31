namespace RealEstateApp.Core.Application.DTOs
{
    public class PropiedadDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public int TipoPropiedadId { get; set; }
        public string TipoPropiedad { get; set; } = null!;
        public int TipoVentaId { get; set; }
        public string TipoVenta { get; set; } = null!;
        public decimal Precio { get; set; }
        public decimal TamañoMetros { get; set; }
        public int CantidadHabitaciones { get; set; }
        public int CantidadBanos { get; set; }
        public string Descripcion { get; set; } = null!;
        public List<string> UrlsImagenes { get; set; } = new();
        public List<string> Mejoras { get; set; } = new();
        public string NombreAgente { get; set; } = null!;
        public string IdAgente { get; set; } = null!;
        public string EstadoPropiedad { get; set; } = null!;
    }
}
