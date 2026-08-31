using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.DTOs.Cuenta;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Infrastructure.Identity.Entidades;
using RealEstateApp.WebApp.Filtros;
using RealEstateApp.WebApp.Interfaces.Servicios;
using RealEstateApp.WebApp.ViewModels.Cuenta;

namespace RealEstateApp.WebApp.Controllers;

[Authorize]
public class CuentaController : Controller
{
    private readonly IServicioCuentaWebApp _servicioCuenta;
    private readonly SignInManager<UsuarioAplicacion> _signInManager;
    private readonly UserManager<UsuarioAplicacion> _userManager;
    private readonly IServicioEmail _servicioEmail;

    public CuentaController(
        IServicioCuentaWebApp servicioCuenta,
        SignInManager<UsuarioAplicacion> signInManager,
        UserManager<UsuarioAplicacion> userManager,
        IServicioEmail servicioEmail)
    {
        _servicioCuenta = servicioCuenta;
        _signInManager = signInManager;
        _userManager = userManager;
        _servicioEmail = servicioEmail;
    }

    // Registro

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Registro() => View(new RegistroViewModel());

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Registro(RegistroViewModel modelo)
    {
        if (!ModelState.IsValid) return View(modelo);

        var dto = new RegistrarUsuarioDto
        {
            Nombre = modelo.Nombre,
            Apellido = modelo.Apellido,
            Telefono = modelo.Telefono,
            NombreUsuario = modelo.NombreUsuario,
            Correo = modelo.Correo,
            Contrasena = modelo.Contrasena,
            ConfirmarContrasena = modelo.ConfirmarContrasena,
            TipoUsuario = modelo.TipoUsuario
        };

        var resultado = modelo.TipoUsuario == "Agente"
            ? await _servicioCuenta.RegistrarAgenteAsync(dto)
            : await _servicioCuenta.RegistrarClienteAsync(dto);

        if (resultado.TieneError)
        {
            ModelState.AddModelError(string.Empty, resultado.MensajeError ?? "No fue posible registrar la cuenta.");
            return View(modelo);
        }

        // Construir la URL de activación completa con esquema+host real y enviar el correo.
        // Esto se hace aquí porque solo el controller tiene acceso a Request.Scheme / Url.Action().
        if (resultado.UsuarioId is not null && resultado.TokenActivacion is not null)
        {
            var enlace = Url.Action(
                nameof(ActivarCuenta), "Cuenta",
                new { usuarioId = resultado.UsuarioId, token = resultado.TokenActivacion },
                Request.Scheme)!;

            try
            {
                await _servicioEmail.EnviarEmailActivacionAsync(
                    modelo.Correo,
                    $"{modelo.Nombre} {modelo.Apellido}".Trim(),
                    enlace);
            }
            catch
            {
                // No bloqueamos el registro si el SMTP no está configurado en dev
            }
        }

        TempData["Exito"] = $"¡Cuenta creada exitosamente! Te hemos enviado un correo de activación a {modelo.Correo}. " +
                            "Por favor revisa tu bandeja de entrada (y la carpeta de spam) y haz clic en el enlace para activar tu cuenta antes de iniciar sesión.";
        return RedirectToAction(nameof(Login));
    }

    // Activar cuenta

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> ActivarCuenta(string usuarioId, string token)
    {
        ViewBag.Mensaje = await _servicioCuenta.ActivarCuentaAsync(usuarioId, token);
        return View();
    }

    // Login

    [AllowAnonymous]
    [ServiceFilter(typeof(FiltroRolDesarrollador))]
    [HttpGet]
    public IActionResult Login() => View(new LoginViewModel());

    [AllowAnonymous]
    [ServiceFilter(typeof(FiltroRolDesarrollador))]
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel modelo)
    {
        if (!ModelState.IsValid) return View(modelo);

        var respuesta = await _servicioCuenta.LoginAsync(new SolicitudLoginDto
        {
            UsuarioOCorreo = modelo.UsuarioOCorreo,
            Contrasena = modelo.Contrasena
        });

        if (respuesta.TieneError)
        {
            ModelState.AddModelError(string.Empty, respuesta.MensajeError ?? "Credenciales inválidas.");
            return View(modelo);
        }

        // Evita que un usuario con el rol de desarrollador entre
        if (respuesta.Roles.Contains("Desarrollador") && !respuesta.Roles.Any(r => r is "Administrador" or "Agente" or "Cliente"))
        {
            ModelState.AddModelError(string.Empty, "No tienes permisos para acceder a esta aplicación.");
            return View(modelo);
        }

        var usuario = await _userManager.FindByIdAsync(respuesta.Id);
        if (usuario is null)
        {
            ModelState.AddModelError(string.Empty, "Usuario no encontrado.");
            return View(modelo);
        }

        await _signInManager.SignInAsync(usuario, modelo.RecordarSesion);

        // Redirigir según el rol del usuario
        if (respuesta.Roles.Contains("Administrador"))
            return RedirectToAction("Index", "Administrador");

        if (respuesta.Roles.Contains("Agente"))
            return RedirectToAction("Index", "Agente");

        if (respuesta.Roles.Contains("Cliente"))
            return RedirectToAction("Index", "Inicio");

        ModelState.AddModelError(string.Empty, "No tienes permisos para acceder a esta aplicación.");
        return View(modelo);
    }

    // ── Recuperar contraseña

    [AllowAnonymous]
    [HttpGet]
    public IActionResult RecuperarContrasena() => View(new RecuperarContrasenaViewModel());

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> RecuperarContrasena(RecuperarContrasenaViewModel modelo)
    {
        if (!ModelState.IsValid) return View(modelo);

        // Generamos el token aunque el correo no exista (evitamos enumerar usuarios)
        var resultado = await _servicioCuenta.GenerarTokenRecuperacionAsync(modelo.Correo);

        if (resultado is not null)
        {
            var enlace = Url.Action(
                "RestablecerContrasena", "Cuenta",
                new { usuarioId = resultado.Value.UsuarioId, token = resultado.Value.Token },
                Request.Scheme)!;

            var usuario = await _userManager.FindByEmailAsync(modelo.Correo);
            var nombre = usuario is not null ? $"{usuario.Nombre} {usuario.Apellido}".Trim() : modelo.Correo;

            try { await _servicioEmail.EnviarEmailRecuperacionAsync(modelo.Correo, nombre, enlace); }
            catch { /* no exponer fallos de SMTP al usuario */ }
        }

        // Siempre mostramos el mismo mensaje (seguridad)
        TempData["MensajeRecuperacion"] = "Si el correo está registrado, recibirás un enlace en breve.";
        return RedirectToAction(nameof(RecuperarContrasena));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult RestablecerContrasena(string usuarioId, string token)
    {
        if (string.IsNullOrWhiteSpace(usuarioId) || string.IsNullOrWhiteSpace(token))
            return RedirectToAction(nameof(Login));

        return View(new RestablecerContrasenaViewModel { UsuarioId = usuarioId, Token = token });
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> RestablecerContrasena(RestablecerContrasenaViewModel modelo)
    {
        if (!ModelState.IsValid) return View(modelo);

        var error = await _servicioCuenta.RestablecerContrasenaAsync(
            modelo.UsuarioId, modelo.Token, modelo.NuevaContrasena);

        if (error is not null)
        {
            ModelState.AddModelError(string.Empty, error);
            return View(modelo);
        }

        TempData["Exito"] = "Tu contraseña ha sido actualizada. Ya puedes iniciar sesión.";
        return RedirectToAction(nameof(Login));
    }

    // ── Cerrar sesión

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CerrarSesion()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Inicio");
    }

    // Acceso denegado

    [Authorize]
    [HttpGet]
    public IActionResult AccesoDenegado()
    {
        ViewBag.UrlRetorno = User.IsInRole("Administrador") ? Url.Action("Index", "Administrador")
            : User.IsInRole("Agente") ? Url.Action("Index", "Agente")
            : User.IsInRole("Desarrollador") ? Url.Action("MiPerfil", "Desarrollador")
            : Url.Action("Index", "Inicio");

        return View();
    }
}
