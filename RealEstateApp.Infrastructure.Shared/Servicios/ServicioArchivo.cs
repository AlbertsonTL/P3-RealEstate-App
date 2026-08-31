using Microsoft.AspNetCore.Http;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApp.Infrastructure.Shared.Servicios
{
    public class ServicioArchivo : IServicioArchivo
    {
        // Límite de tamaño para evitar que un usuario suba archivos enormes
        private const long TamañoMaximoBytes = 5 * 1024 * 1024; // 5 MB

        private static readonly string[] ExtensionesValidas = [".jpg", ".jpeg", ".png", ".webp"];
        private static readonly string[] ContentTypesValidos =
            ["image/jpeg", "image/png", "image/webp"];

        public async Task<string> GuardarImagenAsync(IFormFile archivo, string subcarpeta)
        {
            if (archivo == null || archivo.Length == 0) return string.Empty;

            // Filtro sencillo de File Type / MIME Type Spoofing
            // Valida el tipo MIME reportado y se contrasta con la firma.
            if (archivo.Length > TamañoMaximoBytes) return string.Empty;

            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (!ExtensionesValidas.Contains(extension)) return string.Empty;
            if (!ContentTypesValidos.Contains(archivo.ContentType?.ToLowerInvariant())) return string.Empty;

            var nombreUnico = $"{Guid.NewGuid()}{extension}";
            var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenes", subcarpeta);

            if (!Directory.Exists(rutaCarpeta))
            {
                Directory.CreateDirectory(rutaCarpeta);
            }

            var rutaCompleta = Path.Combine(rutaCarpeta, nombreUnico);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            return $"/imagenes/{subcarpeta}/{nombreUnico}";
        }

        public async Task EliminarImagenAsync(string rutaRelativa)
        {
            if (string.IsNullOrEmpty(rutaRelativa)) return;

            var rutaFisica = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", rutaRelativa.TrimStart('/'));

            if (File.Exists(rutaFisica))
            {
                await Task.Run(() => File.Delete(rutaFisica));
            }
        }
    }
}
