# 07. Tiến độ

Mỗi người tự cập nhật task của mình. Trạng thái: Chưa bắt đầu | Đang làm | Chờ review | Xong.

| Task | Người | Trạng thái | Nhánh | PR | Ghi chú |
|---|---|---|---|---|---|
| T1 | Hải | Xong | develop | - | Đã xong FileXml, GiaoDien, script CSDL, CapNhatDb đã thử với DB thật. Còn bộ docs (đang làm) |
| T2 | Hải | Chưa bắt đầu | - | - | T2 | Hải | Xong | feature/hai-dangnhap | #1 | Đăng nhập, phân quyền, nhân viên, đổi mật khẩu. Rồi: |
| T3 | Quốc | Chưa bắt đầu | - | - | Chờ T1 |
| T4 | Quốc | Đang làm | - | - | Có CRUD, BanSoDoControl dùng lại; build đạt, đã kiểm tra sinh mã BA. Còn thử CRUD với SQL Server, tích hợp T5 và nối mục Bàn ở frmMain (file do Hải sở hữu) |
| T5 | Quốc | Đang làm | - | - | Đã tạo frmOrder: chọn bàn/món, tạo hóa đơn, xóa dòng chưa gửi, chặn sửa món đã gửi, gửi riêng dòng mới và kiểm tra kho theo tổng số lượng. Còn chạy thử luồng trên SQL Server và nối mục Gọi món ở frmMain (file do Hải sở hữu); Thanh toán chờ T7 |
| T6 | Quốc | Chờ review | feature/quoc-bep | - | Đã tạo frmBep: 3 cột Chờ, Đang làm, Xong; chữ to, thẻ món đủ thông tin; nút Nhận làm và Làm xong; cập nhật TrangThaiBep vào DB bằng SqlParameter; Timer tự làm mới 4s qua fx.TaoXml; cột Xong giữ 5 phút gần nhất; chỉ lấy món thuộc hóa đơn Đang phục vụ |
| T7 | Hải | Chờ review | - | - | Đã tạo frmThanhToan; giao dịch thanh toán, cập nhật XML và tìm khách bằng SĐT. Cần chạy thử với SQL Server và các hàm tích điểm/trừ kho đã hoàn thiện |
| T8 | Hải | Chờ review | - | - | Đã tạo XSLT in hóa đơn, gộp dữ liệu theo hóa đơn được chọn và mở HTML bằng trình duyệt. Cần chạy thử từ ứng dụng và kiểm tra bản in thực tế |
| T9 | Duy | Chưa bắt đầu | - | - | Chờ T1 |
| T10 | Duy | Chưa bắt đầu | - | - | Chờ T1 |
| T11 | Duy | Chưa bắt đầu | - | - | Chờ T3, T10 |
| T12 | Duy | Chưa bắt đầu | - | - | Chờ T10 |
| T13 | Duy | Chưa bắt đầu | - | - | Chờ T3 |
| T14 | Quốc | Chờ review | feature/quoc-thongke | - | Đã tạo frmThongKe: 2 DateTimePicker lấy đến hết ngày cuối, chỉ tính HĐ Đã thanh toán, truy vấn SQL dùng SqlParameter; hiển thị tổng doanh thu, số HĐ, doanh thu TB, bảng doanh thu theo ngày, top 5 món bán chạy kèm biểu đồ cột DataVisualization |
| T15 | Cả nhóm | Chưa bắt đầu | - | - | Sau khi xong tất cả |
| T16 | Cả nhóm | Chưa bắt đầu | - | - | Làm song song |

Checkpoint 1: sau T5. Checkpoint 2: sau T8 và T11. Ngày cụ thể: điền khi biết hạn của thầy.