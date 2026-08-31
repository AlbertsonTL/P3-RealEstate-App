using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.WebApp.Interfaces.Servicios;
using RealEstateApp.WebApp.ViewModels.Agente;
using RealEstateApp.WebApp.ViewModels.Shared;
using System.Security.Claims;

namespace RealEstateApp.WebApp.Controllers;

[Authorize(Roles = "Agente")]
public class AgenteController : Controller
{
    private readonly IServicioPropiedad _servicioPropiedad;
    private readonly IServicioCuentaWebApp _servicioCuenta;
    private readonly IServicioChat _servicioChat;
    private readonly IServicioOferta _servicioOferta;

    public AgenteController(
        IServicioPropiedad servicioPropiedad,
        IServicioCuentaWebApp servicioCuenta,
        IServicioChat servicioChat,
        IServicioOferta servicioOferta)
    {
        _servicioPropiedad = servicioPropiedad;
        _servicioCuenta = servicioCuenta;
        _servicioChat = servicioChat;
        _servicioOferta = servicioOferta;
    }

    public async Task<IActionResult> Index()
    {
        var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var propiedades = await _servicioPropiedad.ObtenerPropiedadesAgenteAsync(agenteId, true);
        return View(propiedades);
    }

    // Mi Perfil

    [HttpGet]
    public async Task<IActionResult> MiPerfil()
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var perfil = await _servicioCuenta.ObtenerPerfilUsuarioAsync(usuarioId);

        ViewBag.PerfilViewModel = perfil ?? new EditarPerfilViewModel();
        ViewBag.ContrasenaViewModel = new CambiarContrasenaViewModel();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarPerfil(EditarPerfilViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.PerfilViewModel = modelo;
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
        if (!ModelState.IsValid)
        {
            var usuarioId2 = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            ViewBag.PerfilViewModel = await _servicioCuenta.ObtenerPerfilUsuarioAsync(usuarioId2) ?? new EditarPerfilViewModel();
            ViewBag.ContrasenaViewModel = modelo;
            ViewBag.SeccionActiva = "contrasena";
            return View("MiPerfil");
        }

        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var error = await _servicioCuenta.CambiarContrasenaAsync(usuarioId, modelo.ContrasenaActual, modelo.NuevaContrasena);

        if (error is not null)
        {
            ModelState.AddModelError(nameof(modelo.ContrasenaActual), error);
            ViewBag.PerfilViewModel = await _servicioCuenta.ObtenerPerfilUsuarioAsync(usuarioId) ?? new EditarPerfilViewModel();
            ViewBag.ContrasenaViewModel = modelo;
            ViewBag.SeccionActiva = "contrasena";
            return View("MiPerfil");
        }

        TempData["Exito"] = "Contraseña cambiada correctamente.";
        return RedirectToAction(nameof(MiPerfil));
    }

    // Mantenimiento / Propiedades

    public async Task<IActionResult> Mantenimiento()
    {
        var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var propiedades = await _servicioPropiedad.ObtenerPropiedadesAgenteAsync(agenteId, false);
        return View(propiedades);
    }

    [HttpGet]
    public async Task<IActionResult> CrearPropiedad()
    {
        var vm = await CargarCatalogosAsync(new CrearPropiedadViewModel());
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearPropiedad(CrearPropiedadViewModel modelo)
    {
        if (!modelo.Imagenes.Any(i => i is not null))
        {
            ModelState.AddModelError(nameof(modelo.Imagenes), "Debes seleccionar al menos una imagen.");
        }

        if (!ModelState.IsValid) return View(await CargarCatalogosAsync(modelo));
        var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _servicioPropiedad.CrearPropiedadAsync(modelo, agenteId);
        return RedirectToAction(nameof(Mantenimiento));
    }

    [HttpGet]
    public async Task<IActionResult> EditarPropiedad(int id)
    {
        var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var vm = await _servicioPropiedad.ObtenerPropiedadParaEditarAsync(id, agenteId);
        if (vm is null)
        {
            TempData["Error"] = "La propiedad no existe o no pertenece al agente actual.";
            return RedirectToAction(nameof(Mantenimiento));
        }

        vm = await CargarCatalogosAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarPropiedad(EditarPropiedadViewModel modelo)
    {
        modelo.MejorasSeleccionadas ??= [];
        modelo.EliminarImagenIds ??= [];
        modelo.ImagenesExistentes ??= [];
        var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var snapshot = await _servicioPropiedad.ObtenerPropiedadParaEditarAsync(modelo.Id, agenteId);
        if (snapshot is null)
        {
            TempData["Error"] = "La propiedad no existe o no pertenece al agente actual.";
            return RedirectToAction(nameof(Mantenimiento));
        }

        var idsImagenesReales = snapshot.ImagenesExistentes.Select(i => i.Id).ToHashSet();
        var eliminarValidos = modelo.EliminarImagenIds.Where(idsImagenesReales.Contains).ToHashSet();
        var restantes = idsImagenesReales.Count - eliminarValidos.Count;
        var nuevas = modelo.Imagenes?.Count(i => i is not null) ?? 0;
        if (restantes + nuevas > 4)
        {
            ModelState.AddModelError(nameof(modelo.Imagenes), "La propiedad admite como maximo 4 imagenes en total.");
        }

        if (!ModelState.IsValid)
        {
            modelo.ImagenesExistentes = snapshot.ImagenesExistentes;
            return View(await CargarCatalogosAsync(modelo));
        }

        await _servicioPropiedad.EditarPropiedadAsync(modelo);
        TempData["Exito"] = "Propiedad actualizada correctamente.";
        return RedirectToAction(nameof(Mantenimiento));
    }

    [HttpGet]
    public async Task<IActionResult> EliminarPropiedad(int id)
    {
        var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var vm = await _servicioPropiedad.ObtenerPropiedadParaEliminarAsync(id, agenteId);
        if (vm is null)
        {
            TempData["Error"] = "La propiedad no existe o no pertenece al agente actual.";
            return RedirectToAction(nameof(Mantenimiento));
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmarEliminar(int id)
    {
        var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        if (await _servicioPropiedad.ObtenerPropiedadParaEliminarAsync(id, agenteId) is null)
        {
            TempData["Error"] = "La propiedad no existe o no pertenece al agente actual.";
            return RedirectToAction(nameof(Mantenimiento));
        }

        await _servicioPropiedad.EliminarPropiedadAsync(id);
        TempData["Exito"] = "Propiedad eliminada correctamente.";
        return RedirectToAction(nameof(Mantenimiento));
    }

    // Chat

    public async Task<IActionResult> ChatClientes(int propiedadId)
    {
        var acceso = await AsegurarPropiedadDelAgenteAsync(propiedadId);
        if (acceso != null) return acceso;

        var agenteId   = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var clienteIds = await _servicioChat.ObtenerClientesConMensajesAsync(propiedadId, agenteId);
        var clientes   = await _servicioCuenta.ObtenerInfoClientesAsync(clienteIds);
        return View(new ChatClientesViewModel { PropiedadId = propiedadId, Clientes = clientes });
    }

    [HttpGet]
    public async Task<IActionResult> ChatConCliente(int propiedadId, string clienteId)
    {
        var acceso = await AsegurarPropiedadDelAgenteAsync(propiedadId);
        if (acceso != null) return acceso;

        var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var mensajes = await _servicioChat.ObtenerConversacionAsync(propiedadId, clienteId, agenteId, agenteId);
        return View(new ChatConClienteViewModel { PropiedadId = propiedadId, ClienteId = clienteId, Mensajes = mensajes });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResponderChat(int propiedadId, string clienteId, string contenido)
    {
        var acceso = await AsegurarPropiedadDelAgenteAsync(propiedadId);
        if (acceso != null) return acceso;

        if (string.IsNullOrWhiteSpace(contenido))
        {
            TempData["Error"] = "Debes escribir un mensaje para responder.";
            return RedirectToAction(nameof(ChatConCliente), new { propiedadId, clienteId });
        }

        var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _servicioChat.EnviarMensajeAsync(propiedadId, agenteId, clienteId, contenido.Trim());
        return RedirectToAction(nameof(ChatConCliente), new { propiedadId, clienteId });
    }

    // Ofertas

    public async Task<IActionResult> OfertasClientes(int propiedadId)
    {
        var acceso = await AsegurarPropiedadDelAgenteAsync(propiedadId);
        if (acceso != null) return acceso;

        var clienteIds = await _servicioOferta.ObtenerClientesConOfertasAsync(propiedadId);
        var clientes   = await _servicioCuenta.ObtenerInfoClientesAsync(clienteIds);
        return View(new OfertasClientesViewModel { PropiedadId = propiedadId, Clientes = clientes });
    }

    [HttpGet]
    public async Task<IActionResult> OfertasDeCliente(int propiedadId, string clienteId)
    {
        var acceso = await AsegurarPropiedadDelAgenteAsync(propiedadId);
        if (acceso != null) return acceso;

        var ofertas = await _servicioOferta.ObtenerOfertasClientePropiedadAsync(propiedadId, clienteId);
        return View(new OfertasDeClienteViewModel { PropiedadId = propiedadId, ClienteId = clienteId, Ofertas = ofertas.ToList() });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AceptarOferta(int ofertaId, int propiedadId, string clienteId)
    {
        var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        if (!await _servicioOferta.OfertaPerteneceAPropiedadDelAgenteAsync(ofertaId, propiedadId, agenteId))
        {
            TempData["Error"] = "No tiene permiso para gestionar esta oferta.";
            return RedirectToAction(nameof(Mantenimiento));
        }

        await _servicioOferta.AceptarOfertaAsync(ofertaId, agenteId);
        TempData["Exito"] = "Oferta aceptada correctamente.";
        return RedirectToAction(nameof(OfertasDeCliente), new { propiedadId, clienteId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RechazarOferta(int ofertaId, int propiedadId, string clienteId)
    {
        var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        if (!await _servicioOferta.OfertaPerteneceAPropiedadDelAgenteAsync(ofertaId, propiedadId, agenteId))
        {
            TempData["Error"] = "No tiene permiso para gestionar esta oferta.";
            return RedirectToAction(nameof(Mantenimiento));
        }

        await _servicioOferta.RechazarOfertaAsync(ofertaId);
        TempData["Exito"] = "Oferta rechazada.";
        return RedirectToAction(nameof(OfertasDeCliente), new { propiedadId, clienteId });
    }

    // Helpers privados

    private async Task<CrearPropiedadViewModel> CargarCatalogosAsync(CrearPropiedadViewModel vm)
    {
        vm.TiposPropiedad = (await _servicioCuenta.ObtenerTiposPropiedadesAsync()).ToList();
        vm.TiposVenta = (await _servicioCuenta.ObtenerTiposVentasAsync()).ToList();
        vm.MejorasDisponibles = (await _servicioCuenta.ObtenerMejorasAsync()).ToList();
        return vm;
    }

    private async Task<EditarPropiedadViewModel> CargarCatalogosAsync(EditarPropiedadViewModel vm)
    {
        vm.TiposPropiedad = (await _servicioCuenta.ObtenerTiposPropiedadesAsync()).ToList();
        vm.TiposVenta = (await _servicioCuenta.ObtenerTiposVentasAsync()).ToList();
        vm.MejorasDisponibles = (await _servicioCuenta.ObtenerMejorasAsync()).ToList();
        return vm;
    }

    private async Task<IActionResult?> AsegurarPropiedadDelAgenteAsync(int propiedadId)
    {
        var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        if (!await _servicioPropiedad.EsPropiedadDelAgenteAsync(propiedadId, agenteId))
        {
            TempData["Error"] = "No tiene acceso a esta propiedad o no existe.";
            return RedirectToAction(nameof(Mantenimiento));
        }
        return null;
    }
}
