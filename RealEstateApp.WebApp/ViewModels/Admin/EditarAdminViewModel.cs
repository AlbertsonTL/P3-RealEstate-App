using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RealEstateApp.WebApp.ViewModels.Admin;

public class EditarAdminViewModel
{
    [Required] public string Id { get; set; } = string.Empty;
    [Required] public string Nombre { get; set; } = string.Empty;
    [Required] public string Apellido { get; set; } = string.Empty;
    [Required] public string Cedula { get; set; } = string.Empty;
    [Required] public string Telefono { get; set; } = string.Empty;
    [Required, EmailAddress] public string Correo { get; set; } = string.Empty;
    [Required] public string NombreUsuario { get; set; } = string.Empty;
    [DataType(DataType.Password)] public string Contrasena { get; set; } = string.Empty;
    public string? UrlFoto { get; set; }
    public IFormFile? Foto { get; set; }
}
