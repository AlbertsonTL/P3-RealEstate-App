namespace RealEstateApp.Core.Application.DTOs.Cuenta
{
    public class RespuestaRegistro
    {
        public bool TieneError { get; set; }
        public string? MensajeError { get; set; }

        /// <summary>
        /// Id del usuario recién creado.
        /// El controller lo usa para construir la URL de activación con Url.Action().
        /// </summary>
        public string? UsuarioId { get; set; }

        /// <summary>
        /// Token de confirmación de email en Base64Url.
        /// El controller lo usa como query-param en la URL de activación.
        /// </summary>
        public string? TokenActivacion { get; set; }
    }
}
