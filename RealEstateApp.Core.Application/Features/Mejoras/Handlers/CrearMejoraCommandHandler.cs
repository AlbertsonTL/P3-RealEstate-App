using AutoMapper;
using MediatR;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.Mejoras.Commands;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;

namespace RealEstateApp.Core.Application.Features.Mejoras.Handlers
{
    public class CrearMejoraCommandHandler : IRequestHandler<CrearMejoraCommand, MejoraDto>
    {
        private readonly IRepositorioGenerico<Mejora> _repositorio;
        private readonly IMapper _mapper;

        public CrearMejoraCommandHandler(IRepositorioGenerico<Mejora> repositorio, IMapper mapper)
        {
            _repositorio = repositorio;
            _mapper = mapper;
        }

        public async Task<MejoraDto> Handle(CrearMejoraCommand request, CancellationToken cancellationToken)
        {
            var mejora = _mapper.Map<Mejora>(request);
            mejora = await _repositorio.AgregarAsync(mejora);
            return _mapper.Map<MejoraDto>(mejora);
        }
    }
}
