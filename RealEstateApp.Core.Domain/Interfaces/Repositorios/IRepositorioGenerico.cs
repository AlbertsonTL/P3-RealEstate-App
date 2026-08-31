using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealEstateApp.Core.Domain.Interfaces.Repositorios
{
    public interface IRepositorioGenerico<T> where T : class
    {
        Task<T?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<T>> ObtenerTodosAsync();
        Task<T> AgregarAsync(T entidad);
        Task ActualizarAsync(T entidad);
        Task EliminarAsync(T entidad);
    }
}
