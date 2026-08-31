using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Enumeraciones;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;
using RealEstateApp.WebApp.Interfaces.Servicios;
using RealEstateApp.WebApp.ViewModels.Shared;

namespace RealEstateApp.WebApp.Servicios;

public class ServicioOferta : IServicioOferta
{
    private readonly IRepositorioOferta _repositorioOferta;
    private readonly IRepositorioPropiedad _repositorioPropiedad;

    public ServicioOferta(IRepositorioOferta repositorioOferta, IRepositorioPropiedad repositorioPropiedad)
    {
        _repositorioOferta = repositorioOferta;
        _repositorioPropiedad = repositorioPropiedad;
    }

    public async Task<IEnumerable<OfertaViewModel>> ObtenerOfertasClientePropiedadAsync(int propiedadId, string clienteId)
    {
        var ofertas = await _repositorioOferta.ObtenerPorPropiedadYClienteAsync(propiedadId, clienteId);
        return ofertas.OrderByDescending(o => o.FechaOferta).Select(o => new OfertaViewModel
        {
            Id = o.Id,
            CifraOfertada = o.CifraOfertada,
            FechaOferta = o.FechaOferta,
            Estado = o.Estado
        });
    }

    public async Task<IEnumerable<string>> ObtenerClientesConOfertasAsync(int propiedadId)
    {
        var ofertas = await _repositorioOferta.ObtenerPorPropiedadAsync(propiedadId);
        return ofertas.Select(o => o.ClienteId).Distinct();
    }

    public async Task CrearOfertaAsync(int propiedadId, string clienteId, decimal cifra)
    {
        if (await _repositorioOferta.ExisteOfertaAceptadaAsync(propiedadId))
        {
            throw new InvalidOperationException("Esta propiedad ya tiene una oferta aceptada.");
        }

        if (await _repositorioOferta.ExisteOfertaPendienteDelClienteAsync(propiedadId, clienteId))
        {
            throw new InvalidOperationException("Ya tienes una oferta pendiente para esta propiedad.");
        }

        await _repositorioOferta.AgregarAsync(new Oferta
        {
            PropiedadId = propiedadId,
            ClienteId = clienteId,
            CifraOfertada = cifra,
            FechaOferta = DateTime.UtcNow,
            Estado = EstadoOferta.Pendiente
        });
    }

    public async Task AceptarOfertaAsync(int ofertaId, string agenteId)
    {
        var oferta = await _repositorioOferta.ObtenerPorIdAsync(ofertaId);
        if (oferta is null)
        {
            throw new InvalidOperationException("Oferta no encontrada.");
        }

        var propiedad = await _repositorioPropiedad.ObtenerPorIdAsync(oferta.PropiedadId);
        if (propiedad is null || propiedad.AgenteId != agenteId)
        {
            throw new UnauthorizedAccessException("No puedes aceptar ofertas de propiedades que no te pertenecen.");
        }

        oferta.Estado = EstadoOferta.Aceptada;
        await _repositorioOferta.ActualizarAsync(oferta);
        await _repositorioOferta.RechazarTodasPendientesExceptoAsync(oferta.PropiedadId, oferta.Id);

        propiedad.Estado = EstadoPropiedad.Vendida;
        await _repositorioPropiedad.ActualizarAsync(propiedad);
    }

    public async Task RechazarOfertaAsync(int ofertaId)
    {
        var oferta = await _repositorioOferta.ObtenerPorIdAsync(ofertaId);
        if (oferta is null) return;
        oferta.Estado = EstadoOferta.Rechazada;
        await _repositorioOferta.ActualizarAsync(oferta);
    }

    public async Task<bool> OfertaPerteneceAPropiedadDelAgenteAsync(int ofertaId, int propiedadId, string agenteId)
    {
        var oferta = await _repositorioOferta.ObtenerPorIdAsync(ofertaId);
        if (oferta is null || oferta.PropiedadId != propiedadId)
        {
            return false;
        }

        var propiedad = await _repositorioPropiedad.ObtenerPorIdAsync(propiedadId);
        return propiedad is not null && string.Equals(propiedad.AgenteId, agenteId, StringComparison.Ordinal);
    }
}
