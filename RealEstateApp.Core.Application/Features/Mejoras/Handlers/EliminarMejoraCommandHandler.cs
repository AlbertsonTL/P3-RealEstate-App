using MediatR;
using RealEstateApp.Core.Application.Features.Mejoras.Commands;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;

namespace RealEstateApp.Core.Application.Features.Mejoras.Handlers
{
    public class EliminarMejoraCommandHandler : IRequestHandler<EliminarMejoraCommand, Unit>
    {
        private readonly IRepositorioGenerico<Mejora> _repositorio;

        public EliminarMejoraCommandHandler(IRepositorioGenerico<Mejora> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<Unit> Handle(EliminarMejoraCommand request, CancellationToken cancellationToken)
        {
            var mejora = await _repositorio.ObtenerPorIdAsync(request.Id);
            if (mejora != null)
            {
                await _repositorio.EliminarAsync(mejora);
            }
            return Unit.Value;
        }
    }
}
