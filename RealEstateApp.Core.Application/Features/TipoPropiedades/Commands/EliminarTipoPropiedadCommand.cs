using MediatR;

namespace RealEstateApp.Core.Application.Features.TipoPropiedades.Commands
{
    public class EliminarTipoPropiedadCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
