using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Enumeraciones;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;
using RealEstateApp.WebApp.Interfaces.Servicios;
using RealEstateApp.WebApp.ViewModels.Agente;
using RealEstateApp.WebApp.ViewModels.Publico;
using RealEstateApp.Core.Application.DTOs;

namespace RealEstateApp.WebApp.Servicios;

public class ServicioPropiedad : IServicioPropiedad
{
    private readonly IRepositorioPropiedad _repositorioPropiedad;
    private readonly IRepositorioPropiedadFavorita _repositorioFavorita;
    private readonly IRepositorioOferta _repositorioOferta;
    private readonly IRepositorioGenerico<TipoPropiedad> _repositorioTipoPropiedad;
    private readonly IRepositorioGenerico<TipoVenta> _repositorioTipoVenta;
    private readonly IRepositorioGenerico<Mejora> _repositorioMejora;
    private readonly IRepositorioGenerico<ImagenPropiedad> _repositorioImagen;
    private readonly IRepositorioGenerico<PropiedadMejora> _repositorioPropiedadMejora;
    private readonly IServicioArchivo _servicioArchivo;

    public ServicioPropiedad(
        IRepositorioPropiedad repositorioPropiedad,
        IRepositorioPropiedadFavorita repositorioFavorita,
        IRepositorioOferta repositorioOferta,
        IRepositorioGenerico<TipoPropiedad> repositorioTipoPropiedad,
        IRepositorioGenerico<TipoVenta> repositorioTipoVenta,
        IRepositorioGenerico<Mejora> repositorioMejora,
        IRepositorioGenerico<ImagenPropiedad> repositorioImagen,
        IRepositorioGenerico<PropiedadMejora> repositorioPropiedadMejora,
        IServicioArchivo servicioArchivo)
    {
        _repositorioPropiedad = repositorioPropiedad;
        _repositorioFavorita = repositorioFavorita;
        _repositorioOferta = repositorioOferta;
        _repositorioTipoPropiedad = repositorioTipoPropiedad;
        _repositorioTipoVenta = repositorioTipoVenta;
        _repositorioMejora = repositorioMejora;
        _repositorioImagen = repositorioImagen;
        _repositorioPropiedadMejora = repositorioPropiedadMejora;
        _servicioArchivo = servicioArchivo;
    }

    public async Task<IEnumerable<PropiedadResumenViewModel>> ObtenerDisponiblesConFiltrosAsync(FiltrosPropiedadDto filtros)
    {
        var modelo = new RealEstateApp.Core.Domain.Modelos.FiltrosPropiedad
        {
            CodigoBusqueda = filtros.CodigoBusqueda,
            TipoPropiedadId = filtros.TipoPropiedadId,
            PrecioMinimo = filtros.PrecioMinimo,
            PrecioMaximo = filtros.PrecioMaximo,
            CantidadHabitaciones = filtros.CantidadHabitaciones,
            CantidadBanos = filtros.CantidadBanos
        };

        var propiedades = await _repositorioPropiedad.ObtenerConFiltrosAsync(modelo);
        return propiedades
            .Where(p => p.Estado == EstadoPropiedad.Disponible)
            .OrderByDescending(p => p.FechaCreacion)
            .Select(p => MapearResumen(p));
    }

    public async Task<PropiedadDetalleViewModel?> ObtenerDetalleAsync(int id, string? clienteId)
    {
        var propiedad = await _repositorioPropiedad.ObtenerPorIdAsync(id);
        if (propiedad is null)
        {
            return null;
        }

        var esFavorita = !string.IsNullOrWhiteSpace(clienteId) && await _repositorioFavorita.EsFavoritaAsync(id, clienteId);
        var tieneOfertaAceptada = await _repositorioOferta.ExisteOfertaAceptadaAsync(id);
        var tieneOfertaPendientePropia = !string.IsNullOrWhiteSpace(clienteId) &&
                                         await _repositorioOferta.ExisteOfertaPendienteDelClienteAsync(id, clienteId);

        return new PropiedadDetalleViewModel
        {
            Id = propiedad.Id,
            Codigo = propiedad.Codigo,
            TipoPropiedad = propiedad.TipoPropiedad?.Nombre ?? string.Empty,
            TipoVenta = propiedad.TipoVenta?.Nombre ?? string.Empty,
            Precio = propiedad.Precio,
            CantidadHabitaciones = propiedad.CantidadHabitaciones,
            CantidadBanos = propiedad.CantidadBanos,
            TamañoMetros = propiedad.TamañoMetros,
            Descripcion = propiedad.Descripcion,
            UrlsImagenes = propiedad.Imagenes?.Select(i => i.UrlImagen).ToList() ?? [],
            NombresMejoras = propiedad.PropiedadesMejoras?.Select(m => m.Mejora?.Nombre ?? string.Empty).Where(x => !string.IsNullOrWhiteSpace(x)).ToList() ?? [],
            AgenteId = propiedad.AgenteId,
            Estado = propiedad.Estado,
            EsFavorita = esFavorita,
            PuedeHacerOferta = propiedad.Estado == EstadoPropiedad.Disponible && !tieneOfertaAceptada && !tieneOfertaPendientePropia
        };
    }

    public async Task CrearPropiedadAsync(CrearPropiedadViewModel modelo, string agenteId)
    {
        var propiedad = new Propiedad
        {
            TipoPropiedadId = modelo.TipoPropiedadId,
            TipoVentaId = modelo.TipoVentaId,
            Precio = modelo.Precio,
            Descripcion = modelo.Descripcion,
            TamañoMetros = modelo.TamañoMetros,
            CantidadHabitaciones = modelo.CantidadHabitaciones,
            CantidadBanos = modelo.CantidadBanos,
            Estado = EstadoPropiedad.Disponible,
            AgenteId = agenteId,
            FechaCreacion = DateTime.UtcNow
        };

        var creada = await _repositorioPropiedad.AgregarAsync(propiedad);
        await GuardarImagenesAsync(creada.Id, modelo.Imagenes, 4);
        await GuardarMejorasAsync(creada.Id, modelo.MejorasSeleccionadas);
    }

    public async Task EditarPropiedadAsync(EditarPropiedadViewModel modelo)
    {
        var propiedad = await _repositorioPropiedad.ObtenerPorIdAsync(modelo.Id);
        if (propiedad is null) return;

        var eliminarIds = modelo.EliminarImagenIds?.Distinct().ToHashSet() ?? [];
        foreach (var imagen in propiedad.Imagenes.Where(i => eliminarIds.Contains(i.Id)).ToList())
        {
            await _servicioArchivo.EliminarImagenAsync(imagen.UrlImagen);
            await _repositorioImagen.EliminarAsync(imagen);
        }

        propiedad = await _repositorioPropiedad.ObtenerPorIdAsync(modelo.Id);
        if (propiedad is null) return;

        propiedad.TipoPropiedadId = modelo.TipoPropiedadId;
        propiedad.TipoVentaId = modelo.TipoVentaId;
        propiedad.Precio = modelo.Precio;
        propiedad.Descripcion = modelo.Descripcion;
        propiedad.TamañoMetros = modelo.TamañoMetros;
        propiedad.CantidadHabitaciones = modelo.CantidadHabitaciones;
        propiedad.CantidadBanos = modelo.CantidadBanos;

        await _repositorioPropiedad.ActualizarAsync(propiedad);

        // Sincronizar mejoras (reemplaza el conjunto anterior)
        var mejorasAnteriores = propiedad.PropiedadesMejoras.ToList();
        foreach (var pm in mejorasAnteriores)
        {
            await _repositorioPropiedadMejora.EliminarAsync(pm);
        }

        var mejorasNuevas = modelo.MejorasSeleccionadas ?? [];
        await GuardarMejorasAsync(propiedad.Id, mejorasNuevas);

        propiedad = await _repositorioPropiedad.ObtenerPorIdAsync(modelo.Id);
        if (propiedad is null) return;

        var nuevas = modelo.Imagenes?.Where(i => i is not null).ToList() ?? [];
        var cupo = 4 - propiedad.Imagenes.Count;
        if (cupo > 0 && nuevas.Count > 0)
        {
            await GuardarImagenesAsync(propiedad.Id, nuevas, cupo);
        }
    }

    public async Task EliminarPropiedadAsync(int id)
    {
        var propiedad = await _repositorioPropiedad.ObtenerPorIdAsync(id);
        if (propiedad is null) return;

        // MEJORA: antes solo se borraba el registro de la propiedad (las filas de
        // ImagenPropiedad se iban por cascada de EF), pero los archivos físicos en
        // wwwroot/imagenes/propiedades quedaban huérfanos en disco para siempre.
        foreach (var imagen in propiedad.Imagenes.ToList())
        {
            await _servicioArchivo.EliminarImagenAsync(imagen.UrlImagen);
        }

        await _repositorioPropiedad.EliminarAsync(propiedad);
    }

    public async Task<EditarPropiedadViewModel?> ObtenerPropiedadParaEditarAsync(int id, string agenteId)
    {
        var propiedad = await _repositorioPropiedad.ObtenerPorIdAsync(id);
        if (propiedad is null || !string.Equals(propiedad.AgenteId, agenteId, StringComparison.Ordinal))
        {
            return null;
        }

        return new EditarPropiedadViewModel
        {
            Id = propiedad.Id,
            TipoPropiedadId = propiedad.TipoPropiedadId,
            TipoVentaId = propiedad.TipoVentaId,
            Precio = propiedad.Precio,
            Descripcion = propiedad.Descripcion,
            TamañoMetros = propiedad.TamañoMetros,
            CantidadHabitaciones = propiedad.CantidadHabitaciones,
            CantidadBanos = propiedad.CantidadBanos,
            MejorasSeleccionadas = propiedad.PropiedadesMejoras.Select(pm => pm.MejoraId).ToList(),
            ImagenesExistentes = propiedad.Imagenes
                .OrderBy(i => i.Id)
                .Select(i => new ImagenPropiedadEdicionItem { Id = i.Id, Url = i.UrlImagen })
                .ToList()
        };
    }

    public async Task<bool> EsPropiedadDelAgenteAsync(int propiedadId, string agenteId)
    {
        var propiedad = await _repositorioPropiedad.ObtenerPorIdAsync(propiedadId);
        return propiedad is not null && string.Equals(propiedad.AgenteId, agenteId, StringComparison.Ordinal);
    }

    public async Task<EliminarPropiedadViewModel?> ObtenerPropiedadParaEliminarAsync(int id, string agenteId)
    {
        var propiedad = await _repositorioPropiedad.ObtenerPorIdAsync(id);
        if (propiedad is null || !string.Equals(propiedad.AgenteId, agenteId, StringComparison.Ordinal))
        {
            return null;
        }

        return new EliminarPropiedadViewModel
        {
            Id = propiedad.Id,
            Codigo = propiedad.Codigo,
            TipoPropiedad = propiedad.TipoPropiedad?.Nombre ?? string.Empty,
            TipoVenta = propiedad.TipoVenta?.Nombre ?? string.Empty,
            Precio = propiedad.Precio,
            Descripcion = propiedad.Descripcion
        };
    }

    public async Task<IEnumerable<PropiedadResumenViewModel>> ObtenerPropiedadesAgenteAsync(string agenteId, bool incluirVendidas)
    {
        var propiedades = await _repositorioPropiedad.ObtenerPorAgenteAsync(agenteId);
        if (!incluirVendidas)
        {
            propiedades = propiedades.Where(p => p.Estado == EstadoPropiedad.Disponible);
        }

        return propiedades.OrderByDescending(p => p.FechaCreacion).Select(p => MapearResumen(p));
    }

    public async Task<IEnumerable<PropiedadResumenViewModel>> ObtenerFavoritasClienteAsync(string clienteId)
    {
        var favoritas = await _repositorioFavorita.ObtenerFavoritasPorClienteAsync(clienteId);
        return favoritas.Select(f => MapearResumen(f.Propiedad, true));
    }

    public async Task<bool> ToggleFavoritaAsync(int propiedadId, string clienteId)
    {
        var favorita = await _repositorioFavorita.ObtenerFavoritaAsync(propiedadId, clienteId);
        if (favorita is null)
        {
            await _repositorioFavorita.AgregarAsync(new PropiedadFavorita { PropiedadId = propiedadId, ClienteId = clienteId });
            return true;
        }

        await _repositorioFavorita.EliminarAsync(favorita);
        return false;
    }

    private async Task GuardarImagenesAsync(int propiedadId, IEnumerable<IFormFile?> imagenes, int limite)
    {
        foreach (var imagen in imagenes.Where(i => i is not null).Take(limite))
        {
            var ruta = await _servicioArchivo.GuardarImagenAsync(imagen!, "propiedades");
            if (string.IsNullOrWhiteSpace(ruta)) continue;

            await _repositorioImagen.AgregarAsync(new ImagenPropiedad
            {
                PropiedadId = propiedadId,
                UrlImagen = ruta,
                EsPrincipal = false
            });
        }
    }

    private async Task GuardarMejorasAsync(int propiedadId, IEnumerable<int> mejoras)
    {
        foreach (var mejoraId in mejoras.Distinct())
        {
            await _repositorioPropiedadMejora.AgregarAsync(new PropiedadMejora
            {
                PropiedadId = propiedadId,
                MejoraId = mejoraId
            });
        }
    }

    private static PropiedadResumenViewModel MapearResumen(Propiedad propiedad, bool esFavorita = false)
    {
        return new PropiedadResumenViewModel
        {
            Id = propiedad.Id,
            Codigo = propiedad.Codigo,
            TipoPropiedadId = propiedad.TipoPropiedadId,
            TipoPropiedad = propiedad.TipoPropiedad?.Nombre ?? string.Empty,
            TipoVenta = propiedad.TipoVenta?.Nombre ?? string.Empty,
            Precio = propiedad.Precio,
            CantidadHabitaciones = propiedad.CantidadHabitaciones,
            CantidadBanos = propiedad.CantidadBanos,
            TamañoMetros = propiedad.TamañoMetros,
            UrlImagenPrincipal = propiedad.Imagenes?.FirstOrDefault(i => i.EsPrincipal)?.UrlImagen
                                ?? propiedad.Imagenes?.FirstOrDefault()?.UrlImagen
                                ?? "/images/placeholder-property.jpg",
            Estado = propiedad.Estado,
            EsFavorita = esFavorita
        };
    }
}
