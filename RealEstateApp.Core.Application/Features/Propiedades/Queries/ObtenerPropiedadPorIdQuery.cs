using MediatR;
using RealEstateApp.Core.Application.DTOs;

namespace RealEstateApp.Core.Application.Features.Propiedades.Queries
{
    public class ObtenerPropiedadPorIdQuery : IRequest<PropiedadDto>
    {
        public int Id { get; set; }
    }
}
