namespace RealEstateApp.Infrastructure.Shared.Configuracion
{
    /// <summary>
    /// Maps the "EmailSettings" section from appsettings.json.
    /// </summary>
    public class EmailSettings
    {
        public string Host        { get; set; } = null!;
        public int    Port        { get; set; }
        public string SenderName  { get; set; } = null!;
        public string SenderEmail { get; set; } = null!;
        public string UserName    { get; set; } = null!;
        public string Password    { get; set; } = null!;
    }
}
