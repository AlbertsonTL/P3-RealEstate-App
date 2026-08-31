using MediatR;
using RealEstateApp.Core.Application.DTOs;

namespace RealEstateApp.Core.Application.Features.Agentes.Queries
{
    public class ObtenerTodosAgentesQuery : IRequest<IEnumerable<AgenteDto>>
    {
    }
}
