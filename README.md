# ỨNG DỤNG DESKTOP QUẢN LÝ CỬA HÀNG QUẦN ÁO

## 1. Giới thiệu
Đây là ứng dụng Quản Lý Cửa Hàng Quần Áo được phát triển phục vụ cho mục đích học tập môn Lập trình trực quan. Ứng dụng cung cấp giải pháp toàn diện cho việc quản lý hoạt động kinh doanh của cửa hàng thời trang.

## 2. Công nghệ sử dụng
- **Ngôn ngữ lập trình:** C#
- **Nền tảng:** .NET 9.0 (Avalonia UI Framework)
- **Cơ sở dữ liệu:** SQLite
- **Công cụ phát triển:** Visual Studio 2022
- **Kiến trúc:** MVVM Pattern

## 3. Chức năng chính

### Hệ thống Đăng nhập & Phân quyền
- Giao diện đăng nhập hiện đại với xác thực bảo mật
- **Admin:** Có toàn quyền hệ thống (Thêm, Sửa, Xóa dữ liệu, Quản lý tài khoản)
- **User:** Quyền hạn chế, chỉ được phép xem và tìm kiếm dữ liệu

### Quản lý Sản phẩm & Kho hàng
- **Danh mục sản phẩm:** Quản lý thông tin chi tiết sản phẩm (mã, tên, giá, kích cỡ, màu sắc)
- **Quản lý kho:** Theo dõi tồn kho, nhập hàng, xuất hàng
- **Hình ảnh sản phẩm:** Upload và quản lý hình ảnh sản phẩm

### Quản lý Khách hàng & Bán hàng
- **Hồ sơ khách hàng:** Quản lý thông tin cá nhân, lịch sử mua hàng
- **Tạo hóa đơn:** Giao diện thân thiện cho việc tạo đơn hàng
- **Thanh toán:** Hỗ trợ nhiều hình thức thanh toán

### Báo cáo & Thống kê
- **Doanh thu:** Báo cáo doanh thu theo ngày, tháng, năm
- **Sản phẩm bán chạy:** Thống kê sản phẩm được ưa chuộng
- **Tồn kho:** Cảnh báo sản phẩm sắp hết hàng

### Tìm kiếm & Lọc dữ liệu
- **Tìm kiếm thông minh:** Hỗ trợ tìm kiếm theo mã hoặc tên (hỗ trợ tiếng Việt có dấu và không dấu)
- **Lọc dữ liệu:** Lọc sản phẩm theo danh mục, giá, kích cỡ
- **Sắp xếp:** Sắp xếp dữ liệu theo nhiều tiêu chí

## 4. Tài khoản Demo
Các tài khoản được khởi tạo sẵn trong cơ sở dữ liệu:

| Vai trò | Tên đăng nhập | Mật khẩu | Quyền hạn |
|---------|---------------|----------|-----------|
| Quản trị viên | `admin` | `123456` | Toàn quyền (Full Access) |
| Nhân viên | `user` | `123456` | Chỉ xem và tìm kiếm (Read Only) |

*Note : Tài khoản chỉ dùng để kiểm thử . Sau khi đăng nhập thành công với vai trò admin có thể tạo tài khoản nhân viên và cấp quyền admin để sử dụng được các tính năng . 


## 5. Hướng dẫn cài đặt

### Yêu cầu hệ thống
- **Hệ điều hành:** Windows 10/11
- **Runtime:** .NET 6.0 Runtime (đã tích hợp sẵn trong bản phân phối)

### Cách 1: Sử dụng bản phân phối (Khuyến nghị)
1. Tải file `Store-Application-Distribution.zip`
2. Giải nén file zip vào thư mục mong muốn
3. Chạy file `Store.exe`
4. Đăng nhập bằng tài khoản demo ở trên

### Cách 2: Biên dịch từ mã nguồn
1. **Tải mã nguồn:**
   ```bash
   git clone [https://github.com/Nguyencuong4283/IT008-StoreManage.git]
   ```
   Hoặc tải trực tiếp file ZIP từ repository

2. **Mở project:**
   - Mở file `Store.csproj` bằng Visual Studio 2022
   - Restore NuGet packages

3. **Chạy ứng dụng:**
   - Nhấn F5 hoặc Start để chạy debug
   - Hoặc build Release và chạy file exe

## 6. Hướng dẫn sử dụng

### Bước 1: Đăng nhập
1. Khởi động ứng dụng `Store.exe`
2. Nhập tài khoản và mật khẩu (xem bảng tài khoản demo)
3. Nhấn "Đăng nhập"

### Bước 2: Sử dụng các chức năng
- **Trang chủ:** Xem tổng quan doanh thu và thống kê
- **Sản phẩm:** Quản lý danh mục sản phẩm
- **Khách hàng:** Quản lý thông tin khách hàng
- **Hóa đơn:** Tạo và quản lý đơn hàng
- **Nhập kho:** Quản lý việc nhập hàng
- **Báo cáo:** Xem các báo cáo thống kê

### Bước 3: Thoát ứng dụng
- Sử dụng menu "Đăng xuất" để thoát an toàn
- Hoặc đóng cửa sổ ứng dụng

## 7. Lưu ý quan trọng

- **Dữ liệu:** Được lưu trữ cục bộ trong database SQLite
- **Khởi tạo:** Lần đầu chạy sẽ tự động tạo database và dữ liệu mẫu
- **Quyền truy cập:** Nếu gặp lỗi, hãy chạy với quyền Administrator
- **Backup:** Nên sao lưu file database định kỳ
- **Môi trường:** Project phục vụ mục đích học tập, không sử dụng cho môi trường thực tế

## 8. Nhóm tác giả
Đồ án được thực hiện bởi :
   Sinh viên       |     MSSV
- Lê Văn An        |   24520051
- Nguyễn Cao Cường |   24520237

## 9. Liên hệ hỗ trợ
Nếu gặp vấn đề khi sử dụng hoặc cần hỗ trợ kỹ thuật, vui lòng liên hệ qua:
- Email: levanan1902006@gmail.com

## 10 Cài đặt bổ dung

# Hướng dẫn cấu hình Email để sử dụng tính năng quên mật khẩu (gửi email)

## Bước 1: Tạo file cấu hình

1. Copy file `appsettings.example.json` thành `appsettings.json`
2. File `appsettings.json` sẽ KHÔNG được commit lên Git (đã có trong .gitignore)

## Bước 2: Lấy App Password từ Gmail

### Yêu cầu:
- Phải bật xác thực 2 bước (2FA) trước

### Các bước:
1. Truy cập: https://myaccount.google.com/security
2. Tìm "Xác minh 2 bước" và bật nó
3. Sau khi bật 2FA, tìm "Mật khẩu ứng dụng" (App passwords)
4. Chọn "Mail" và "Windows Computer"
5. Google sẽ tạo mật khẩu 16 ký tự (ví dụ: `abcd efgh ijkl mnop`)
6. Copy mật khẩu này (bỏ dấu cách)

## Bước 3: Cấu hình appsettings.json

Mở file `appsettings.json` và điền thông tin:

```json
{
  "EmailSettings": {
    "FromEmail": "youremail@gmail.com",
    "AppPassword": "abcdefghijklmnop"
  }
}
```

## Bước 4: Bảo mật

- ✅ File `appsettings.json` đã được thêm vào `.gitignore`
- ✅ KHÔNG BAO GIỜ commit file này lên Git
- ✅ Mỗi developer cần tạo file riêng trên máy của mình
- ✅ Trên production, dùng biến môi trường hoặc Azure Key Vault

## Lưu ý:

- Nếu không dùng Gmail, thay đổi SMTP server trong `EmailService.cs`
- Outlook: `smtp.office365.com`, port 587
- Yahoo: `smtp.mail.yahoo.com`, port 587

## Troubleshooting:

**Lỗi: "File appsettings.json không tồn tại"**
- Tạo file từ `appsettings.example.json`

**Lỗi: "Authentication failed"**
- Kiểm tra lại App Password
- Đảm bảo đã bật 2FA
- Thử tạo lại App Password mới


---
*Cảm ơn bạn đã sử dụng ứng dụng Quản lý Cửa hàng Quần áo!*

