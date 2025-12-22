# IT008-StoreManage
Đồ án IT008
-Thành viên thực hiện :
 + Lê Văn An
 + Nguyễn Cao Cường

-Mô tả : 
    +Công nghệ : 
                . Avalonia
                . SQLite
                . Github (quản lí code)
    +Sản phẩm : Ứng dụng desktop quản lí cửa hàng quần áo .
    +Đối tượng sử dụng : Nhân viên + Quản lí
    +Tính năng : Thực hiện các yêu cầu trong quản lí cửa hàng quần áo .



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

