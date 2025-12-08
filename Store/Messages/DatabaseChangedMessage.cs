using CommunityToolkit.Mvvm.Messaging.Messages;
using Store.Models;

namespace Store.Messages
{
    // Message dùng chung cho bảng Khách Hàng
    // Kế thừa ValueChangedMessage<string> để chứa nội dung Action ("Insert", "Update", "Delete")
    public class KhachHangChangedMessage : ValueChangedMessage<string>
    {
        public KhachHangChangedMessage(string action) : base(action)
        {
        }
    }

    // Các message khác (giữ nguyên hoặc cập nhật tương tự nếu cần)
    public class HoaDonChangedMessage : ValueChangedMessage<string>
    {
        public HoaDonChangedMessage(string action) : base(action) { }
    }
    
    public class SanPhamChangedMessage : ValueChangedMessage<string>
    {
        public SanPhamChangedMessage(string action) : base(action) { }
    }
    
    public class DatabaseChangedMessage : ValueChangedMessage<string>
    {
        public string TableName { get; }
        public DatabaseChangedMessage(string tableName, string action) : base(action)
        {
            TableName = tableName;
        }
    }
    
    public class IncomeChangedMessage : ValueChangedMessage<string>
    {
        public IncomeChangedMessage(string action) : base(action) { }
    }
}