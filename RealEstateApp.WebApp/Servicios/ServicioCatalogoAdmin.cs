using MediatR;
using RealEstateApp.Core.Application.Features.Mejoras.Commands;
using RealEstateApp.Core.Application.Features.Mejoras.Queries;
using RealEstateApp.Core.Application.Features.Propiedades.Queries;
using RealEstateApp.Core.Application.Features.TipoPropiedades.Commands;
using RealEstateApp.Core.Application.Features.TipoPropiedades.Queries;
using RealEstateApp.Core.Application.Features.TipoVentas.Commands;
using RealEstateApp.Core.Application.Features.TipoVentas.Queries;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.WebApp.Interfaces.Servicios;
using RealEstateApp.WebApp.ViewModels.Admin;

namespace RealEstateApp.WebApp.Servicios;

/// <summary>
/// Implementación del servicio de catálogos administrativos.
/// Centraliza las operaciones CRUD sobre TipoPropiedad, TipoVenta y Mejoras
/// usando el patrón Mediator (CQRS) para respetar la Onion Architecture.
/// </summary>
public class ServicioCatalogoAdmin : IServicioCatalogoAdmin
{
    private readonly IMediator _mediador;

    public ServicioCatalogoAdmin(IMediator mediador)
    {
        _mediador = mediador;
    }

    // ── TipoPropiedad ────────────────────────────────────────────────────────

    public async Task<List<TipoPropiedad>> ObtenerTiposPropiedadesAsync()
    {
        var dtos = await _mediador.Send(new ObtenerTiposPropiedadesQuery());
        return dtos.Select(d => new TipoPropiedad
        {
            Id = d.Id,
            Nombre = d.Nombre,
            Descripcion = d.Descripcion
        }).ToList();
    }

    public async Task<List<TipoCatalogoConConteoViewModel>> ObtenerTiposPropiedadesConConteoAsync()
    {
        // Obtenemos tipos y propiedades vía CQRS y cruzamos en memoria
        var tipos = await _mediador.Send(new ObtenerTiposPropiedadesQuery());
        var propiedades = await _mediador.Send(new ObtenerTodasPropiedadesQuery());

        var conteos = propiedades
            .GroupBy(p => p.TipoPropiedadId)
            .ToDictionary(g => g.Key, g => g.Count());

        return tipos.OrderBy(t => t.Nombre).Select(t => new TipoCatalogoConConteoViewModel
        {
            Id = t.Id,
            Nombre = t.Nombre,
            Descripcion = t.Descripcion,
            CantidadPropiedades = conteos.TryGetValue(t.Id, out var c) ? c : 0
        }).ToList();
    }

    public async Task<TipoPropiedad?> ObtenerTipoPropiedadPorIdAsync(int id)
    {
        var dto = await _mediador.Send(new ObtenerTipoPropiedadPorIdQuery { Id = id });
        if (dto is null) return null;
        return new TipoPropiedad { Id = dto.Id, Nombre = dto.Nombre, Descripcion = dto.Descripcion };
    }

    public async Task CrearTipoPropiedadAsync(CatalogoFormViewModel modelo)
    {
        await _mediador.Send(new CrearTipoPropiedadCommand
        {
            Nombre = modelo.Nombre.Trim(),
            Descripcion = modelo.Descripcion.Trim()
        });
    }

    public async Task ActualizarTipoPropiedadAsync(CatalogoFormViewModel modelo)
    {
        await _mediador.Send(new ActualizarTipoPropiedadCommand
        {
            Id = modelo.Id,
            Nombre = modelo.Nombre.Trim(),
            Descripcion = modelo.Descripcion.Trim()
        });
    }

    public async Task<(bool Exito, string? MensajeError)> EliminarTipoPropiedadAsync(int id)
    {
        var tipo = await _mediador.Send(new ObtenerTipoPropiedadPorIdQuery { Id = id });
        if (tipo is null)
            return (false, "El tipo de propiedad no existe.");

        // Verificar si está en uso contando desde las propiedades vía CQRS
        var propiedades = await _mediador.Send(new ObtenerTodasPropiedadesQuery());
        bool enUso = propiedades.Any(p => p.TipoPropiedadId == id);
        if (enUso)
            return (false, "No se puede eliminar porque está siendo usado por una o más propiedades.");

        await _mediador.Send(new EliminarTipoPropiedadCommand { Id = id });
        return (true, null);
    }

    // ── TipoVenta ────────────────────────────────────────────────────────────

    public async Task<List<TipoVenta>> ObtenerTiposVentasAsync()
    {
        var dtos = await _mediador.Send(new ObtenerTiposVentasQuery());
        return dtos.Select(d => new TipoVenta
        {
            Id = d.Id,
            Nombre = d.Nombre,
            Descripcion = d.Descripcion
        }).ToList();
    }

    public async Task<List<TipoCatalogoConConteoViewModel>> ObtenerTiposVentasConConteoAsync()
    {
        var tipos = await _mediador.Send(new ObtenerTiposVentasQuery());
        var propiedades = await _mediador.Send(new ObtenerTodasPropiedadesQuery());

        var conteos = propiedades
            .GroupBy(p => p.TipoVentaId)
            .ToDictionary(g => g.Key, g => g.Count());

        return tipos.OrderBy(t => t.Nombre).Select(t => new TipoCatalogoConConteoViewModel
        {
            Id = t.Id,
            Nombre = t.Nombre,
            Descripcion = t.Descripcion,
            CantidadPropiedades = conteos.TryGetValue(t.Id, out var c) ? c : 0
        }).ToList();
    }

    public async Task<TipoVenta?> ObtenerTipoVentaPorIdAsync(int id)
    {
        var dto = await _mediador.Send(new ObtenerTipoVentaPorIdQuery { Id = id });
        if (dto is null) return null;
        return new TipoVenta { Id = dto.Id, Nombre = dto.Nombre, Descripcion = dto.Descripcion };
    }

    public async Task CrearTipoVentaAsync(CatalogoFormViewModel modelo)
    {
        await _mediador.Send(new CrearTipoVentaCommand
        {
            Nombre = modelo.Nombre.Trim(),
            Descripcion = modelo.Descripcion.Trim()
        });
    }

    public async Task ActualizarTipoVentaAsync(CatalogoFormViewModel modelo)
    {
        await _mediador.Send(new ActualizarTipoVentaCommand
        {
            Id = modelo.Id,
            Nombre = modelo.Nombre.Trim(),
            Descripcion = modelo.Descripcion.Trim()
        });
    }

    public async Task<(bool Exito, string? MensajeError)> EliminarTipoVentaAsync(int id)
    {
        var tipo = await _mediador.Send(new ObtenerTipoVentaPorIdQuery { Id = id });
        if (tipo is null)
            return (false, "El tipo de venta no existe.");

        var propiedades = await _mediador.Send(new ObtenerTodasPropiedadesQuery());
        bool enUso = propiedades.Any(p => p.TipoVentaId == id);
        if (enUso)
            return (false, "No se puede eliminar porque está siendo usado por una o más propiedades.");

        await _mediador.Send(new EliminarTipoVentaCommand { Id = id });
        return (true, null);
    }

    // ── Mejoras ─────────────────────────────────────────────────────────────

    public async Task<List<Mejora>> ObtenerMejorasAsync()
    {
        var dtos = await _mediador.Send(new ObtenerMejorasQuery());
        return dtos.Select(d => new Mejora
        {
            Id = d.Id,
            Nombre = d.Nombre,
            Descripcion = d.Descripcion
        }).ToList();
    }

    public async Task<Mejora?> ObtenerMejoraPorIdAsync(int id)
    {
        var dto = await _mediador.Send(new ObtenerMejoraPorIdQuery { Id = id });
        if (dto is null) return null;
        return new Mejora { Id = dto.Id, Nombre = dto.Nombre, Descripcion = dto.Descripcion };
    }

    public async Task CrearMejoraAsync(CatalogoFormViewModel modelo)
    {
        await _mediador.Send(new CrearMejoraCommand
        {
            Nombre = modelo.Nombre.Trim(),
            Descripcion = modelo.Descripcion.Trim()
        });
    }

    public async Task ActualizarMejoraAsync(CatalogoFormViewModel modelo)
    {
        await _mediador.Send(new ActualizarMejoraCommand
        {
            Id = modelo.Id,
            Nombre = modelo.Nombre.Trim(),
            Descripcion = modelo.Descripcion.Trim()
        });
    }

    public async Task<(bool Exito, string? MensajeError)> EliminarMejoraAsync(int id)
    {
        var mejora = await _mediador.Send(new ObtenerMejoraPorIdQuery { Id = id });
        if (mejora is null)
            return (false, "La mejora no existe.");

        // La mejora usa cascade a PropiedadesMejoras, se elimina correctamente sin bloquear.
        await _mediador.Send(new EliminarMejoraCommand { Id = id });
        return (true, null);
    }

    // ── Utilidades ───────────────────────────────────────────────────────────

    public async Task<Dictionary<string, int>> ObtenerCantidadPropiedadesPorAgenteAsync()
    {
        var propiedades = await _mediador.Send(new ObtenerTodasPropiedadesQuery());
        return propiedades
            .Where(p => p.IdAgente != null)
            .GroupBy(p => p.IdAgente!)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}
