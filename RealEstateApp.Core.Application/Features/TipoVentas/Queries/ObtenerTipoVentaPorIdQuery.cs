using MediatR;
using RealEstateApp.Core.Application.DTOs;

namespace RealEstateApp.Core.Application.Features.TipoVentas.Queries
{
    public class ObtenerTipoVentaPorIdQuery : IRequest<TipoVentaDto>
    {
        public int Id { get; set; }
    }
}
