using AutoMapper;
using MediatR;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.TipoPropiedades.Queries;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;

namespace RealEstateApp.Core.Application.Features.TipoPropiedades.Handlers
{
    public class ObtenerTiposPropiedadesQueryHandler : IRequestHandler<ObtenerTiposPropiedadesQuery, IEnumerable<TipoPropiedadDto>>
    {
        private readonly IRepositorioGenerico<TipoPropiedad> _repositorio;
        private readonly IMapper _mapper;

        public ObtenerTiposPropiedadesQueryHandler(IRepositorioGenerico<TipoPropiedad> repositorio, IMapper mapper)
        {
            _repositorio = repositorio;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TipoPropiedadDto>> Handle(ObtenerTiposPropiedadesQuery request, CancellationToken cancellationToken)
        {
            var tipos = await _repositorio.ObtenerTodosAsync();
            return _mapper.Map<IEnumerable<TipoPropiedadDto>>(tipos);
        }
    }
}
