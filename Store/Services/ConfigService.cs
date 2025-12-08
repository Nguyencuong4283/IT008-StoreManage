using System;
using System.IO;
using System.Text.Json;

namespace Store.Services
{
    public class EmailConfig
    {
        public string FromEmail { get; set; } = string.Empty;
        public string AppPassword { get; set; } = string.Empty;
    }

    public class AppSettings
    {
        public EmailConfig EmailSettings { get; set; } = new EmailConfig();
    }

    public static class ConfigService
    {
        private static AppSettings? _settings;
        private static readonly string ConfigPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

        public static AppSettings GetSettings()
        {
            if (_settings != null)
                return _settings;

            try
            {
                if (!File.Exists(ConfigPath))
                {
                    throw new FileNotFoundException(
                        "File appsettings.json không tồn tại. Vui lòng tạo file từ appsettings.example.json và cấu hình email.");
                }

                string json = File.ReadAllText(ConfigPath);
                _settings = JsonSerializer.Deserialize<AppSettings>(json);

                if (_settings == null || 
                    string.IsNullOrEmpty(_settings.EmailSettings.FromEmail) ||
                    string.IsNullOrEmpty(_settings.EmailSettings.AppPassword))
                {
                    throw new InvalidOperationException(
                        "Cấu hình email chưa đầy đủ trong appsettings.json");
                }

                return _settings;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi đọc cấu hình: {ex.Message}", ex);
            }
        }

        public static void ReloadSettings()
        {
            _settings = null;
        }
    }
}
