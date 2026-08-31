using MediatR;
using RealEstateApp.Core.Application.DTOs;

namespace RealEstateApp.Core.Application.Features.Propiedades.Queries
{
    public class ObtenerPropiedadPorCodigoQuery : IRequest<PropiedadDto>
    {
        public string Codigo { get; set; } = null!;
    }
}
