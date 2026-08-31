namespace RealEstateApp.Core.Application.DTOs
{
    public class AgenteDto
    {
        public string Id { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public int CantidadPropiedades { get; set; }
    }
}
