namespace RealEstateApp.Core.Domain.Entidades
{
    public class ImagenPropiedad : EntidadBase
    {
        public int PropiedadId { get; set; }
        public Propiedad Propiedad { get; set; } = null!;
        
        public string UrlImagen { get; set; } = null!;
        public bool EsPrincipal { get; set; }
    }
}
