using RealEstateApp.Core.Domain.Interfaces.Repositorios;
using RealEstateApp.WebApp.ViewModels.Agente;
using RealEstateApp.WebApp.ViewModels.Publico;
using RealEstateApp.Core.Application.DTOs;

namespace RealEstateApp.WebApp.Interfaces.Servicios;

public interface IServicioPropiedad
{
    Task<IEnumerable<PropiedadResumenViewModel>> ObtenerDisponiblesConFiltrosAsync(FiltrosPropiedadDto filtros);
    Task<PropiedadDetalleViewModel?> ObtenerDetalleAsync(int id, string? clienteId);
    Task CrearPropiedadAsync(CrearPropiedadViewModel modelo, string agenteId);
    Task EditarPropiedadAsync(EditarPropiedadViewModel modelo);
    Task EliminarPropiedadAsync(int id);
    Task<EditarPropiedadViewModel?> ObtenerPropiedadParaEditarAsync(int id, string agenteId);
    Task<EliminarPropiedadViewModel?> ObtenerPropiedadParaEliminarAsync(int id, string agenteId);
    Task<bool> EsPropiedadDelAgenteAsync(int propiedadId, string agenteId);
    Task<IEnumerable<PropiedadResumenViewModel>> ObtenerPropiedadesAgenteAsync(string agenteId, bool incluirVendidas);
    Task<IEnumerable<PropiedadResumenViewModel>> ObtenerFavoritasClienteAsync(string clienteId);
    Task<bool> ToggleFavoritaAsync(int propiedadId, string clienteId);
}
