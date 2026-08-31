using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.WebApp.ViewModels.Admin;

namespace RealEstateApp.WebApp.Interfaces.Servicios;

/// <summary>
/// Servicio de catálogos administrativos (TipoPropiedad, TipoVenta, Mejoras).
/// Encapsula el acceso a datos para separar la responsabilidad del controlador.
/// </summary>
public interface IServicioCatalogoAdmin
{
    // TipoPropiedad
    Task<List<TipoPropiedad>> ObtenerTiposPropiedadesAsync();
    Task<List<TipoCatalogoConConteoViewModel>> ObtenerTiposPropiedadesConConteoAsync();
    Task<TipoPropiedad?> ObtenerTipoPropiedadPorIdAsync(int id);
    Task CrearTipoPropiedadAsync(CatalogoFormViewModel modelo);
    Task ActualizarTipoPropiedadAsync(CatalogoFormViewModel modelo);
    /// <returns>(Exito, MensajeError)</returns>
    Task<(bool Exito, string? MensajeError)> EliminarTipoPropiedadAsync(int id);

    // TipoVenta
    Task<List<TipoVenta>> ObtenerTiposVentasAsync();
    Task<List<TipoCatalogoConConteoViewModel>> ObtenerTiposVentasConConteoAsync();
    Task<TipoVenta?> ObtenerTipoVentaPorIdAsync(int id);
    Task CrearTipoVentaAsync(CatalogoFormViewModel modelo);
    Task ActualizarTipoVentaAsync(CatalogoFormViewModel modelo);
    /// <returns>(Exito, MensajeError)</returns>
    Task<(bool Exito, string? MensajeError)> EliminarTipoVentaAsync(int id);

    // Mejoras
    Task<List<Mejora>> ObtenerMejorasAsync();
    Task<Mejora?> ObtenerMejoraPorIdAsync(int id);
    Task CrearMejoraAsync(CatalogoFormViewModel modelo);
    Task ActualizarMejoraAsync(CatalogoFormViewModel modelo);
    /// <returns>(Exito, MensajeError)</returns>
    Task<(bool Exito, string? MensajeError)> EliminarMejoraAsync(int id);

    // Utilidades
    Task<Dictionary<string, int>> ObtenerCantidadPropiedadesPorAgenteAsync();
}
