using MediatR;
using RealEstateApp.Core.Application.DTOs;

namespace RealEstateApp.Core.Application.Features.TipoPropiedades.Commands
{
    public class CrearTipoPropiedadCommand : IRequest<TipoPropiedadDto>
    {
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
    }
}
