using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using QuanLyCuaHangDoAn;
using QuanLyCuaHangDoAn.Class;

namespace QuanLyCuaHangDoAn.GUI
{
    public class frmOrder : Form
    {
        private const string TrangThaiBanTrong = "Trống";
        private const string TrangThaiBanCoKhach = "Có khách";
        private const string TrangThaiHoaDonDangPhucVu = "Đang phục vụ";
        private const string TrangThaiMonConBan = "Còn bán";
        private const string TrangThaiBepCho = "Chờ";

        private readonly FileXml fx = new FileXml();
        private readonly FlowLayoutPanel flpBan = new FlowLayoutPanel();
        private readonly FlowLayoutPanel flpMon = new FlowLayoutPanel();
        private readonly FlowLayoutPanel flpChiTiet = new FlowLayoutPanel();
        private readonly ComboBox cboDanhMuc = new ComboBox();
        private readonly TextBox txtTim = new TextBox();
        private readonly NumericUpDown nudSoLuong = new NumericUpDown();
        private readonly Label lblBanDangChon = new Label();
        private readonly Label lblMonDangChon = new Label();
        private readonly Label lblTamTinh = new Label();
        private readonly Button btnTaoHoaDon = GiaoDien.TaoNut("Tạo hóa đơn", GiaoDien.MauChinh);
        private readonly Button btnThemMon = GiaoDien.TaoNut("Thêm món", GiaoDien.MauXanh);
        private readonly Button btnGuiBep = GiaoDien.TaoNut("Gửi bếp", GiaoDien.MauChinh);
        private readonly Button btnThanhToan = GiaoDien.TaoNut("Thanh toán", GiaoDien.MauChinhDam);
        private readonly ToolTip toolTip = new ToolTip();
        private readonly List<string> bangChoDongBo = new List<string>();

        private DataTable danhMuc;
        private DataTable monAn;
        private DataTable ban;
        private DataTable hoaDon;
        private DataTable chiTietHoaDon;
        private DataTable chiTietDangChon;
        private DataRow monDangChon;
        private string maBanDangChon = "";
        private string trangThaiBanDangChon = "";
        private string maHoaDonDangChon = "";
        private bool coThayDoiChuaGui;
        private bool choDongBo;
        private string hanhDongChoDongBo = "";

        public frmOrder()
        {
            KhoiTaoGiaoDien();
            Load += frmOrder_Load;
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Gọi món";
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = GiaoDien.FontThuong;
            ClientSize = new Size(1380, 760);
            MinimumSize = new Size(1080, 620);

            var noiDung = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Padding = new Padding(8)
            };
            noiDung.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            noiDung.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 39));
            noiDung.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36));

            Panel cotBan = TaoCot("BÀN");
            Panel cotMon = TaoCot("THỰC ĐƠN");
            Panel cotHoaDon = TaoCot("HÓA ĐƠN");

            flpBan.Dock = DockStyle.Fill;
            flpBan.AutoScroll = true;
            flpBan.WrapContents = true;
            flpBan.Padding = new Padding(6);

            btnTaoHoaDon.Dock = DockStyle.Bottom;
            btnTaoHoaDon.Height = 44;
            btnTaoHoaDon.Margin = new Padding(4);
            cotBan.Controls.Add(flpBan);
            cotBan.Controls.Add(btnTaoHoaDon);

            var timKiem = new Panel { Dock = DockStyle.Top, Height = 82, Padding = new Padding(6) };
            cboDanhMuc.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDanhMuc.Dock = DockStyle.Top;
            cboDanhMuc.Height = 30;
            var lblTim = new Label
            {
                Text = "Tìm món",
                Dock = DockStyle.Top,
                Height = 20,
                TextAlign = ContentAlignment.MiddleLeft
            };
            txtTim.Dock = DockStyle.Bottom;
            txtTim.Height = 30;
            timKiem.Controls.Add(txtTim);
            timKiem.Controls.Add(lblTim);
            timKiem.Controls.Add(cboDanhMuc);

            flpMon.Dock = DockStyle.Fill;
            flpMon.AutoScroll = true;
            flpMon.WrapContents = true;
            flpMon.Padding = new Padding(6);

            var chonMon = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 54,
                Padding = new Padding(4),
                WrapContents = false
            };
            lblMonDangChon.AutoSize = false;
            lblMonDangChon.Width = 205;
            lblMonDangChon.Height = 38;
            lblMonDangChon.TextAlign = ContentAlignment.MiddleLeft;
            nudSoLuong.Minimum = 1;
            nudSoLuong.Maximum = int.MaxValue;
            nudSoLuong.DecimalPlaces = 0;
            nudSoLuong.Width = 80;
            nudSoLuong.Height = 38;
            btnThemMon.Width = 120;
            chonMon.Controls.Add(lblMonDangChon);
            chonMon.Controls.Add(nudSoLuong);
            chonMon.Controls.Add(btnThemMon);
            cotMon.Controls.Add(flpMon);
            cotMon.Controls.Add(chonMon);
            cotMon.Controls.Add(timKiem);

            lblBanDangChon.Dock = DockStyle.Top;
            lblBanDangChon.Height = 36;
            lblBanDangChon.Font = GiaoDien.FontDam;
            lblBanDangChon.TextAlign = ContentAlignment.MiddleLeft;
            lblBanDangChon.Padding = new Padding(8, 0, 0, 0);
            flpChiTiet.Dock = DockStyle.Fill;
            flpChiTiet.AutoScroll = true;
            flpChiTiet.FlowDirection = FlowDirection.TopDown;
            flpChiTiet.WrapContents = false;
            flpChiTiet.Padding = new Padding(6);

            var nutHoaDon = new Panel { Dock = DockStyle.Bottom, Height = 118, Padding = new Padding(6) };
            lblTamTinh.Dock = DockStyle.Top;
            lblTamTinh.Height = 32;
            lblTamTinh.Font = GiaoDien.FontDam;
            lblTamTinh.TextAlign = ContentAlignment.MiddleRight;
            var nutThanhToan = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 48,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false
            };
            btnGuiBep.Width = 120;
            btnThanhToan.Width = 130;
            btnThanhToan.Enabled = false;
            toolTip.SetToolTip(btnThanhToan, "Chức năng thanh toán sẽ được nối với frmThanhToan ở T7.");
            nutThanhToan.Controls.Add(btnThanhToan);
            nutThanhToan.Controls.Add(btnGuiBep);
            nutHoaDon.Controls.Add(nutThanhToan);
            nutHoaDon.Controls.Add(lblTamTinh);
            cotHoaDon.Controls.Add(flpChiTiet);
            cotHoaDon.Controls.Add(nutHoaDon);
            cotHoaDon.Controls.Add(lblBanDangChon);

            noiDung.Controls.Add(cotBan, 0, 0);
            noiDung.Controls.Add(cotMon, 1, 0);
            noiDung.Controls.Add(cotHoaDon, 2, 0);
            Controls.Add(noiDung);
            Controls.Add(GiaoDien.TaoTieuDe("GỌI MÓN"));

            btnTaoHoaDon.Click += btnTaoHoaDon_Click;
            btnThemMon.Click += btnThemMon_Click;
            btnGuiBep.Click += btnGuiBep_Click;
            cboDanhMuc.SelectedIndexChanged += cboDanhMuc_SelectedIndexChanged;
            txtTim.TextChanged += txtTim_TextChanged;
            flpChiTiet.Resize += flpChiTiet_Resize;
            CapNhatTrangThaiNut();
        }

        private static Panel TaoCot(string tieuDe)
        {
            var cot = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4) };
            var lblTieuDe = new Label
            {
                Text = tieuDe,
                Dock = DockStyle.Top,
                Height = 38,
                Font = GiaoDien.FontDam,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0)
            };
            cot.Controls.Add(lblTieuDe);
            return cot;
        }

        private void frmOrder_Load(object sender, EventArgs e)
        {
            if (!KiemTraQuyenBanHang())
            {
                Close();
                return;
            }

            try
            {
                TaiDuLieu();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể tải bàn, thực đơn hoặc hóa đơn", ex);
            }
        }

        private void TaiDuLieu()
        {
            fx.TaoXml("SELECT * FROM DanhMuc", "DanhMuc", "DanhMuc.xml");
            fx.TaoXml("SELECT * FROM MonAn", "MonAn", "MonAn.xml");
            fx.TaoXml("SELECT * FROM Ban", "Ban", "Ban.xml");
            fx.TaoXml("SELECT * FROM HoaDon", "HoaDon", "HoaDon.xml");
            fx.TaoXml("SELECT * FROM ChiTietHoaDon", "ChiTietHoaDon", "ChiTietHoaDon.xml");

            danhMuc = fx.HienThi("DanhMuc.xml");
            monAn = fx.HienThi("MonAn.xml");
            ban = fx.HienThi("Ban.xml");
            hoaDon = fx.HienThi("HoaDon.xml");
            chiTietHoaDon = fx.HienThi("ChiTietHoaDon.xml");

            HienThiDanhMuc();
            HienThiBan();
            HienThiMonAn();
        }

        private void HienThiDanhMuc()
        {
            var luaChon = new List<DanhMucLuaChon>
            {
                new DanhMucLuaChon("", "Tất cả danh mục")
            };
            foreach (DataRow dong in danhMuc.Rows)
                luaChon.Add(new DanhMucLuaChon(Convert.ToString(dong["MaDM"]), Convert.ToString(dong["TenDM"])));

            cboDanhMuc.DataSource = luaChon;
            cboDanhMuc.DisplayMember = "TenDM";
            cboDanhMuc.ValueMember = "MaDM";
            cboDanhMuc.SelectedIndex = 0;
        }

        private void HienThiBan()
        {
            flpBan.SuspendLayout();
            try
            {
                XoaVaGiaiPhong(flpBan.Controls);
                foreach (DataRow dong in ban.Rows)
                {
                    string trangThai = Convert.ToString(dong["TrangThai"]);
                    Color mau;
                    if (trangThai == TrangThaiBanTrong)
                        mau = GiaoDien.MauXanh;
                    else if (trangThai == TrangThaiBanCoKhach)
                        mau = GiaoDien.MauDo;
                    else
                        throw new InvalidOperationException("Bàn " + Convert.ToString(dong["MaBan"]) + " có trạng thái không hợp lệ: " + trangThai);

                    var btnBan = GiaoDien.TaoNut(
                        Convert.ToString(dong["TenBan"]) + Environment.NewLine
                        + Convert.ToString(dong["SoGhe"]) + " ghế",
                        mau);
                    btnBan.Width = 132;
                    btnBan.Height = 72;
                    btnBan.Margin = new Padding(5);
                    btnBan.Tag = new BanLuaChon(
                        Convert.ToString(dong["MaBan"]),
                        Convert.ToString(dong["TenBan"]),
                        Convert.ToInt32(dong["SoGhe"], CultureInfo.InvariantCulture),
                        trangThai);
                    btnBan.Click += btnBan_Click;
                    flpBan.Controls.Add(btnBan);
                }
            }
            finally
            {
                flpBan.ResumeLayout();
            }
        }

        private void HienThiMonAn()
        {
            flpMon.SuspendLayout();
            try
            {
                XoaVaGiaiPhong(flpMon.Controls);
                monDangChon = null;
                lblMonDangChon.Text = "Chọn món";

                var luaChon = cboDanhMuc.SelectedItem as DanhMucLuaChon;
                string maDM = luaChon == null ? "" : luaChon.MaDM;
                string tuKhoa = txtTim.Text.Trim();
                foreach (DataRow dong in monAn.Rows)
                {
                    if (Convert.ToString(dong["TrangThai"]) != TrangThaiMonConBan)
                        continue;
                    if (maDM.Length > 0 && !string.Equals(Convert.ToString(dong["MaDM"]), maDM, StringComparison.OrdinalIgnoreCase))
                        continue;
                    if (tuKhoa.Length > 0
                        && Convert.ToString(dong["TenMon"]).IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) < 0)
                        continue;

                    var btnMon = GiaoDien.TaoNut(
                        Convert.ToString(dong["TenMon"]) + Environment.NewLine
                        + DinhDangTien(Convert.ToDecimal(dong["DonGia"])),
                        GiaoDien.MauChinh);
                    btnMon.Width = 158;
                    btnMon.Height = 76;
                    btnMon.Margin = new Padding(5);
                    btnMon.Tag = dong;
                    btnMon.Click += btnMon_Click;
                    flpMon.Controls.Add(btnMon);
                }
            }
            finally
            {
                flpMon.ResumeLayout();
            }

            CapNhatTrangThaiNut();
        }

        private void btnBan_Click(object sender, EventArgs e)
        {
            if (choDongBo)
                return;

            var btn = sender as Button;
            var luaChon = btn == null ? null : btn.Tag as BanLuaChon;
            if (luaChon == null)
                return;

            if (coThayDoiChuaGui && !string.Equals(luaChon.MaBan, maBanDangChon, StringComparison.Ordinal))
            {
                MessageBox.Show("Vui lòng gửi bếp các món đang chọn trước khi đổi bàn.", "Chưa gửi bếp",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                ChonBan(luaChon);
            }
            catch (Exception ex)
            {
                HienLoi("Không thể mở hóa đơn của bàn", ex);
            }
        }

        private void ChonBan(BanLuaChon luaChon)
        {
            List<DataRow> hoaDonDangMo = hoaDon.AsEnumerable()
                .Where(dong => string.Equals(Convert.ToString(dong["MaBan"]), luaChon.MaBan, StringComparison.OrdinalIgnoreCase)
                    && Convert.ToString(dong["TrangThai"]) == TrangThaiHoaDonDangPhucVu)
                .ToList();
            if (hoaDonDangMo.Count > 1)
                throw new InvalidOperationException("Bàn có nhiều hóa đơn đang phục vụ. Vui lòng kiểm tra dữ liệu.");
            if (hoaDonDangMo.Count == 1 && luaChon.TrangThai != TrangThaiBanCoKhach)
                throw new InvalidOperationException("Bàn có hóa đơn đang phục vụ nhưng trạng thái bàn không phải Có khách.");

            maBanDangChon = luaChon.MaBan;
            trangThaiBanDangChon = luaChon.TrangThai;
            maHoaDonDangChon = "";
            chiTietDangChon = chiTietHoaDon.Clone();
            coThayDoiChuaGui = false;
            monDangChon = null;
            if (hoaDonDangMo.Count == 1)
            {
                DataRow dongHoaDon = hoaDonDangMo[0];
                maHoaDonDangChon = Convert.ToString(dongHoaDon["MaHD"]);
                foreach (DataRow dong in chiTietHoaDon.Rows)
                    if (string.Equals(Convert.ToString(dong["MaHD"]), maHoaDonDangChon, StringComparison.OrdinalIgnoreCase))
                        chiTietDangChon.ImportRow(dong);
            }
            else if (trangThaiBanDangChon == TrangThaiBanCoKhach)
            {
                MessageBox.Show("Bàn đang có khách nhưng không tìm thấy hóa đơn đang phục vụ.", "Dữ liệu không khớp",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            lblBanDangChon.Text = maHoaDonDangChon.Length == 0
                ? luaChon.TenBan + " — chưa có hóa đơn"
                : luaChon.TenBan + " — " + maHoaDonDangChon;
            HienThiMonAn();
            HienThiChiTiet();
            CapNhatTrangThaiNut();
        }

        private void btnMon_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            monDangChon = btn == null ? null : btn.Tag as DataRow;
            if (monDangChon != null)
                lblMonDangChon.Text = Convert.ToString(monDangChon["TenMon"]);
            CapNhatTrangThaiNut();
        }

        private void btnTaoHoaDon_Click(object sender, EventArgs e)
        {
            if (choDongBo)
            {
                if (hanhDongChoDongBo == "tạo hóa đơn")
                    ThuDongBo();
                return;
            }
            if (!KiemTraQuyenBanHang())
                return;
            if (string.IsNullOrEmpty(maBanDangChon))
            {
                MessageBox.Show("Vui lòng chọn bàn trống trước.", "Chưa chọn bàn",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                fx.TaoXml("SELECT * FROM Ban", "Ban", "Ban.xml");
                fx.TaoXml("SELECT * FROM HoaDon", "HoaDon", "HoaDon.xml");
                DataTable banMoi = fx.HienThi("Ban.xml");
                DataTable hoaDonMoi = fx.HienThi("HoaDon.xml");
                DataRow dongBan = TimDong(banMoi, "MaBan", maBanDangChon);
                if (dongBan == null || Convert.ToString(dongBan["TrangThai"]) != TrangThaiBanTrong)
                    throw new InvalidOperationException("Bàn đã được sử dụng hoặc không còn tồn tại.");
                if (hoaDonMoi.AsEnumerable().Any(dong =>
                    string.Equals(Convert.ToString(dong["MaBan"]), maBanDangChon, StringComparison.OrdinalIgnoreCase)
                    && Convert.ToString(dong["TrangThai"]) == TrangThaiHoaDonDangPhucVu))
                    throw new InvalidOperationException("Bàn đã có hóa đơn đang phục vụ.");
                if (!PhienDangNhap.IsLoggedIn || string.IsNullOrEmpty(PhienDangNhap.MaNV))
                    throw new InvalidOperationException("Phiên đăng nhập không có mã nhân viên.");

                string maHoaDon = fx.TaoMa("HD", "HoaDon.xml", "MaHD");
                if (fx.TonTai("HoaDon.xml", "MaHD", maHoaDon))
                    throw new InvalidOperationException("Mã hóa đơn đã tồn tại. Vui lòng làm mới dữ liệu rồi thử lại.");

                var giaTri = new Dictionary<string, string>
                {
                    { "MaHD", maHoaDon },
                    { "MaBan", maBanDangChon },
                    { "MaNV", PhienDangNhap.MaNV },
                    { "NgayLap", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture) },
                    { "TrangThai", TrangThaiHoaDonDangPhucVu },
                    { "PhanTramGiam", "0" },
                    { "TongTien", "0" }
                };
                string[] thuTuCot = { "MaHD", "MaBan", "MaNV", "MaKH", "NgayLap", "TrangThai", "PhanTramGiam", "TongTien" };
                var giaTriTheoThuTu = TaoGiaTriTheoThuTu(thuTuCot, giaTri);
                fx.Them("HoaDon.xml", "HoaDon", giaTriTheoThuTu);

                if (fx.Sua("Ban.xml", new Dictionary<string, string> { { "MaBan", maBanDangChon } },
                    new Dictionary<string, string> { { "TrangThai", TrangThaiBanCoKhach } }) == 0)
                    throw new InvalidOperationException("Không tìm thấy bàn cần cập nhật.");

                maHoaDonDangChon = maHoaDon;
                trangThaiBanDangChon = TrangThaiBanCoKhach;
                bangChoDongBo.Clear();
                bangChoDongBo.Add("Ban");
                bangChoDongBo.Add("HoaDon");
                choDongBo = true;
                hanhDongChoDongBo = "tạo hóa đơn";
                CapNhatTrangThaiNut();
                ThuDongBo();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể tạo hóa đơn", ex);
            }
        }

        private void btnThemMon_Click(object sender, EventArgs e)
        {
            if (!KiemTraQuyenBanHang() || choDongBo)
                return;
            if (string.IsNullOrEmpty(maHoaDonDangChon) || monDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn bàn có hóa đơn và món cần thêm.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string maMon = Convert.ToString(monDangChon["MaMon"]);
            int soLuong = Convert.ToInt32(nudSoLuong.Value);
            try
            {
                if (!Kho.DuNguyenLieu(maMon, soLuong))
                {
                    MessageBox.Show("Hết", "Không đủ nguyên liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataRow dongChiTiet = chiTietDangChon.AsEnumerable()
                    .FirstOrDefault(dong => string.Equals(Convert.ToString(dong["MaMon"]), maMon, StringComparison.OrdinalIgnoreCase));
                if (dongChiTiet == null)
                {
                    dongChiTiet = chiTietDangChon.NewRow();
                    dongChiTiet["MaHD"] = maHoaDonDangChon;
                    dongChiTiet["MaMon"] = maMon;
                    dongChiTiet["SoLuong"] = soLuong;
                    dongChiTiet["DonGia"] = Convert.ToDecimal(monDangChon["DonGia"]);
                    dongChiTiet["TrangThaiBep"] = TrangThaiBepCho;
                    chiTietDangChon.Rows.Add(dongChiTiet);
                }
                else
                {
                    int soLuongCu = Convert.ToInt32(dongChiTiet["SoLuong"]);
                    if (soLuong > int.MaxValue - soLuongCu)
                        throw new InvalidOperationException("Số lượng món vượt giới hạn cho phép.");
                    dongChiTiet["SoLuong"] = soLuongCu + soLuong;
                    dongChiTiet["DonGia"] = Convert.ToDecimal(monDangChon["DonGia"]);
                    dongChiTiet["TrangThaiBep"] = TrangThaiBepCho;
                }

                coThayDoiChuaGui = true;
                HienThiChiTiet();
                CapNhatTrangThaiNut();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể thêm món", ex);
            }
        }

        private void btnGuiBep_Click(object sender, EventArgs e)
        {
            if (choDongBo)
            {
                if (hanhDongChoDongBo == "gửi bếp")
                    ThuDongBo();
                return;
            }
            if (!KiemTraQuyenBanHang())
                return;
            if (string.IsNullOrEmpty(maHoaDonDangChon) || !coThayDoiChuaGui)
            {
                MessageBox.Show("Chưa có món mới để gửi bếp.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (chiTietDangChon.Rows.Count == 0)
            {
                MessageBox.Show("Hóa đơn chưa có món.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                fx.TaoXml("SELECT * FROM Ban", "Ban", "Ban.xml");
                fx.TaoXml("SELECT * FROM HoaDon", "HoaDon", "HoaDon.xml");
                fx.TaoXml("SELECT * FROM ChiTietHoaDon", "ChiTietHoaDon", "ChiTietHoaDon.xml");

                DataTable banMoi = fx.HienThi("Ban.xml");
                DataTable hoaDonMoi = fx.HienThi("HoaDon.xml");
                DataTable chiTietMoi = fx.HienThi("ChiTietHoaDon.xml");
                DataRow dongBan = TimDong(banMoi, "MaBan", maBanDangChon);
                DataRow dongHoaDon = TimDong(hoaDonMoi, "MaHD", maHoaDonDangChon);
                if (dongBan == null || Convert.ToString(dongBan["TrangThai"]) != TrangThaiBanCoKhach)
                    throw new InvalidOperationException("Bàn không còn ở trạng thái Có khách.");
                if (dongHoaDon == null || Convert.ToString(dongHoaDon["TrangThai"]) != TrangThaiHoaDonDangPhucVu
                    || !string.Equals(Convert.ToString(dongHoaDon["MaBan"]), maBanDangChon, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Không tìm thấy hóa đơn đang phục vụ của bàn đã chọn.");

                foreach (DataRow dong in chiTietDangChon.Rows)
                {
                    dong["TrangThaiBep"] = TrangThaiBepCho;
                    string maMon = Convert.ToString(dong["MaMon"]);
                    DataRow dongDaLuu = TimDongChiTiet(chiTietMoi, maHoaDonDangChon, maMon);
                    var giaTri = new Dictionary<string, string>
                    {
                        { "SoLuong", Convert.ToInt32(dong["SoLuong"]).ToString(CultureInfo.InvariantCulture) },
                        { "DonGia", Convert.ToDecimal(dong["DonGia"]).ToString("0", CultureInfo.InvariantCulture) },
                        { "TrangThaiBep", TrangThaiBepCho }
                    };
                    if (dongDaLuu == null)
                    {
                        giaTri["MaHD"] = maHoaDonDangChon;
                        giaTri["MaMon"] = maMon;
                        string[] thuTuCot = { "MaHD", "MaMon", "SoLuong", "DonGia", "TrangThaiBep" };
                        fx.Them("ChiTietHoaDon.xml", "ChiTietHoaDon", TaoGiaTriTheoThuTu(thuTuCot, giaTri));
                    }
                    else if (fx.Sua("ChiTietHoaDon.xml",
                        new Dictionary<string, string> { { "MaHD", maHoaDonDangChon }, { "MaMon", maMon } },
                        giaTri) == 0)
                    {
                        throw new InvalidOperationException("Không tìm thấy món cần cập nhật trong hóa đơn.");
                    }
                }

                decimal tongTien = TinhTamTinh();
                if (fx.Sua("HoaDon.xml", new Dictionary<string, string> { { "MaHD", maHoaDonDangChon } },
                    new Dictionary<string, string>
                    {
                        { "TongTien", tongTien.ToString("0", CultureInfo.InvariantCulture) }
                    }) == 0)
                    throw new InvalidOperationException("Không tìm thấy hóa đơn cần cập nhật.");

                bangChoDongBo.Clear();
                bangChoDongBo.Add("Ban");
                bangChoDongBo.Add("HoaDon");
                bangChoDongBo.Add("ChiTietHoaDon");
                choDongBo = true;
                hanhDongChoDongBo = "gửi bếp";
                CapNhatTrangThaiNut();
                ThuDongBo();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể gửi món xuống bếp", ex);
            }
        }

        private void ThuDongBo()
        {
            if (!KiemTraQuyenBanHang() || !choDongBo)
                return;
            try
            {
                while (bangChoDongBo.Count > 0)
                {
                    string tenBang = bangChoDongBo[0];
                    fx.CapNhatDb(tenBang, tenBang + ".xml");
                    bangChoDongBo.RemoveAt(0);
                }
            }
            catch (Exception ex)
            {
                CapNhatTrangThaiNut();
                MessageBox.Show(
                    "XML đã thay đổi nhưng chưa đồng bộ được dữ liệu. Nhấn nút \"" + hanhDongChoDongBo
                    + "\" để thử đồng bộ lại.\r\n\r\n" + ex.Message,
                    "Lỗi đồng bộ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            choDongBo = false;
            hanhDongChoDongBo = "";
            coThayDoiChuaGui = false;
            string maBanCanMo = maBanDangChon;
            try
            {
                TaiDuLieu();
                if (!string.IsNullOrEmpty(maBanCanMo))
                {
                    BanLuaChon luaChon = TaoBanLuaChon(maBanCanMo);
                    if (luaChon != null)
                        ChonBan(luaChon);
                }
            }
            catch (Exception ex)
            {
                HienLoi("Dữ liệu đã đồng bộ nhưng không thể tải lại", ex);
            }
            CapNhatTrangThaiNut();
        }

        private void cboDanhMuc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (monAn != null)
                HienThiMonAn();
        }

        private void txtTim_TextChanged(object sender, EventArgs e)
        {
            if (monAn != null)
                HienThiMonAn();
        }

        private void flpChiTiet_Resize(object sender, EventArgs e)
        {
            if (chiTietDangChon != null)
                HienThiChiTiet();
        }

        private void HienThiChiTiet()
        {
            flpChiTiet.SuspendLayout();
            try
            {
                XoaVaGiaiPhong(flpChiTiet.Controls);
                if (chiTietDangChon != null)
                {
                    int chieuRong = Math.Max(240, flpChiTiet.ClientSize.Width - 30);
                    foreach (DataRow dong in chiTietDangChon.Rows)
                    {
                        string maMon = Convert.ToString(dong["MaMon"]);
                        DataRow thongTinMon = TimDong(monAn, "MaMon", maMon);
                        string tenMon = thongTinMon == null ? maMon : Convert.ToString(thongTinMon["TenMon"]);
                        int soLuong = Convert.ToInt32(dong["SoLuong"]);
                        decimal donGia = Convert.ToDecimal(dong["DonGia"]);
                        var dongHienThi = new Panel
                        {
                            Width = chieuRong,
                            Height = 58,
                            Margin = new Padding(3),
                            BackColor = GiaoDien.MauNenNhat
                        };
                        var lblTen = new Label
                        {
                            Text = tenMon,
                            Dock = DockStyle.Top,
                            Height = 26,
                            Font = GiaoDien.FontDam
                        };
                        var lblGia = new Label
                        {
                            Text = soLuong.ToString(CultureInfo.InvariantCulture) + " × " + DinhDangTien(donGia)
                                + " = " + DinhDangTien(donGia * soLuong)
                                + "   (" + Convert.ToString(dong["TrangThaiBep"]) + ")",
                            Dock = DockStyle.Fill,
                            TextAlign = ContentAlignment.MiddleLeft
                        };
                        dongHienThi.Controls.Add(lblGia);
                        dongHienThi.Controls.Add(lblTen);
                        flpChiTiet.Controls.Add(dongHienThi);
                    }
                }
            }
            finally
            {
                flpChiTiet.ResumeLayout();
            }

            lblTamTinh.Text = "Tạm tính: " + DinhDangTien(TinhTamTinh());
            CapNhatTrangThaiNut();
        }

        private decimal TinhTamTinh()
        {
            if (chiTietDangChon == null)
                return 0m;
            return chiTietDangChon.AsEnumerable().Sum(dong =>
                Convert.ToDecimal(dong["SoLuong"]) * Convert.ToDecimal(dong["DonGia"]));
        }

        private void CapNhatTrangThaiNut()
        {
            bool coQuyen = CoQuyenBanHang();
            bool dangTaoHoaDon = choDongBo && hanhDongChoDongBo == "tạo hóa đơn";
            bool dangGuiBep = choDongBo && hanhDongChoDongBo == "gửi bếp";
            bool coHoaDon = !string.IsNullOrEmpty(maHoaDonDangChon);
            bool coMon = chiTietDangChon != null && chiTietDangChon.Rows.Count > 0;

            btnTaoHoaDon.Text = dangTaoHoaDon ? "Đồng bộ" : "Tạo hóa đơn";
            btnGuiBep.Text = dangGuiBep ? "Đồng bộ" : "Gửi bếp";
            btnTaoHoaDon.Enabled = coQuyen && (dangTaoHoaDon
                || (!choDongBo && !coThayDoiChuaGui && !coHoaDon
                    && !string.IsNullOrEmpty(maBanDangChon) && trangThaiBanDangChon == TrangThaiBanTrong));
            btnGuiBep.Enabled = coQuyen && (dangGuiBep
                || (!choDongBo && coHoaDon && coThayDoiChuaGui && coMon));
            btnThemMon.Enabled = coQuyen && !choDongBo && coHoaDon && monDangChon != null;
            btnThanhToan.Enabled = false;
            flpBan.Enabled = !choDongBo && !coThayDoiChuaGui;
            flpMon.Enabled = !choDongBo;
            cboDanhMuc.Enabled = !choDongBo;
            txtTim.Enabled = !choDongBo;
            nudSoLuong.Enabled = !choDongBo;
        }

        private bool KiemTraQuyenBanHang()
        {
            if (CoQuyenBanHang())
                return true;
            MessageBox.Show("Chỉ Admin hoặc Thu ngân mới được gọi món.", "Không đủ quyền",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        private static bool CoQuyenBanHang()
        {
            return PhienDangNhap.IsLoggedIn
                && (string.Equals(PhienDangNhap.ChucVu, "Admin", StringComparison.Ordinal)
                    || string.Equals(PhienDangNhap.ChucVu, "ThuNgan", StringComparison.Ordinal));
        }

        private BanLuaChon TaoBanLuaChon(string maBan)
        {
            DataRow dong = TimDong(ban, "MaBan", maBan);
            if (dong == null)
                return null;
            return new BanLuaChon(
                Convert.ToString(dong["MaBan"]),
                Convert.ToString(dong["TenBan"]),
                Convert.ToInt32(dong["SoGhe"], CultureInfo.InvariantCulture),
                Convert.ToString(dong["TrangThai"]));
        }

        private static DataRow TimDong(DataTable bang, string cotKhoa, string giaTri)
        {
            return bang.AsEnumerable().FirstOrDefault(dong =>
                string.Equals(Convert.ToString(dong[cotKhoa]), giaTri, StringComparison.OrdinalIgnoreCase));
        }

        private static DataRow TimDongChiTiet(DataTable bang, string maHoaDon, string maMon)
        {
            return bang.AsEnumerable().FirstOrDefault(dong =>
                string.Equals(Convert.ToString(dong["MaHD"]), maHoaDon, StringComparison.OrdinalIgnoreCase)
                && string.Equals(Convert.ToString(dong["MaMon"]), maMon, StringComparison.OrdinalIgnoreCase));
        }

        private static void XoaVaGiaiPhong(Control.ControlCollection controls)
        {
            foreach (Control control in controls.Cast<Control>().ToList())
            {
                controls.Remove(control);
                control.Dispose();
            }
        }

        private static SortedDictionary<string, string> TaoGiaTriTheoThuTu(
            string[] thuTuCot,
            IDictionary<string, string> giaTri)
        {
            var ketQua = new SortedDictionary<string, string>(Comparer<string>.Create((a, b) =>
                Array.IndexOf(thuTuCot, a).CompareTo(Array.IndexOf(thuTuCot, b))));
            foreach (string tenCot in thuTuCot)
                if (giaTri.ContainsKey(tenCot))
                    ketQua.Add(tenCot, giaTri[tenCot]);
            return ketQua;
        }

        private static string DinhDangTien(decimal soTien)
        {
            return soTien.ToString("#,##0", CultureInfo.GetCultureInfo("vi-VN")) + " đ";
        }

        private static void HienLoi(string noiDung, Exception ex)
        {
            MessageBox.Show(noiDung + ".\r\n\r\n" + ex.Message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private sealed class DanhMucLuaChon
        {
            public string MaDM { get; private set; }
            public string TenDM { get; private set; }

            public DanhMucLuaChon(string maDM, string tenDM)
            {
                MaDM = maDM;
                TenDM = tenDM;
            }
        }

        private sealed class BanLuaChon
        {
            public string MaBan { get; private set; }
            public string TenBan { get; private set; }
            public int SoGhe { get; private set; }
            public string TrangThai { get; private set; }

            public BanLuaChon(string maBan, string tenBan, int soGhe, string trangThai)
            {
                MaBan = maBan;
                TenBan = tenBan;
                SoGhe = soGhe;
                TrangThai = trangThai;
            }
        }
    }
}
