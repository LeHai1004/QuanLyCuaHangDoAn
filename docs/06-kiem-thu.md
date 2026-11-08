# 06. Kiểm thử

Mỗi task chạy thử theo mục "Xong khi" trước khi mở Pull Request. Ghi kết quả: Đạt / Lỗi (kèm ghi chú).

## Phiếu kiểm thử theo task
| Task | Cách thử | Kết quả mong đợi | Kết quả |
|---|---|---|---|
| T1 | F5 chạy Form1 | Bảng 8 món, đơn giá dạng `55.000 đ`, có file `Data/MonAn.xml` | Đạt |
| T1 | Sửa XML rồi `CapNhatDb`, SELECT trong SSMS | DB đổi theo XML, hai chiều đều đúng | Đạt |
| T2 | Đăng nhập admin, thungan1, bep1 (mật khẩu 123456) | Menu đúng quyền; sai mật khẩu báo lỗi | |
| T3 | Thêm, sửa, xóa, tìm món | Mã tự sinh; XML và DB khớp | |
| T4 | Đổi trạng thái bàn | Màu xanh/đỏ đúng | |
| T5 | Gọi món, gửi bếp | Tạm tính đúng; bàn Có khách; món là Chờ | |
| T6 | Mở 2 cửa sổ: thu ngân và bếp | Món mới hiện trong vài giây | |
| T7 | Thanh toán | Tổng tiền đúng; bàn Trống; hóa đơn có trong XML và DB | |
| T8 | Bấm In hóa đơn | Trình duyệt mở đúng hóa đơn | |
| T9 | Thanh toán khách hạng Thường, Bạc, Vàng | Giảm 0, 5, 10%; điểm tăng; đủ điểm thì lên hạng | |
| T10 | CRUD nguyên liệu | NL005 (cà phê bột) tô đỏ vì dưới định mức | |
| T11 | Bán Cà phê sữa | Kho giảm đúng; cảnh báo; thiếu nguyên liệu món tự Hết | |
| T12 | Lưu phiếu nhập | TonKho tăng; xem lại được lịch sử | |
| T13 | Nhập XML đúng và sai | Đúng nhập được; sai báo rõ dòng, cột | |
| T14 | Thống kê một khoảng ngày | Khớp hóa đơn (mẫu: HD001 114.000, HD002 75.000) | |

## Kịch bản demo (T15)
1. Đăng nhập thu ngân, chọn bàn trống, gọi 2 Phở bò + 2 Trà đá, gửi bếp.
2. Mở cửa sổ bếp, nhận làm, làm xong.
3. Thanh toán với khách KH002 (Bạc): giảm 5%, tổng 114.000đ, điểm tăng.
4. In hóa đơn (XSLT) mở trình duyệt.
5. Admin xem kho: món trừ đúng, nguyên liệu sắp hết có cảnh báo.
6. Admin xuất thực đơn XML, nhập lại, thử file sai để thấy báo lỗi XSD.
7. Xem thống kê doanh thu.

Chạy trên máy sạch: cài SQL Server, chạy `QLCHang.sql` rồi `Dulieumau.sql`, mở solution, F5.