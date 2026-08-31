using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entidades;
using RealEstateApp.Core.Domain.Interfaces.Repositorios;
using RealEstateApp.Infrastructure.Data.Contexto;

namespace RealEstateApp.Infrastructure.Data.Repositorios
{
    public class RepositorioChatMensaje : RepositorioGenerico<ChatMensaje>, IRepositorioChatMensaje
    {
        private readonly AplicacionDbContext _dbContext;

        public RepositorioChatMensaje(AplicacionDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ChatMensaje>> ObtenerConversacionAsync(int propiedadId, string clienteId, string agenteId)
        {
            return await _dbContext.ChatMensajes
                .Where(m => m.PropiedadId == propiedadId && 
                           ((m.RemitenteId == clienteId && m.DestinatarioId == agenteId) || 
                            (m.RemitenteId == agenteId && m.DestinatarioId == clienteId)))
                .OrderBy(m => m.FechaEnvio)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> ObtenerClientesConMensajesAsync(int propiedadId, string agenteId)
        {
            // Clientes que han participado en la conversación de esta propiedad,
            // ya sea como remitente o como destinatario, excluyendo siempre al
            // propio agente (de lo contrario el agente aparecería listado como
            // "cliente" apenas respondiera un mensaje).
            var remitentes = _dbContext.ChatMensajes
                .Where(m => m.PropiedadId == propiedadId && m.RemitenteId != agenteId)
                .Select(m => m.RemitenteId);

            var destinatarios = _dbContext.ChatMensajes
                .Where(m => m.PropiedadId == propiedadId && m.DestinatarioId != agenteId)
                .Select(m => m.DestinatarioId);

            return await remitentes
                .Union(destinatarios)
                .Distinct()
                .ToListAsync();
        }
    }
}
