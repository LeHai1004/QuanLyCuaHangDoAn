# 02. Kế hoạch

Mỗi task là một chức năng làm trọn vẹn (dữ liệu, xử lý, giao diện) và chạy thử được. ⭐ là task tạo tính năng đặc biệt.

| ID | Việc | Người | Cần có trước | Xong khi |
|---|---|---|---|---|
| T1 | Repo, solution WinForms, .gitignore, script CSDL, FileXml, GiaoDien, hàm rỗng KhachHang/Kho | Hải | - | Cả nhóm clone về chạy được; HienThi đọc XML lên bảng |
| T2 | Đăng nhập, phân quyền, nhân viên, đổi mật khẩu | Hải | T1 | 3 vai trò đăng nhập đúng, menu đúng quyền, mật khẩu lưu băm |
| T3 | Danh mục và món ăn | Quốc | T1 | Thêm, sửa, xóa, tìm chạy; mã tự sinh; XML và DB khớp |
| T4 | Bàn và sơ đồ bàn | Quốc | T1 | Màu bàn theo trạng thái; CRUD bàn |
| T5 | Gọi món (frmOrder), gửi bếp | Quốc | T2, T3, T4 | Tạm tính đúng; bàn thành Có khách; món gửi bếp là Chờ |
| T6 ⭐ | Màn hình bếp | Quốc | T5 | Món mới hiện trong vài giây; đổi trạng thái bên thu ngân thấy |
| T7 | Thanh toán, lưu hóa đơn, trả bàn | Hải | T5 | Tổng tiền đúng; bàn về Trống; hóa đơn có trong XML và DB |
| T8 ⭐ | In hóa đơn bằng XSLT | Hải | T7 | Bấm in mở trình duyệt đúng hóa đơn đó |
| T9 ⭐ | Khách hàng, tích điểm, hạng | Duy | T1 | CRUD khách; thanh toán xong điểm tăng, lên hạng thì giảm giá đúng |
| T10 | Nguyên liệu và nhà cung cấp | Duy | T1 | CRUD cả hai; nguyên liệu dưới định mức tô đỏ |
| T11 ⭐ | Công thức món và kho thông minh | Duy | T3, T10 | Thanh toán xong kho giảm đúng; có cảnh báo; thiếu nguyên liệu tự Hết |
| T12 | Phiếu nhập hàng | Duy | T10 | Lưu phiếu thì tồn kho tăng; xem lại lịch sử |
| T13 | Kiểm tra XSD, nhập/xuất thực đơn XML | Duy | T3 | File đúng nhập được; file sai báo rõ lỗi; xuất rồi nhập lại được |
| T14 | Thống kê doanh thu và món bán chạy | Quốc | T7 | Số liệu khớp hóa đơn; chọn khoảng ngày được |
| T15 | Kiểm thử cả nhóm, sửa lỗi, tập demo | Cả nhóm | Tất cả | Chạy hết luồng ở 01-yeu-cau.md trên máy sạch |
| T16 | Viết báo cáo Word theo mẫu của thầy | Cả nhóm | Song song | Đủ mục lục mẫu, ảnh giao diện |

Nếu gấp thì bỏ T12 và T14 trước. Giữ T13 vì thầy yêu cầu kiểm tra hợp lệ.

## Phân công
| Người | Task | Đặc biệt |
|---|---|---|
| Hải (nhóm trưởng) | T1, T2, T7, T8; giữ repo, Trello, review PR, tích hợp | Hóa đơn XSLT |
| Quốc | T3, T4, T5, T6, T14 | Màn hình bếp |
| Duy | T9, T10, T11, T12, T13 | Tích điểm; Kho thông minh |

Checkpoint 1: sau T5 (chạy tới gửi bếp). Checkpoint 2: sau T8 và T11 (chạy trọn luồng). Điền ngày cụ thể khi biết hạn của thầy.