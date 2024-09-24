using Domain.DTOs.EmailDTOs;
using MimeKit.Text;

namespace Infrastructure.Services.EmailService;

public interface IEmailSender
{
    Task SendEmail(EmailMessageDto model,TextFormat format);
}