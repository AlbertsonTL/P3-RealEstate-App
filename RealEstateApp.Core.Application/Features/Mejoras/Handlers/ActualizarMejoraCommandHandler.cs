using AutoMapper;
using MediatR;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.Mejoras.Commands;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;

namespace RealEstateApp.Core.Application.Features.Mejoras.Handlers
{
    public class ActualizarMejoraCommandHandler : IRequestHandler<ActualizarMejoraCommand, MejoraDto>
    {
        private readonly IRepositorioGenerico<Mejora> _repositorio;
        private readonly IMapper _mapper;

        public ActualizarMejoraCommandHandler(IRepositorioGenerico<Mejora> repositorio, IMapper mapper)
        {
            _repositorio = repositorio;
            _mapper = mapper;
        }

        public async Task<MejoraDto> Handle(ActualizarMejoraCommand request, CancellationToken cancellationToken)
        {
            var mejora = await _repositorio.ObtenerPorIdAsync(request.Id);
            if (mejora == null) return null!;

            mejora.Nombre = request.Nombre;
            mejora.Descripcion = request.Descripcion;

            await _repositorio.ActualizarAsync(mejora);
            return _mapper.Map<MejoraDto>(mejora);
        }
    }
}
