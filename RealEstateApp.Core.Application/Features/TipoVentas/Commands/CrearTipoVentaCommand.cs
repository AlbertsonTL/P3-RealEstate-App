using MediatR;
using RealEstateApp.Core.Application.DTOs;

namespace RealEstateApp.Core.Application.Features.TipoVentas.Commands
{
    public class CrearTipoVentaCommand : IRequest<TipoVentaDto>
    {
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
    }
}
