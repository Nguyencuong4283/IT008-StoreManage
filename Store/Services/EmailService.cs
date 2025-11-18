using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;

public class EmailService
{
    private readonly string _fromEmail;
    private readonly string _appPassword;

    public EmailService()
    {
        // Lấy email và mật khẩu ứng dụng từ biến môi trường
        _fromEmail = Environment.GetEnvironmentVariable("EMAIL_FROM");
        _appPassword = Environment.GetEnvironmentVariable("EMAIL_APP_PASSWORD");

        if (string.IsNullOrEmpty(_fromEmail) || string.IsNullOrEmpty(_appPassword))
        {
            throw new InvalidOperationException("Email hoặc App Password chưa được thiết lập trong biến môi trường.");
        }
    }

    public async Task SendResetCode(string toEmail, string code)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Your App", _fromEmail));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "Mã xác nhận quên mật khẩu";

        message.Body = new TextPart("plain")
        {
            Text = $"Mã khôi phục mật khẩu của bạn: {code}"
        };

        using var client = new MailKit.Net.Smtp.SmtpClient();

        try
        {
            // Kết nối đến Gmail SMTP
            await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_fromEmail, _appPassword);
            await client.SendAsync(message);
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}
