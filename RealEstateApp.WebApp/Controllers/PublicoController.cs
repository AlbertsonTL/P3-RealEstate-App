using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.WebApp.Interfaces.Servicios;
using System.Security.Claims;

namespace RealEstateApp.WebApp.Controllers;

[AllowAnonymous]
public class PublicoController : Controller
{
    private readonly IServicioPropiedad _servicioPropiedad;
    private readonly IServicioCuentaWebApp _servicioCuenta;
    private readonly IServicioChat _servicioChat;
    private readonly IServicioOferta _servicioOferta;

    public PublicoController(
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

    [HttpGet]
    public async Task<IActionResult> DetallePropiedad(int id)
    {
        var clienteId = User.IsInRole("Cliente") ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
        var detalle = await _servicioPropiedad.ObtenerDetalleAsync(id, clienteId);
        if (detalle is null) return NotFound();

        var agente = await _servicioCuenta.ObtenerAgentePorIdAsync(detalle.AgenteId);
        if (agente is not null)
        {
            detalle.NombreAgente = agente.Value.Nombre;
            detalle.UrlFotoAgente = agente.Value.UrlFoto;
            detalle.TelefonoAgente = agente.Value.Telefono;
            detalle.EmailAgente = agente.Value.Correo;
        }

        if (User.IsInRole("Cliente") && clienteId is not null)
        {
            detalle.Mensajes = await _servicioChat.ObtenerConversacionAsync(id, clienteId, detalle.AgenteId, clienteId);
            detalle.Ofertas = (await _servicioOferta.ObtenerOfertasClientePropiedadAsync(id, clienteId)).ToList();
        }

        if (User.IsInRole("Agente"))
        {
            var agenteActualId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            detalle.EsPropietarioAgente = string.Equals(agenteActualId, detalle.AgenteId, StringComparison.Ordinal);
        }

        return View(detalle);
    }
}
