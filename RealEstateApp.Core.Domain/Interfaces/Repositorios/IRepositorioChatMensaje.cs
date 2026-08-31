using System.Collections.Generic;
using System.Threading.Tasks;
using RealEstateApp.Core.Domain.Entidades;

namespace RealEstateApp.Core.Domain.Interfaces.Repositorios
{
    public interface IRepositorioChatMensaje : IRepositorioGenerico<ChatMensaje>
    {
        Task<IEnumerable<ChatMensaje>> ObtenerConversacionAsync(int propiedadId, string clienteId, string agenteId);
        Task<IEnumerable<string>> ObtenerClientesConMensajesAsync(int propiedadId, string agenteId);
    }
}
