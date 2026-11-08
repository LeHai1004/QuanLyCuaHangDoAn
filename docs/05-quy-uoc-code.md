# 05. Quy ước code

## Cấu trúc thư mục
```
QuanLyCuaHangDoAn/
  QuanLyCuaHangDoAn.slnx
  AGENTS.md, README.md
  .github/copilot-instructions.md
  docs/                      00 đến 09
  QuanLyCuaHangDoAn/
    Database/                QLCHang.sql, Dulieumau.sql
    Class/                   FileXml.cs, GiaoDien.cs, KhoKhachHang.cs (KhachHang, Kho), ...
    GUI/                     mỗi form do 1 người sở hữu
    Data/                    *.xsd viết tay (Copy if newer); *.xml sinh lúc chạy
    Xslt/                    HoaDon.xslt, ThucDon.xslt (Copy if newer)
```

## Đặt tên
- Tên không dấu. Class PascalCase, biến camelCase, form bắt đầu bằng `frm`.
- Control: `txt`, `btn`, `dgv`, `cbo`, `lbl`.
- Tiền tố mã: DM, MA, BA, NV, KH, HD, NL, NCC, PN + 3 chữ số.
- Mỗi form do một người sở hữu. File dùng chung (FileXml, GiaoDien, Program, frmMain) chỉ Hải sửa; ai cần thêm hàm thì báo Hải.

## Giá trị trạng thái (gõ đúng dấu)
| Cột | Giá trị |
|---|---|
| Ban.TrangThai | Trống \| Có khách |
| HoaDon.TrangThai | Đang phục vụ \| Đã thanh toán |
| ChiTietHoaDon.TrangThaiBep | Chờ \| Đang làm \| Xong |
| MonAn.TrangThai | Còn bán \| Hết |
| NhanVien.ChucVu | Admin \| ThuNgan \| Bep |
| KhachHang.Hang | Thường \| Bạc \| Vàng |

## Hợp đồng hàm
`FileXml` là lớp THƯỜNG, phải `var fx = new FileXml();`.

| Hàm | Người viết | Làm gì |
|---|---|---|
| fx.HienThi(tenFile) | Hải | Đọc XML ra DataTable |
| fx.TaoXml(sql, tenBang, tenFile) | Hải | Lấy dữ liệu SQL ghi ra XML (và XSD nếu chưa có). sql là chuỗi cố định trong code |
| fx.TonTai(tenFile, cot, giaTri) | Hải | Kiểm tra trùng mã |
| fx.TaoMa(tienTo, tenFile, cot) | Hải | Sinh mã mới, ví dụ MA009 |
| fx.Them(tenFile, tenBang, giaTri) | Hải | Thêm dòng (IDictionary cột-giá trị, đúng thứ tự cột) |
| fx.Sua(tenFile, khoa, giaTriMoi) | Hải | Sửa dòng theo khóa; trả số dòng sửa |
| fx.Xoa(tenFile, khoa) | Hải | Xóa dòng theo khóa; trả số dòng xóa |
| fx.CapNhatDb(tenBang, tenFile) | Hải | Đồng bộ XML sang SQL Server (thêm, sửa, xóa). Gọi bảng cha trước, bảng con sau |
| fx.KiemTraXsd(tenXml, tenXsd, out loi) | Hải (khung), Duy (T13) | Kiểm tra XML theo XSD, chặn DTD |
| fx.XuatHtml(tenXml, tenXslt, thamSo, tenHtml) | Hải | XML sang HTML bằng XSLT |
| FileXml.MoTrinhDuyet(tenFile) | Hải | Mở file bằng trình duyệt (static) |
| KhachHang.PhanTramGiam(maKH) | Duy | Trả 0, 5 hoặc 10 (phần trăm) |
| KhachHang.CongDiem(maKH, tongTien) | Duy | Cộng điểm, tự nâng hạng |
| Kho.TruKhoTheoHoaDon(maHD) | Duy | Trừ kho theo công thức |
| Kho.DuNguyenLieu(maMon, soLuong) | Duy | Kho còn đủ làm món này không |

Muốn đổi hợp đồng: báo cả nhóm, được đồng ý rồi mới sửa và ghi vào `08-quyet-dinh.md`.

## Bảo mật
- SQL dùng `SqlParameter`, không nối chuỗi từ ô nhập.
- Nhập XML từ ngoài: tắt DTD (đã có trong KiemTraXsd).
- Mật khẩu: SHA-256 của chuỗi `TenDN:matKhauGoc`, UTF-8, 64 ký tự hex thường. Tài khoản mẫu admin / thungan1 / bep1, mật khẩu gốc 123456.
- Kiểm tra quyền trong code, không chỉ ẩn menu.
- Chuỗi kết nối dùng Windows Authentication (`Data Source=.`), không commit mật khẩu `sa`.

## Git
Nhánh `feature/<ten>-<viec>`, gộp vào `develop` bằng Pull Request, có người review. Commit: `feat|fix|docs|refactor|chore(pham-vi): noi dung`. Không commit bin, obj, .vs, *.mdf.