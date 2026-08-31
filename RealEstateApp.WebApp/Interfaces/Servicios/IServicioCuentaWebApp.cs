using Microsoft.AspNetCore.Mvc.Rendering;
using RealEstateApp.Core.Application.DTOs.Cuenta;
using RealEstateApp.WebApp.ViewModels.Admin;
using RealEstateApp.WebApp.ViewModels.Agente;
using RealEstateApp.WebApp.ViewModels.Shared;

namespace RealEstateApp.WebApp.Interfaces.Servicios;

public interface IServicioCuentaWebApp
{
    // Autenticación y registro
    Task<RespuestaRegistro> RegistrarClienteAsync(RegistrarUsuarioDto solicitud);
    Task<RespuestaRegistro> RegistrarAgenteAsync(RegistrarUsuarioDto solicitud);
    Task<RespuestaAutenticacion> LoginAsync(SolicitudLoginDto solicitud);
    Task<string> ActivarCuentaAsync(string usuarioId, string token);

    // Recuperación de contraseña
    /// <summary>
    /// Inicia el flujo de recuperación. Devuelve (usuarioId, token) para que el
    /// servicio de correo (Issue #12) envíe el enlace. Devuelve null si el correo no existe.
    /// </summary>
    Task<(string UsuarioId, string Token)?> GenerarTokenRecuperacionAsync(string correo);

    /// <summary>
    /// Aplica el restablecimiento de contraseña. Devuelve null en éxito o un mensaje de error.
    /// </summary>
    Task<string?> RestablecerContrasenaAsync(string usuarioId, string token, string nuevaContrasena);

    // Catálogos
    Task<IEnumerable<SelectListItem>> ObtenerTiposPropiedadesAsync();
    Task<IEnumerable<SelectListItem>> ObtenerTiposVentasAsync();
    Task<IEnumerable<SelectListItem>> ObtenerMejorasAsync();

    // Agentes (perfil legado + listados)
    Task<IEnumerable<(string Id, string Nombre, string UrlFoto)>> ObtenerAgentesActivosAsync();
    Task<(string Id, string Nombre, string UrlFoto, string Telefono, string Correo)?> ObtenerAgentePorIdAsync(string agenteId);

    /// <summary>Obtiene el ViewModel de edición de perfil del Agente (legado).</summary>
    Task<EditarPerfilAgenteViewModel?> ObtenerPerfilAgenteAsync(string agenteId);

    /// <summary>Guarda los cambios de perfil del Agente incluyendo foto (legado).</summary>
    Task EditarAgentePerfilAsync(string agenteId, EditarPerfilAgenteViewModel modelo);

    // Perfil genérico (todos los roles)

    /// <summary>
    /// Devuelve el perfil editable de cualquier usuario identificado por su Id.
    /// Retorna null si el usuario no existe.
    /// </summary>
    Task<EditarPerfilViewModel?> ObtenerPerfilUsuarioAsync(string usuarioId);

    /// <summary>
    /// Persiste los cambios de nombre, apellido, teléfono y foto de perfil.
    /// Guarda la foto en /images/perfiles/ y actualiza UrlFoto del usuario.
    /// </summary>
    Task EditarPerfilUsuarioAsync(string usuarioId, EditarPerfilViewModel modelo);

    /// <summary>
    /// Cambia la contraseña del usuario verificando la contraseña actual.
    /// Devuelve null si tuvo éxito, o un mensaje de error descriptivo si falló.
    /// </summary>
    Task<string?> CambiarContrasenaAsync(string usuarioId, string contrasenaActual, string nuevaContrasena);

    // Clientes (info para el Agente)

    /// <summary>
    /// Obtiene los datos de resumen (username, nombre completo, foto) de una lista de Ids de clientes.
    /// Los Ids que no existan en la BD son omitidos.
    /// </summary>
    Task<List<ClienteResumenViewModel>> ObtenerInfoClientesAsync(IEnumerable<string> clienteIds);

    // Dashboard admin
    Task<DashboardViewModel> ObtenerEstadisticasAsync();

    Task EliminarAgenteYPropiedadesAsync(string agenteId);
}
