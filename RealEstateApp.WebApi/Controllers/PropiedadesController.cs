using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Features.Propiedades.Queries;

namespace RealEstateApp.WebApi.Controllers
{
    [Authorize(Roles = "Administrador,Desarrollador")]
    public class PropiedadesController : ControladorBaseApi
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PropiedadDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Listar()
        {
            var propiedades = await Mediador.Send(new ObtenerTodasPropiedadesQuery());
            if (propiedades == null || !propiedades.Any()) return NoContent();
            return Ok(propiedades);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PropiedadDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]  // ✅ BUG FIX #2: era 404 NotFound
        public async Task<IActionResult> GetById(int id)
        {
            var propiedad = await Mediador.Send(new ObtenerPropiedadPorIdQuery { Id = id });
            if (propiedad is null) return NoContent();  // ✅ era NotFound()
            return Ok(propiedad);
        }

        [HttpGet("codigo/{codigo}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PropiedadDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]  // ✅ BUG FIX #2: era 404 NotFound
        public async Task<IActionResult> GetByCode(string codigo)
        {
            var propiedad = await Mediador.Send(new ObtenerPropiedadPorCodigoQuery { Codigo = codigo });
            if (propiedad is null) return NoContent();  // ✅ era NotFound()
            return Ok(propiedad);
        }
    }
}
