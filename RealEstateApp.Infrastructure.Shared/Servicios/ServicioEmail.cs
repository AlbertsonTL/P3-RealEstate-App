using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Infrastructure.Shared.Configuracion;

namespace RealEstateApp.Infrastructure.Shared.Servicios
{
    public class ServicioEmail : IServicioEmail
    {
        private readonly EmailSettings _cfg;

        public ServicioEmail(IOptions<EmailSettings> options)
        {
            _cfg = options.Value;
        }

        // helpers

        private MimeMessage CrearMensaje(string destinatario, string asunto, string htmlBody)
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(_cfg.SenderName, _cfg.SenderEmail));
            msg.To.Add(MailboxAddress.Parse(destinatario));
            msg.Subject = asunto;
            msg.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();
            return msg;
        }

        private async Task EnviarAsync(MimeMessage mensaje)
        {
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_cfg.Host, _cfg.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_cfg.UserName, _cfg.Password);
            await smtp.SendAsync(mensaje);
            await smtp.DisconnectAsync(true);
        }

        // métodos públicos

        public async Task EnviarEmailActivacionAsync(string destinatario, string nombre, string enlaceActivacion)
        {
            var html = $@"
            <div style='font-family:sans-serif;max-width:600px;margin:auto;'>
                <div style='background:#067EE3;padding:32px 40px;border-radius:12px 12px 0 0;'>
                    <h1 style='color:#fff;margin:0;font-size:22px;'>🏠 RealEstateApp</h1>
                </div>
                <div style='background:#fff;padding:32px 40px;border:1px solid #e5e7eb;border-top:none;border-radius:0 0 12px 12px;'>
                    <h2 style='color:#090d18;margin:0 0 12px;'>¡Bienvenido, {nombre}!</h2>
                    <p style='color:#4b5563;line-height:1.6;'>
                        Gracias por registrarte. Activa tu cuenta haciendo clic en el botón de abajo.
                    </p>
                    <a href='{enlaceActivacion}'
                       style='display:inline-block;margin:20px 0;padding:12px 28px;
                              background:#067EE3;color:#fff;text-decoration:none;
                              border-radius:8px;font-weight:600;font-size:15px;'>
                        Activar mi cuenta
                    </a>
                    <p style='color:#9ca3af;font-size:13px;'>
                        Si no creaste esta cuenta, puedes ignorar este mensaje.
                    </p>
                </div>
            </div>";

            await EnviarAsync(CrearMensaje(destinatario, "Activa tu cuenta en RealEstateApp", html));
        }

        public async Task EnviarEmailBienvenidaAsync(string destinatario, string nombre)
        {
            var html = $@"
            <div style='font-family:sans-serif;max-width:600px;margin:auto;'>
                <div style='background:#067EE3;padding:32px 40px;border-radius:12px 12px 0 0;'>
                    <h1 style='color:#fff;margin:0;font-size:22px;'>🏠 RealEstateApp</h1>
                </div>
                <div style='background:#fff;padding:32px 40px;border:1px solid #e5e7eb;border-top:none;border-radius:0 0 12px 12px;'>
                    <h2 style='color:#090d18;margin:0 0 12px;'>¡Cuenta activada, {nombre}!</h2>
                    <p style='color:#4b5563;line-height:1.6;'>
                        Tu cuenta ha sido verificada con éxito. Ya puedes explorar propiedades,
                        publicar inmuebles y aprovechar todas las funciones de la plataforma.
                    </p>
                    <p style='color:#9ca3af;font-size:13px;margin-top:24px;'>
                        © RealEstateApp — Todos los derechos reservados.
                    </p>
                </div>
            </div>";

            await EnviarAsync(CrearMensaje(destinatario, "¡Tu cuenta está lista! — RealEstateApp", html));
        }

        public async Task EnviarEmailRecuperacionAsync(string destinatario, string nombre, string enlaceRecuperacion)
        {
            var html = $@"
            <div style='font-family:sans-serif;max-width:600px;margin:auto;'>
                <div style='background:#067EE3;padding:32px 40px;border-radius:12px 12px 0 0;'>
                    <h1 style='color:#fff;margin:0;font-size:22px;'>🏠 RealEstateApp</h1>
                </div>
                <div style='background:#fff;padding:32px 40px;border:1px solid #e5e7eb;border-top:none;border-radius:0 0 12px 12px;'>
                    <h2 style='color:#090d18;margin:0 0 12px;'>Recuperación de contraseña</h2>
                    <p style='color:#4b5563;line-height:1.6;'>
                        Hola <strong>{nombre}</strong>, recibimos una solicitud para restablecer
                        la contraseña de tu cuenta. Haz clic en el botón de abajo; el enlace
                        expira en <strong>30 minutos</strong>.
                    </p>
                    <a href='{enlaceRecuperacion}'
                       style='display:inline-block;margin:20px 0;padding:12px 28px;
                              background:#dc2626;color:#fff;text-decoration:none;
                              border-radius:8px;font-weight:600;font-size:15px;'>
                        Restablecer contraseña
                    </a>
                    <p style='color:#9ca3af;font-size:13px;'>
                        Si no solicitaste esto, ignora este correo — tu contraseña no cambiará.
                    </p>
                </div>
            </div>";

            await EnviarAsync(CrearMensaje(destinatario, "Recupera tu contraseña — RealEstateApp", html));
        }
    }
}
