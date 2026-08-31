using MediatR;
using RealEstateApp.Core.Application.DTOs;

namespace RealEstateApp.Core.Application.Features.TipoPropiedades.Commands
{
    public class ActualizarTipoPropiedadCommand : IRequest<TipoPropiedadDto>
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
    }
}
