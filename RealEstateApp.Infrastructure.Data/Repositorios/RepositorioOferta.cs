using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Enumeraciones;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;
using RealEstateApp.Infrastructure.Data.Contexto;

namespace RealEstateApp.Infrastructure.Data.Repositorios
{
    public class RepositorioOferta : RepositorioGenerico<Oferta>, IRepositorioOferta
    {
        private readonly AplicacionDbContext _dbContext;

        public RepositorioOferta(AplicacionDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Oferta>> ObtenerPorPropiedadYClienteAsync(int propiedadId, string clienteId)
        {
            return await _dbContext.Ofertas
                .Where(o => o.PropiedadId == propiedadId && o.ClienteId == clienteId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Oferta>> ObtenerPorPropiedadAsync(int propiedadId)
        {
            return await _dbContext.Ofertas
                .Where(o => o.PropiedadId == propiedadId)
                .ToListAsync();
        }

        public async Task<bool> ExisteOfertaAceptadaAsync(int propiedadId)
        {
            return await _dbContext.Ofertas
                .AnyAsync(o => o.PropiedadId == propiedadId && o.Estado == EstadoOferta.Aceptada);
        }

        public async Task<bool> ExisteOfertaPendienteDelClienteAsync(int propiedadId, string clienteId)
        {
            return await _dbContext.Ofertas
                .AnyAsync(o => o.PropiedadId == propiedadId && o.ClienteId == clienteId && o.Estado == EstadoOferta.Pendiente);
        }

        public async Task RechazarTodasPendientesExceptoAsync(int propiedadId, int ofertaAceptadaId)
        {
            await _dbContext.Ofertas
                .Where(o => o.PropiedadId == propiedadId && o.Id != ofertaAceptadaId && o.Estado == EstadoOferta.Pendiente)
                .ExecuteUpdateAsync(s => s.SetProperty(o => o.Estado, EstadoOferta.Rechazada));
        }
    }
}
