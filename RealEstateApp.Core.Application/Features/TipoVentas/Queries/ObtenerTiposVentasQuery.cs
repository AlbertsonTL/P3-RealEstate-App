using MediatR;
using RealEstateApp.Core.Application.DTOs;

namespace RealEstateApp.Core.Application.Features.TipoVentas.Queries
{
    public class ObtenerTiposVentasQuery : IRequest<IEnumerable<TipoVentaDto>>
    {
    }
}
