using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        private readonly DataGridView dgvChiTiet = new DataGridView();
        private DataGridView flpChiTiet { get { return dgvChiTiet; } }
        private readonly ComboBox cboDanhMuc = new ComboBox();
        private readonly TextBox txtTim = new TextBox();
        private readonly NumericUpDown nudSoLuong = new NumericUpDown();
        private readonly Label lblBanDangChon = new Label();
        private readonly Label lblThongTinHD = new Label();
        private readonly Label lblMonDangChon = new Label();
        private readonly Label lblTamTinh = new Label();
        private readonly Label lblTieuDeTamTinh = new Label();
        private readonly Button btnTaoHoaDon = GiaoDien.TaoNut("Tạo hóa đơn", GiaoDien.MauChinh);
        private readonly Button btnThemMon = GiaoDien.TaoNut("Thêm món", GiaoDien.MauXanh);
        private readonly Button btnXoaDong = GiaoDien.TaoNut("Xóa dòng", GiaoDien.MauChinhDam);
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
        private DataRow dongChiTietDangChon;
        private DataRow monDangChon;
        private BanCard banCardDangChon;
        private MonCard monCardDangChon;
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
            StartPosition = FormStartPosition.CenterParent;
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(245, 246, 248);
            Font = new Font("Segoe UI", 10F);
            ClientSize = new Size(1280, 720);
            MinimumSize = new Size(1296, 759);
            DoubleBuffered = true;

            Panel cotBan = TaoCot("pnlBan", "BÀN", 20, 250,
                AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom);
            Panel cotMon = TaoCot("pnlThucDon", "THỰC ĐƠN", 285, 560,
                AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom);
            Panel cotHoaDon = TaoCot("pnlHoaDon", "HÓA ĐƠN", 860, 400,
                AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom);

            Label lblTrong = new Label
            {
                Name = "lblChuGiaiTrong",
                Text = "● Trống",
                Left = 15,
                Top = 40,
                Width = 75,
                Height = 16,
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(22, 163, 74)
            };
            Label lblCoKhach = new Label
            {
                Name = "lblChuGiaiCoKhach",
                Text = "● Có khách",
                Left = 92,
                Top = 40,
                Width = 100,
                Height = 16,
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(220, 38, 38)
            };
            cotBan.Controls.Add(lblTrong);
            cotBan.Controls.Add(lblCoKhach);

            flpBan.Name = "flpBan";
            flpBan.Location = new Point(9, 65);
            flpBan.Size = new Size(224 + SystemInformation.VerticalScrollBarWidth, 455);
            flpBan.AutoScroll = true;
            flpBan.WrapContents = true;
            flpBan.FlowDirection = FlowDirection.LeftToRight;
            flpBan.Padding = Padding.Empty;
            flpBan.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
            cotBan.Controls.Add(flpBan);

            DinhDangNut(btnTaoHoaDon, "btnTaoHoaDon", new Size(220, 44),
                Color.FromArgb(232, 101, 26), Color.White, Color.FromArgb(232, 101, 26), false);
            btnTaoHoaDon.Location = new Point(15, 535);
            btnTaoHoaDon.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            cotBan.Controls.Add(btnTaoHoaDon);
            lblBanDangChon.Name = "lblBanDangChon";
            lblBanDangChon.Text = "Bàn đang chọn: —";
            lblBanDangChon.Location = new Point(15, 585);
            lblBanDangChon.Size = new Size(220, 20);
            lblBanDangChon.Font = new Font("Segoe UI", 9F);
            lblBanDangChon.ForeColor = Color.FromArgb(107, 114, 128);
            lblBanDangChon.AutoEllipsis = true;
            lblBanDangChon.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            cotBan.Controls.Add(lblBanDangChon);

            cboDanhMuc.Name = "cboDanhMuc";
            cboDanhMuc.Location = new Point(15, 45);
            cboDanhMuc.Size = new Size(250, 32);
            cboDanhMuc.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDanhMuc.Font = new Font("Segoe UI", 10F);

            Label lblTimPlaceholder = new Label
            {
                Name = "lblTimPlaceholder",
                Text = "Tìm món...",
                Left = 275,
                Top = 28,
                Width = 255,
                Height = 15,
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(107, 114, 128)
            };
            txtTim.Name = "txtTim";
            txtTim.Location = new Point(275, 45);
            txtTim.Size = new Size(255, 32);
            txtTim.BorderStyle = BorderStyle.FixedSingle;
            txtTim.Font = new Font("Segoe UI", 10F);
            cotMon.Controls.Add(lblTimPlaceholder);
            cotMon.Controls.Add(cboDanhMuc);
            cotMon.Controls.Add(txtTim);

            flpMon.Name = "flpMon";
            flpMon.Location = new Point(15, 90);
            flpMon.Size = new Size(522 + SystemInformation.VerticalScrollBarWidth, 430);
            flpMon.AutoScroll = true;
            flpMon.WrapContents = true;
            flpMon.FlowDirection = FlowDirection.LeftToRight;
            flpMon.Padding = Padding.Empty;
            flpMon.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
            cotMon.Controls.Add(flpMon);

            lblMonDangChon.Name = "lblMonDangChon";
            lblMonDangChon.Text = "Đã chọn: —";
            lblMonDangChon.Location = new Point(0, 0);
            lblMonDangChon.Size = new Size(230, 44);
            lblMonDangChon.Font = new Font("Segoe UI", 9.5F);
            lblMonDangChon.ForeColor = Color.FromArgb(55, 65, 81);
            lblMonDangChon.AutoEllipsis = true;
            lblMonDangChon.TextAlign = ContentAlignment.MiddleLeft;

            Label lblSoLuong = new Label
            {
                Name = "lblSoLuong",
                Text = "SL",
                Location = new Point(242, 0),
                Size = new Size(20, 44),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(107, 114, 128),
                TextAlign = ContentAlignment.MiddleCenter
            };
            nudSoLuong.Name = "nudSoLuong";
            nudSoLuong.Location = new Point(265, 6);
            nudSoLuong.Size = new Size(70, 32);
            nudSoLuong.Minimum = 1;
            nudSoLuong.Maximum = 99;
            nudSoLuong.DecimalPlaces = 0;
            nudSoLuong.Font = new Font("Segoe UI", 10F);

            DinhDangNut(btnThemMon, "btnThemMon", new Size(130, 44),
                Color.FromArgb(22, 163, 74), Color.White, Color.FromArgb(22, 163, 74), false);
            btnThemMon.Location = new Point(400, 0);

            Panel chonMon = new Panel
            {
                Name = "pnlChonMon",
                Location = new Point(15, 535),
                Size = new Size(530, 44),
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            chonMon.Controls.Add(lblMonDangChon);
            chonMon.Controls.Add(lblSoLuong);
            chonMon.Controls.Add(nudSoLuong);
            chonMon.Controls.Add(btnThemMon);
            cotMon.Controls.Add(chonMon);

            lblThongTinHD.Name = "lblThongTinHD";
            lblThongTinHD.Text = "Bàn: — | Mã HD: —";
            lblThongTinHD.Location = new Point(15, 40);
            lblThongTinHD.Size = new Size(370, 20);
            lblThongTinHD.Font = new Font("Segoe UI", 9.5F);
            lblThongTinHD.ForeColor = Color.FromArgb(107, 114, 128);
            lblThongTinHD.AutoEllipsis = true;
            cotHoaDon.Controls.Add(lblThongTinHD);

            KhoiTaoBangChiTiet();
            flpChiTiet.Name = "dgvChiTiet";
            flpChiTiet.Location = new Point(15, 70);
            flpChiTiet.Size = new Size(370, 360);
            flpChiTiet.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cotHoaDon.Controls.Add(flpChiTiet);

            Panel pnlTamTinh = new Panel
            {
                Name = "pnlTamTinh",
                Location = new Point(15, 445),
                Size = new Size(370, 60),
                BackColor = Color.FromArgb(255, 243, 234)
            };
            lblTieuDeTamTinh.Name = "lblTieuDeTamTinh";
            lblTieuDeTamTinh.Text = "Tạm tính";
            lblTieuDeTamTinh.Location = new Point(12, 0);
            lblTieuDeTamTinh.Size = new Size(100, 60);
            lblTieuDeTamTinh.Font = new Font("Segoe UI", 10F);
            lblTieuDeTamTinh.ForeColor = Color.FromArgb(107, 114, 128);
            lblTieuDeTamTinh.TextAlign = ContentAlignment.MiddleLeft;
            lblTamTinh.Name = "lblTamTinh";
            lblTamTinh.Text = "0 đ";
            lblTamTinh.Location = new Point(112, 0);
            lblTamTinh.Size = new Size(246, 60);
            lblTamTinh.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTamTinh.ForeColor = Color.FromArgb(232, 101, 26);
            lblTamTinh.TextAlign = ContentAlignment.MiddleRight;
            pnlTamTinh.Controls.Add(lblTieuDeTamTinh);
            pnlTamTinh.Controls.Add(lblTamTinh);
            cotHoaDon.Controls.Add(pnlTamTinh);

            DinhDangNut(btnXoaDong, "btnXoaDong", new Size(100, 44), Color.White,
                Color.FromArgb(220, 38, 38), Color.FromArgb(220, 38, 38), true);
            DinhDangNut(btnGuiBep, "btnGuiBep", new Size(120, 44),
                Color.FromArgb(37, 99, 235), Color.White, Color.FromArgb(37, 99, 235), false);
            DinhDangNut(btnThanhToan, "btnThanhToan", new Size(134, 44),
                Color.FromArgb(232, 101, 26), Color.White, Color.FromArgb(232, 101, 26), false);
            btnXoaDong.Location = new Point(15, 520);
            btnGuiBep.Location = new Point(123, 520);
            btnThanhToan.Location = new Point(251, 520);
            cotHoaDon.Controls.Add(btnXoaDong);
            cotHoaDon.Controls.Add(btnGuiBep);
            cotHoaDon.Controls.Add(btnThanhToan);

            Panel pnlHeader = new Panel
            {
                Name = "pnlHeader",
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(232, 101, 26)
            };
            Label lblTieuDe = new Label
            {
                Name = "lblTieuDe",
                Text = "GỌI MÓN",
                Location = new Point(30, 10),
                Size = new Size(400, 28),
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White
            };
            Label lblMoTa = new Label
            {
                Name = "lblMoTa",
                Text = "Chọn bàn, chọn món, gửi bếp và thanh toán",
                Location = new Point(32, 42),
                Size = new Size(500, 18),
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.White
            };
            pnlHeader.Controls.Add(lblTieuDe);
            pnlHeader.Controls.Add(lblMoTa);

            Controls.Add(cotBan);
            Controls.Add(cotMon);
            Controls.Add(cotHoaDon);
            Controls.Add(pnlHeader);

            btnTaoHoaDon.Click += btnTaoHoaDon_Click;
            btnThemMon.Click += btnThemMon_Click;
            btnXoaDong.Click += btnXoaDong_Click;
            btnGuiBep.Click += btnGuiBep_Click;
            btnThanhToan.Click += btnThanhToan_Click;
            cboDanhMuc.SelectedIndexChanged += cboDanhMuc_SelectedIndexChanged;
            txtTim.TextChanged += txtTim_TextChanged;
            flpChiTiet.Resize += flpChiTiet_Resize;
            CapNhatTrangThaiNut();
        }

        private static Panel TaoCot(string ten, string tieuDe, int left, int width, AnchorStyles anchor)
        {
            var cot = new Panel
            {
                Name = ten,
                Location = new Point(left, 85),
                Size = new Size(width, 615),
                BackColor = Color.White,
                Anchor = anchor
            };
            cot.Paint += cot_Paint;
            var lblTieuDe = new Label
            {
                Name = "lbl" + ten.Substring(3) + "TieuDe",
                Text = tieuDe,
                Location = new Point(15, 12),
                Size = new Size(width - 30, 20),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(232, 101, 26)
            };
            cot.Controls.Add(lblTieuDe);
            return cot;
        }

        private void KhoiTaoBangChiTiet()
        {
            flpChiTiet.BackgroundColor = Color.White;
            flpChiTiet.BorderStyle = BorderStyle.None;
            flpChiTiet.RowHeadersVisible = false;
            flpChiTiet.AllowUserToAddRows = false;
            flpChiTiet.AllowUserToDeleteRows = false;
            flpChiTiet.AllowUserToResizeRows = false;
            flpChiTiet.ReadOnly = true;
            flpChiTiet.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            flpChiTiet.MultiSelect = false;
            flpChiTiet.AutoGenerateColumns = false;
            flpChiTiet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            flpChiTiet.ColumnHeadersHeight = 34;
            flpChiTiet.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            flpChiTiet.EnableHeadersVisualStyles = false;
            flpChiTiet.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 243, 234);
            flpChiTiet.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(154, 52, 18);
            flpChiTiet.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            flpChiTiet.DefaultCellStyle.BackColor = Color.White;
            flpChiTiet.DefaultCellStyle.ForeColor = Color.FromArgb(17, 24, 39);
            flpChiTiet.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            flpChiTiet.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 243, 234);
            flpChiTiet.DefaultCellStyle.SelectionForeColor = Color.FromArgb(17, 24, 39);
            flpChiTiet.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            flpChiTiet.GridColor = Color.FromArgb(229, 231, 235);
            flpChiTiet.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            flpChiTiet.RowTemplate.Height = 32;
            flpChiTiet.ScrollBars = ScrollBars.Vertical;

            var colMon = new DataGridViewTextBoxColumn
            {
                Name = "colMon",
                HeaderText = "Món",
                FillWeight = 55,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            var colSoLuong = new DataGridViewTextBoxColumn
            {
                Name = "colSoLuong",
                HeaderText = "SL",
                FillWeight = 15,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };
            var colThanhTien = new DataGridViewTextBoxColumn
            {
                Name = "colThanhTien",
                HeaderText = "Thành tiền",
                FillWeight = 30,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle =
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N0",
                    FormatProvider = CultureInfo.GetCultureInfo("vi-VN")
                }
            };
            var colTrangThai = new DataGridViewTextBoxColumn
            {
                Name = "colTrangThaiBep",
                HeaderText = "Bếp",
                FillWeight = 20,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            flpChiTiet.Columns.AddRange(colMon, colSoLuong, colThanhTien, colTrangThai);
            flpChiTiet.CellClick += chiTiet_Click;
        }

        private static void DinhDangNut(Button btn, string ten, Size size, Color nen,
            Color chu, Color vien, bool coVien)
        {
            btn.Name = ten;
            btn.Size = size;
            btn.Margin = Padding.Empty;
            btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btn.BackColor = nen;
            btn.ForeColor = chu;
            btn.FlatStyle = FlatStyle.Flat;
            btn.UseVisualStyleBackColor = false;
            btn.Cursor = Cursors.Hand;
            btn.FlatAppearance.BorderSize = coVien ? 1 : 0;
            btn.FlatAppearance.BorderColor = vien;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(nen, 0.1F);
        }

        private static void cot_Paint(object sender, PaintEventArgs e)
        {
            Control cot = (Control)sender;
            using (Pen vien = new Pen(Color.FromArgb(229, 231, 235), 1F))
                e.Graphics.DrawRectangle(vien, 0, 0, cot.Width - 1, cot.Height - 1);
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
                banCardDangChon = null;
                foreach (DataRow dong in ban.Rows)
                {
                    string trangThai = Convert.ToString(dong["TrangThai"]);
                    if (trangThai != TrangThaiBanTrong && trangThai != TrangThaiBanCoKhach)
                        throw new InvalidOperationException("Bàn " + Convert.ToString(dong["MaBan"]) + " có trạng thái không hợp lệ: " + trangThai);

                    var luaChon = new BanLuaChon(
                        Convert.ToString(dong["MaBan"]),
                        Convert.ToString(dong["TenBan"]),
                        Convert.ToInt32(dong["SoGhe"], CultureInfo.InvariantCulture),
                        trangThai);
                    var theBan = new BanCard(luaChon);
                    theBan.Tag = luaChon;
                    theBan.DangChon = string.Equals(luaChon.MaBan, maBanDangChon, StringComparison.OrdinalIgnoreCase);
                    if (theBan.DangChon)
                        banCardDangChon = theBan;
                    theBan.Click += btnBan_Click;
                    flpBan.Controls.Add(theBan);
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
                monCardDangChon = null;
                lblMonDangChon.Text = "Đã chọn: —";

                var luaChon = cboDanhMuc.SelectedItem as DanhMucLuaChon;
                string maDM = luaChon == null ? "" : luaChon.MaDM;
                string tuKhoa = txtTim.Text.Trim();
                foreach (DataRow dong in monAn.Rows)
                {
                    if (maDM.Length > 0 && !string.Equals(Convert.ToString(dong["MaDM"]), maDM, StringComparison.OrdinalIgnoreCase))
                        continue;
                    if (tuKhoa.Length > 0
                        && Convert.ToString(dong["TenMon"]).IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) < 0)
                        continue;

                    var theMon = new MonCard(dong);
                    theMon.Tag = dong;
                    theMon.Click += btnMon_Click;
                    flpMon.Controls.Add(theMon);
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

            var theBan = sender as BanCard;
            var luaChon = theBan == null ? null : theBan.Tag as BanLuaChon;
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
                if (banCardDangChon != null)
                    banCardDangChon.DangChon = false;
                banCardDangChon = theBan;
                banCardDangChon.DangChon = true;
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
            dongChiTietDangChon = null;
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

            lblBanDangChon.Text = "Bàn đang chọn: " + luaChon.TenBan;
            lblThongTinHD.Text = "Bàn: " + luaChon.TenBan + " | Mã HD: "
                + (maHoaDonDangChon.Length == 0 ? "—" : maHoaDonDangChon);
            HienThiMonAn();
            HienThiChiTiet();
            CapNhatTrangThaiNut();
        }

        private void btnMon_Click(object sender, EventArgs e)
        {
            var theMon = sender as MonCard;
            monDangChon = theMon == null ? null : theMon.Tag as DataRow;
            if (monCardDangChon != null)
                monCardDangChon.DangChon = false;
            monCardDangChon = theMon;
            if (monCardDangChon != null)
                monCardDangChon.DangChon = true;
            if (monDangChon != null)
                lblMonDangChon.Text = "Đã chọn: " + Convert.ToString(monDangChon["TenMon"]);
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
                DataRow dongChiTiet = chiTietDangChon.AsEnumerable()
                    .FirstOrDefault(dong => string.Equals(Convert.ToString(dong["MaMon"]), maMon, StringComparison.OrdinalIgnoreCase));
                int soLuongCu = dongChiTiet == null ? 0 : Convert.ToInt32(dongChiTiet["SoLuong"]);
                if (soLuong > int.MaxValue - soLuongCu)
                    throw new InvalidOperationException("Số lượng món vượt giới hạn cho phép.");
                if (dongChiTiet != null && DaGuiBep(dongChiTiet))
                {
                    MessageBox.Show("Món này đã gửi bếp, không thể tăng số lượng. Vui lòng thêm món khác.",
                        "Món đã gửi bếp", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int tongSoLuong = soLuongCu + soLuong;
                if (!Kho.DuNguyenLieu(maMon, tongSoLuong))
                {
                    MessageBox.Show("Không đủ nguyên liệu cho tổng số lượng món trong hóa đơn.",
                        "Không đủ nguyên liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

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
                    dongChiTiet["SoLuong"] = tongSoLuong;
                    dongChiTiet["DonGia"] = Convert.ToDecimal(monDangChon["DonGia"]);
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

        private void btnXoaDong_Click(object sender, EventArgs e)
        {
            if (!KiemTraQuyenBanHang() || choDongBo)
                return;
            if (dongChiTietDangChon == null || chiTietDangChon == null
                || !chiTietDangChon.AsEnumerable().Any(dong => ReferenceEquals(dong, dongChiTietDangChon)))
            {
                MessageBox.Show("Vui lòng chọn một dòng món trong hóa đơn.", "Chưa chọn dòng",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (DaGuiBep(dongChiTietDangChon)
                || !string.Equals(Convert.ToString(dongChiTietDangChon["TrangThaiBep"]), TrangThaiBepCho, StringComparison.Ordinal))
            {
                MessageBox.Show("Món đã gửi bếp nên không thể xóa khỏi hóa đơn.", "Món đã gửi bếp",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa dòng món này khỏi hóa đơn?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            chiTietDangChon.Rows.Remove(dongChiTietDangChon);
            dongChiTietDangChon = null;
            coThayDoiChuaGui = chiTietDangChon.AsEnumerable().Any(dong => !DaGuiBep(dong));
            HienThiChiTiet();
        }

        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            if (!KiemTraQuyenBanHang() || choDongBo)
                return;
            if (string.IsNullOrEmpty(maHoaDonDangChon) || coThayDoiChuaGui)
            {
                MessageBox.Show("Vui lòng gửi bếp hết các món trước khi thanh toán.", "Chưa thể thanh toán",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string maHoaDonVuaTra = maHoaDonDangChon;
                using (frmThanhToan f = new frmThanhToan(maHoaDonDangChon))
                {
                    if (f.ShowDialog(this) != DialogResult.OK)
                        return;
                }
                MessageBox.Show("Đã thanh toán hóa đơn " + maHoaDonVuaTra + ".", "Thanh toán thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                maBanDangChon = "";
                trangThaiBanDangChon = "";
                maHoaDonDangChon = "";
                chiTietDangChon = null;
                dongChiTietDangChon = null;
                lblBanDangChon.Text = "Bàn đang chọn: —";
                lblThongTinHD.Text = "Bàn: — | Mã HD: —";
                TaiDuLieu();
                HienThiChiTiet();
                CapNhatTrangThaiNut();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể thanh toán", ex);
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
                List<DataRow> dongChoGui = chiTietDangChon.AsEnumerable()
                    .Where(dong => !DaGuiBep(dong))
                    .ToList();
                if (dongChoGui.Count == 0)
                {
                    MessageBox.Show("Chưa có món mới để gửi bếp.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    coThayDoiChuaGui = false;
                    CapNhatTrangThaiNut();
                    return;
                }

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

                foreach (DataRow dong in dongChoGui)
                {
                    string maMon = Convert.ToString(dong["MaMon"]);
                    DataRow dongDaLuu = TimDongChiTiet(chiTietMoi, maHoaDonDangChon, maMon);
                    if (dongDaLuu != null)
                        throw new InvalidOperationException("Món đã được lưu trong hóa đơn; vui lòng tải lại trước khi gửi bếp.");
                    var giaTri = new Dictionary<string, string>
                    {
                        { "SoLuong", Convert.ToInt32(dong["SoLuong"]).ToString(CultureInfo.InvariantCulture) },
                        { "DonGia", Convert.ToDecimal(dong["DonGia"]).ToString("0", CultureInfo.InvariantCulture) },
                        { "TrangThaiBep", TrangThaiBepCho }
                    };
                    giaTri["MaHD"] = maHoaDonDangChon;
                    giaTri["MaMon"] = maMon;
                    string[] thuTuCot = { "MaHD", "MaMon", "SoLuong", "DonGia", "TrangThaiBep" };
                    fx.Them("ChiTietHoaDon.xml", "ChiTietHoaDon", TaoGiaTriTheoThuTu(thuTuCot, giaTri));
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
                flpChiTiet.Rows.Clear();
                if (chiTietDangChon != null)
                {
                    foreach (DataRow dong in chiTietDangChon.Rows)
                    {
                        string maMon = Convert.ToString(dong["MaMon"]);
                        DataRow thongTinMon = TimDong(monAn, "MaMon", maMon);
                        string tenMon = thongTinMon == null ? maMon : Convert.ToString(thongTinMon["TenMon"]);
                        int soLuong = Convert.ToInt32(dong["SoLuong"]);
                        decimal donGia = Convert.ToDecimal(dong["DonGia"]);
                        string trangThaiBep = Convert.ToString(dong["TrangThaiBep"]);
                        int viTri = flpChiTiet.Rows.Add(tenMon, soLuong, donGia * soLuong, trangThaiBep);
                        DataGridViewRow dongHienThi = flpChiTiet.Rows[viTri];
                        dongHienThi.Height = 32;
                        dongHienThi.Tag = dong;

                        Color mauTrangThai;
                        switch (trangThaiBep)
                        {
                            case "Chờ":
                                mauTrangThai = Color.FromArgb(217, 119, 6);
                                break;
                            case "Đang làm":
                                mauTrangThai = Color.FromArgb(37, 99, 235);
                                break;
                            case "Xong":
                                mauTrangThai = Color.FromArgb(22, 163, 74);
                                break;
                            default:
                                mauTrangThai = Color.FromArgb(107, 114, 128);
                                break;
                        }
                        dongHienThi.Cells["colTrangThaiBep"].Style.ForeColor = mauTrangThai;
                        dongHienThi.Cells["colTrangThaiBep"].Style.SelectionForeColor = mauTrangThai;
                        dongHienThi.Selected = ReferenceEquals(dong, dongChiTietDangChon);
                    }
                }
            }
            finally
            {
                flpChiTiet.ResumeLayout();
            }

            lblTamTinh.Text = DinhDangTien(TinhTamTinh());
            CapNhatTrangThaiNut();
        }

        private void chiTiet_Click(object sender, EventArgs e)
        {
            DataGridViewCellEventArgs cell = e as DataGridViewCellEventArgs;
            if (cell == null || cell.RowIndex < 0 || cell.RowIndex >= flpChiTiet.Rows.Count)
                return;
            dongChiTietDangChon = flpChiTiet.Rows[cell.RowIndex].Tag as DataRow;
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
            btnXoaDong.Enabled = coQuyen && !choDongBo && coHoaDon && dongChiTietDangChon != null;
            btnGuiBep.Enabled = coQuyen && (dangGuiBep
                || (!choDongBo && coHoaDon && coThayDoiChuaGui && coMon));
            btnThemMon.Enabled = coQuyen && !choDongBo && coHoaDon && monDangChon != null;
            btnThanhToan.Enabled = coQuyen && !choDongBo && coHoaDon && coMon && !coThayDoiChuaGui;
            flpBan.Enabled = !choDongBo && !coThayDoiChuaGui;
            flpMon.Enabled = !choDongBo;
            cboDanhMuc.Enabled = !choDongBo;
            txtTim.Enabled = !choDongBo;
            nudSoLuong.Enabled = !choDongBo;
        }

        private bool DaGuiBep(DataRow dong)
        {
            return TimDongChiTiet(chiTietHoaDon, Convert.ToString(dong["MaHD"]), Convert.ToString(dong["MaMon"])) != null;
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

        private sealed class BanCard : Panel
        {
            private readonly bool coKhach;
            private readonly string tenBan;
            private readonly int soGhe;
            private bool dangHover;
            private bool dangChon;

            public bool DangChon
            {
                get { return dangChon; }
                set
                {
                    if (dangChon == value)
                        return;
                    dangChon = value;
                    Invalidate();
                }
            }

            public BanCard(BanLuaChon ban)
            {
                Name = "theBan_" + ban.MaBan;
                tenBan = ban.TenBan;
                soGhe = ban.SoGhe;
                coKhach = ban.TrangThai == TrangThaiBanCoKhach;
                Size = new Size(104, 80);
                Margin = new Padding(4);
                Cursor = Cursors.Hand;
                DoubleBuffered = true;
                ResizeRedraw = true;
                BackColor = coKhach
                    ? Color.FromArgb(254, 242, 242)
                    : Color.FromArgb(236, 253, 243);
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                dangHover = true;
                Invalidate();
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                dangHover = false;
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.Clear(BackColor);

                Color mauVien = dangChon
                    ? Color.FromArgb(232, 101, 26)
                    : coKhach ? Color.FromArgb(220, 38, 38) : Color.FromArgb(22, 163, 74);
                using (Pen vien = new Pen(mauVien, dangChon || dangHover ? 3F : 2F))
                {
                    e.Graphics.DrawRectangle(vien, 2, 2, Width - 5, Height - 5);
                }

                Color mauChu = coKhach ? Color.FromArgb(153, 27, 27) : Color.FromArgb(22, 101, 52);
                using (Font fontTen = new Font("Segoe UI", 11F, FontStyle.Bold))
                using (Font fontGhe = new Font("Segoe UI", 9F))
                {
                    TextRenderer.DrawText(e.Graphics, tenBan, fontTen,
                        new Rectangle(5, 20, Width - 10, 21), mauChu,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                    TextRenderer.DrawText(e.Graphics, soGhe + " ghế", fontGhe,
                        new Rectangle(5, 42, Width - 10, 16), mauChu,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
            }
        }

        private sealed class MonCard : Panel
        {
            private readonly DataRow mon;
            private readonly bool conBan;
            private bool dangHover;
            private bool dangChon;

            public bool DangChon
            {
                get { return dangChon; }
                set
                {
                    if (dangChon == value)
                        return;
                    dangChon = value;
                    Invalidate();
                }
            }

            public MonCard(DataRow mon)
            {
                this.mon = mon;
                conBan = Convert.ToString(mon["TrangThai"]) == TrangThaiMonConBan;
                Name = "theMon_" + Convert.ToString(mon["MaMon"]);
                Size = new Size(164, 96);
                Margin = new Padding(5);
                Cursor = conBan ? Cursors.Hand : Cursors.Default;
                Enabled = conBan;
                DoubleBuffered = true;
                ResizeRedraw = true;
                BackColor = conBan ? Color.White : Color.FromArgb(243, 244, 246);
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                dangHover = conBan;
                Invalidate();
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                dangHover = false;
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Color nen = !conBan
                    ? Color.FromArgb(243, 244, 246)
                    : dangChon ? Color.FromArgb(255, 243, 234) : Color.White;
                Color mauVien = dangChon || dangHover
                    ? Color.FromArgb(232, 101, 26)
                    : Color.FromArgb(229, 231, 235);
                e.Graphics.Clear(nen);
                using (Pen vien = new Pen(mauVien, dangChon ? 2F : 1F))
                {
                    e.Graphics.DrawRectangle(vien, 0, 0, Width - 1, Height - 1);
                }
                using (SolidBrush thanhCam = new SolidBrush(Color.FromArgb(232, 101, 26)))
                    e.Graphics.FillRectangle(thanhCam, 1, 1, 4, Height - 2);

                Color mauChu = conBan ? Color.FromArgb(17, 24, 39) : Color.FromArgb(156, 163, 175);
                Color mauGia = conBan ? Color.FromArgb(232, 101, 26) : Color.FromArgb(156, 163, 175);
                using (Font fontTen = new Font("Segoe UI", 10.5F, FontStyle.Bold))
                using (Font fontGia = new Font("Segoe UI", 10F, FontStyle.Bold))
                {
                    Rectangle vungTen = conBan
                        ? new Rectangle(12, 12, Width - 22, 48)
                        : new Rectangle(12, 20, Width - 22, 40);
                    TextRenderer.DrawText(e.Graphics, Convert.ToString(mon["TenMon"]), fontTen,
                        vungTen, mauChu,
                        TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter);
                    TextRenderer.DrawText(e.Graphics, DinhDangTien(Convert.ToDecimal(mon["DonGia"])), fontGia,
                        new Rectangle(12, 69, Width - 22, 18), mauGia,
                        TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                }

                if (!conBan)
                {
                    using (Font fontHet = new Font("Segoe UI", 8.5F, FontStyle.Italic))
                    {
                        TextRenderer.DrawText(e.Graphics, "Hết", fontHet,
                            new Rectangle(Width - 37, 3, 32, 16), Color.FromArgb(156, 163, 175),
                            TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
                    }
                }
            }
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
