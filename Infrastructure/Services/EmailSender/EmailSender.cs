using Domain.DTOs.EmailDTOs;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace Infrastructure.Services.EmailService;

public class EmailSender(EmailConfiguration configuration) : IEmailSender
{
    #region SendEmail

    public async Task SendEmail(EmailMessageDto message, TextFormat format)
    {
        var emailMessage = CreateEmailMessage(message, format);
        await SendAsync(emailMessage);
    }

    #endregion

    #region CreateEmailMessage

    private MimeMessage CreateEmailMessage(EmailMessageDto message, TextFormat format)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("mail", configuration.From));
        emailMessage.To.AddRange(message.To);
        emailMessage.Subject = message.Subject;
        emailMessage.Body = new TextPart(format) { Text = message.Content };
        return emailMessage;
    }

    #endregion

    #region SendAsync

    private async Task SendAsync(MimeMessage mailMessage)
    {
        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(configuration.SmtpServer, configuration.Port, true);
            client.AuthenticationMechanisms.Remove("OAUTH2");
            await client.AuthenticateAsync(configuration.UserName, configuration.Password);

            await client.SendAsync(mailMessage);
        }
        finally
        {
            await client.DisconnectAsync(true);
            client.Dispose();
        }
    }

    #endregion
}