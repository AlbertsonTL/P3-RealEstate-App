using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Infrastructure.Identity.Entidades;
using RealEstateApp.WebApp.Interfaces.Servicios;
using RealEstateApp.WebApp.ViewModels.Admin;
using RealEstateApp.WebApp.ViewModels.Shared;
using System.Security.Claims;

namespace RealEstateApp.WebApp.Controllers;

[Authorize(Roles = "Administrador")]
public class AdministradorController : Controller
{
    private readonly IServicioCuentaWebApp _servicioCuenta;
    private readonly IServicioCatalogoAdmin _servicioCatalogo;
    private readonly UserManager<UsuarioAplicacion> _userManager;
    public AdministradorController(
        IServicioCuentaWebApp servicioCuenta,
        IServicioCatalogoAdmin servicioCatalogo,
        UserManager<UsuarioAplicacion> userManager)
    {
        _servicioCuenta = servicioCuenta;
        _servicioCatalogo = servicioCatalogo;
        _userManager = userManager;
    }

    // Dashboard
    public async Task<IActionResult> Index()
    {
        var vm = await _servicioCuenta.ObtenerEstadisticasAsync();
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var vm = await _servicioCuenta.ObtenerEstadisticasAsync();
        return View("Index", vm);
    }

    // Mi Perfil
    [HttpGet]
    public async Task<IActionResult> MiPerfil()
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        ViewBag.PerfilViewModel = await _servicioCuenta.ObtenerPerfilUsuarioAsync(usuarioId) ?? new EditarPerfilViewModel();
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
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        if (!ModelState.IsValid)
        {
            ViewBag.PerfilViewModel = await _servicioCuenta.ObtenerPerfilUsuarioAsync(usuarioId) ?? new EditarPerfilViewModel();
            ViewBag.ContrasenaViewModel = modelo;
            ViewBag.SeccionActiva = "contrasena";
            return View("MiPerfil");
        }
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

    // Agentes
    [HttpGet]
    public async Task<IActionResult> Agentes()
    {
        var agentes = await _userManager.GetUsersInRoleAsync("Agente");
        var propiedadesPorAgente = await _servicioCatalogo.ObtenerCantidadPropiedadesPorAgenteAsync();
        var vm = agentes
            .OrderBy(a => a.Nombre).ThenBy(a => a.Apellido)
            .Select(a => new ListadoAgentesAdminViewModel
            {
                Id = a.Id,
                Nombre = a.Nombre,
                Apellido = a.Apellido,
                Correo = a.Email ?? string.Empty,
                EstaActivo = a.EstaActivo,
                CantidadPropiedades = propiedadesPorAgente.TryGetValue(a.Id, out var c) ? c : 0
            }).ToList();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstadoAgente(string id)
    {
        var agente = await _userManager.FindByIdAsync(id);
        if (agente is null || !await _userManager.IsInRoleAsync(agente, "Agente"))
        {
            TempData["Error"] = "El agente no existe.";
            return RedirectToAction(nameof(Agentes));
        }
        agente.EstaActivo = !agente.EstaActivo;
        var resultado = await _userManager.UpdateAsync(agente);
        TempData[resultado.Succeeded ? "Exito" : "Error"] = resultado.Succeeded
            ? $"Estado actualizado: {(agente.EstaActivo ? "Activo" : "Inactivo")}."
            : "No fue posible actualizar el estado del agente.";
        return RedirectToAction(nameof(Agentes));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarAgente(string id)
    {
        var agente = await _userManager.FindByIdAsync(id);
        if (agente is null || !await _userManager.IsInRoleAsync(agente, "Agente"))
        {
            TempData["Error"] = "El agente no existe.";
            return RedirectToAction(nameof(Agentes));
        }

        try
        {
            await _servicioCuenta.EliminarAgenteYPropiedadesAsync(id);
            TempData["Exito"] = "Agente y propiedades asociadas eliminados correctamente.";
            return RedirectToAction(nameof(Agentes));
        }
        catch (Exception)
        {
            TempData["Error"] = "No fue posible eliminar el agente y sus propiedades asociadas.";
            return RedirectToAction(nameof(Agentes));
        }
    }

    // Administradores
    [HttpGet]
    public async Task<IActionResult> Administradores()
    {
        var admins = await _userManager.GetUsersInRoleAsync("Administrador");
        return View(admins.OrderBy(x => x.Nombre).ThenBy(x => x.Apellido).ToList());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearAdministrador(CrearAdminViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = ObtenerErroresModelState();
            return RedirectToAction(nameof(Administradores));
        }
        var resultado = await CrearUsuarioPorRolAsync(modelo, "Administrador", true);
        TempData[resultado.Exito ? "Exito" : "Error"] = resultado.Exito
            ? "Administrador creado correctamente."
            : resultado.Error ?? "No fue posible crear el administrador.";
        return RedirectToAction(nameof(Administradores));
    }

    // Fix 4: bloquea auto-edición en GET
    [HttpGet]
    public async Task<IActionResult> EditarAdministrador(string id)
    {
        var usuarioActualId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.Equals(id, usuarioActualId, StringComparison.Ordinal))
        {
            TempData["Error"] = "Para editar tus propios datos usa la sección \"Mi Perfil\".";
            return RedirectToAction(nameof(Administradores));
        }
        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario is null || !await _userManager.IsInRoleAsync(usuario, "Administrador"))
        {
            TempData["Error"] = "El administrador no existe.";
            return RedirectToAction(nameof(Administradores));
        }
        return View(new EditarAdminViewModel
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Cedula = usuario.Cedula ?? string.Empty,
            Telefono = usuario.Telefono,
            Correo = usuario.Email ?? string.Empty,
            NombreUsuario = usuario.UserName ?? string.Empty
        });
    }

    // Fix 4: bloquea auto-edición en POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarAdministrador(EditarAdminViewModel modelo)
    {
        var usuarioActualId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.Equals(modelo.Id, usuarioActualId, StringComparison.Ordinal))
        {
            TempData["Error"] = "Para editar tus propios datos usa la sección \"Mi Perfil\".";
            return RedirectToAction(nameof(Administradores));
        }
        if (!ModelState.IsValid)
        {
            TempData["Error"] = ObtenerErroresModelState();
            return View(modelo);
        }
        var resultado = await EditarUsuarioPorRolAsync(modelo, "Administrador");
        if (!resultado.Exito)
        {
            TempData["Error"] = resultado.Error ?? "No fue posible actualizar el administrador.";
            return View(modelo);
        }
        TempData["Exito"] = "Administrador actualizado correctamente.";
        return RedirectToAction(nameof(Administradores));
    }

    // Fix 5: Activar / Inactivar administrador
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstadoAdministrador(string id)
    {
        var usuarioActualId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.Equals(id, usuarioActualId, StringComparison.Ordinal))
        {
            TempData["Error"] = "No puedes cambiar tu propio estado.";
            return RedirectToAction(nameof(Administradores));
        }
        var admin = await _userManager.FindByIdAsync(id);
        if (admin is null || !await _userManager.IsInRoleAsync(admin, "Administrador"))
        {
            TempData["Error"] = "El administrador no existe.";
            return RedirectToAction(nameof(Administradores));
        }
        admin.EstaActivo = !admin.EstaActivo;
        var resultado = await _userManager.UpdateAsync(admin);
        TempData[resultado.Succeeded ? "Exito" : "Error"] = resultado.Succeeded
            ? $"Estado actualizado: {(admin.EstaActivo ? "Activo" : "Inactivo")}."
            : "No fue posible actualizar el estado del administrador.";
        return RedirectToAction(nameof(Administradores));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarAdministrador(string id)
    {
        var usuarioActualId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.Equals(id, usuarioActualId, StringComparison.Ordinal))
        {
            TempData["Error"] = "No puedes eliminar tu propio usuario.";
            return RedirectToAction(nameof(Administradores));
        }
        var resultado = await EliminarUsuarioPorRolAsync(id, "Administrador");
        TempData[resultado ? "Exito" : "Error"] = resultado
            ? "Administrador eliminado correctamente."
            : "No fue posible eliminar el administrador.";
        return RedirectToAction(nameof(Administradores));
    }

    // Desarrolladores
    [HttpGet]
    public async Task<IActionResult> Desarrolladores()
    {
        var desarrolladores = await _userManager.GetUsersInRoleAsync("Desarrollador");
        return View(desarrolladores.OrderBy(x => x.Nombre).ThenBy(x => x.Apellido).ToList());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearDesarrollador(CrearAdminViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = ObtenerErroresModelState();
            return RedirectToAction(nameof(Desarrolladores));
        }
        var resultado = await CrearUsuarioPorRolAsync(modelo, "Desarrollador", true);
        TempData[resultado.Exito ? "Exito" : "Error"] = resultado.Exito
            ? "Desarrollador creado correctamente."
            : resultado.Error ?? "No fue posible crear el desarrollador.";
        return RedirectToAction(nameof(Desarrolladores));
    }

    [HttpGet]
    public async Task<IActionResult> EditarDesarrollador(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario is null || !await _userManager.IsInRoleAsync(usuario, "Desarrollador"))
        {
            TempData["Error"] = "El desarrollador no existe.";
            return RedirectToAction(nameof(Desarrolladores));
        }
        return View(new EditarAdminViewModel
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Cedula = usuario.Cedula ?? string.Empty,
            Telefono = usuario.Telefono,
            Correo = usuario.Email ?? string.Empty,
            NombreUsuario = usuario.UserName ?? string.Empty
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarDesarrollador(EditarAdminViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = ObtenerErroresModelState();
            return View(modelo);
        }
        var resultado = await EditarUsuarioPorRolAsync(modelo, "Desarrollador");
        if (!resultado.Exito)
        {
            TempData["Error"] = resultado.Error ?? "No fue posible actualizar el desarrollador.";
            return View(modelo);
        }
        TempData["Exito"] = "Desarrollador actualizado correctamente.";
        return RedirectToAction(nameof(Desarrolladores));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarDesarrollador(string id)
    {
        var resultado = await EliminarUsuarioPorRolAsync(id, "Desarrollador");
        TempData[resultado ? "Exito" : "Error"] = resultado
            ? "Desarrollador eliminado correctamente."
            : "No fue posible eliminar el desarrollador.";
        return RedirectToAction(nameof(Desarrolladores));
    }

    // Clientes
    [HttpGet]
    public async Task<IActionResult> Clientes()
    {
        var clientes = await _userManager.GetUsersInRoleAsync("Cliente");
        return View(clientes.OrderBy(x => x.Nombre).ThenBy(x => x.Apellido).ToList());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearCliente(CrearAdminViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = ObtenerErroresModelState();
            return RedirectToAction(nameof(Clientes));
        }
        // Los clientes creados por un administrador quedan activos de inmediato
        // (no requieren pasar por el correo de activación, a diferencia del autoregistro público).
        var resultado = await CrearUsuarioPorRolAsync(modelo, "Cliente", true);
        TempData[resultado.Exito ? "Exito" : "Error"] = resultado.Exito
            ? "Cliente creado correctamente."
            : resultado.Error ?? "No fue posible crear el cliente.";
        return RedirectToAction(nameof(Clientes));
    }

    [HttpGet]
    public async Task<IActionResult> EditarCliente(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario is null || !await _userManager.IsInRoleAsync(usuario, "Cliente"))
        {
            TempData["Error"] = "El cliente no existe.";
            return RedirectToAction(nameof(Clientes));
        }
        return View(new EditarAdminViewModel
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Cedula = usuario.Cedula ?? string.Empty,
            Telefono = usuario.Telefono,
            Correo = usuario.Email ?? string.Empty,
            NombreUsuario = usuario.UserName ?? string.Empty
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarCliente(EditarAdminViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = ObtenerErroresModelState();
            return View(modelo);
        }
        var resultado = await EditarUsuarioPorRolAsync(modelo, "Cliente");
        if (!resultado.Exito)
        {
            TempData["Error"] = resultado.Error ?? "No fue posible actualizar el cliente.";
            return View(modelo);
        }
        TempData["Exito"] = "Cliente actualizado correctamente.";
        return RedirectToAction(nameof(Clientes));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstadoCliente(string id)
    {
        var cliente = await _userManager.FindByIdAsync(id);
        if (cliente is null || !await _userManager.IsInRoleAsync(cliente, "Cliente"))
        {
            TempData["Error"] = "El cliente no existe.";
            return RedirectToAction(nameof(Clientes));
        }
        cliente.EstaActivo = !cliente.EstaActivo;
        var resultado = await _userManager.UpdateAsync(cliente);
        TempData[resultado.Succeeded ? "Exito" : "Error"] = resultado.Succeeded
            ? $"Estado actualizado: {(cliente.EstaActivo ? "Activo" : "Inactivo")}."
            : "No fue posible actualizar el estado del cliente.";
        return RedirectToAction(nameof(Clientes));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarCliente(string id)
    {
        var resultado = await EliminarUsuarioPorRolAsync(id, "Cliente");
        TempData[resultado ? "Exito" : "Error"] = resultado
            ? "Cliente eliminado correctamente."
            : "No fue posible eliminar el cliente.";
        return RedirectToAction(nameof(Clientes));
    }

    // Catálogos — Tipos de Propiedad (Fix 1 + Fix 7)
    [HttpGet]
    public async Task<IActionResult> TiposPropiedades()
    {
        var vm = await _servicioCatalogo.ObtenerTiposPropiedadesConConteoAsync();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearTipoPropiedad(CatalogoFormViewModel modelo)
    {
        if (!ModelState.IsValid) { TempData["Error"] = "Debes completar nombre y descripción."; return RedirectToAction(nameof(TiposPropiedades)); }
        await _servicioCatalogo.CrearTipoPropiedadAsync(modelo);
        TempData["Exito"] = "Tipo de propiedad creado correctamente.";
        return RedirectToAction(nameof(TiposPropiedades));
    }

    [HttpGet]
    public async Task<IActionResult> EditarTipoPropiedad(int id)
    {
        var tipo = await _servicioCatalogo.ObtenerTipoPropiedadPorIdAsync(id);
        if (tipo is null) { TempData["Error"] = "El tipo de propiedad no existe."; return RedirectToAction(nameof(TiposPropiedades)); }
        return View(new CatalogoFormViewModel { Id = tipo.Id, Nombre = tipo.Nombre, Descripcion = tipo.Descripcion });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarTipoPropiedad(CatalogoFormViewModel modelo)
    {
        if (!ModelState.IsValid) { TempData["Error"] = "Debes completar nombre y descripción."; return View(modelo); }
        try { await _servicioCatalogo.ActualizarTipoPropiedadAsync(modelo); TempData["Exito"] = "Tipo de propiedad actualizado correctamente."; }
        catch (KeyNotFoundException) { TempData["Error"] = "El tipo de propiedad no existe."; }
        return RedirectToAction(nameof(TiposPropiedades));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarTipoPropiedad(int id)
    {
        var (exito, error) = await _servicioCatalogo.EliminarTipoPropiedadAsync(id);
        TempData[exito ? "Exito" : "Error"] = exito ? "Tipo de propiedad eliminado correctamente." : error;
        return RedirectToAction(nameof(TiposPropiedades));
    }

    // Catálogos — Tipos de Venta (Fix 1 + Fix 7)
    [HttpGet]
    public async Task<IActionResult> TiposVentas()
    {
        var vm = await _servicioCatalogo.ObtenerTiposVentasConConteoAsync();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearTipoVenta(CatalogoFormViewModel modelo)
    {
        if (!ModelState.IsValid) { TempData["Error"] = "Debes completar nombre y descripción."; return RedirectToAction(nameof(TiposVentas)); }
        await _servicioCatalogo.CrearTipoVentaAsync(modelo);
        TempData["Exito"] = "Tipo de venta creado correctamente.";
        return RedirectToAction(nameof(TiposVentas));
    }

    [HttpGet]
    public async Task<IActionResult> EditarTipoVenta(int id)
    {
        var tipo = await _servicioCatalogo.ObtenerTipoVentaPorIdAsync(id);
        if (tipo is null) { TempData["Error"] = "El tipo de venta no existe."; return RedirectToAction(nameof(TiposVentas)); }
        return View(new CatalogoFormViewModel { Id = tipo.Id, Nombre = tipo.Nombre, Descripcion = tipo.Descripcion });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarTipoVenta(CatalogoFormViewModel modelo)
    {
        if (!ModelState.IsValid) { TempData["Error"] = "Debes completar nombre y descripción."; return View(modelo); }
        try { await _servicioCatalogo.ActualizarTipoVentaAsync(modelo); TempData["Exito"] = "Tipo de venta actualizado correctamente."; }
        catch (KeyNotFoundException) { TempData["Error"] = "El tipo de venta no existe."; }
        return RedirectToAction(nameof(TiposVentas));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarTipoVenta(int id)
    {
        var (exito, error) = await _servicioCatalogo.EliminarTipoVentaAsync(id);
        TempData[exito ? "Exito" : "Error"] = exito ? "Tipo de venta eliminado correctamente." : error;
        return RedirectToAction(nameof(TiposVentas));
    }

    // Catálogos — Mejoras (Fix 1)
    [HttpGet]
    public async Task<IActionResult> Mejoras()
    {
        var mejoras = await _servicioCatalogo.ObtenerMejorasAsync();
        return View(mejoras);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearMejora(CatalogoFormViewModel modelo)
    {
        if (!ModelState.IsValid) { TempData["Error"] = "Debes completar nombre y descripción."; return RedirectToAction(nameof(Mejoras)); }
        await _servicioCatalogo.CrearMejoraAsync(modelo);
        TempData["Exito"] = "Mejora creada correctamente.";
        return RedirectToAction(nameof(Mejoras));
    }

    [HttpGet]
    public async Task<IActionResult> EditarMejora(int id)
    {
        var mejora = await _servicioCatalogo.ObtenerMejoraPorIdAsync(id);
        if (mejora is null) { TempData["Error"] = "La mejora no existe."; return RedirectToAction(nameof(Mejoras)); }
        return View(new CatalogoFormViewModel { Id = mejora.Id, Nombre = mejora.Nombre, Descripcion = mejora.Descripcion });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarMejora(CatalogoFormViewModel modelo)
    {
        if (!ModelState.IsValid) { TempData["Error"] = "Debes completar nombre y descripción."; return View(modelo); }
        try { await _servicioCatalogo.ActualizarMejoraAsync(modelo); TempData["Exito"] = "Mejora actualizada correctamente."; }
        catch (KeyNotFoundException) { TempData["Error"] = "La mejora no existe."; }
        return RedirectToAction(nameof(Mejoras));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarMejora(int id)
    {
        var (exito, error) = await _servicioCatalogo.EliminarMejoraAsync(id);
        TempData[exito ? "Exito" : "Error"] = exito ? "Mejora eliminada correctamente." : error;
        return RedirectToAction(nameof(Mejoras));
    }

    // Helpers privados

    /// <summary>Resultado de una operación de creación/edición de usuario, con mensaje de error en español.</summary>
    private readonly record struct ResultadoOperacion(bool Exito, string? Error)
    {
        public static ResultadoOperacion Ok() => new(true, null);
        public static ResultadoOperacion Fallo(string error) => new(false, error);
    }

    private async Task<ResultadoOperacion> CrearUsuarioPorRolAsync(CrearAdminViewModel modelo, string rol, bool estaActivo)
    {
        var nombreUsuario = modelo.NombreUsuario.Trim();
        var correo = modelo.Correo.Trim();

        if (await _userManager.FindByNameAsync(nombreUsuario) is not null)
            return ResultadoOperacion.Fallo("El nombre de usuario ya está en uso. Por favor elige otro.");

        if (await _userManager.FindByEmailAsync(correo) is not null)
            return ResultadoOperacion.Fallo("Este correo electrónico ya está registrado.");

        var usuario = new UsuarioAplicacion
        {
            UserName = nombreUsuario,
            Email = correo,
            Nombre = modelo.Nombre.Trim(),
            Apellido = modelo.Apellido.Trim(),
            Cedula = modelo.Cedula.Trim(),
            Telefono = modelo.Telefono.Trim(),
            PhoneNumber = modelo.Telefono.Trim(),
            EstaActivo = estaActivo,
            FechaRegistro = DateTime.UtcNow,
            // Los usuarios creados directamente por un administrador ya se consideran
            // verificados: no necesitan pasar por el flujo de activación por correo.
            EmailConfirmed = true
        };

        var resultado = await _userManager.CreateAsync(usuario, modelo.Contrasena);
        if (!resultado.Succeeded)
            return ResultadoOperacion.Fallo(string.Join(" ", resultado.Errors.Select(e => e.Description)));

        var resultadoRol = await _userManager.AddToRoleAsync(usuario, rol);
        if (resultadoRol.Succeeded) return ResultadoOperacion.Ok();

        await _userManager.DeleteAsync(usuario);
        return ResultadoOperacion.Fallo(string.Join(" ", resultadoRol.Errors.Select(e => e.Description)));
    }

    private async Task<ResultadoOperacion> EditarUsuarioPorRolAsync(EditarAdminViewModel modelo, string rol)
    {
        var usuario = await _userManager.FindByIdAsync(modelo.Id);
        if (usuario is null || !await _userManager.IsInRoleAsync(usuario, rol))
            return ResultadoOperacion.Fallo("El usuario no existe.");

        var nombreUsuario = modelo.NombreUsuario.Trim();
        var correo = modelo.Correo.Trim();

        var u1 = await _userManager.FindByNameAsync(nombreUsuario);
        if (u1 is not null && !string.Equals(u1.Id, usuario.Id, StringComparison.Ordinal))
            return ResultadoOperacion.Fallo("El nombre de usuario ya está en uso por otro usuario.");

        var u2 = await _userManager.FindByEmailAsync(correo);
        if (u2 is not null && !string.Equals(u2.Id, usuario.Id, StringComparison.Ordinal))
            return ResultadoOperacion.Fallo("Este correo electrónico ya está registrado por otro usuario.");

        usuario.Nombre = modelo.Nombre.Trim();
        usuario.Apellido = modelo.Apellido.Trim();
        usuario.Cedula = modelo.Cedula.Trim();
        usuario.Telefono = modelo.Telefono.Trim();
        usuario.PhoneNumber = modelo.Telefono.Trim();

        if (!string.Equals(usuario.UserName, nombreUsuario, StringComparison.Ordinal))
        {
            var rNombre = await _userManager.SetUserNameAsync(usuario, nombreUsuario);
            if (!rNombre.Succeeded)
                return ResultadoOperacion.Fallo(string.Join(" ", rNombre.Errors.Select(e => e.Description)));
        }

        if (!string.Equals(usuario.Email, correo, StringComparison.OrdinalIgnoreCase))
        {
            var rCorreo = await _userManager.SetEmailAsync(usuario, correo);
            if (!rCorreo.Succeeded)
                return ResultadoOperacion.Fallo(string.Join(" ", rCorreo.Errors.Select(e => e.Description)));
        }

        if (!string.IsNullOrWhiteSpace(modelo.Contrasena))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
            var rp = await _userManager.ResetPasswordAsync(usuario, token, modelo.Contrasena);
            if (!rp.Succeeded)
                return ResultadoOperacion.Fallo(string.Join(" ", rp.Errors.Select(e => e.Description)));
        }

        var resultadoFinal = await _userManager.UpdateAsync(usuario);
        return resultadoFinal.Succeeded
            ? ResultadoOperacion.Ok()
            : ResultadoOperacion.Fallo(string.Join(" ", resultadoFinal.Errors.Select(e => e.Description)));
    }

    private async Task<bool> EliminarUsuarioPorRolAsync(string id, string rol)
    {
        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario is null || !await _userManager.IsInRoleAsync(usuario, rol)) return false;
        return (await _userManager.DeleteAsync(usuario)).Succeeded;
    }

    /// <summary>Une los mensajes de validación del ModelState en un solo texto en español.</summary>
    private string ObtenerErroresModelState()
        => string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
}
