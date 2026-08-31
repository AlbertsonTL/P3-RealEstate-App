using MediatR;
using RealEstateApp.Core.Application.Features.TipoPropiedades.Commands;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;

namespace RealEstateApp.Core.Application.Features.TipoPropiedades.Handlers
{
    public class EliminarTipoPropiedadCommandHandler : IRequestHandler<EliminarTipoPropiedadCommand, Unit>
    {
        private readonly IRepositorioGenerico<TipoPropiedad> _repositorio;

        public EliminarTipoPropiedadCommandHandler(IRepositorioGenerico<TipoPropiedad> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<Unit> Handle(EliminarTipoPropiedadCommand request, CancellationToken cancellationToken)
        {
            var tipo = await _repositorio.ObtenerPorIdAsync(request.Id);
            if (tipo != null)
            {
                await _repositorio.EliminarAsync(tipo);
            }
            return Unit.Value;
        }
    }
}
