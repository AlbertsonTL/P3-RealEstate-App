using System.Collections.Generic;

namespace RealEstateApp.Core.Domain.Entidades
{
    public class TipoPropiedad : EntidadBase
    {
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;

        // Relación
        public ICollection<Propiedad> Propiedades { get; set; } = new List<Propiedad>();
    }
}
