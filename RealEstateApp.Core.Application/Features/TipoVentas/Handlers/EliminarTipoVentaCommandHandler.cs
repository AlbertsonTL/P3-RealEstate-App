using MediatR;
using RealEstateApp.Core.Application.Features.TipoVentas.Commands;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;

namespace RealEstateApp.Core.Application.Features.TipoVentas.Handlers
{
    public class EliminarTipoVentaCommandHandler : IRequestHandler<EliminarTipoVentaCommand, Unit>
    {
        private readonly IRepositorioGenerico<TipoVenta> _repositorio;

        public EliminarTipoVentaCommandHandler(IRepositorioGenerico<TipoVenta> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<Unit> Handle(EliminarTipoVentaCommand request, CancellationToken cancellationToken)
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
