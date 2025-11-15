namespace Store.Messages
{
    // Message chung cho mọi thay đổi database
    public class DatabaseChangedMessage
    {
        public string TableName { get; set; }
        public string Action { get; set; } // "Insert", "Update", "Delete"
        
        public DatabaseChangedMessage(string tableName, string action)
        {
            TableName = tableName;
            Action = action;
        }
    }
    
    // Message cụ thể cho từng bảng
    public class HoaDonChangedMessage
    {
        public string Action { get; set; }
        
        public HoaDonChangedMessage(string action)
        {
            Action = action;
        }
    }
    
    public class SanPhamChangedMessage
    {
        public string Action { get; set; }
        
        public SanPhamChangedMessage(string action)
        {
            Action = action;
        }
    }
    
    public class KhachHangChangedMessage
    {
        public string Action { get; set; }
        
        public KhachHangChangedMessage(string action)
        {
            Action = action;
        }
    }
}
