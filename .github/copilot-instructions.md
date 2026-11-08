# Luật cho Copilot - Quản lý cửa hàng đồ ăn

Đọc `AGENTS.md` trước mỗi lần làm việc. Dưới đây là luật ngắn.

- Dự án WinForms C# (.NET Framework 4.8), SQL Server + XML/XSD/XSLT. Không web, không API.
- Chỉ sửa file thuộc task được giao. Không sửa form của người khác. `FileXml.cs`, `GiaoDien.cs`, `Program.cs`, `frmMain` chỉ Hải sửa.
- Tên trong code không dấu (class PascalCase, biến camelCase, form `frm...`, control `txt/btn/dgv/cbo/lbl`). Chữ hiển thị cho người dùng có dấu tiếng Việt.
- `FileXml` là lớp thường: phải `var fx = new FileXml();` rồi mới gọi `fx.TaoXml(sql, tenBang, tenFile)`, `fx.HienThi(tenFile)`...
- Giao diện: dùng `GiaoDien.TaoTieuDe`, `TaoNut`, `LamDepBang`, `DinhDangTien`. Không tự đặt màu/font riêng. Tiền hiển thị `55.000 đ`.
- SQL: luôn dùng `SqlParameter`, không nối chuỗi từ ô nhập. Không commit mật khẩu thật.
- Kiểm tra quyền trong code, không chỉ ẩn menu.
- Giá trị trạng thái gõ đúng chính tả, đúng dấu (xem `AGENTS.md`). Giảm giá là phần trăm 0, 5, 10.
- Viết theo phong cách của nhóm, không sao chép nguyên từ project mẫu của thầy.
- Làm xong task: cập nhật `docs/07-tien-do.md`. Gặp lỗi lạ ghi vào `docs/09-loi-da-gap.md`. Đổi quyết định ghi vào `docs/08-quyet-dinh.md`.
- Code chạy thử được mới coi là xong. Không tự tin 100%, luôn nhắc người dùng chạy thử theo mục "Xong khi" của task.