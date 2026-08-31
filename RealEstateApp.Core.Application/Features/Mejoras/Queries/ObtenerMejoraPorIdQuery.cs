using MediatR;
using RealEstateApp.Core.Application.DTOs;

namespace RealEstateApp.Core.Application.Features.Mejoras.Queries
{
    public class ObtenerMejoraPorIdQuery : IRequest<MejoraDto>
    {
        public int Id { get; set; }
    }
}
