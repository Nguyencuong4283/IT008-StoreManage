# IT008-StoreManage
Đồ án IT008
-Thành viên thực hiện :
 + Lê Văn An
 + Nguyễn Cao Cường
 + Trần Đình Sang

-Mô tả : 
    +Công nghệ : 
                . Avalonia
                . SQLite
                . Github (quản lí code)
    +Sản phẩm : Ứng dụng desktop quản lí cửa hàng quần áo .
    +Đối tượng sử dụng : Nhân viên + Quản lí
    +Tính năng : Thực hiện các yêu cầu trong quản lí cửa hàng quần áo .
-Cấu trúc thư mục :
    Store/
    ├── Assets/         # Hình ảnh, icon, tài nguyên tĩnh dùng cho giao diện
    ├── Docs/           # Tài liệu dự án (mô tả, báo cáo, hướng dẫn sử dụng)
    ├── Helpers/        # Các hàm tiện ích, class dùng chung (format, validate, ...)
    ├── Messages/       # Định nghĩa thông báo, message box, nội dung hiển thị
    ├── Models/         # Các lớp Model biểu diễn dữ liệu và thực thể nghiệp vụ
    ├── Scripts/        # Script hỗ trợ (seed data, xử lý dữ liệu, tiện ích)
    ├── Services/       # Xử lý nghiệp vụ, giao tiếp dữ liệu, API, database
    ├── Styles/         # File style giao diện (CSS/XAML styles, themes)
    ├── ViewModels/     # ViewModel theo mô hình MVVM, xử lý logic cho View
    ├── Views/          # Giao diện người dùng (Window, Page, UserControl)
    │
    ├── App.xaml        # Cấu hình tài nguyên và khởi tạo ứng dụng
    ├── app.manifest    # Cấu hình quyền và thông tin ứng dụng
    ├── appsettings.json        # Cấu hình runtime (KHÔNG commit nếu có mật khẩu)
    ├── appsettings.example.json # File mẫu cấu hình
    ├── Program.cs     # Điểm vào chính của chương trình
    └── ViewLocator.cs # Ánh xạ ViewModel ↔ View
