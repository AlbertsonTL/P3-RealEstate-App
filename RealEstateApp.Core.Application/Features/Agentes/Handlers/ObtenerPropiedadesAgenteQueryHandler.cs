using AutoMapper;
using MediatR;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.Agentes.Queries;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;

namespace RealEstateApp.Core.Application.Features.Agentes.Handlers
{
    public class ObtenerPropiedadesAgenteQueryHandler : IRequestHandler<ObtenerPropiedadesAgenteQuery, IEnumerable<PropiedadDto>>
    {
        private readonly IRepositorioPropiedad _repositorio;
        private readonly IMapper _mapper;

        public ObtenerPropiedadesAgenteQueryHandler(IRepositorioPropiedad repositorio, IMapper mapper)
        {
            _repositorio = repositorio;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PropiedadDto>> Handle(ObtenerPropiedadesAgenteQuery request, CancellationToken cancellationToken)
        {
            var propiedades = await _repositorio.ObtenerPorAgenteAsync(request.AgenteId);
            return _mapper.Map<IEnumerable<PropiedadDto>>(propiedades);
        }
    }
}
