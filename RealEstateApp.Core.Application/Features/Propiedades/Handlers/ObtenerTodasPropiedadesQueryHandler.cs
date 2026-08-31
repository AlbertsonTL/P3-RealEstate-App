using AutoMapper;
using MediatR;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.Propiedades.Queries;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;

namespace RealEstateApp.Core.Application.Features.Propiedades.Handlers
{
    public class ObtenerTodasPropiedadesQueryHandler : IRequestHandler<ObtenerTodasPropiedadesQuery, IEnumerable<PropiedadDto>>
    {
        private readonly IRepositorioPropiedad _repositorio;
        private readonly IMapper _mapper;

        public ObtenerTodasPropiedadesQueryHandler(IRepositorioPropiedad repositorio, IMapper mapper)
        {
            _repositorio = repositorio;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PropiedadDto>> Handle(ObtenerTodasPropiedadesQuery request, CancellationToken cancellationToken)
        {
            var propiedades = await _repositorio.ObtenerTodosAsync();
            return _mapper.Map<IEnumerable<PropiedadDto>>(propiedades);
        }
    }
}
