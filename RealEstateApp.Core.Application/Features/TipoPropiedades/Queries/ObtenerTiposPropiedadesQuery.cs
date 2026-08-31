using MediatR;
using RealEstateApp.Core.Application.DTOs;

namespace RealEstateApp.Core.Application.Features.TipoPropiedades.Queries
{
    public class ObtenerTiposPropiedadesQuery : IRequest<IEnumerable<TipoPropiedadDto>>
    {
    }
}
