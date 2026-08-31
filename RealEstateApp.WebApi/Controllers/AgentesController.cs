using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.Agentes.Commands;
using RealEstateApp.Core.Application.Features.Agentes.Queries;

namespace RealEstateApp.WebApi.Controllers
{
    [Authorize(Roles = "Administrador,Desarrollador")]
    public class AgentesController : ControladorBaseApi
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AgenteDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> List()
        {
            var agentes = await Mediador.Send(new ObtenerTodosAgentesQuery());
            if (agentes == null || !agentes.Any()) return NoContent();
            return Ok(agentes);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AgenteDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetById(string id)
        {
            var agente = await Mediador.Send(new ObtenerAgentePorIdQuery { Id = id });
            if (agente is null) return NoContent();
            return Ok(agente);
        }

        [HttpGet("{id}/propiedades")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PropiedadDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAgentProperties(string id)
        {
            var propiedades = await Mediador.Send(new ObtenerPropiedadesAgenteQuery { AgenteId = id });
            if (propiedades == null || !propiedades.Any()) return NoContent();
            return Ok(propiedades);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("{id}/cambiar-estado")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ChangeStatus(string id, [FromBody] CambiarEstadoAgenteCommand command)
        {
            if (id != command.Id) return BadRequest();
            await Mediador.Send(command);
            return NoContent();
        }
    }
}
