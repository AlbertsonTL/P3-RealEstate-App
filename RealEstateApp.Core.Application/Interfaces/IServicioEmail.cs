namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IServicioEmail
    {
        Task EnviarEmailActivacionAsync(string destinatario, string nombre, string enlaceActivacion);
        Task EnviarEmailBienvenidaAsync(string destinatario, string nombre);
        Task EnviarEmailRecuperacionAsync(string destinatario, string nombre, string enlaceRecuperacion);
    }
}
