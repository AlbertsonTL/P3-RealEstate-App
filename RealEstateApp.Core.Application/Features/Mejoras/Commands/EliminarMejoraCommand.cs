using MediatR;

namespace RealEstateApp.Core.Application.Features.Mejoras.Commands
{
    public class EliminarMejoraCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
