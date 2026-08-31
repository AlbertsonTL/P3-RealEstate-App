using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.TipoPropiedades.Commands;
using RealEstateApp.Core.Application.Features.TipoPropiedades.Queries;

namespace RealEstateApp.WebApi.Controllers
{
    [Authorize(Roles = "Administrador,Desarrollador")]
    public class TipoPropiedadesController : ControladorBaseApi
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> List()
        {
            var result = await Mediador.Send(new ObtenerTiposPropiedadesQuery());
            if (result == null || !result.Any()) return NoContent();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediador.Send(new ObtenerTipoPropiedadPorIdQuery { Id = id });
            if (result is null) return NoContent();
            return Ok(result);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> Create(CrearTipoPropiedadCommand command)
        {
            var result = await Mediador.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ActualizarTipoPropiedadCommand command)
        {
            if (id != command.Id) return BadRequest();
            return Ok(await Mediador.Send(command));
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await Mediador.Send(new EliminarTipoPropiedadCommand { Id = id });
            return NoContent();
        }
    }
}
