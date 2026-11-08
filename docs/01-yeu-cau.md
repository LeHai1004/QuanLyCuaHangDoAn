# 01. Yêu cầu

## Người dùng và quyền
| Vai trò | Quyền |
|---|---|
| Admin | Toàn bộ chức năng quản lý, thống kê, nhập/xuất XML, xem cảnh báo kho |
| Thu ngân | Bàn, gọi món, gửi bếp, thanh toán, in hóa đơn, khách hàng |
| Bếp | Chỉ màn hình bếp |

Quyền phải kiểm tra trong code, không chỉ ẩn menu.

## Chức năng
### Cơ bản (thầy yêu cầu)
- Thêm, sửa, xóa, tìm kiếm trên các danh mục.
- Kiểm tra hợp lệ file XML bằng XSD, báo lỗi rõ dòng, cột.
- Cập nhật dữ liệu từ XML vào SQL Server.
- Nhập/xuất thực đơn ra XML.

### Nghiệp vụ
- Đăng nhập, phân quyền theo vai trò, đổi mật khẩu.
- Quản lý: danh mục, món ăn, bàn, nhân viên, khách hàng, nguyên liệu, nhà cung cấp, công thức, phiếu nhập.
- Gọi món theo bàn, gửi bếp, thanh toán, lưu hóa đơn, trả bàn.
- Thống kê doanh thu theo khoảng ngày, 5 món bán chạy.

### Đặc biệt
1. Hóa đơn in đẹp bằng XSLT, mở trên trình duyệt.
2. Màn hình bếp: 3 cột Chờ, Đang làm, Xong; tự làm mới vài giây một lần.
3. Tích điểm và hạng thành viên: 10.000đ = 1 điểm; Thường dưới 100 điểm; Bạc từ 100 giảm 5%; Vàng từ 300 giảm 10%. Giảm giá theo hạng hiện tại trước khi cộng điểm hóa đơn này.
4. Kho thông minh: thanh toán xong trừ kho theo công thức; TonKho nhỏ hơn hoặc bằng DinhMuc thì cảnh báo; thiếu nguyên liệu thì món tự đổi sang "Hết" và không cho gọi.

## Luồng làm việc
1. Mọi nhân viên đăng nhập, menu hiện theo vai trò.
2. Thu ngân chọn bàn trống, tạo hóa đơn, bàn thành "Có khách".
3. Thu ngân chọn món và số lượng; món thiếu nguyên liệu bị khóa.
4. Thu ngân bấm "Gửi bếp", các món có trạng thái "Chờ".
5. Bếp đổi sang "Đang làm" rồi "Xong".
6. Thu ngân thanh toán: nhập SĐT khách (nếu có), hệ thống giảm giá theo hạng.
7. Hệ thống: hóa đơn "Đã thanh toán", bàn "Trống", cộng điểm, trừ kho, cảnh báo nếu gần hết.
8. Thu ngân bấm "In hóa đơn": XSLT biến XML thành HTML mở trên trình duyệt.
9. Hệ thống cập nhật dữ liệu XML vào SQL Server.
10. Admin xem thống kê, nhập hàng, sửa món, xem cảnh báo kho.

## Phi chức năng
- Chạy trên Windows, .NET Framework 4.8, SQL Server (cả nhóm dùng chung tên server `.`).
- Mật khẩu lưu dạng băm SHA-256, không commit mật khẩu thật.
- Giao diện thống nhất qua `GiaoDien.cs`.