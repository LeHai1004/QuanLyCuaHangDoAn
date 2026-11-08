# 03. Thiết kế CSDL

CSDL `QLCuaHangDoAn`, 12 bảng. Script: `Database/QLCHang.sql` (tạo, chạy lại sẽ xóa sạch dữ liệu cũ) rồi `Database/Dulieumau.sql` (dữ liệu mẫu). Mỗi bảng có một file XML cùng tên (`MonAn.xml`) và một file XSD (`MonAn.xsd`) trong thư mục `Data`.

| Bảng | Cột | Ghi chú |
|---|---|---|
| DanhMuc | MaDM nvarchar(10) PK, TenDM nvarchar(50) | Mã DM |
| MonAn | MaMon nvarchar(10) PK, TenMon nvarchar(100), MaDM FK, DonGia decimal(18,0), DVT nvarchar(20), TrangThai nvarchar(10) | Còn bán \| Hết. Mã MA |
| Ban | MaBan nvarchar(10) PK, TenBan nvarchar(30), SoGhe int, TrangThai nvarchar(15) | Trống \| Có khách. Mã BA |
| NhanVien | MaNV nvarchar(10) PK, TenNV nvarchar(100), SDT varchar(15), ChucVu nvarchar(10), TenDN varchar(30) duy nhất, MatKhau varchar(64) | Admin \| ThuNgan \| Bep. Mật khẩu SHA-256 |
| KhachHang | MaKH nvarchar(10) PK, TenKH nvarchar(100), SDT varchar(15) duy nhất, DiemTichLuy int, Hang nvarchar(10) | Thường \| Bạc \| Vàng |
| HoaDon | MaHD nvarchar(10) PK, MaBan FK, MaNV FK, MaKH FK (được trống), NgayLap datetime, TrangThai nvarchar(15), PhanTramGiam decimal(5,2), TongTien decimal(18,0) | Đang phục vụ \| Đã thanh toán. PhanTramGiam là 0, 5, 10 |
| ChiTietHoaDon | MaHD FK, MaMon FK (cùng PK), SoLuong int, DonGia decimal(18,0), TrangThaiBep nvarchar(10) | Chờ \| Đang làm \| Xong |
| NguyenLieu | MaNL nvarchar(10) PK, TenNL nvarchar(100), DVT nvarchar(20), TonKho decimal(18,2), DinhMuc decimal(18,2) | TonKho <= DinhMuc thì cảnh báo |
| CongThuc | MaMon FK, MaNL FK (cùng PK), SoLuong decimal(18,3) | Một phần món cần bao nhiêu nguyên liệu |
| NhaCungCap | MaNCC nvarchar(10) PK, TenNCC nvarchar(100), SDT varchar(15), DiaChi nvarchar(200) | |
| PhieuNhap | MaPN nvarchar(10) PK, MaNCC FK, MaNV FK, NgayNhap datetime | |
| ChiTietPhieuNhap | MaPN FK, MaNL FK (cùng PK), SoLuong decimal(18,2), DonGia decimal(18,0) | Nhập xong TonKho tăng |

## Quan hệ chính
- Một danh mục có nhiều món. Một món có nhiều dòng công thức.
- Một hóa đơn thuộc một bàn, một nhân viên, có thể thuộc một khách; có nhiều dòng chi tiết.
- Một phiếu nhập thuộc một nhà cung cấp và có nhiều nguyên liệu.

## Luật tính toán
| Luật | Nội dung |
|---|---|
| Tích điểm | 10.000đ = 1 điểm. Thường dưới 100 điểm; Bạc từ 100 giảm 5%; Vàng từ 300 giảm 10%. Giảm giá tính theo hạng hiện tại của khách trước khi cộng điểm hóa đơn này |
| Kho thông minh | Thanh toán xong trừ TonKho = SoLuong công thức x số món bán. TonKho <= DinhMuc thì cảnh báo. Khi gọi món, kho không đủ thì món tự đổi sang Hết và không cho gọi |

TonKho trong dữ liệu mẫu là tồn lúc bắt đầu, không tính lại từ phiếu nhập.