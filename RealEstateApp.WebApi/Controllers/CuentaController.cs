using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.DTOs.Cuenta;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApp.WebApi.Controllers
{
    public class CuentaController : ControladorBaseApi
    {
        private readonly IServicioCuenta _servicioCuenta;

        public CuentaController(IServicioCuenta servicioCuenta)
        {
            _servicioCuenta = servicioCuenta;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RespuestaAutenticacion))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(SolicitudLoginDto solicitud)
        {
            var respuesta = await _servicioCuenta.LoginAsync(solicitud);

            if (respuesta.TieneError)
                return Unauthorized(respuesta.MensajeError);

            // ✅ BUG FIX #5: Solo Administrador y Desarrollador pueden usar la API.
            // Clientes y Agentes deben ser rechazados con 401.
            var rolesPermitidos = new[] { "Administrador", "Desarrollador" };
            if (!respuesta.Roles.Any(r => rolesPermitidos.Contains(r)))
                return Unauthorized("Acceso denegado. Solo Administradores y Desarrolladores pueden acceder a la API.");

            return Ok(respuesta);
        }

        [HttpPost("registrar-administrador")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Administrador")]
        public async Task<IActionResult> RegistrarAdministrador(RegistrarAdminDto solicitud)
        {
            var respuesta = await _servicioCuenta.RegistrarAdministradorAsync(solicitud);
            if (respuesta.TieneError) return BadRequest(respuesta.MensajeError);
            return Ok();
        }

        [HttpPost("registrar-desarrollador")]
        public async Task<IActionResult> RegistrarDesarrollador(RegistrarAdminDto solicitud)
        {
            var respuesta = await _servicioCuenta.RegistrarDesarrolladorAsync(solicitud);
            if (respuesta.TieneError) return BadRequest(respuesta.MensajeError);
            return Ok();
        }
    }
}
