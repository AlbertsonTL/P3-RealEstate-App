using System.Collections.Generic;
using System.Threading.Tasks;
using RealEstateApp.Core.Domain.Entidades;

namespace RealEstateApp.Core.Domain.Interfaces.Repositorios
{
    public interface IRepositorioPropiedadFavorita : IRepositorioGenerico<PropiedadFavorita>
    {
        Task<IEnumerable<PropiedadFavorita>> ObtenerFavoritasPorClienteAsync(string clienteId);
        Task<PropiedadFavorita?> ObtenerFavoritaAsync(int propiedadId, string clienteId);
        Task<bool> EsFavoritaAsync(int propiedadId, string clienteId);
    }
}
