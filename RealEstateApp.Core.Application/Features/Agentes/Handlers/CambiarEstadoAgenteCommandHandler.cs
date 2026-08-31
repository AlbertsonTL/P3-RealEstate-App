using MediatR;
using RealEstateApp.Core.Application.Features.Agentes.Commands;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApp.Core.Application.Features.Agentes.Handlers
{
    public class CambiarEstadoAgenteCommandHandler : IRequestHandler<CambiarEstadoAgenteCommand, bool>
    {
        private readonly IServicioCuenta _servicioCuenta;

        public CambiarEstadoAgenteCommandHandler(IServicioCuenta servicioCuenta)
        {
            _servicioCuenta = servicioCuenta;
        }

        public async Task<bool> Handle(CambiarEstadoAgenteCommand request, CancellationToken cancellationToken)
        {
            return await _servicioCuenta.CambiarEstadoUsuarioAsync(request.Id, request.Estado);
        }
    }
}
