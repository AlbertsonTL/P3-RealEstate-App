using MediatR;
using RealEstateApp.Core.Application.DTOs;

namespace RealEstateApp.Core.Application.Features.TipoPropiedades.Queries
{
    public class ObtenerTipoPropiedadPorIdQuery : IRequest<TipoPropiedadDto>
    {
        public int Id { get; set; }
    }
}
