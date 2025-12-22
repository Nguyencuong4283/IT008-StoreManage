# Coding Conventions

## 1. Nguyên tắc chung
- Tuân thủ Clean Code
- Code dễ đọc, dễ bảo trì
- Một hàm – một nhiệm vụ

## 2. Quy ước đặt tên
### 2.1 Class
- PascalCase
- Ví dụ: `CustomerService`, `FlightPageViewModel`

### 2.2 Method
- PascalCase
- Động từ + danh từ
- Ví dụ: `CreateTicket()`, `LoadFlights()`

### 2.3 Biến
- camelCase
- Có ý nghĩa
- Ví dụ: `totalPrice`, `flightList`

## 3. Cấu trúc thư mục
- Models: chứa các lớp dữ liệu
- Services: xử lý nghiệp vụ
- ViewModels: xử lý logic cho View

## 4. Comment & Documentation
- Comment cho logic phức tạp
- Không comment code hiển nhiên

## 5. Quy ước Git
- Commit message rõ nghĩa
- Không commit file build (`bin/`, `obj/`)

## 6  Các tên biến trong Model + Servies lấy theo thiết kế CSDL
