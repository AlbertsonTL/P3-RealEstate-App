using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Enumeraciones;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;
using RealEstateApp.Core.Domain.Modelos;
using RealEstateApp.Infrastructure.Data.Contexto;

namespace RealEstateApp.Infrastructure.Data.Repositorios
{
    public class RepositorioPropiedad : RepositorioGenerico<Propiedad>, IRepositorioPropiedad
    {
        private readonly AplicacionDbContext _dbContext;

        public RepositorioPropiedad(AplicacionDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Propiedad?> ObtenerPorCodigoAsync(string codigo)
        {
            return await _dbContext.Propiedades
                .AsNoTracking()
                .Include(p => p.Imagenes)
                .Include(p => p.TipoPropiedad)
                .Include(p => p.TipoVenta)
                .Include(p => p.PropiedadesMejoras).ThenInclude(pm => pm.Mejora)
                .FirstOrDefaultAsync(p => p.Codigo == codigo);
        }

        public async Task<IEnumerable<Propiedad>> ObtenerPorAgenteAsync(string agenteId)
        {
            return await _dbContext.Propiedades
                .AsNoTracking()
                .Include(p => p.Imagenes)
                .Include(p => p.TipoPropiedad)
                .Include(p => p.TipoVenta)
                .Where(p => p.AgenteId == agenteId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Propiedad>> ObtenerDisponiblesAsync()
        {
            return await _dbContext.Propiedades
                .AsNoTracking()
                .Include(p => p.Imagenes)
                .Include(p => p.TipoPropiedad)
                .Include(p => p.TipoVenta)
                .Where(p => p.Estado == EstadoPropiedad.Disponible)
                .ToListAsync();
        }

        public async Task<IEnumerable<Propiedad>> ObtenerConFiltrosAsync(FiltrosPropiedad filtros)
        {
            IQueryable<Propiedad> query = _dbContext.Propiedades
                .AsNoTracking()
                .Include(p => p.Imagenes)
                .Include(p => p.TipoPropiedad)
                .Include(p => p.TipoVenta);

            if (filtros != null)
            {
                if (!string.IsNullOrEmpty(filtros.CodigoBusqueda))
                {
                    query = query.Where(p => p.Codigo == filtros.CodigoBusqueda);
                }
                if (filtros.TipoPropiedadId.HasValue)
                {
                    query = query.Where(p => p.TipoPropiedadId == filtros.TipoPropiedadId.Value);
                }
                if (filtros.PrecioMinimo.HasValue)
                {
                    query = query.Where(p => p.Precio >= filtros.PrecioMinimo.Value);
                }
                if (filtros.PrecioMaximo.HasValue)
                {
                    query = query.Where(p => p.Precio <= filtros.PrecioMaximo.Value);
                }
                if (filtros.CantidadHabitaciones.HasValue)
                {
                    query = query.Where(p => p.CantidadHabitaciones == filtros.CantidadHabitaciones.Value);
                }
                if (filtros.CantidadBanos.HasValue)
                {
                    query = query.Where(p => p.CantidadBanos == filtros.CantidadBanos.Value);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<bool> ExisteCodigoAsync(string codigo)
        {
            return await _dbContext.Propiedades.AnyAsync(p => p.Codigo == codigo);
        }
        
        public override async Task<Propiedad?> ObtenerPorIdAsync(int id)
        {
            return await _dbContext.Propiedades
                .AsNoTracking()
                .Include(p => p.Imagenes)
                .Include(p => p.TipoPropiedad)
                .Include(p => p.TipoVenta)
                .Include(p => p.PropiedadesMejoras).ThenInclude(pm => pm.Mejora)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public override async Task<Propiedad> AgregarAsync(Propiedad entidad)
        {
            entidad.Codigo = await GenerarCodigoUnicoAsync();
            return await base.AgregarAsync(entidad);
        }

        private async Task<string> GenerarCodigoUnicoAsync()
        {
            var random = new System.Random();
            string codigo;
            do
            {
                codigo = random.Next(100000, 999999).ToString();
            } while (await ExisteCodigoAsync(codigo));

            return codigo;
        }
    }
}
