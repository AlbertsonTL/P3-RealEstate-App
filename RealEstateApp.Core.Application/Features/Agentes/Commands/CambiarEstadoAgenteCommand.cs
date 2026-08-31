using MediatR;

namespace RealEstateApp.Core.Application.Features.Agentes.Commands
{
    public class CambiarEstadoAgenteCommand : IRequest<bool>
    {
        public string Id { get; set; } = null!;
        public bool Estado { get; set; }
    }
}
