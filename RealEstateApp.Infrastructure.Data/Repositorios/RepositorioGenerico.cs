using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;
using RealEstateApp.Infrastructure.Data.Contexto;

namespace RealEstateApp.Infrastructure.Data.Repositorios
{
    public class RepositorioGenerico<T> : IRepositorioGenerico<T> where T : class
    {
        private readonly AplicacionDbContext _dbContext;

        public RepositorioGenerico(AplicacionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual async Task<T> AgregarAsync(T entidad)
        {
            await _dbContext.Set<T>().AddAsync(entidad);
            await _dbContext.SaveChangesAsync();
            return entidad;
        }

        public virtual async Task ActualizarAsync(T entidad)
        {
            _dbContext.Entry(entidad).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task EliminarAsync(T entidad)
        {
            _dbContext.Set<T>().Remove(entidad);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<T?> ObtenerPorIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> ObtenerTodosAsync()
        {
            return await _dbContext.Set<T>().AsNoTracking().ToListAsync();
        }
    }
}
