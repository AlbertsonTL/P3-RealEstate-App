using Microsoft.AspNetCore.Http;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IServicioArchivo
    {
        Task<string> GuardarImagenAsync(IFormFile archivo, string subcarpeta);
        Task EliminarImagenAsync(string rutaRelativa);
    }
}
