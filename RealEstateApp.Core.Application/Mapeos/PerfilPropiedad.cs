using AutoMapper;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Domain.Entidades;

namespace RealEstateApp.Core.Application.Mapeos
{
    public class PerfilPropiedad : Profile
    {
        public PerfilPropiedad()
        {
            CreateMap<Propiedad, PropiedadDto>()
                .ForMember(dest => dest.TipoPropiedadId, opt => opt.MapFrom(src => src.TipoPropiedadId))
                .ForMember(dest => dest.TipoPropiedad, opt => opt.MapFrom(src => src.TipoPropiedad.Nombre))
                .ForMember(dest => dest.TipoVentaId, opt => opt.MapFrom(src => src.TipoVentaId))
                .ForMember(dest => dest.TipoVenta, opt => opt.MapFrom(src => src.TipoVenta.Nombre))
                .ForMember(dest => dest.UrlsImagenes, opt => opt.MapFrom(src => src.Imagenes.Select(i => i.UrlImagen)))
                .ForMember(dest => dest.Mejoras, opt => opt.MapFrom(src => src.PropiedadesMejoras.Select(pm => pm.Mejora.Nombre)))
                .ForMember(dest => dest.EstadoPropiedad, opt => opt.MapFrom(src => src.Estado.ToString()));
        }
    }
}
