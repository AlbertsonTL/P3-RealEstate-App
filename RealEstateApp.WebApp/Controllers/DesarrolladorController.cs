using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.WebApp.Interfaces.Servicios;
using RealEstateApp.WebApp.ViewModels.Shared;
using System.Security.Claims;

namespace RealEstateApp.WebApp.Controllers;

/// <summary>
/// Controlador para el rol Desarrollador.
/// Solo tiene acceso a su perfil; no puede gestionar la aplicación.
/// </summary>
[Authorize(Roles = "Desarrollador")]
public class DesarrolladorController : Controller
{
    private readonly IServicioCuentaWebApp _servicioCuenta;

    public DesarrolladorController(IServicioCuentaWebApp servicioCuenta)
    {
        _servicioCuenta = servicioCuenta;
    }

    // Mi Perfil

    [HttpGet]
    public async Task<IActionResult> MiPerfil()
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        ViewBag.PerfilViewModel     = await _servicioCuenta.ObtenerPerfilUsuarioAsync(usuarioId) ?? new EditarPerfilViewModel();
        ViewBag.ContrasenaViewModel = new CambiarContrasenaViewModel();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarPerfil(EditarPerfilViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.PerfilViewModel     = modelo;
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
            ViewBag.PerfilViewModel     = await _servicioCuenta.ObtenerPerfilUsuarioAsync(usuarioId) ?? new EditarPerfilViewModel();
            ViewBag.ContrasenaViewModel = modelo;
            ViewBag.SeccionActiva = "contrasena";
            return View("MiPerfil");
        }

        var error = await _servicioCuenta.CambiarContrasenaAsync(usuarioId, modelo.ContrasenaActual, modelo.NuevaContrasena);
        if (error is not null)
        {
            ModelState.AddModelError(nameof(modelo.ContrasenaActual), error);
            ViewBag.PerfilViewModel     = await _servicioCuenta.ObtenerPerfilUsuarioAsync(usuarioId) ?? new EditarPerfilViewModel();
            ViewBag.ContrasenaViewModel = modelo;
            ViewBag.SeccionActiva = "contrasena";
            return View("MiPerfil");
        }

        TempData["Exito"] = "Contraseña cambiada correctamente.";
        return RedirectToAction(nameof(MiPerfil));
    }
}
