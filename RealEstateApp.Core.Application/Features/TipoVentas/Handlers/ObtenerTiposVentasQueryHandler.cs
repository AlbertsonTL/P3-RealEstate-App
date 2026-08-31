using AutoMapper;
using MediatR;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.TipoVentas.Queries;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;

namespace RealEstateApp.Core.Application.Features.TipoVentas.Handlers
{
    public class ObtenerTiposVentasQueryHandler : IRequestHandler<ObtenerTiposVentasQuery, IEnumerable<TipoVentaDto>>
    {
        private readonly IRepositorioGenerico<TipoVenta> _repositorio;
        private readonly IMapper _mapper;

        public ObtenerTiposVentasQueryHandler(IRepositorioGenerico<TipoVenta> repositorio, IMapper mapper)
        {
            _repositorio = repositorio;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TipoVentaDto>> Handle(ObtenerTiposVentasQuery request, CancellationToken cancellationToken)
        {
            var tipos = await _repositorio.ObtenerTodosAsync();
            return _mapper.Map<IEnumerable<TipoVentaDto>>(tipos);
        }
    }
}
