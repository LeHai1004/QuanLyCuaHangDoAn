/* tao-csdl.sql - CSDL QLCuaHangDoAn (12 bang)
   CHAY LAI SE XOA SACH DU LIEU CU. Chi chay tren may ca nhan. */
USE master;
GO
IF DB_ID(N'QLCuaHangDoAn') IS NOT NULL
BEGIN
    ALTER DATABASE QLCuaHangDoAn SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QLCuaHangDoAn;
END
GO
CREATE DATABASE QLCuaHangDoAn;
GO
USE QLCuaHangDoAn;
GO

CREATE TABLE DanhMuc (
    MaDM  nvarchar(10) NOT NULL,
    TenDM nvarchar(50) NOT NULL,
    CONSTRAINT PK_DanhMuc PRIMARY KEY (MaDM)
);

CREATE TABLE MonAn (
    MaMon     nvarchar(10)  NOT NULL,
    TenMon    nvarchar(100) NOT NULL,
    MaDM      nvarchar(10)  NOT NULL,
    DonGia    decimal(18,0) NOT NULL,
    DVT       nvarchar(20)  NOT NULL,
    TrangThai nvarchar(10)  NOT NULL DEFAULT N'Còn bán',
    CONSTRAINT PK_MonAn PRIMARY KEY (MaMon),
    CONSTRAINT FK_MonAn_DanhMuc FOREIGN KEY (MaDM) REFERENCES DanhMuc(MaDM),
    CONSTRAINT CK_MonAn_DonGia CHECK (DonGia >= 0),
    CONSTRAINT CK_MonAn_TrangThai CHECK (TrangThai IN (N'Còn bán', N'Hết'))
);

CREATE TABLE Ban (
    MaBan     nvarchar(10) NOT NULL,
    TenBan    nvarchar(30) NOT NULL,
    SoGhe     int          NOT NULL,
    TrangThai nvarchar(15) NOT NULL DEFAULT N'Trống',
    CONSTRAINT PK_Ban PRIMARY KEY (MaBan),
    CONSTRAINT CK_Ban_SoGhe CHECK (SoGhe > 0),
    CONSTRAINT CK_Ban_TrangThai CHECK (TrangThai IN (N'Trống', N'Có khách'))
);

CREATE TABLE NhanVien (
    MaNV    nvarchar(10)  NOT NULL,
    TenNV   nvarchar(100) NOT NULL,
    SDT     varchar(15)   NULL,
    ChucVu  nvarchar(10)  NOT NULL,
    TenDN   varchar(30)   NOT NULL,
    MatKhau varchar(64)   NOT NULL,
    CONSTRAINT PK_NhanVien PRIMARY KEY (MaNV),
    CONSTRAINT UQ_NhanVien_TenDN UNIQUE (TenDN),
    CONSTRAINT CK_NhanVien_ChucVu CHECK (ChucVu IN (N'Admin', N'ThuNgan', N'Bep'))
);

CREATE TABLE KhachHang (
    MaKH        nvarchar(10)  NOT NULL,
    TenKH       nvarchar(100) NOT NULL,
    SDT         varchar(15)   NOT NULL,
    DiemTichLuy int           NOT NULL DEFAULT 0,
    Hang        nvarchar(10)  NOT NULL DEFAULT N'Thường',
    CONSTRAINT PK_KhachHang PRIMARY KEY (MaKH),
    CONSTRAINT UQ_KhachHang_SDT UNIQUE (SDT),
    CONSTRAINT CK_KhachHang_Diem CHECK (DiemTichLuy >= 0),
    CONSTRAINT CK_KhachHang_Hang CHECK (Hang IN (N'Thường', N'Bạc', N'Vàng'))
);

CREATE TABLE HoaDon (
    MaHD          nvarchar(10)  NOT NULL,
    MaBan         nvarchar(10)  NOT NULL,
    MaNV          nvarchar(10)  NOT NULL,
    MaKH          nvarchar(10)  NULL,
    NgayLap       datetime      NOT NULL DEFAULT GETDATE(),
    TrangThai     nvarchar(15)  NOT NULL DEFAULT N'Đang phục vụ',
    PhanTramGiam  decimal(5,2)  NOT NULL DEFAULT 0,   -- don vi %, vi du 5.00 la giam 5%
    TongTien      decimal(18,0) NOT NULL DEFAULT 0,
    CONSTRAINT PK_HoaDon PRIMARY KEY (MaHD),
    CONSTRAINT FK_HoaDon_Ban FOREIGN KEY (MaBan) REFERENCES Ban(MaBan),
    CONSTRAINT FK_HoaDon_NhanVien FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV),
    CONSTRAINT FK_HoaDon_KhachHang FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH),
    CONSTRAINT CK_HoaDon_TrangThai CHECK (TrangThai IN (N'Đang phục vụ', N'Đã thanh toán')),
    CONSTRAINT CK_HoaDon_Giam CHECK (PhanTramGiam >= 0 AND PhanTramGiam <= 100),
    CONSTRAINT CK_HoaDon_TongTien CHECK (TongTien >= 0)
);

CREATE TABLE ChiTietHoaDon (
    MaHD         nvarchar(10)  NOT NULL,
    MaMon        nvarchar(10)  NOT NULL,
    SoLuong      int           NOT NULL,
    DonGia       decimal(18,0) NOT NULL,
    TrangThaiBep nvarchar(10)  NOT NULL DEFAULT N'Chờ',
    CONSTRAINT PK_ChiTietHoaDon PRIMARY KEY (MaHD, MaMon),
    CONSTRAINT FK_CTHD_HoaDon FOREIGN KEY (MaHD) REFERENCES HoaDon(MaHD) ON DELETE CASCADE,
    CONSTRAINT FK_CTHD_MonAn FOREIGN KEY (MaMon) REFERENCES MonAn(MaMon),
    CONSTRAINT CK_CTHD_SoLuong CHECK (SoLuong > 0),
    CONSTRAINT CK_CTHD_DonGia CHECK (DonGia >= 0),
    CONSTRAINT CK_CTHD_Bep CHECK (TrangThaiBep IN (N'Chờ', N'Đang làm', N'Xong'))
);

CREATE TABLE NguyenLieu (
    MaNL    nvarchar(10)  NOT NULL,
    TenNL   nvarchar(100) NOT NULL,
    DVT     nvarchar(20)  NOT NULL,
    TonKho  decimal(18,2) NOT NULL DEFAULT 0,
    DinhMuc decimal(18,2) NOT NULL DEFAULT 0,
    CONSTRAINT PK_NguyenLieu PRIMARY KEY (MaNL),
    CONSTRAINT CK_NguyenLieu_DinhMuc CHECK (DinhMuc >= 0)
);

CREATE TABLE CongThuc (
    MaMon   nvarchar(10)  NOT NULL,
    MaNL    nvarchar(10)  NOT NULL,
    SoLuong decimal(18,3) NOT NULL,
    CONSTRAINT PK_CongThuc PRIMARY KEY (MaMon, MaNL),
    CONSTRAINT FK_CongThuc_MonAn FOREIGN KEY (MaMon) REFERENCES MonAn(MaMon) ON DELETE CASCADE,
    CONSTRAINT FK_CongThuc_NguyenLieu FOREIGN KEY (MaNL) REFERENCES NguyenLieu(MaNL),
    CONSTRAINT CK_CongThuc_SoLuong CHECK (SoLuong > 0)
);

CREATE TABLE NhaCungCap (
    MaNCC  nvarchar(10)  NOT NULL,
    TenNCC nvarchar(100) NOT NULL,
    SDT    varchar(15)   NULL,
    DiaChi nvarchar(200) NULL,
    CONSTRAINT PK_NhaCungCap PRIMARY KEY (MaNCC)
);

CREATE TABLE PhieuNhap (
    MaPN    nvarchar(10) NOT NULL,
    MaNCC   nvarchar(10) NOT NULL,
    MaNV    nvarchar(10) NOT NULL,
    NgayNhap datetime    NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_PhieuNhap PRIMARY KEY (MaPN),
    CONSTRAINT FK_PhieuNhap_NCC FOREIGN KEY (MaNCC) REFERENCES NhaCungCap(MaNCC),
    CONSTRAINT FK_PhieuNhap_NhanVien FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV)
);

CREATE TABLE ChiTietPhieuNhap (
    MaPN    nvarchar(10)  NOT NULL,
    MaNL    nvarchar(10)  NOT NULL,
    SoLuong decimal(18,2) NOT NULL,
    DonGia  decimal(18,0) NOT NULL,
    CONSTRAINT PK_ChiTietPhieuNhap PRIMARY KEY (MaPN, MaNL),
    CONSTRAINT FK_CTPN_PhieuNhap FOREIGN KEY (MaPN) REFERENCES PhieuNhap(MaPN) ON DELETE CASCADE,
    CONSTRAINT FK_CTPN_NguyenLieu FOREIGN KEY (MaNL) REFERENCES NguyenLieu(MaNL),
    CONSTRAINT CK_CTPN_SoLuong CHECK (SoLuong > 0),
    CONSTRAINT CK_CTPN_DonGia CHECK (DonGia >= 0)
);
GO

SELECT COUNT(*) AS SoBang FROM sys.tables;   -- phai ra 12