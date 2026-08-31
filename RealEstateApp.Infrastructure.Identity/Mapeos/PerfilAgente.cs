using AutoMapper;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Infrastructure.Identity.Entidades;

namespace RealEstateApp.Infrastructure.Identity.Mapeos
{
    public class PerfilAgente : Profile
    {
        public PerfilAgente()
        {
            CreateMap<UsuarioAplicacion, AgenteDto>()
                .ForMember(dest => dest.CantidadPropiedades, opt => opt.Ignore());
        }
    }
}
