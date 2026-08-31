using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using RealEstateApp.Core.Application.DTOs.Cuenta;
using RealEstateApp.Core.Application.Features.Mejoras.Queries;
using RealEstateApp.Core.Application.Features.Propiedades.Queries;
using RealEstateApp.Core.Application.Features.TipoPropiedades.Queries;
using RealEstateApp.Core.Application.Features.TipoVentas.Queries;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Infrastructure.Identity.Entidades;
using RealEstateApp.WebApp.Interfaces.Servicios;
using RealEstateApp.WebApp.ViewModels.Admin;
using RealEstateApp.WebApp.ViewModels.Agente;
using RealEstateApp.WebApp.ViewModels.Shared;

namespace RealEstateApp.WebApp.Servicios;

public class ServicioCuentaWebApp : IServicioCuentaWebApp
{
    private readonly IServicioCuenta _servicioCuentaCore;
    private readonly UserManager<UsuarioAplicacion> _userManager;
    private readonly IServicioArchivo _servicioArchivo;
    private readonly MediatR.IMediator _mediador;

    public ServicioCuentaWebApp(
        IServicioCuenta servicioCuentaCore,
        UserManager<UsuarioAplicacion> userManager,
        MediatR.IMediator mediador,
        IServicioArchivo servicioArchivo)
    {
        _servicioCuentaCore = servicioCuentaCore;
        _userManager = userManager;
        _mediador = mediador;
        _servicioArchivo = servicioArchivo;
    }

    // Autenticación
    public Task<RespuestaRegistro> RegistrarClienteAsync(RegistrarUsuarioDto solicitud)
        => _servicioCuentaCore.RegistrarClienteAsync(solicitud);

    public Task<RespuestaRegistro> RegistrarAgenteAsync(RegistrarUsuarioDto solicitud)
        => _servicioCuentaCore.RegistrarAgenteAsync(solicitud);

    public Task<RespuestaAutenticacion> LoginAsync(SolicitudLoginDto solicitud)
        => _servicioCuentaCore.LoginAsync(solicitud);

    public Task<string> ActivarCuentaAsync(string usuarioId, string token)
        => _servicioCuentaCore.ActivarCuentaAsync(usuarioId, token);

    public Task<(string UsuarioId, string Token)?> GenerarTokenRecuperacionAsync(string correo)
        => _servicioCuentaCore.GenerarTokenRecuperacionAsync(correo);

    public Task<string?> RestablecerContrasenaAsync(string usuarioId, string token, string nuevaContrasena)
        => _servicioCuentaCore.RestablecerContrasenaAsync(usuarioId, token, nuevaContrasena);

    // Catálogos
    public async Task<IEnumerable<SelectListItem>> ObtenerTiposPropiedadesAsync()
    {
        var result = await _mediador.Send(new ObtenerTiposPropiedadesQuery());
        return result.OrderBy(t => t.Nombre).Select(t => new SelectListItem(t.Nombre, t.Id.ToString())).ToList();
    }

    public async Task<IEnumerable<SelectListItem>> ObtenerTiposVentasAsync()
    {
        var result = await _mediador.Send(new ObtenerTiposVentasQuery());
        return result.OrderBy(t => t.Nombre).Select(t => new SelectListItem(t.Nombre, t.Id.ToString())).ToList();
    }

    public async Task<IEnumerable<SelectListItem>> ObtenerMejorasAsync()
    {
        var result = await _mediador.Send(new ObtenerMejorasQuery());
        return result.OrderBy(t => t.Nombre).Select(t => new SelectListItem(t.Nombre, t.Id.ToString())).ToList();
    }

    // Agentes (legado)
    public async Task<IEnumerable<(string Id, string Nombre, string UrlFoto)>> ObtenerAgentesActivosAsync()
    {
        var usuarios = await _userManager.GetUsersInRoleAsync("Agente");
        return usuarios
            .Where(u => u.EstaActivo)
            .OrderBy(u => u.Nombre)
            .ThenBy(u => u.Apellido)
            .Select(u => (u.Id, $"{u.Nombre} {u.Apellido}", u.UrlFoto ?? "/images/placeholder-agent.jpg"));
    }

    public async Task<(string Id, string Nombre, string UrlFoto, string Telefono, string Correo)?> ObtenerAgentePorIdAsync(string agenteId)
    {
        var u = await _userManager.FindByIdAsync(agenteId);
        if (u is null) return null;
        return (u.Id, $"{u.Nombre} {u.Apellido}", u.UrlFoto ?? "/images/placeholder-agent.jpg", u.Telefono, u.Email ?? string.Empty);
    }

    public async Task<EditarPerfilAgenteViewModel?> ObtenerPerfilAgenteAsync(string agenteId)
    {
        var u = await _userManager.FindByIdAsync(agenteId);
        if (u is null) return null;
        return new EditarPerfilAgenteViewModel { Nombre = u.Nombre, Apellido = u.Apellido, Telefono = u.Telefono };
    }

    public async Task EditarAgentePerfilAsync(string agenteId, EditarPerfilAgenteViewModel modelo)
    {
        var u = await _userManager.FindByIdAsync(agenteId);
        if (u is null) return;
        u.Nombre = modelo.Nombre;
        u.Apellido = modelo.Apellido;
        u.Telefono = modelo.Telefono;

        if (modelo.FotoUsuario is not null && modelo.FotoUsuario.Length > 0)
        {
            if (!string.IsNullOrEmpty(u.UrlFoto))
                await _servicioArchivo.EliminarImagenAsync(u.UrlFoto);

            u.UrlFoto = await _servicioArchivo.GuardarImagenAsync(modelo.FotoUsuario, "perfiles");
        }

        await _userManager.UpdateAsync(u);
    }

    // Perfil genérico (todos los roles)

    /// <inheritdoc/>
    public async Task<EditarPerfilViewModel?> ObtenerPerfilUsuarioAsync(string usuarioId)
    {
        var u = await _userManager.FindByIdAsync(usuarioId);
        if (u is null) return null;

        return new EditarPerfilViewModel
        {
            Nombre = u.Nombre,
            Apellido = u.Apellido,
            Telefono = u.Telefono,
            Email = u.Email ?? string.Empty,   // ← carga el email actual
            UrlFotoActual = u.UrlFoto
        };
    }

    /// <inheritdoc/>
    public async Task EditarPerfilUsuarioAsync(string usuarioId, EditarPerfilViewModel modelo)
    {
        var u = await _userManager.FindByIdAsync(usuarioId);
        if (u is null) return;

        u.Nombre = modelo.Nombre;
        u.Apellido = modelo.Apellido;
        u.Telefono = modelo.Telefono;

        // Actualizar email si fue modificado
        if (!string.IsNullOrWhiteSpace(modelo.Email) &&
            !string.Equals(u.Email, modelo.Email, StringComparison.OrdinalIgnoreCase))
        {
            await _userManager.SetEmailAsync(u, modelo.Email);
            await _userManager.SetUserNameAsync(u, modelo.Email);
        }

        // Guardar foto si se subió una nueva
        if (modelo.FotoUsuario is not null && modelo.FotoUsuario.Length > 0)
        {
            if (!string.IsNullOrEmpty(u.UrlFoto))
                await _servicioArchivo.EliminarImagenAsync(u.UrlFoto);

            u.UrlFoto = await _servicioArchivo.GuardarImagenAsync(modelo.FotoUsuario, "perfiles");
        }

        await _userManager.UpdateAsync(u);
    }

    /// <inheritdoc/>
    public async Task<string?> CambiarContrasenaAsync(string usuarioId, string contrasenaActual, string nuevaContrasena)
    {
        var u = await _userManager.FindByIdAsync(usuarioId);
        if (u is null) return "Usuario no encontrado.";

        var resultado = await _userManager.ChangePasswordAsync(u, contrasenaActual, nuevaContrasena);

        if (resultado.Succeeded) return null;

        var error = resultado.Errors.FirstOrDefault();
        return error?.Description ?? "No se pudo cambiar la contraseña.";
    }

    // Clientes (info para el Agente)

    /// <inheritdoc/>
    public async Task<List<ClienteResumenViewModel>> ObtenerInfoClientesAsync(IEnumerable<string> clienteIds)
    {
        var resultado = new List<ClienteResumenViewModel>();
        foreach (var id in clienteIds)
        {
            var u = await _userManager.FindByIdAsync(id);
            if (u is null) continue;

            resultado.Add(new ClienteResumenViewModel
            {
                Id = u.Id,
                NombreUsuario = u.UserName ?? id,
                NombreCompleto = $"{u.Nombre} {u.Apellido}".Trim(),
                UrlFoto = u.UrlFoto
            });
        }
        return resultado;
    }

    // Dashboard admin
    public async Task<DashboardViewModel> ObtenerEstadisticasAsync()
    {
        var propiedades = await _mediador.Send(new ObtenerTodasPropiedadesQuery());
        var propiedadesDisponibles = propiedades.Count(p => p.EstadoPropiedad == "Disponible");
        var propiedadesVendidas = propiedades.Count(p => p.EstadoPropiedad == "Vendida");

        var agentes = await _userManager.GetUsersInRoleAsync("Agente");
        var clientes = await _userManager.GetUsersInRoleAsync("Cliente");
        var devs = await _userManager.GetUsersInRoleAsync("Desarrollador");

        return new DashboardViewModel
        {
            PropiedadesDisponibles = propiedadesDisponibles,
            PropiedadesVendidas = propiedadesVendidas,
            AgentesActivos = agentes.Count(a => a.EstaActivo),
            AgentesInactivos = agentes.Count(a => !a.EstaActivo),
            ClientesActivos = clientes.Count(c => c.EstaActivo),
            ClientesInactivos = clientes.Count(c => !c.EstaActivo),
            DesarrolladoresActivos = devs.Count(d => d.EstaActivo),
            DesarrolladoresInactivos = devs.Count(d => !d.EstaActivo)
        };
    }

    public Task EliminarAgenteYPropiedadesAsync(string agenteId)
        => _servicioCuentaCore.EliminarAgenteYPropiedadesAsync(agenteId);
}