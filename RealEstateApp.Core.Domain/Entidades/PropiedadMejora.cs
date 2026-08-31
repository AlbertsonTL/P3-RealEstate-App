namespace RealEstateApp.Core.Domain.Entidades
{
    public class PropiedadMejora : EntidadBase
    {
        public int PropiedadId { get; set; }
        public Propiedad Propiedad { get; set; } = null!;
        
        public int MejoraId { get; set; }
        public Mejora Mejora { get; set; } = null!;
    }
}
