using AutoMapper;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Infrastructure.Identity.Entidades;

namespace RealEstateApp.Infrastructure.Identity.Mapeos
{
    public class PerfilUsuario : Profile
    {
        public PerfilUsuario()
        {
            CreateMap<UsuarioAplicacion, AdminDto>().ReverseMap();
            CreateMap<UsuarioAplicacion, DesarrolladorDto>().ReverseMap();
        }
    }
}
