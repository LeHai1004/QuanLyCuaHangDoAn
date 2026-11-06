/* du-lieu-mau.sql - chay SAU tao-csdl.sql */
USE QLCuaHangDoAn;
GO

/* QUY UOC MAT KHAU (hop dong, khong tu y doi):
   MatKhau = SHA-256 cua chuoi (TenDN + ":" + matKhauGoc), ma hoa UTF-8,
   ghi ra 64 ky tu hex viet thuong.
   Tai khoan mau: admin / thungan1 / bep1, mat khau goc deu la 123456 */

INSERT INTO DanhMuc (MaDM, TenDM) VALUES
(N'DM001', N'Món chính'),
(N'DM002', N'Đồ uống'),
(N'DM003', N'Tráng miệng');

INSERT INTO MonAn (MaMon, TenMon, MaDM, DonGia, DVT, TrangThai) VALUES
(N'MA001', N'Phở bò',            N'DM001', 55000, N'tô',   N'Còn bán'),
(N'MA002', N'Bún chả',           N'DM001', 50000, N'phần', N'Còn bán'),
(N'MA003', N'Cơm gà xối mỡ',     N'DM001', 50000, N'đĩa',  N'Còn bán'),
(N'MA004', N'Mì xào hải sản',    N'DM001', 60000, N'đĩa',  N'Còn bán'),
(N'MA005', N'Trà đá',            N'DM002', 5000,  N'ly',   N'Còn bán'),
(N'MA006', N'Cà phê sữa',        N'DM002', 25000, N'ly',   N'Còn bán'),
(N'MA007', N'Nước cam',          N'DM002', 30000, N'ly',   N'Còn bán'),
(N'MA008', N'Chè khúc bạch',     N'DM003', 35000, N'ly',   N'Còn bán');

INSERT INTO Ban (MaBan, TenBan, SoGhe, TrangThai) VALUES
(N'BA001', N'Bàn 1', 2, N'Trống'),
(N'BA002', N'Bàn 2', 4, N'Trống'),
(N'BA003', N'Bàn 3', 4, N'Trống'),
(N'BA004', N'Bàn 4', 6, N'Trống'),
(N'BA005', N'Bàn 5', 6, N'Trống'),
(N'BA006', N'Bàn 6', 8, N'Trống');

INSERT INTO NhanVien (MaNV, TenNV, SDT, ChucVu, TenDN, MatKhau) VALUES
(N'NV001', N'Quản trị viên', '0905000001', N'Admin',   'admin',    '2bbbe3f67242a7ac6ad6a3e207dc34b10d250ce40b8958ffed6a68085f2d8e7f'),
(N'NV002', N'Thu ngân 1',    '0905000002', N'ThuNgan', 'thungan1', '1aa938333e8c3d01816bb8f208767b95312d37f3a8788dc0d4ed6eaab3ef99ff'),
(N'NV003', N'Bếp 1',         '0905000003', N'Bep',     'bep1',     '74226fe213d4a8bfbc86d1ced22b9954b6f870a42c2d832f2cbeae88ab151b0d');

/* 3 khach: du 3 hang de demo (Thuong < 100 diem, Bac >= 100, Vang >= 300) */
INSERT INTO KhachHang (MaKH, TenKH, SDT, DiemTichLuy, Hang) VALUES
(N'KH001', N'Nguyễn Văn An',  '0900000001', 40,  N'Thường'),
(N'KH002', N'Trần Thị Bình',  '0900000002', 150, N'Bạc'),
(N'KH003', N'Lê Minh Châu',   '0900000003', 320, N'Vàng');

/* TonKho la ton luc bat dau (khong tinh lai tu phieu nhap).
   NL005 co TonKho <= DinhMuc de demo canh bao sap het. */
INSERT INTO NguyenLieu (MaNL, TenNL, DVT, TonKho, DinhMuc) VALUES
(N'NL001', N'Bánh phở',       N'kg',   20.00, 5.00),
(N'NL002', N'Thịt bò',        N'kg',   8.00,  3.00),
(N'NL003', N'Thịt gà',        N'kg',   10.00, 3.00),
(N'NL004', N'Gạo',            N'kg',   30.00, 10.00),
(N'NL005', N'Cà phê bột',     N'kg',   1.00,  2.00),
(N'NL006', N'Sữa đặc',        N'hộp',  40.00, 10.00),
(N'NL007', N'Cam',            N'trái', 50.00, 15.00),
(N'NL008', N'Hải sản hỗn hợp',N'kg',   5.00,  2.00),
(N'NL009', N'Bún tươi',       N'kg',   15.00, 5.00),
(N'NL010', N'Thịt heo',       N'kg',   12.00, 4.00),
(N'NL011', N'Mì trứng',       N'kg',   10.00, 3.00);

INSERT INTO CongThuc (MaMon, MaNL, SoLuong) VALUES
(N'MA001', N'NL001', 0.150), (N'MA001', N'NL002', 0.100),
(N'MA002', N'NL009', 0.200), (N'MA002', N'NL010', 0.150),
(N'MA003', N'NL004', 0.200), (N'MA003', N'NL003', 0.250),
(N'MA004', N'NL011', 0.150), (N'MA004', N'NL008', 0.120),
(N'MA006', N'NL005', 0.020), (N'MA006', N'NL006', 0.030),
(N'MA007', N'NL007', 2.000);

INSERT INTO NhaCungCap (MaNCC, TenNCC, SDT, DiaChi) VALUES
(N'NCC001', N'Cửa hàng Thực phẩm Minh Phát', '0236000001', N'12 Nguyễn Văn Linh, Đà Nẵng'),
(N'NCC002', N'Đại lý Gạo Tám Thơm',          '0236000002', N'45 Hàm Nghi, Đà Nẵng');

INSERT INTO PhieuNhap (MaPN, MaNCC, MaNV, NgayNhap) VALUES
(N'PN001', N'NCC001', N'NV001', '20261101 08:00');

INSERT INTO ChiTietPhieuNhap (MaPN, MaNL, SoLuong, DonGia) VALUES
(N'PN001', N'NL002', 10.00, 220000),
(N'PN001', N'NL010', 8.00,  95000);

/* 2 hoa don da thanh toan de thu thong ke.
   HD001: (2 pho + 2 tra da) = 120000, KH002 hang Bac giam 5% -> 114000 */
INSERT INTO HoaDon (MaHD, MaBan, MaNV, MaKH, NgayLap, TrangThai, PhanTramGiam, TongTien) VALUES
(N'HD001', N'BA001', N'NV002', N'KH002', '20261105 12:30', N'Đã thanh toán', 5.00, 114000),
(N'HD002', N'BA002', N'NV002', NULL,     '20261105 18:45', N'Đã thanh toán', 0.00, 75000);

INSERT INTO ChiTietHoaDon (MaHD, MaMon, SoLuong, DonGia, TrangThaiBep) VALUES
(N'HD001', N'MA001', 2, 55000, N'Xong'),
(N'HD001', N'MA005', 2, 5000,  N'Xong'),
(N'HD002', N'MA003', 1, 50000, N'Xong'),
(N'HD002', N'MA006', 1, 25000, N'Xong');
GO

SELECT 'DanhMuc' AS Bang, COUNT(*) AS SoDong FROM DanhMuc UNION ALL
SELECT 'MonAn', COUNT(*) FROM MonAn UNION ALL
SELECT 'Ban', COUNT(*) FROM Ban UNION ALL
SELECT 'NhanVien', COUNT(*) FROM NhanVien UNION ALL
SELECT 'KhachHang', COUNT(*) FROM KhachHang UNION ALL
SELECT 'NguyenLieu', COUNT(*) FROM NguyenLieu UNION ALL
SELECT 'CongThuc', COUNT(*) FROM CongThuc UNION ALL
SELECT 'NhaCungCap', COUNT(*) FROM NhaCungCap UNION ALL
SELECT 'PhieuNhap', COUNT(*) FROM PhieuNhap UNION ALL
SELECT 'ChiTietPhieuNhap', COUNT(*) FROM ChiTietPhieuNhap UNION ALL
SELECT 'HoaDon', COUNT(*) FROM HoaDon UNION ALL
SELECT 'ChiTietHoaDon', COUNT(*) FROM ChiTietHoaDon;