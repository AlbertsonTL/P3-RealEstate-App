using AutoMapper;
using MediatR;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.Mejoras.Queries;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;

namespace RealEstateApp.Core.Application.Features.Mejoras.Handlers
{
    public class ObtenerMejorasQueryHandler : IRequestHandler<ObtenerMejorasQuery, IEnumerable<MejoraDto>>
    {
        private readonly IRepositorioGenerico<Mejora> _repositorio;
        private readonly IMapper _mapper;

        public ObtenerMejorasQueryHandler(IRepositorioGenerico<Mejora> repositorio, IMapper mapper)
        {
            _repositorio = repositorio;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MejoraDto>> Handle(ObtenerMejorasQuery request, CancellationToken cancellationToken)
        {
            var mejoras = await _repositorio.ObtenerTodosAsync();
            return _mapper.Map<IEnumerable<MejoraDto>>(mejoras);
        }
    }
}
