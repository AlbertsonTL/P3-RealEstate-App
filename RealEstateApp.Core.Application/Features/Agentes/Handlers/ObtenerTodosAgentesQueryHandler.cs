using MediatR;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.Agentes.Queries;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApp.Core.Application.Features.Agentes.Handlers
{
    public class ObtenerTodosAgentesQueryHandler : IRequestHandler<ObtenerTodosAgentesQuery, IEnumerable<AgenteDto>>
    {
        private readonly IServicioCuenta _servicioCuenta;

        public ObtenerTodosAgentesQueryHandler(IServicioCuenta servicioCuenta)
        {
            _servicioCuenta = servicioCuenta;
        }

        public async Task<IEnumerable<AgenteDto>> Handle(ObtenerTodosAgentesQuery request, CancellationToken cancellationToken)
        {
            return await _servicioCuenta.ObtenerAgentesAsync();
        }
    }
}
