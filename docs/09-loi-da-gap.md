# 09. Lỗi đã gặp và cách sửa

Ai gặp lỗi lạ thì ghi vào đây để khỏi mắc lại.

| # | Lỗi | Nguyên nhân | Cách sửa |
|---|---|---|---|
| 1 | CS0120: `FileXml.HienThi` cần đối tượng | `FileXml` là lớp thường, gọi như hàm static | `var fx = new FileXml(); fx.HienThi("MonAn.xml");` |
| 2 | CS7036: `TaoXml` thiếu tham số `tenBang` | `TaoXml` cần 3 tham số | `fx.TaoXml("SELECT * FROM MonAn", "MonAn", "MonAn.xml");` |
| 3 | Không đọc được chuỗi kết nối | Thiếu reference | Thêm `System.Configuration` vào References |
| 4 | Bảng hiện cột lệch, tiêu đề không dấu | Chưa dùng lớp giao diện chung | Gọi `GiaoDien.LamDepBang(dgv, tieuDeCot)` và `GiaoDien.DinhDangTien(dgv, "DonGia")` sau khi gán DataSource |
| 5 | Git Changes báo 0 thay đổi dù vừa sửa | Đã commit và push trước đó | Kiểm tra View all commits hoặc GitHub nhánh develop |