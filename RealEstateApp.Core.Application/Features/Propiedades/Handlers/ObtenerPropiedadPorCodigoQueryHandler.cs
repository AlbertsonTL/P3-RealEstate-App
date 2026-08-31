using AutoMapper;
using MediatR;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.Propiedades.Queries;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;

namespace RealEstateApp.Core.Application.Features.Propiedades.Handlers
{
    public class ObtenerPropiedadPorCodigoQueryHandler : IRequestHandler<ObtenerPropiedadPorCodigoQuery, PropiedadDto>
    {
        private readonly IRepositorioPropiedad _repositorio;
        private readonly IMapper _mapper;

        public ObtenerPropiedadPorCodigoQueryHandler(IRepositorioPropiedad repositorio, IMapper mapper)
        {
            _repositorio = repositorio;
            _mapper = mapper;
        }

        public async Task<PropiedadDto> Handle(ObtenerPropiedadPorCodigoQuery request, CancellationToken cancellationToken)
        {
            var propiedad = await _repositorio.ObtenerPorCodigoAsync(request.Codigo);
            return _mapper.Map<PropiedadDto>(propiedad);
        }
    }
}
