using AutoMapper;
using MediatR;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.TipoVentas.Commands;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;

namespace RealEstateApp.Core.Application.Features.TipoVentas.Handlers
{
    public class CrearTipoVentaCommandHandler : IRequestHandler<CrearTipoVentaCommand, TipoVentaDto>
    {
        private readonly IRepositorioGenerico<TipoVenta> _repositorio;
        private readonly IMapper _mapper;

        public CrearTipoVentaCommandHandler(IRepositorioGenerico<TipoVenta> repositorio, IMapper mapper)
        {
            _repositorio = repositorio;
            _mapper = mapper;
        }

        public async Task<TipoVentaDto> Handle(CrearTipoVentaCommand request, CancellationToken cancellationToken)
        {
            var tipo = _mapper.Map<TipoVenta>(request);
            tipo = await _repositorio.AgregarAsync(tipo);
            return _mapper.Map<TipoVentaDto>(tipo);
        }
    }
}
