using System;
using RealEstateApp.Core.Domain.Enumeraciones;

namespace RealEstateApp.Core.Domain.Entidades
{
    public class Oferta : EntidadBase
    {
        public int PropiedadId { get; set; }
        public Propiedad Propiedad { get; set; } = null!;
        
        public string ClienteId { get; set; } = null!; // FK hacia usuario Identity
        public decimal CifraOfertada { get; set; }
        public DateTime FechaOferta { get; set; }
        public EstadoOferta Estado { get; set; }
    }
}
