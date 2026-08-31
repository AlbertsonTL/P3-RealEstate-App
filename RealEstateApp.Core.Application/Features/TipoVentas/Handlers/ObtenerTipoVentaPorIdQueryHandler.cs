using AutoMapper;
using MediatR;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.TipoVentas.Queries;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;

namespace RealEstateApp.Core.Application.Features.TipoVentas.Handlers
{
    public class ObtenerTipoVentaPorIdQueryHandler : IRequestHandler<ObtenerTipoVentaPorIdQuery, TipoVentaDto>
    {
        private readonly IRepositorioGenerico<TipoVenta> _repositorio;
        private readonly IMapper _mapper;

        public ObtenerTipoVentaPorIdQueryHandler(IRepositorioGenerico<TipoVenta> repositorio, IMapper mapper)
        {
            _repositorio = repositorio;
            _mapper = mapper;
        }

        public async Task<TipoVentaDto> Handle(ObtenerTipoVentaPorIdQuery request, CancellationToken cancellationToken)
        {
            var tipo = await _repositorio.ObtenerPorIdAsync(request.Id);
            return _mapper.Map<TipoVentaDto>(tipo);
        }
    }
}
