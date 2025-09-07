using AutoParts_ShopAndForum.Infrastructure.Options;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace AutoParts_ShopAndForum.Service;

public class EmailSender(IOptions<SmtpOptions> options) : IEmailSender
{
    private readonly SmtpOptions _options = options.Value;
    
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        const string fromAddress = "autoparts_noreply@gmail.com";

        using var body = new TextPart(TextFormat.Html);
        
        body.Text = htmlMessage;
        
        using var message = new MimeMessage();
        
        message.From.Add(new MailboxAddress(fromAddress, fromAddress));
        message.To.Add(new MailboxAddress(null, email));
        message.Subject = subject;
        message.Body = body;

        using var client = new SmtpClient();
        await client.ConnectAsync(_options.Host, _options.Port);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}