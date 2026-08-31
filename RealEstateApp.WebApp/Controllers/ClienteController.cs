using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.WebApp.Interfaces.Servicios;
using RealEstateApp.WebApp.ViewModels.Cliente;
using RealEstateApp.WebApp.ViewModels.Publico;
using RealEstateApp.WebApp.ViewModels.Shared;
using System.Security.Claims;

namespace RealEstateApp.WebApp.Controllers;

[Authorize(Roles = "Cliente")]
public class ClienteController : Controller
{
    private readonly IServicioPropiedad _servicioPropiedad;
    private readonly IServicioChat _servicioChat;
    private readonly IServicioOferta _servicioOferta;
    private readonly IServicioCuentaWebApp _servicioCuenta;

    public ClienteController(
        IServicioPropiedad servicioPropiedad,
        IServicioChat servicioChat,
        IServicioOferta servicioOferta,
        IServicioCuentaWebApp servicioCuenta)
    {
        _servicioPropiedad = servicioPropiedad;
        _servicioChat = servicioChat;
        _servicioOferta = servicioOferta;
        _servicioCuenta = servicioCuenta;
    }

    // Favoritos

    // FIX: spec exige los mismos filtros del Home en todas las pantallas de listado de propiedades
    [HttpGet]
    public async Task<IActionResult> MisPropiedades(FiltrosPropiedadViewModel filtros)
    {
        var clienteId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var favoritas = (await _servicioPropiedad.ObtenerFavoritasClienteAsync(clienteId)).AsEnumerable();

        if (!string.IsNullOrWhiteSpace(filtros.CodigoBusqueda))
            favoritas = favoritas.Where(p => p.Codigo.Contains(filtros.CodigoBusqueda, StringComparison.OrdinalIgnoreCase));
        if (filtros.TipoPropiedadId.HasValue)
            favoritas = favoritas.Where(p => p.TipoPropiedadId == filtros.TipoPropiedadId);
        if (filtros.PrecioMinimo.HasValue)
            favoritas = favoritas.Where(p => p.Precio >= filtros.PrecioMinimo.Value);
        if (filtros.PrecioMaximo.HasValue)
            favoritas = favoritas.Where(p => p.Precio <= filtros.PrecioMaximo.Value);
        if (filtros.CantidadHabitaciones.HasValue)
            favoritas = favoritas.Where(p => p.CantidadHabitaciones == filtros.CantidadHabitaciones.Value);
        if (filtros.CantidadBanos.HasValue)
            favoritas = favoritas.Where(p => p.CantidadBanos == filtros.CantidadBanos.Value);

        var tipos = await _servicioCuenta.ObtenerTiposPropiedadesAsync();
        filtros.TiposPropiedad = tipos.ToList();
        var lista = favoritas.ToList();
        foreach (var p in lista) p.EsFavorita = true;

        return View(new HomeViewModel { Propiedades = lista, Filtros = filtros });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleFavorita(int propiedadId)
    {
        var clienteId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var esFavorita = await _servicioPropiedad.ToggleFavoritaAsync(propiedadId, clienteId);
        return Json(new { esFavorita });
    }

    // Mensajes y ofertas

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnviarMensaje(int propiedadId, string agenteId, string contenido)
    {
        if (string.IsNullOrWhiteSpace(contenido))
        {
            TempData["Error"] = "Escriba un mensaje antes de enviar.";
            return RedirectToAction("DetallePropiedad", "Publico", new { id = propiedadId });
        }

        var clienteId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _servicioChat.EnviarMensajeAsync(propiedadId, clienteId, agenteId, contenido.Trim());
        TempData["Exito"] = "Mensaje enviado.";
        return RedirectToAction("DetallePropiedad", "Publico", new { id = propiedadId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NuevaOferta(NuevaOfertaViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Revise los datos de la oferta.";
            return RedirectToAction("DetallePropiedad", "Publico", new { id = modelo.PropiedadId });
        }

        var clienteId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            await _servicioOferta.CrearOfertaAsync(modelo.PropiedadId, clienteId, modelo.CifraOfertada);
            TempData["Exito"] = "Oferta registrada correctamente.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("DetallePropiedad", "Publico", new { id = modelo.PropiedadId });
    }

    // Mi Perfil

    [HttpGet]
    public async Task<IActionResult> MiPerfil()
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        ViewBag.PerfilViewModel    = await _servicioCuenta.ObtenerPerfilUsuarioAsync(usuarioId) ?? new EditarPerfilViewModel();
        ViewBag.ContrasenaViewModel = new CambiarContrasenaViewModel();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarPerfil(EditarPerfilViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.PerfilViewModel    = modelo;
            ViewBag.ContrasenaViewModel = new CambiarContrasenaViewModel();
            ViewBag.SeccionActiva = "perfil";
            return View("MiPerfil");
        }

        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _servicioCuenta.EditarPerfilUsuarioAsync(usuarioId, modelo);
        TempData["Exito"] = "Perfil actualizado correctamente.";
        return RedirectToAction(nameof(MiPerfil));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarContrasena(CambiarContrasenaViewModel modelo)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        if (!ModelState.IsValid)
        {
            ViewBag.PerfilViewModel    = await _servicioCuenta.ObtenerPerfilUsuarioAsync(usuarioId) ?? new EditarPerfilViewModel();
            ViewBag.ContrasenaViewModel = modelo;
            ViewBag.SeccionActiva = "contrasena";
            return View("MiPerfil");
        }

        var error = await _servicioCuenta.CambiarContrasenaAsync(usuarioId, modelo.ContrasenaActual, modelo.NuevaContrasena);
        if (error is not null)
        {
            ModelState.AddModelError(nameof(modelo.ContrasenaActual), error);
            ViewBag.PerfilViewModel    = await _servicioCuenta.ObtenerPerfilUsuarioAsync(usuarioId) ?? new EditarPerfilViewModel();
            ViewBag.ContrasenaViewModel = modelo;
            ViewBag.SeccionActiva = "contrasena";
            return View("MiPerfil");
        }

        TempData["Exito"] = "Contraseña cambiada correctamente.";
        return RedirectToAction(nameof(MiPerfil));
    }
}
