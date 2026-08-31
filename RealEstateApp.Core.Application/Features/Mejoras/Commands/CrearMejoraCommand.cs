using MediatR;
using RealEstateApp.Core.Application.DTOs;

namespace RealEstateApp.Core.Application.Features.Mejoras.Commands
{
    public class CrearMejoraCommand : IRequest<MejoraDto>
    {
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
    }
}
