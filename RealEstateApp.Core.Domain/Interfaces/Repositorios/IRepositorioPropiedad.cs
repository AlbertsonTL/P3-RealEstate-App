using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Modelos;

namespace RealEstateApp.Core.Domain.Interfaces.Repositorios
{
    public interface IRepositorioPropiedad : IRepositorioGenerico<Propiedad>
    {
        Task<Propiedad?> ObtenerPorCodigoAsync(string codigo);
        Task<IEnumerable<Propiedad>> ObtenerPorAgenteAsync(string agenteId);
        Task<IEnumerable<Propiedad>> ObtenerDisponiblesAsync();
        Task<IEnumerable<Propiedad>> ObtenerConFiltrosAsync(FiltrosPropiedad filtros);
        Task<bool> ExisteCodigoAsync(string codigo);
    }
}
