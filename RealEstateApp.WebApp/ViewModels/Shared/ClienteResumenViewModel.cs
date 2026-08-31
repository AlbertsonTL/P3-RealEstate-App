namespace RealEstateApp.WebApp.ViewModels.Shared;

/// <summary>
/// Resumen de información pública de un cliente,
/// usado en listas de chats y ofertas del Agente.
/// </summary>
public class ClienteResumenViewModel
{
    public string Id            { get; set; } = null!;
    public string NombreUsuario { get; set; } = null!;
    public string NombreCompleto { get; set; } = null!;
    public string? UrlFoto      { get; set; }
}
