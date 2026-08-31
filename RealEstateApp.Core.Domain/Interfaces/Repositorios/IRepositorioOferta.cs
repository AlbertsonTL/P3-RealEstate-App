using System.Collections.Generic;
using System.Threading.Tasks;
using RealEstateApp.Core.Domain.Entidades;

namespace RealEstateApp.Core.Domain.Interfaces.Repositorios
{
    public interface IRepositorioOferta : IRepositorioGenerico<Oferta>
    {
        Task<IEnumerable<Oferta>> ObtenerPorPropiedadYClienteAsync(int propiedadId, string clienteId);
        Task<IEnumerable<Oferta>> ObtenerPorPropiedadAsync(int propiedadId);
        Task<bool> ExisteOfertaAceptadaAsync(int propiedadId);
        Task<bool> ExisteOfertaPendienteDelClienteAsync(int propiedadId, string clienteId);
        Task RechazarTodasPendientesExceptoAsync(int propiedadId, int ofertaAceptadaId);
    }
}
