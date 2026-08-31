using AutoMapper;
using MediatR;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.TipoPropiedades.Commands;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;

namespace RealEstateApp.Core.Application.Features.TipoPropiedades.Handlers
{
    public class ActualizarTipoPropiedadCommandHandler : IRequestHandler<ActualizarTipoPropiedadCommand, TipoPropiedadDto>
    {
        private readonly IRepositorioGenerico<TipoPropiedad> _repositorio;
        private readonly IMapper _mapper;

        public ActualizarTipoPropiedadCommandHandler(IRepositorioGenerico<TipoPropiedad> repositorio, IMapper mapper)
        {
            _repositorio = repositorio;
            _mapper = mapper;
        }

        public async Task<TipoPropiedadDto> Handle(ActualizarTipoPropiedadCommand request, CancellationToken cancellationToken)
        {
            var tipo = await _repositorio.ObtenerPorIdAsync(request.Id);
            if (tipo == null) return null!;

            tipo.Nombre = request.Nombre;
            tipo.Descripcion = request.Descripcion;

            await _repositorio.ActualizarAsync(tipo);
            return _mapper.Map<TipoPropiedadDto>(tipo);
        }
    }
}
