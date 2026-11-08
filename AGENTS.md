# AGENTS.md - Quản lý cửa hàng đồ ăn

## Dự án
Ứng dụng Windows (WinForms, C#, .NET Framework 4.8) quản lý cửa hàng đồ ăn: món, bàn, gọi món, thanh toán, kho nguyên liệu.
Dữ liệu nằm trong SQL Server (CSDL `QLCuaHangDoAn`, 12 bảng) và được đưa ra XML/XSD/XSLT.
KHÔNG phải web. KHÔNG dùng API. Môn Công nghệ XML, lớp 126CNX02. Nhóm: Hải, Quốc, Duy.

## Người dùng phần mềm
- Admin: quản lý toàn bộ, thống kê, nhập/xuất XML.
- Thu ngân: chọn bàn, gọi món, gửi bếp, thanh toán, in hóa đơn.
- Bếp: xem món, đổi trạng thái Chờ / Đang làm / Xong.
- Khách hàng: không dùng phần mềm, chỉ có trong dữ liệu (tích điểm, hóa đơn).

## Luật cho AI (bắt buộc)
1. Mỗi form chỉ do một người sở hữu. Chỉ sửa file của người đang giao việc, không sửa form của người khác.
2. File dùng chung (`Class/FileXml.cs`, `Class/GiaoDien.cs`, `Program.cs`, `frmMain`) chỉ Hải sửa.
3. Tên trong code không dấu: class PascalCase, biến camelCase, form bắt đầu bằng `frm`. Control: `txt`, `btn`, `dgv`, `cbo`, `lbl`.
4. Chữ hiển thị cho người dùng có dấu tiếng Việt. Tiền hiển thị dạng `55.000 đ`.
5. Viết theo phong cách của nhóm, không sao chép nguyên từ project mẫu của thầy (QuanLySieuThi).
6. Mọi câu SQL dùng `SqlParameter`, không nối chuỗi từ ô nhập. Câu SQL truyền vào `TaoXml` phải là chuỗi cố định trong code.
7. Không commit mật khẩu thật hay chuỗi kết nối có mật khẩu. Chuỗi kết nối dùng Windows Authentication.
8. Kiểm tra quyền trong code, không chỉ ẩn menu.
9. Không sửa `bin`, `obj`, `.vs`. Làm xong task thì cập nhật `docs/07-tien-do.md`.

## Cách dùng FileXml (QUAN TRỌNG)
`FileXml` là lớp thường, KHÔNG static. Phải tạo đối tượng trước:
```csharp
var fx = new FileXml();
fx.TaoXml("SELECT * FROM MonAn", "MonAn", "MonAn.xml"); // sql, tenBang, tenFile
DataTable dt = fx.HienThi("MonAn.xml");
string ma = fx.TaoMa("MA", "MonAn.xml", "MaMon");
```
Các hàm: `HienThi`, `TaoXml`, `TonTai`, `TaoMa`, `Them`, `Sua`, `Xoa`, `CapNhatDb`, `KiemTraXsd`, `XuatHtml`, `MoTrinhDuyet`.
`Them/Sua/Xoa` nhận cặp cột-giá trị (`IDictionary<string,string>`), không dùng XPath tự do.
File XML sinh lúc chạy vào thư mục `Data` cạnh file .exe.

## Giao diện dùng chung (Class/GiaoDien.cs)
Gọi `GiaoDien.TaoTieuDe`, `TaoNut`, `LamDepBang`, `DinhDangTien`. Không tự đặt màu/font riêng.
Màu chính cam `(230,100,20)`, nền trắng, font Segoe UI 10. Bảng chỉ dùng ở màn Admin; thu ngân và bếp dùng thẻ/nút.

## Hợp đồng giữa các người (không tự ý đổi)
Mật khẩu: SHA-256 của chuỗi `TenDN:matKhauGoc`, UTF-8, 64 ký tự hex thường. Tài khoản mẫu: admin / thungan1 / bep1, mật khẩu gốc 123456.
Giá trị trạng thái (gõ đúng dấu):
- Ban.TrangThai: Trống | Có khách
- HoaDon.TrangThai: Đang phục vụ | Đã thanh toán
- ChiTietHoaDon.TrangThaiBep: Chờ | Đang làm | Xong
- MonAn.TrangThai: Còn bán | Hết
- NhanVien.ChucVu: Admin | ThuNgan | Bep
- KhachHang.Hang: Thường | Bạc | Vàng
Giảm giá lưu theo phần trăm: 0, 5 hoặc 10. Tích điểm: 10.000đ = 1 điểm; Bạc từ 100 điểm giảm 5%; Vàng từ 300 điểm giảm 10%.
Mã: DM, MA, BA, NV, KH, HD, NL, NCC, PN + 3 chữ số.
Hàm chéo (gọi tên đúng): `KhachHang.PhanTramGiam(maKH)`, `KhachHang.CongDiem(maKH, tongTien)`, `Kho.TruKhoTheoHoaDon(maHD)`, `Kho.DuNguyenLieu(maMon, soLuong)`.

## Git
Làm trên nhánh `feature/<ten>-<viec>`, gộp vào `develop` bằng Pull Request. Không push thẳng `main`/`develop`. Commit dạng `feat|fix|docs|refactor|chore(...)`: nội dung ngắn.

## Tài liệu
Xem thư mục `docs/` (00 khảo sát đến 09 lỗi đã gặp).