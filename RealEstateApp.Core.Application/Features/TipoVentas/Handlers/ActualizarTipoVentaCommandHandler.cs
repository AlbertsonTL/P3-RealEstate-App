using AutoMapper;
using MediatR;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.TipoVentas.Commands;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;

namespace RealEstateApp.Core.Application.Features.TipoVentas.Handlers
{
    public class ActualizarTipoVentaCommandHandler : IRequestHandler<ActualizarTipoVentaCommand, TipoVentaDto>
    {
        private readonly IRepositorioGenerico<TipoVenta> _repositorio;
        private readonly IMapper _mapper;

        public ActualizarTipoVentaCommandHandler(IRepositorioGenerico<TipoVenta> repositorio, IMapper mapper)
        {
            _repositorio = repositorio;
            _mapper = mapper;
        }

        public async Task<TipoVentaDto> Handle(ActualizarTipoVentaCommand request, CancellationToken cancellationToken)
        {
            var tipo = await _repositorio.ObtenerPorIdAsync(request.Id);
            if (tipo == null) return null!;

            tipo.Nombre = request.Nombre;
            tipo.Descripcion = request.Descripcion;

            await _repositorio.ActualizarAsync(tipo);
            return _mapper.Map<TipoVentaDto>(tipo);
        }
    }
}
