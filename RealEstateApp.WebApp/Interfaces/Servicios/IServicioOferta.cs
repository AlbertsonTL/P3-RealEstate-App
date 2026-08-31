using RealEstateApp.WebApp.ViewModels.Shared;

namespace RealEstateApp.WebApp.Interfaces.Servicios;

public interface IServicioOferta
{
    Task<IEnumerable<OfertaViewModel>> ObtenerOfertasClientePropiedadAsync(int propiedadId, string clienteId);
    Task<IEnumerable<string>> ObtenerClientesConOfertasAsync(int propiedadId);
    Task CrearOfertaAsync(int propiedadId, string clienteId, decimal cifra);
    Task AceptarOfertaAsync(int ofertaId, string agenteId);
    Task RechazarOfertaAsync(int ofertaId);
    Task<bool> OfertaPerteneceAPropiedadDelAgenteAsync(int ofertaId, int propiedadId, string agenteId);
}
