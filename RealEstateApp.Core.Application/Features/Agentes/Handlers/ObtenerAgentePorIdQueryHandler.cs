using MediatR;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.Agentes.Queries;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApp.Core.Application.Features.Agentes.Handlers
{
    public class ObtenerAgentePorIdQueryHandler : IRequestHandler<ObtenerAgentePorIdQuery, AgenteDto?>
    {
        private readonly IServicioCuenta _servicioCuenta;

        public ObtenerAgentePorIdQueryHandler(IServicioCuenta servicioCuenta)
        {
            _servicioCuenta = servicioCuenta;
        }

        public async Task<AgenteDto?> Handle(ObtenerAgentePorIdQuery request, CancellationToken cancellationToken)
        {
            return await _servicioCuenta.ObtenerAgentePorIdAsync(request.Id);
        }
    }
}
