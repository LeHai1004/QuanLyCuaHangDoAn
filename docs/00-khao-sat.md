# 00. Khảo sát và ý tưởng

## Bài toán
Quán ăn nhỏ thường ghi order bằng giấy, tính tiền bằng tay, kiểm kho bằng mắt. Hậu quả: nhầm món, tính sai tiền, hết nguyên liệu bất ngờ, khó biết doanh thu và khách quen.

## Giải pháp
Ứng dụng Windows (WinForms) giúp quán quản lý món, bàn, gọi món, thanh toán và kho nguyên liệu. Dữ liệu lưu trong SQL Server, đưa ra file XML để hiển thị, sửa, kiểm tra bằng XSD và in hóa đơn bằng XSLT.

## Người dùng
| Vai trò | Việc chính |
|---|---|
| Admin (chủ quán) | Quản lý món, bàn, nhân viên, khách, kho, nhà cung cấp; xem thống kê; nhập/xuất XML |
| Thu ngân | Chọn bàn, gọi món, gửi bếp, thanh toán, in hóa đơn, tạo khách mới |
| Nhân viên bếp | Xem món cần làm, đổi trạng thái Chờ, Đang làm, Xong |
| Khách hàng | Không dùng phần mềm; chỉ có trong dữ liệu để tích điểm và nhận hóa đơn |

## 4 tính năng đặc biệt
| Tính năng | Lợi ích | Người làm |
|---|---|---|
| Hóa đơn in đẹp (XSLT) | Hóa đơn rõ ràng, quán chuyên nghiệp | Hải |
| Màn hình bếp | Bếp thấy món ngay, ít nhầm món | Quốc |
| Tích điểm, hạng thành viên | Khách quen được giảm giá tự động (Thường, Bạc 5%, Vàng 10%) | Duy |
| Kho thông minh | Bán món thì kho tự giảm, cảnh báo sắp hết, thiếu nguyên liệu thì món tự báo "Hết" | Duy |

## Yêu cầu của thầy (phần cơ bản)
Thêm, sửa, xóa, tìm kiếm, kiểm tra hợp lệ bằng XSD, cập nhật database.

## Công nghệ
WinForms C# (.NET Framework 4.8), SQL Server, XML/XSD/XSLT. Không dùng API (đã chốt, xem `08-quyet-dinh.md`).

## Câu hỏi còn treo cho thầy
1. Đề tài có được làm bằng WinForms như project mẫu không?
2. Hạn nộp, ngày báo cáo và hình thức demo?
3. Dùng LocalDB hay Express, có bắt buộc đồng bộ XML với database không?
4. Yêu cầu tối thiểu về số form hoặc số chức năng?