using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.DTOs.Cuenta;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IServicioCuenta
    {
        Task<RespuestaAutenticacion> LoginAsync(SolicitudLoginDto solicitud);
        Task<RespuestaRegistro> RegistrarClienteAsync(RegistrarUsuarioDto solicitud);
        Task<RespuestaRegistro> RegistrarAgenteAsync(RegistrarUsuarioDto solicitud);
        Task<RespuestaRegistro> RegistrarAdministradorAsync(RegistrarAdminDto solicitud);
        Task<RespuestaRegistro> RegistrarDesarrolladorAsync(RegistrarAdminDto solicitud);
        Task<string> ActivarCuentaAsync(string usuarioId, string token);
        Task<bool> CambiarEstadoUsuarioAsync(string usuarioId, bool estado);
        Task<IEnumerable<AgenteDto>> ObtenerAgentesAsync();
        Task<AgenteDto?> ObtenerAgentePorIdAsync(string id);
        Task<IEnumerable<AdminDto>> ObtenerAdministradoresAsync();
        Task<IEnumerable<DesarrolladorDto>> ObtenerDesarrolladoresAsync();
        Task EliminarAgenteYPropiedadesAsync(string agenteId);

        // Recuperación de contraseña
        /// <summary>
        /// Genera un token de restablecimiento de contraseña para el usuario con el correo indicado.
        /// Devuelve (usuarioId, token) si existe, o null si no se encontró el usuario.
        /// El servicio de correo (Issue #12) deberá usar estos datos para enviar el enlace.
        /// </summary>
        Task<(string UsuarioId, string Token)?> GenerarTokenRecuperacionAsync(string correo);

        /// <summary>
        /// Aplica el nuevo password usando el token generado por GenerarTokenRecuperacionAsync.
        /// Devuelve null en éxito o un mensaje de error.
        /// </summary>
        Task<string?> RestablecerContrasenaAsync(string usuarioId, string token, string nuevaContrasena);
    }
}
