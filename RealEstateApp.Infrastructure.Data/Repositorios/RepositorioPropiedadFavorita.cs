using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;
using RealEstateApp.Infrastructure.Data.Contexto;

namespace RealEstateApp.Infrastructure.Data.Repositorios
{
    public class RepositorioPropiedadFavorita : RepositorioGenerico<PropiedadFavorita>, IRepositorioPropiedadFavorita
    {
        private readonly AplicacionDbContext _dbContext;

        public RepositorioPropiedadFavorita(AplicacionDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<PropiedadFavorita>> ObtenerFavoritasPorClienteAsync(string clienteId)
        {
            return await _dbContext.PropiedadesFavoritas
                .Include(pf => pf.Propiedad)
                .ThenInclude(p => p.Imagenes)
                .Where(pf => pf.ClienteId == clienteId)
                .ToListAsync();
        }

        public async Task<PropiedadFavorita?> ObtenerFavoritaAsync(int propiedadId, string clienteId)
        {
            return await _dbContext.PropiedadesFavoritas
                .FirstOrDefaultAsync(pf => pf.PropiedadId == propiedadId && pf.ClienteId == clienteId);
        }

        public async Task<bool> EsFavoritaAsync(int propiedadId, string clienteId)
        {
            return await _dbContext.PropiedadesFavoritas
                .AnyAsync(pf => pf.PropiedadId == propiedadId && pf.ClienteId == clienteId);
        }
    }
}
