using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ShopManagementAPI.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendAsync(string toEmail, string toName, string subject, string body)
    {
        var emailConfig = _config.GetSection("Email");
        var username = emailConfig["Username"];
        var password = emailConfig["Password"];

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("no-reply", username));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(emailConfig["Host"], int.Parse(emailConfig["Port"]!), SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(username, password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
