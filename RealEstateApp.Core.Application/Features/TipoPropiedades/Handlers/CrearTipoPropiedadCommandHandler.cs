using AutoMapper;
using MediatR;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.TipoPropiedades.Commands;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;

namespace RealEstateApp.Core.Application.Features.TipoPropiedades.Handlers
{
    public class CrearTipoPropiedadCommandHandler : IRequestHandler<CrearTipoPropiedadCommand, TipoPropiedadDto>
    {
        private readonly IRepositorioGenerico<TipoPropiedad> _repositorio;
        private readonly IMapper _mapper;

        public CrearTipoPropiedadCommandHandler(IRepositorioGenerico<TipoPropiedad> repositorio, IMapper mapper)
        {
            _repositorio = repositorio;
            _mapper = mapper;
        }

        public async Task<TipoPropiedadDto> Handle(CrearTipoPropiedadCommand request, CancellationToken cancellationToken)
        {
            var tipo = _mapper.Map<TipoPropiedad>(request);
            tipo = await _repositorio.AgregarAsync(tipo);
            return _mapper.Map<TipoPropiedadDto>(tipo);
        }
    }
}
