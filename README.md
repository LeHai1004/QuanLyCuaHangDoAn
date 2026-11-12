# Quản lý cửa hàng đồ ăn

Đồ án môn Công nghệ XML, lớp 126CNX02. Nhóm: Hải, Quốc, Duy.

Ứng dụng Windows (WinForms) giúp quán ăn quản lý món, bàn, gọi món, thanh toán và kho nguyên liệu. Dữ liệu lưu trong SQL Server, đưa ra file XML để hiển thị, sửa, kiểm tra (XSD) và in hóa đơn bằng XSLT.

## Công nghệ

C# WinForms, .NET Framework 4.8 | SQL Server | XML, XSD, XSLT. Không dùng web, không dùng API.

## Người dùng

| Vai trò | Làm gì |
|---|---|
| Admin | Quản lý món, bàn, nhân viên, khách, kho; xem thống kê |
| ThuNgan | Chọn bàn, gọi món, gửi bếp, thanh toán, in hóa đơn |
| Bep | Xem món cần làm, đổi trạng thái Chờ, Đang làm, Xong |

Khách hàng không dùng phần mềm, chỉ có trong dữ liệu để tích điểm và nhận hóa đơn.

## 4 tính năng đặc biệt

1. Hóa đơn in bằng XSLT (Hải)
2. Màn hình bếp (Quốc)
3. Tích điểm, hạng thành viên (Duy)
4. Kho thông minh (Duy)

## Cài đặt để chạy

1. Cài Visual Studio mới (mở được file `.slnx`) và SQL Server (Express hoặc bản đầy đủ) kèm SSMS.
2. Clone repo, chuyển sang nhánh develop: `git switch develop`
3. Trong SSMS chạy lần lượt `QuanLyCuaHangDoAn/Database/QLCHang.sql`, rồi `Dulieumau.sql` (chạy lại sẽ xóa sạch dữ liệu cũ).
4. Mở `QuanLyCuaHangDoAn/App.config`, đổi `Data Source=.` thành tên server SQL của máy bạn, rồi chạy 1 lần để Git bỏ qua thay đổi này:
   `git update-index --skip-worktree QuanLyCuaHangDoAn/App.config`
5. Mở `QuanLyCuaHangDoAn.slnx`, bấm F5.

Tài khoản mẫu (mật khẩu đều là `123456`): `admin`, `thungan1`, `bep1`.

## Cấu trúc thư mục

```
QuanLyCuaHangDoAn/
  Database/   QLCHang.sql (tạo CSDL, 12 bảng), Dulieumau.sql (dữ liệu mẫu)
  Class/      FileXml, GiaoDien, TaiKhoan, PhienDangNhap, KhoKhachHang
  GUI/        các form (mỗi form do 1 người sở hữu)
  Data/       *.xsd (file *.xml sinh lúc chạy)
  Xslt/       HoaDon.xslt, ThucDon.xslt
docs/         kế hoạch, thiết kế CSDL, giao diện, quy ước, tiến độ
```

## Làm việc nhóm

Đọc thẻ "Quy tắc làm việc" trên Trello và `docs/05-quy-uoc-code.md`. Tóm tắt: mỗi task một nhánh `feature/<ten>-<viec>`, tạo Pull Request vào `develop`, người khác chạy thử rồi mới merge. Tiến độ ở `docs/07-tien-do.md`.

## Tiến độ

T1 (nền tảng) và T2 (đăng nhập, phân quyền, nhân viên, đổi mật khẩu) đã xong. Các task còn lại xem `docs/02-ke-hoach.md`.
