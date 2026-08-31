using AutoMapper;
using MediatR;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.Mejoras.Queries;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;

namespace RealEstateApp.Core.Application.Features.Mejoras.Handlers
{
    public class ObtenerMejoraPorIdQueryHandler : IRequestHandler<ObtenerMejoraPorIdQuery, MejoraDto>
    {
        private readonly IRepositorioGenerico<Mejora> _repositorio;
        private readonly IMapper _mapper;

        public ObtenerMejoraPorIdQueryHandler(IRepositorioGenerico<Mejora> repositorio, IMapper mapper)
        {
            _repositorio = repositorio;
            _mapper = mapper;
        }

        public async Task<MejoraDto> Handle(ObtenerMejoraPorIdQuery request, CancellationToken cancellationToken)
        {
            var mejora = await _repositorio.ObtenerPorIdAsync(request.Id);
            return _mapper.Map<MejoraDto>(mejora);
        }
    }
}
