using MediatR;
using RealEstateApp.Core.Application.DTOs;

namespace RealEstateApp.Core.Application.Features.Agentes.Queries
{
    public class ObtenerPropiedadesAgenteQuery : IRequest<IEnumerable<PropiedadDto>>
    {
        public string AgenteId { get; set; } = null!;
    }
}
