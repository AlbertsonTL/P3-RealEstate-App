using MediatR;
using RealEstateApp.Core.Application.DTOs;

namespace RealEstateApp.Core.Application.Features.Mejoras.Queries
{
    public class ObtenerMejorasQuery : IRequest<IEnumerable<MejoraDto>>
    {
    }
}
