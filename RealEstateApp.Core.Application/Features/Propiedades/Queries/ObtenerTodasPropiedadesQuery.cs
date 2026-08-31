using MediatR;
using RealEstateApp.Core.Application.DTOs;

namespace RealEstateApp.Core.Application.Features.Propiedades.Queries
{
    public class ObtenerTodasPropiedadesQuery : IRequest<IEnumerable<PropiedadDto>>
    {
    }
}
