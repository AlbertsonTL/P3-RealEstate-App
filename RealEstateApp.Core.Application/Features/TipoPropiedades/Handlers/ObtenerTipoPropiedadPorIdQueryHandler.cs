using AutoMapper;
using MediatR;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.TipoPropiedades.Queries;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;

namespace RealEstateApp.Core.Application.Features.TipoPropiedades.Handlers
{
    public class ObtenerTipoPropiedadPorIdQueryHandler : IRequestHandler<ObtenerTipoPropiedadPorIdQuery, TipoPropiedadDto>
    {
        private readonly IRepositorioGenerico<TipoPropiedad> _repositorio;
        private readonly IMapper _mapper;

        public ObtenerTipoPropiedadPorIdQueryHandler(IRepositorioGenerico<TipoPropiedad> repositorio, IMapper mapper)
        {
            _repositorio = repositorio;
            _mapper = mapper;
        }

        public async Task<TipoPropiedadDto> Handle(ObtenerTipoPropiedadPorIdQuery request, CancellationToken cancellationToken)
        {
            var tipo = await _repositorio.ObtenerPorIdAsync(request.Id);
            return _mapper.Map<TipoPropiedadDto>(tipo);
        }
    }
}
