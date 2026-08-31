namespace RealEstateApp.Infrastructure.Identity.Configuracion
{
    public class JwtSettings
    {
        public string Key { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public double ExpirationMinutes { get; set; }

        /// <summary>Alias en inglés para compatibilidad con pruebas unitarias.</summary>
        public double DurationInMinutes
        {
            get => ExpirationMinutes;
            set => ExpirationMinutes = value;
        }
    }
}
