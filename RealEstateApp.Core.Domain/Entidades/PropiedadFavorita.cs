namespace RealEstateApp.Core.Domain.Entidades
{
    public class PropiedadFavorita : EntidadBase
    {
        public int PropiedadId { get; set; }
        public Propiedad Propiedad { get; set; } = null!;
        
        public string ClienteId { get; set; } = null!; // FK hacia usuario Identity
    }
}
