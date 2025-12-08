using CommunityToolkit.Mvvm.Messaging;
using Store.Messages;

namespace Store.Helpers
{
    /// <summary>
    /// Helper class để gửi thông báo khi database thay đổi
    /// </summary>
    public static class DatabaseNotifier
    {
        public static void NotifyHoaDonChanged(string action = "Changed")
        {
            WeakReferenceMessenger.Default.Send(new HoaDonChangedMessage(action));
        }
        
        public static void NotifySanPhamChanged(string action = "Changed")
        {
            WeakReferenceMessenger.Default.Send(new SanPhamChangedMessage(action));
        }
        
        public static void NotifyKhachHangChanged(string action = "Changed")
        {
            WeakReferenceMessenger.Default.Send(new KhachHangChangedMessage(action));
        }
        
        public static void NotifyDatabaseChanged(string tableName, string action = "Changed")
        {
            WeakReferenceMessenger.Default.Send(new DatabaseChangedMessage(tableName, action));
        }
    }
}
