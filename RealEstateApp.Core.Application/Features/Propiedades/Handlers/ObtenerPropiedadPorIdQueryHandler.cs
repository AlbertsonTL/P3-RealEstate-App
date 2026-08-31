using AutoMapper;
using MediatR;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.Propiedades.Queries;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;

namespace RealEstateApp.Core.Application.Features.Propiedades.Handlers
{
    public class ObtenerPropiedadPorIdQueryHandler : IRequestHandler<ObtenerPropiedadPorIdQuery, PropiedadDto>
    {
        private readonly IRepositorioPropiedad _repositorio;
        private readonly IMapper _mapper;

        public ObtenerPropiedadPorIdQueryHandler(IRepositorioPropiedad repositorio, IMapper mapper)
        {
            _repositorio = repositorio;
            _mapper = mapper;
        }

        public async Task<PropiedadDto> Handle(ObtenerPropiedadPorIdQuery request, CancellationToken cancellationToken)
        {
            var propiedad = await _repositorio.ObtenerPorIdAsync(request.Id);
            return _mapper.Map<PropiedadDto>(propiedad);
        }
    }
}
