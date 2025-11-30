using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;
using Store.Services;

public class EmailService
{
    private readonly string _fromEmail;
    private readonly string _appPassword;

    public EmailService()
    {
        // Đọc cấu hình từ file appsettings.json (file này KHÔNG được commit lên Git)
        var settings = ConfigService.GetSettings();
        _fromEmail = settings.EmailSettings.FromEmail;
        _appPassword = settings.EmailSettings.AppPassword;

        if (string.IsNullOrEmpty(_fromEmail) || string.IsNullOrEmpty(_appPassword))
        {
            throw new InvalidOperationException("Email hoặc App Password chưa được cấu hình trong appsettings.json");
        }
    }

    // Constructor cho phép truyền email và password từ bên ngoài (dùng cho testing)
    public EmailService(string fromEmail, string appPassword)
    {
        _fromEmail = fromEmail;
        _appPassword = appPassword;

        if (string.IsNullOrEmpty(_fromEmail) || string.IsNullOrEmpty(_appPassword))
        {
            throw new InvalidOperationException("Email hoặc App Password không hợp lệ.");
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

    public async Task SendAccountInfo(string toEmail, string username, string tempPassword)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Store App", _fromEmail));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "Thông tin tài khoản của bạn";

        message.Body = new TextPart("html")
        {
            Text = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #667EEA;'>Thông tin tài khoản</h2>
                    <p>Xin chào,</p>
                    <p>Dưới đây là thông tin đăng nhập của bạn:</p>
                    <div style='background-color: #f5f5f5; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                        <p><strong>Tên đăng nhập:</strong> {username}</p>
                        <p><strong>Mật khẩu tạm thời:</strong> {tempPassword}</p>
                    </div>
                    <p style='color: #e74c3c;'><strong>Lưu ý:</strong> Vui lòng đổi mật khẩu sau khi đăng nhập để bảo mật tài khoản.</p>
                    <p>Trân trọng,<br/>Store App Team</p>
                </body>
                </html>"
        };

        using var client = new MailKit.Net.Smtp.SmtpClient();

        try
        {
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
