using System;
using System.Collections.Generic;
using RealEstateApp.Core.Domain.Enumeraciones;

namespace RealEstateApp.Core.Domain.Entidades
{
    public class Propiedad : EntidadBase
    {
        public string Codigo { get; set; } = null!; // 6 dígitos únicos, generado automático
        
        public int TipoPropiedadId { get; set; }
        public TipoPropiedad TipoPropiedad { get; set; } = null!;
        
        public int TipoVentaId { get; set; }
        public TipoVenta TipoVenta { get; set; } = null!;
        
        public decimal Precio { get; set; }
        public string Descripcion { get; set; } = null!;
        public decimal TamañoMetros { get; set; }
        public int CantidadHabitaciones { get; set; }
        public int CantidadBanos { get; set; }
        public EstadoPropiedad Estado { get; set; }
        
        public string AgenteId { get; set; } = null!; // FK hacia usuario Identity
        
        public DateTime FechaCreacion { get; set; }

        // Relaciones
        public ICollection<ImagenPropiedad> Imagenes { get; set; } = new List<ImagenPropiedad>();
        public ICollection<PropiedadMejora> PropiedadesMejoras { get; set; } = new List<PropiedadMejora>();
        public ICollection<Oferta> Ofertas { get; set; } = new List<Oferta>();
        public ICollection<ChatMensaje> Mensajes { get; set; } = new List<ChatMensaje>();
        public ICollection<PropiedadFavorita> Favoritas { get; set; } = new List<PropiedadFavorita>();
    }
}
