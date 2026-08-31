using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.WebApp.Interfaces.Servicios;
using RealEstateApp.WebApp.ViewModels.Agente;
using RealEstateApp.WebApp.ViewModels.Publico;

namespace RealEstateApp.WebApp.Controllers;

[AllowAnonymous]
public class AgentesController : Controller
{
    private readonly IServicioCuentaWebApp _servicioCuenta;
    private readonly IServicioPropiedad _servicioPropiedad;

    public AgentesController(IServicioCuentaWebApp servicioCuenta, IServicioPropiedad servicioPropiedad)
    {
        _servicioCuenta = servicioCuenta;
        _servicioPropiedad = servicioPropiedad;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? busqueda)
    {
        var agentes = await _servicioCuenta.ObtenerAgentesActivosAsync();
        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            agentes = agentes.Where(a => a.Nombre.Contains(busqueda, StringComparison.OrdinalIgnoreCase));
        }

        ViewBag.Busqueda = busqueda;
        return View(agentes.OrderBy(a => a.Nombre).ToList());
    }

    // FIX: spec exige los mismos filtros del Home en todas las pantallas de listado de propiedades
    [HttpGet]
    public async Task<IActionResult> Propiedades(string agenteId, FiltrosPropiedadViewModel filtros)
    {
        var agente = await _servicioCuenta.ObtenerAgentePorIdAsync(agenteId);
        if (agente is null) return NotFound();

        var propiedades = (await _servicioPropiedad.ObtenerPropiedadesAgenteAsync(agenteId, false)).AsEnumerable();

        if (!string.IsNullOrWhiteSpace(filtros.CodigoBusqueda))
            propiedades = propiedades.Where(p => p.Codigo.Contains(filtros.CodigoBusqueda, StringComparison.OrdinalIgnoreCase));
        if (filtros.TipoPropiedadId.HasValue)
            propiedades = propiedades.Where(p => p.TipoPropiedadId == filtros.TipoPropiedadId);
        if (filtros.PrecioMinimo.HasValue)
            propiedades = propiedades.Where(p => p.Precio >= filtros.PrecioMinimo.Value);
        if (filtros.PrecioMaximo.HasValue)
            propiedades = propiedades.Where(p => p.Precio <= filtros.PrecioMaximo.Value);
        if (filtros.CantidadHabitaciones.HasValue)
            propiedades = propiedades.Where(p => p.CantidadHabitaciones == filtros.CantidadHabitaciones.Value);
        if (filtros.CantidadBanos.HasValue)
            propiedades = propiedades.Where(p => p.CantidadBanos == filtros.CantidadBanos.Value);

        var tipos = await _servicioCuenta.ObtenerTiposPropiedadesAsync();
        filtros.TiposPropiedad = tipos.ToList();

        var vm = new DetalleAgenteConPropiedadesViewModel
        {
            NombreAgente    = agente.Value.Nombre,
            UrlFotoAgente   = agente.Value.UrlFoto,
            AgenteId        = agenteId,
            Propiedades     = propiedades.ToList(),
            Filtros         = filtros
        };

        return View(vm);
    }
}
