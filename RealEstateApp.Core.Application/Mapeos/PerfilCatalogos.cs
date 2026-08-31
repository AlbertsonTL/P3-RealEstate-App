using AutoMapper;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.Mejoras.Commands;
using RealEstateApp.Core.Application.Features.TipoPropiedades.Commands;
using RealEstateApp.Core.Application.Features.TipoVentas.Commands;
using RealEstateApp.Core.Domain.Entidades;

namespace RealEstateApp.Core.Application.Mapeos
{
    public class PerfilCatalogos : Profile
    {
        public PerfilCatalogos()
        {
            CreateMap<TipoPropiedad, TipoPropiedadDto>().ReverseMap();
            CreateMap<CrearTipoPropiedadCommand, TipoPropiedad>();
            CreateMap<ActualizarTipoPropiedadCommand, TipoPropiedad>();

            CreateMap<TipoVenta, TipoVentaDto>().ReverseMap();
            CreateMap<CrearTipoVentaCommand, TipoVenta>();
            CreateMap<ActualizarTipoVentaCommand, TipoVenta>();

            CreateMap<Mejora, MejoraDto>().ReverseMap();
            CreateMap<CrearMejoraCommand, Mejora>();
            CreateMap<ActualizarMejoraCommand, Mejora>();
        }
    }
}
