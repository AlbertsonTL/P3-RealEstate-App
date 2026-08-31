using MediatR;
using RealEstateApp.Core.Application.DTOs;

namespace RealEstateApp.Core.Application.Features.Agentes.Queries
{
    public class ObtenerAgentePorIdQuery : IRequest<AgenteDto?>
    {
        public string Id { get; set; } = null!;
    }
}
