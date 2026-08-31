using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.DTOs.Cuenta;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;
using RealEstateApp.Infrastructure.Identity.Entidades;

namespace RealEstateApp.Infrastructure.Identity.Servicios
{
    public class ServicioCuenta : IServicioCuenta
    {
        private readonly UserManager<UsuarioAplicacion> _userManager;
        private readonly SignInManager<UsuarioAplicacion> _signInManager;
        private readonly ServicioJwt _servicioJwt;
        private readonly IRepositorioPropiedad _repositorioPropiedad;
        private readonly IServicioEmail _servicioEmail;

        public ServicioCuenta(
            UserManager<UsuarioAplicacion> userManager,
            SignInManager<UsuarioAplicacion> signInManager,
            ServicioJwt servicioJwt,
            IRepositorioPropiedad repositorioPropiedad,
            IServicioEmail servicioEmail)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _servicioJwt = servicioJwt;
            _repositorioPropiedad = repositorioPropiedad;
            _servicioEmail = servicioEmail;
        }

        // ── Login ────────────────────────────────────────────────────────────

        public async Task<RespuestaAutenticacion> LoginAsync(SolicitudLoginDto solicitud)
        {
            // Buscar primero por nombre de usuario
            UsuarioAplicacion? u = await _userManager.FindByNameAsync(solicitud.UsuarioOCorreo);

            // Si no se encontró por username, intentar por correo de forma segura
            if (u == null)
            {
                try
                {
                    u = await _userManager.FindByEmailAsync(solicitud.UsuarioOCorreo);
                }
                catch (InvalidOperationException)
                {
                    // Existen múltiples cuentas con el mismo correo en la BD (datos legacy).
                    // Retornamos credenciales incorrectas para no exponer el problema.
                    return new() { TieneError = true, MensajeError = "Credenciales incorrectas." };
                }
            }

            if (u == null || !(await _signInManager.CheckPasswordSignInAsync(u, solicitud.Contrasena, false)).Succeeded)
                return new() { TieneError = true, MensajeError = "Credenciales incorrectas." };

            // Bloquear acceso a cuentas no activadas
            if (!u.EstaActivo)
                return new()
                {
                    TieneError = true,
                    MensajeError = "Tu cuenta aún no ha sido activada. Revisa tu correo electrónico y haz clic en el enlace de activación que te enviamos."
                };

            var roles = await _userManager.GetRolesAsync(u);
            return new()
            {
                Id = u.Id,
                NombreCompleto = $"{u.Nombre} {u.Apellido}",
                Email = u.Email!,
                Token = _servicioJwt.GenerarToken(u, roles),
                Roles = roles.ToList()
            };
        }

        // ── Registro ─────────────────────────────────────────────────────────

        public Task<RespuestaRegistro> RegistrarClienteAsync(RegistrarUsuarioDto s)
            => RegistrarBase(s, "Cliente", activo: false);   // espera confirmación de correo

        public Task<RespuestaRegistro> RegistrarAgenteAsync(RegistrarUsuarioDto s)
            => RegistrarBase(s, "Agente", activo: false);

        public Task<RespuestaRegistro> RegistrarAdministradorAsync(RegistrarAdminDto s)
            => RegistrarAdminBase(s, "Administrador");

        public Task<RespuestaRegistro> RegistrarDesarrolladorAsync(RegistrarAdminDto s)
            => RegistrarAdminBase(s, "Desarrollador");

        private async Task<RespuestaRegistro> RegistrarBase(RegistrarUsuarioDto s, string rol, bool activo)
        {
            var u = new UsuarioAplicacion
            {
                UserName = s.NombreUsuario,
                Email = s.Correo,
                Nombre = s.Nombre,
                Apellido = s.Apellido,
                Telefono = s.Telefono,
                EstaActivo = activo
            };

            var r = await _userManager.CreateAsync(u, s.Contrasena);
            if (!r.Succeeded)
                return new() { TieneError = true, MensajeError = string.Join(", ", r.Errors.Select(e => e.Description)) };

            await _userManager.AddToRoleAsync(u, rol);

            // Generar token de confirmación y devolverlo al controller.
            // El controller tiene acceso a HttpContext/Url y es quien construye
            // la URL completa antes de enviar el correo de activación.
            var rawToken = await _userManager.GenerateEmailConfirmationTokenAsync(u);
            var tokenB64 = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken));

            return new()
            {
                TieneError = false,
                UsuarioId = u.Id,
                TokenActivacion = tokenB64
            };
        }

        private async Task<RespuestaRegistro> RegistrarAdminBase(RegistrarAdminDto s, string rol)
        {
            var u = new UsuarioAplicacion
            {
                UserName = s.NombreUsuario,
                Email = s.Correo,
                Nombre = s.Nombre,
                Apellido = s.Apellido,
                Cedula = s.Cedula,
                EstaActivo = true
            };

            var r = await _userManager.CreateAsync(u, s.Contrasena);
            if (r.Succeeded) await _userManager.AddToRoleAsync(u, rol);

            return new()
            {
                TieneError = !r.Succeeded,
                MensajeError = r.Succeeded ? null : string.Join(", ", r.Errors.Select(e => e.Description))
            };
        }

        // ── Activar cuenta ───────────────────────────────────────────────────

        public async Task<string> ActivarCuentaAsync(string usuarioId, string tokenB64)
        {
            var u = await _userManager.FindByIdAsync(usuarioId);
            if (u == null) return "Usuario no encontrado.";

            var rawToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(tokenB64));
            var result = await _userManager.ConfirmEmailAsync(u, rawToken);

            if (!result.Succeeded)
                return "El enlace de activación no es válido o ya fue usado.";

            // Marcar activo y enviar correo de bienvenida
            u.EstaActivo = true;
            await _userManager.UpdateAsync(u);

            try { await _servicioEmail.EnviarEmailBienvenidaAsync(u.Email!, $"{u.Nombre} {u.Apellido}"); }
            catch { /* no bloquear si el SMTP falla */ }

            return "¡Cuenta activada correctamente! Ya puedes iniciar sesión.";
        }

        // ── Recuperar contraseña ─────────────────────────────────────────────

        public async Task<(string UsuarioId, string Token)?> GenerarTokenRecuperacionAsync(string correo)
        {
            var u = await _userManager.FindByEmailAsync(correo);
            if (u == null) return null;

            var rawToken = await _userManager.GeneratePasswordResetTokenAsync(u);
            var tokenB64 = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken));
            return (u.Id, tokenB64);
        }

        public async Task<string?> RestablecerContrasenaAsync(string usuarioId, string tokenB64, string nuevaContrasena)
        {
            var u = await _userManager.FindByIdAsync(usuarioId);
            if (u == null) return "Usuario no encontrado.";

            var rawToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(tokenB64));
            var result = await _userManager.ResetPasswordAsync(u, rawToken, nuevaContrasena);

            if (result.Succeeded) return null;
            return string.Join(", ", result.Errors.Select(e => e.Description));
        }

        // ── Activar/Inactivar ────────────────────────────────────────────────

        public async Task<bool> CambiarEstadoUsuarioAsync(string id, bool estado)
        {
            var u = await _userManager.FindByIdAsync(id);
            if (u == null) return false;
            u.EstaActivo = estado;
            await _userManager.UpdateAsync(u);
            return true;
        }

        // ── Consultas ────────────────────────────────────────────────────────

        public async Task<IEnumerable<AgenteDto>> ObtenerAgentesAsync()
        {
            var us = await _userManager.GetUsersInRoleAsync("Agente");
            return us.Select(u => new AgenteDto { Id = u.Id, Nombre = u.Nombre, Apellido = u.Apellido });
        }

        public async Task<AgenteDto?> ObtenerAgentePorIdAsync(string id)
        {
            var u = await _userManager.FindByIdAsync(id);
            return u == null ? null : new AgenteDto { Id = u.Id, Nombre = u.Nombre };
        }

        public async Task<IEnumerable<AdminDto>> ObtenerAdministradoresAsync()
        {
            var us = await _userManager.GetUsersInRoleAsync("Administrador");
            return us.Select(u => new AdminDto { Id = u.Id, Nombre = u.Nombre });
        }

        public async Task<IEnumerable<DesarrolladorDto>> ObtenerDesarrolladoresAsync()
        {
            var us = await _userManager.GetUsersInRoleAsync("Desarrollador");
            return us.Select(u => new DesarrolladorDto { Id = u.Id, Nombre = u.Nombre });
        }

        public async Task EliminarAgenteYPropiedadesAsync(string id)
        {
            var u = await _userManager.FindByIdAsync(id);
            if (u != null) await _userManager.DeleteAsync(u);
        }
    }
}
