# 08. Quyết định đã chốt

| # | Quyết định | Lý do |
|---|---|---|
| 1 | Làm ứng dụng WinForms (C#, .NET Framework 4.8), không làm web | Đề là ứng dụng Windows, mẫu của thầy (QuanLySieuThi) là WinForms. Còn chờ thầy xác nhận |
| 2 | Không dùng API | Đơn giản hóa, đúng phạm vi môn Công nghệ XML |
| 3 | Dữ liệu SQL Server + XML/XSD/XSLT. XML sinh lúc chạy vào thư mục Data cạnh file .exe; XSD viết tay (Duy, T13), TaoXml không ghi đè XSD đã có | Đáp ứng yêu cầu hiển thị, sửa, kiểm tra hợp lệ, in hóa đơn bằng XML |
| 4 | Giảm giá lưu theo phần trăm: 0, 5, 10 (bản kế hoạch cũ ghi 0.05, 0.10) | Khớp cột `PhanTramGiam decimal(5,2)` |
| 5 | `FileXml` là lớp thường (phải `new FileXml()`), `Them/Sua/Xoa` nhận cặp cột-giá trị, không dùng XPath tự do | Tránh XPath injection; bản kế hoạch cũ ghi như hàm static |
| 6 | Tên file SQL thực tế: `QLCHang.sql`, `Dulieumau.sql` | Đã tạo và chạy được |
| 7 | Dùng `SqlParameter`, chặn DTD khi nhập XML ngoài, kiểm tra quyền trong code | Bảo mật cơ bản |
| 8 | Mật khẩu: SHA-256 của `TenDN:matKhauGoc`, UTF-8, 64 hex thường | Hợp đồng chung cho cả nhóm |
| 9 | Chuỗi kết nối dùng Windows Authentication, `Data Source=.`; cả nhóm dùng chung tên server và `Dulieumau.sql` | Repo Public, không để mật khẩu `sa` |
| 10 | Giao diện thống nhất qua `Class/GiaoDien.cs` (cam `230,100,20`, Segoe UI). Admin dùng bảng đã làm đẹp; thu ngân và bếp dùng thẻ/nút | Đẹp và đồng nhất, không dùng thư viện ngoài để cả nhóm khỏi cài thêm |
| 11 | Mỗi form do một người sở hữu; làm theo nhánh feature + Pull Request vào develop (bật lại rule bảo vệ từ T2) | Tránh xung đột file `.Designer.cs`, `.resx` |