using System;

namespace RealEstateApp.Core.Domain.Entidades
{
    public class ChatMensaje : EntidadBase
    {
        public int PropiedadId { get; set; }
        public Propiedad Propiedad { get; set; } = null!;
        
        public string RemitenteId { get; set; } = null!;    // FK hacia usuario Identity
        public string DestinatarioId { get; set; } = null!; // FK hacia usuario Identity
        public string Contenido { get; set; } = null!;
        public DateTime FechaEnvio { get; set; }
    }
}
