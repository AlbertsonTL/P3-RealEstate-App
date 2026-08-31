using System.Collections.Generic;

namespace RealEstateApp.Core.Domain.Entidades
{
    public class Mejora : EntidadBase
    {
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;

        // Relación
        public ICollection<PropiedadMejora> PropiedadesMejoras { get; set; } = new List<PropiedadMejora>();
    }
}
