# 04. Thiết kế giao diện

## Quy ước chung
- Font Segoe UI 10pt, nền trắng, màu chính cam `(230,100,20)`. Tất cả lấy từ `Class/GiaoDien.cs`, không tự đặt riêng.
- Tiêu đề form: `GiaoDien.TaoTieuDe("...")` (thanh cam phía trên).
- Nút: `GiaoDien.TaoNut(chu, mau)`. Bảng: `GiaoDien.LamDepBang(dgv, tieuDeCot)`; cột tiền: `GiaoDien.DinhDangTien(dgv, "DonGia")` (hiện `55.000 đ`).
- Tiêu đề cột hiển thị tiếng Việt có dấu; tên cột trong code vẫn không dấu.
- Nút Xóa luôn hỏi xác nhận. Báo lỗi bằng MessageBox, nói rõ lỗi ở đâu. Form mở giữa màn hình.
- Đặt tên control: `txt`, `btn`, `dgv`, `cbo`, `lbl`.

## Giao diện theo vai trò
| Vai trò | Kiểu giao diện |
|---|---|
| Admin | Menu bên trái, bên phải là màn CRUD: bảng (DataGridView) bên trái, ô nhập bên phải, hàng nút Thêm, Sửa, Xóa, Lưu, Hủy, Tìm |
| Thu ngân | Không dùng bảng thô: bàn là nút màu, món là thẻ (tên, giá), hóa đơn là danh sách bên phải |
| Bếp | Chữ to, 3 cột thẻ món (Chờ, Đang làm, Xong), bấm một nút để đổi trạng thái |

## Danh sách form
| Form | Ai dùng | Nội dung | Người làm |
|---|---|---|---|
| frmDangNhap | Mọi người | Tên đăng nhập, mật khẩu | Hải |
| frmMain | Mọi người | Menu theo vai trò, thanh trạng thái (tên, vai trò), cảnh báo kho cho Admin | Hải |
| frmNhanVien, frmDoiMatKhau | Admin; mọi người | CRUD nhân viên; đổi mật khẩu | Hải |
| frmThanhToan, frmHoaDon | Thu ngân, Admin | Thanh toán, giảm giá, In hóa đơn; lịch sử hóa đơn | Hải |
| frmDanhMuc, frmMonAn | Admin | CRUD danh mục và món (cột trạng thái Còn bán/Hết) | Quốc |
| frmBan | Admin, Thu ngân | Sơ đồ bàn: xanh là Trống, đỏ là Có khách | Quốc |
| frmOrder | Thu ngân | Gọi món (bố cục bên dưới) | Quốc |
| frmBep | Bếp | 3 cột Chờ, Đang làm, Xong; tự làm mới vài giây | Quốc |
| frmThongKe | Admin | Doanh thu theo khoảng ngày, 5 món bán chạy | Quốc |
| frmKhachHang | Admin, Thu ngân | CRUD khách, hiện điểm và hạng | Duy |
| frmNguyenLieu, frmNhaCungCap | Admin | CRUD; nguyên liệu sắp hết tô đỏ | Duy |
| frmCongThuc | Admin | Chọn món, thêm nguyên liệu và số lượng | Duy |
| frmPhieuNhap | Admin | Chọn nhà cung cấp, thêm nguyên liệu, lưu thì tồn kho tăng | Duy |
| frmNhapXml | Admin | Xuất thực đơn ra XML; nhập file XML và kiểm tra bằng XSD | Duy |

## Bố cục frmOrder
| Cột trái: bàn | Cột giữa: thực đơn | Cột phải: hóa đơn của bàn |
|---|---|---|
| Nút bàn, màu theo trạng thái | Chọn danh mục, ô tìm món, thẻ món (tên, giá), số lượng, nút Thêm món | Món đã gọi (món, SL, tiền), tạm tính, nút Xóa dòng, Gửi bếp, Thanh toán |

## Bố cục frmBep
| Cột Chờ | Cột Đang làm | Cột Xong |
|---|---|---|
| Món mới: bàn, tên món, số lượng, giờ gọi; nút "Nhận làm" | Món đang chế biến; nút "Làm xong" | Món đã xong, chờ phục vụ (mất sau vài phút) |