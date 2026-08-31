using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;
using RealEstateApp.WebApp.Interfaces.Servicios;
using RealEstateApp.WebApp.ViewModels.Publico;
using RealEstateApp.Core.Application.DTOs;
using System.Security.Claims;

namespace RealEstateApp.WebApp.Controllers;

[AllowAnonymous]
public class InicioController : Controller
{
    private readonly IServicioPropiedad _servicioPropiedad;
    private readonly IServicioCuentaWebApp _servicioCuenta;

    public InicioController(IServicioPropiedad servicioPropiedad, IServicioCuentaWebApp servicioCuenta)
    {
        _servicioPropiedad = servicioPropiedad;
        _servicioCuenta = servicioCuenta;
    }

    [HttpGet]
    public async Task<IActionResult> Index(FiltrosPropiedadViewModel filtros)
    {
        var dto = new FiltrosPropiedadDto
        {
            CodigoBusqueda = filtros.CodigoBusqueda,
            TipoPropiedadId = filtros.TipoPropiedadId,
            PrecioMinimo = filtros.PrecioMinimo,
            PrecioMaximo = filtros.PrecioMaximo,
            CantidadHabitaciones = filtros.CantidadHabitaciones,
            CantidadBanos = filtros.CantidadBanos
        };

        var propiedades = (await _servicioPropiedad.ObtenerDisponiblesConFiltrosAsync(dto)).ToList();
        var tipos = await _servicioCuenta.ObtenerTiposPropiedadesAsync();
        filtros.TiposPropiedad = tipos.ToList();

        if (User.IsInRole("Cliente"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var favoritas = (await _servicioPropiedad.ObtenerFavoritasClienteAsync(userId)).Select(p => p.Id).ToHashSet();
            foreach (var p in propiedades)
            {
                p.EsFavorita = favoritas.Contains(p.Id);
            }
        }

        return View(new HomeViewModel { Propiedades = propiedades, Filtros = filtros });
    }
}
