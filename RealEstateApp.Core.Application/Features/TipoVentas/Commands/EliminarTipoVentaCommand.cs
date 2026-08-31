using MediatR;

namespace RealEstateApp.Core.Application.Features.TipoVentas.Commands
{
    public class EliminarTipoVentaCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
