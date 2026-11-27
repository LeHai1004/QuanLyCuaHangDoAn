using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using QuanLyCuaHangDoAn.Class;

namespace QuanLyCuaHangDoAn
{
    public sealed class BanDuocChonEventArgs : EventArgs
    {
        public string MaBan { get; private set; }
        public string TenBan { get; private set; }
        public int SoGhe { get; private set; }
        public string TrangThai { get; private set; }

        internal BanDuocChonEventArgs(string maBan, string tenBan, int soGhe, string trangThai)
        {
            MaBan = maBan;
            TenBan = tenBan;
            SoGhe = soGhe;
            TrangThai = trangThai;
        }
    }

    public sealed class BanSoDoControl : UserControl
    {
        private readonly FileXml fx = new FileXml();
        private readonly FlowLayoutPanel flpDanhSachBan = new FlowLayoutPanel();
        private BanCard banDangChon;

        public event EventHandler<BanDuocChonEventArgs> BanDuocChon;
        public int SoLuongBan { get; private set; }
        public int SoBanTrong { get; private set; }
        public int SoBanCoKhach { get; private set; }

        public BanSoDoControl()
        {
            Name = "banSoDo";
            BackColor = Color.White;
            Margin = Padding.Empty;
            flpDanhSachBan.Dock = DockStyle.Fill;
            flpDanhSachBan.AutoScroll = true;
            flpDanhSachBan.Padding = Padding.Empty;
            flpDanhSachBan.WrapContents = true;
            Controls.Add(flpDanhSachBan);
        }

        public void TaiDanhSachBan()
        {
            fx.TaoXml("SELECT * FROM Ban", "Ban", "Ban.xml");
            DataTable duLieu = fx.HienThi("Ban.xml");
            banDangChon = null;
            SoLuongBan = duLieu.Rows.Count;
            SoBanTrong = 0;
            SoBanCoKhach = 0;
            flpDanhSachBan.SuspendLayout();
            try
            {
                foreach (Control control in flpDanhSachBan.Controls.Cast<Control>().ToList())
                {
                    flpDanhSachBan.Controls.Remove(control);
                    control.Dispose();
                }
                foreach (DataRow dong in duLieu.Rows)
                {
                    string maBan = Convert.ToString(dong["MaBan"]);
                    string tenBan = Convert.ToString(dong["TenBan"]);
                    int soGhe = Convert.ToInt32(dong["SoGhe"], CultureInfo.InvariantCulture);
                    string trangThai = Convert.ToString(dong["TrangThai"]);
                    if (trangThai == "Trống")
                        SoBanTrong++;
                    else if (trangThai == "Có khách")
                        SoBanCoKhach++;
                    else
                        throw new InvalidOperationException("Bàn " + maBan + " có trạng thái không hợp lệ: " + trangThai);

                    var ban = new BanDuocChonEventArgs(maBan, tenBan, soGhe, trangThai);
                    var theBan = new BanCard(ban);
                    theBan.Click += btnBan_Click;
                    flpDanhSachBan.Controls.Add(theBan);
                }
            }
            finally
            {
                flpDanhSachBan.ResumeLayout();
            }
        }

        public void BoChon()
        {
            if (banDangChon == null)
                return;
            banDangChon.DangChon = false;
            banDangChon = null;
        }

        private void btnBan_Click(object sender, EventArgs e)
        {
            BanCard theBan = sender as BanCard;
            if (theBan == null)
                return;
            BoChon();
            banDangChon = theBan;
            banDangChon.DangChon = true;
            BanDuocChon?.Invoke(this, theBan.Ban);
        }

        private sealed class BanCard : Panel
        {
            private readonly BanDuocChonEventArgs ban;
            private bool dangHover;
            private bool dangChon;

            public BanDuocChonEventArgs Ban
            {
                get { return ban; }
            }

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

            public BanCard(BanDuocChonEventArgs ban)
            {
                this.ban = ban;
                Name = "theBan_" + ban.MaBan;
                Size = new Size(200, 120);
                Margin = new Padding(5);
                Cursor = Cursors.Hand;
                DoubleBuffered = true;
                ResizeRedraw = true;
                BackColor = ban.TrangThai == "Trống"
                    ? Color.FromArgb(236, 253, 243)
                    : Color.FromArgb(254, 242, 242);
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
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(BackColor);

                Color mauTrangThai = ban.TrangThai == "Trống"
                    ? Color.FromArgb(22, 163, 74)
                    : Color.FromArgb(220, 38, 38);
                Color mauVien = dangChon ? Color.FromArgb(232, 101, 26) : mauTrangThai;
                float doDayVien = dangChon || dangHover ? 3F : 2F;
                Rectangle vienThe = new Rectangle(1, 1, Width - 3, Height - 3);
                using (GraphicsPath pathThe = TaoChuNhatBoGoc(vienThe, 10))
                using (Pen vien = new Pen(mauVien, doDayVien))
                {
                    g.DrawPath(vien, pathThe);
                }

                Rectangle matBan = new Rectangle((Width - 60) / 2, 12, 60, 26);
                VeGhe(g, matBan, mauTrangThai, Math.Min(ban.SoGhe, 8));
                using (GraphicsPath pathMatBan = TaoChuNhatBoGoc(matBan, 6))
                using (SolidBrush nenMatBan = new SolidBrush(mauTrangThai))
                {
                    g.FillPath(nenMatBan, pathMatBan);
                }

                using (Font fontTen = new Font("Segoe UI", 12F, FontStyle.Bold))
                using (Font fontGhe = new Font("Segoe UI", 9F))
                {
                    TextRenderer.DrawText(g, ban.TenBan, fontTen,
                        new Rectangle(8, 47, Width - 16, 20), Color.FromArgb(17, 24, 39),
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                    TextRenderer.DrawText(g, ban.SoGhe + " ghế", fontGhe,
                        new Rectangle(8, 67, Width - 16, 15), Color.FromArgb(107, 114, 128),
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }

                Rectangle badge = new Rectangle((Width - 80) / 2, 91, 80, 22);
                using (GraphicsPath pathBadge = TaoChuNhatBoGoc(badge, 11))
                using (SolidBrush nenBadge = new SolidBrush(mauTrangThai))
                using (Font fontBadge = new Font("Segoe UI", 8.5F, FontStyle.Bold))
                {
                    g.FillPath(nenBadge, pathBadge);
                    TextRenderer.DrawText(g, ban.TrangThai, fontBadge, badge, Color.White,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
            }

            private static void VeGhe(Graphics g, Rectangle matBan, Color mau, int soGhe)
            {
                int gheTren = (soGhe + 1) / 2;
                int gheDuoi = soGhe / 2;
                using (SolidBrush nenGhe = new SolidBrush(mau))
                {
                    VeHangGhe(g, nenGhe, matBan, gheTren, matBan.Top - 8);
                    VeHangGhe(g, nenGhe, matBan, gheDuoi, matBan.Bottom - 2);
                }
            }

            private static void VeHangGhe(Graphics g, Brush nen, Rectangle matBan, int soGhe, int top)
            {
                for (int i = 0; i < soGhe; i++)
                {
                    int left = matBan.Left + ((i + 1) * matBan.Width) / (soGhe + 1) - 5;
                    g.FillEllipse(nen, left, top, 10, 10);
                }
            }

            private static GraphicsPath TaoChuNhatBoGoc(Rectangle hinh, float banKinh)
            {
                float duongKinh = banKinh * 2;
                GraphicsPath path = new GraphicsPath();
                path.AddArc(hinh.X, hinh.Y, duongKinh, duongKinh, 180, 90);
                path.AddArc(hinh.Right - duongKinh, hinh.Y, duongKinh, duongKinh, 270, 90);
                path.AddArc(hinh.Right - duongKinh, hinh.Bottom - duongKinh, duongKinh, duongKinh, 0, 90);
                path.AddArc(hinh.X, hinh.Bottom - duongKinh, duongKinh, duongKinh, 90, 90);
                path.CloseFigure();
                return path;
            }
        }
    }

    public class frmBan : Form
    {
        private readonly FileXml fx = new FileXml();
        private readonly bool laAdmin;
        private readonly BanSoDoControl banSoDo = new BanSoDoControl();
        private readonly TextBox txtMaBan = new TextBox();
        private readonly TextBox txtTenBan = new TextBox();
        private readonly NumericUpDown nudSoGhe = new NumericUpDown();
        private readonly ComboBox cboTrangThai = new ComboBox();
        private readonly Button btnThem = GiaoDien.TaoNut("Thêm", GiaoDien.MauChinh);
        private readonly Button btnSua = GiaoDien.TaoNut("Sửa", GiaoDien.MauChinh);
        private readonly Button btnXoa = GiaoDien.TaoNut("Xóa", GiaoDien.MauDo);
        private readonly Button btnLuu = GiaoDien.TaoNut("Lưu", GiaoDien.MauXanh);
        private readonly Button btnHuy = GiaoDien.TaoNut("Hủy", GiaoDien.MauChinhDam);
        private readonly Button btnLamMoi = GiaoDien.TaoNut("Làm mới", GiaoDien.MauChinh);
        private Label lblTongBanSo;
        private Label lblBanTrongSo;
        private Label lblBanCoKhachSo;

        private string cheDo = "";
        private bool choDongBo;
        private string hanhDongChoDongBo;
        private string maBanDangChon = "";

        public frmBan()
        {
            laAdmin = LaAdminHienTai();
            KhoiTaoGiaoDien();
            Load += frmBan_Load;
        }

        private void KhoiTaoGiaoDien()
        {
            Text = laAdmin ? "Quản lý bàn" : "Sơ đồ bàn";
            StartPosition = FormStartPosition.CenterParent;
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(245, 246, 248);
            Font = new Font("Segoe UI", 10F);
            ClientSize = new Size(1100, 680);
            MinimumSize = new Size(1116, 719);
            DoubleBuffered = true;

            Panel pnlTheSoDo = new Panel
            {
                Name = "pnlTheSoDo",
                Location = new Point(30, 100),
                Size = new Size(690, 550),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom
            };
            pnlTheSoDo.Paint += pnlThe_Paint;

            Panel pnlThongTin = new Panel
            {
                Name = "pnlThongTin",
                Location = new Point(745, 100),
                Size = new Size(325, 550),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom
            };
            pnlThongTin.Paint += pnlThe_Paint;

            ThemThongKe(pnlTheSoDo);
            Label lblSoDo = new Label
            {
                Name = "lblSoDo",
                Text = "SƠ ĐỒ BÀN",
                Left = 20,
                Top = 95,
                Width = 650,
                Height = 20,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(55, 65, 81)
            };
            pnlTheSoDo.Controls.Add(lblSoDo);

            banSoDo.Location = new Point(20, 125);
            banSoDo.Size = new Size(650, 410);
            banSoDo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            pnlTheSoDo.Controls.Add(banSoDo);
            TaoKhungCrud(pnlThongTin);

            Panel pnlHeader = new Panel
            {
                Name = "pnlHeader",
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(232, 101, 26)
            };
            Label lblTieuDe = new Label
            {
                Name = "lblTieuDe",
                Text = "QUẢN LÝ BÀN",
                Left = 30,
                Top = 12,
                Width = 450,
                Height = 30,
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.White
            };
            Label lblMoTa = new Label
            {
                Name = "lblMoTa",
                Text = "Sơ đồ bàn và thông tin chi tiết",
                Left = 32,
                Top = 50,
                Width = 450,
                Height = 18,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.White
            };
            pnlHeader.Controls.Add(lblTieuDe);
            pnlHeader.Controls.Add(lblMoTa);

            Controls.Add(pnlTheSoDo);
            Controls.Add(pnlThongTin);
            Controls.Add(pnlHeader);

            banSoDo.BanDuocChon += banSoDo_BanDuocChon;
            btnThem.Click += btnThem_Click;
            btnSua.Click += btnSua_Click;
            btnXoa.Click += btnXoa_Click;
            btnLuu.Click += btnLuu_Click;
            btnHuy.Click += btnHuy_Click;
            btnLamMoi.Click += btnLamMoi_Click;
            CapNhatTrangThaiNut();
        }

        private void ThemThongKe(Panel pnlTheSoDo)
        {
            pnlTheSoDo.Controls.Add(TaoOThongKe("pnlThongKeTong", "Tổng bàn",
                Color.FromArgb(17, 24, 39), 20, out lblTongBanSo));
            pnlTheSoDo.Controls.Add(TaoOThongKe("pnlThongKeTrong", "Trống",
                Color.FromArgb(22, 163, 74), 230, out lblBanTrongSo));
            pnlTheSoDo.Controls.Add(TaoOThongKe("pnlThongKeCoKhach", "Có khách",
                Color.FromArgb(220, 38, 38), 440, out lblBanCoKhachSo));
        }

        private static Panel TaoOThongKe(string ten, string nhan, Color mau, int left, out Label lblGiaTri)
        {
            Panel pnl = new Panel
            {
                Name = ten,
                Left = left,
                Top = 20,
                Width = 200,
                Height = 60,
                BackColor = Color.FromArgb(249, 250, 251)
            };
            lblGiaTri = new Label
            {
                Name = "lbl" + ten.Substring(3) + "So",
                Text = "0",
                Left = 0,
                Top = 4,
                Width = 200,
                Height = 28,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = mau,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Label lblNhan = new Label
            {
                Name = "lbl" + ten.Substring(3),
                Text = nhan,
                Left = 0,
                Top = 34,
                Width = 200,
                Height = 18,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(107, 114, 128),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnl.Controls.Add(lblGiaTri);
            pnl.Controls.Add(lblNhan);
            return pnl;
        }

        private void TaoKhungCrud(Panel cha)
        {
            Label lblThongTin = new Label
            {
                Name = "lblThongTin",
                Text = "THÔNG TIN BÀN",
                Left = 20,
                Top = 20,
                Width = 285,
                Height = 24,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(232, 101, 26)
            };
            cha.Controls.Add(lblThongTin);

            ThemTruong(cha, "Mã bàn", "lblMaBan", txtMaBan, "txtMaBan", 65);
            ThemTruong(cha, "Tên bàn", "lblTenBan", txtTenBan, "txtTenBan", 135);
            ThemTruong(cha, "Số ghế", "lblSoGhe", nudSoGhe, "nudSoGhe", 205);
            ThemTruong(cha, "Trạng thái", "lblTrangThai", cboTrangThai, "cboTrangThai", 275);

            txtMaBan.ReadOnly = true;
            txtMaBan.BorderStyle = BorderStyle.FixedSingle;
            txtMaBan.BackColor = Color.FromArgb(243, 244, 246);
            nudSoGhe.Minimum = 1;
            nudSoGhe.Maximum = int.MaxValue;
            nudSoGhe.DecimalPlaces = 0;
            nudSoGhe.ThousandsSeparator = true;
            nudSoGhe.BorderStyle = BorderStyle.FixedSingle;
            cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTrangThai.IntegralHeight = false;
            cboTrangThai.Items.AddRange(new object[] { "Trống", "Có khách" });

            DinhDangNut(btnThem, "btnThem", Color.FromArgb(232, 101, 26), Color.White,
                Color.FromArgb(207, 85, 16), false);
            DinhDangNut(btnSua, "btnSua", Color.FromArgb(37, 99, 235), Color.White,
                Color.FromArgb(29, 78, 216), false);
            DinhDangNut(btnXoa, "btnXoa", Color.FromArgb(220, 38, 38), Color.White,
                Color.FromArgb(185, 28, 28), false);
            DinhDangNut(btnLuu, "btnLuu", Color.FromArgb(22, 163, 74), Color.White,
                Color.FromArgb(21, 128, 61), false);
            DinhDangNut(btnHuy, "btnHuy", Color.FromArgb(107, 114, 128), Color.White,
                Color.FromArgb(75, 85, 99), false);
            DinhDangNut(btnLamMoi, "btnLamMoi", Color.White, Color.FromArgb(232, 101, 26),
                Color.FromArgb(255, 243, 234), true);

            TableLayoutPanel nutGrid = new TableLayoutPanel
            {
                Name = "tlpNut",
                Left = 20,
                Top = 370,
                Width = 285,
                Height = 96,
                ColumnCount = 3,
                RowCount = 2,
                GrowStyle = TableLayoutPanelGrowStyle.FixedSize,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            nutGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34.21F));
            nutGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34.21F));
            nutGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 31.58F));
            nutGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            nutGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            nutGrid.Controls.Add(btnThem, 0, 0);
            nutGrid.Controls.Add(btnSua, 1, 0);
            nutGrid.Controls.Add(btnXoa, 2, 0);
            nutGrid.Controls.Add(btnLuu, 0, 1);
            nutGrid.Controls.Add(btnHuy, 1, 1);
            nutGrid.Controls.Add(btnLamMoi, 2, 1);
            cha.Controls.Add(nutGrid);
        }

        private static void ThemTruong(Panel panel, string nhan, string tenLabel, Control control,
            string tenControl, int top)
        {
            Label lbl = new Label
            {
                Name = tenLabel,
                Text = nhan,
                Left = 20,
                Top = top,
                Width = 285,
                Height = 18,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(107, 114, 128)
            };
            control.Name = tenControl;
            control.Left = 20;
            control.Top = top + 22;
            control.Width = 285;
            control.Height = 36;
            control.Font = new Font("Segoe UI", 11F);
            panel.Controls.Add(lbl);
            panel.Controls.Add(control);
        }

        private static void DinhDangNut(Button btn, string ten, Color nen, Color chu,
            Color hover, bool coVien)
        {
            btn.Name = ten;
            btn.Size = new Size(90, 40);
            btn.Margin = Padding.Empty;
            btn.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btn.BackColor = nen;
            btn.ForeColor = chu;
            btn.FlatStyle = FlatStyle.Flat;
            btn.UseVisualStyleBackColor = false;
            btn.Cursor = Cursors.Hand;
            btn.FlatAppearance.BorderSize = coVien ? 1 : 0;
            btn.FlatAppearance.BorderColor = Color.FromArgb(232, 101, 26);
            btn.FlatAppearance.MouseOverBackColor = hover;
        }

        private static void pnlThe_Paint(object sender, PaintEventArgs e)
        {
            Control pnl = (Control)sender;
            using (Pen vien = new Pen(Color.FromArgb(229, 231, 235), 1F))
            {
                e.Graphics.DrawRectangle(vien, 0, 0, pnl.Width - 1, pnl.Height - 1);
            }
        }

        private void CapNhatThongKe()
        {
            lblTongBanSo.Text = banSoDo.SoLuongBan.ToString(CultureInfo.InvariantCulture);
            lblBanTrongSo.Text = banSoDo.SoBanTrong.ToString(CultureInfo.InvariantCulture);
            lblBanCoKhachSo.Text = banSoDo.SoBanCoKhach.ToString(CultureInfo.InvariantCulture);
        }

        private void frmBan_Load(object sender, EventArgs e)
        {
            try
            {
                banSoDo.TaiDanhSachBan();
                CapNhatThongKe();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể tải sơ đồ bàn", ex);
            }
        }

        private void banSoDo_BanDuocChon(object sender, BanDuocChonEventArgs e)
        {
            maBanDangChon = e.MaBan;
            txtMaBan.Text = e.MaBan;
            txtTenBan.Text = e.TenBan;
            nudSoGhe.Value = Math.Min(nudSoGhe.Maximum, Math.Max(nudSoGhe.Minimum, e.SoGhe));
            cboTrangThai.SelectedItem = e.TrangThai;
            CapNhatTrangThaiNut();
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            try
            {
                banSoDo.TaiDanhSachBan();
                CapNhatThongKe();
                HuyChinhSua();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể làm mới sơ đồ bàn", ex);
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!KiemTraQuyenAdmin() || choDongBo)
                return;
            try
            {
                fx.TaoXml("SELECT * FROM Ban", "Ban", "Ban.xml");
                maBanDangChon = "";
                txtMaBan.Text = fx.TaoMa("BA", "Ban.xml", "MaBan");
                txtTenBan.Clear();
                nudSoGhe.Value = 1;
                cboTrangThai.SelectedItem = "Trống";
                cheDo = "them";
                CapNhatTrangThaiNut();
                txtTenBan.Focus();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể tạo mã bàn", ex);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (!KiemTraQuyenAdmin() || choDongBo)
                return;
            if (string.IsNullOrEmpty(maBanDangChon))
            {
                MessageBox.Show("Vui lòng chọn bàn cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            cheDo = "sua";
            CapNhatTrangThaiNut();
            txtTenBan.Focus();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (!KiemTraQuyenAdmin() || choDongBo)
                return;
            if (string.IsNullOrEmpty(maBanDangChon))
            {
                MessageBox.Show("Vui lòng chọn bàn cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                fx.TaoXml("SELECT * FROM HoaDon", "HoaDon", "HoaDon.xml");
                DataTable hoaDon = fx.HienThi("HoaDon.xml");
                if (hoaDon.AsEnumerable().Any(dong =>
                    string.Equals(Convert.ToString(dong["MaBan"]), maBanDangChon, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("Không thể xóa bàn đã được ghi nhận trong hóa đơn.", "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("Bạn có chắc muốn xóa bàn " + txtTenBan.Text + " không?", "Xác nhận xóa",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                if (fx.Xoa("Ban.xml", new Dictionary<string, string> { { "MaBan", maBanDangChon } }) == 0)
                    throw new InvalidOperationException("Không tìm thấy bàn cần xóa trong file XML.");

                ChoDongBo("xóa bàn");
                ThuDongBo();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể xóa hoặc đồng bộ bàn", ex);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (!KiemTraQuyenAdmin())
                return;
            if (choDongBo)
            {
                ThuDongBo();
                return;
            }
            if (string.IsNullOrEmpty(cheDo))
            {
                MessageBox.Show("Vui lòng chọn Thêm hoặc Sửa trước khi lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string tenBan = txtTenBan.Text.Trim();
            if (tenBan.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập tên bàn.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenBan.Focus();
                return;
            }
            if (tenBan.Length > 30)
            {
                MessageBox.Show("Tên bàn không được vượt quá 30 ký tự.", "Thông tin không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenBan.Focus();
                return;
            }
            if (cboTrangThai.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn trạng thái bàn.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (cheDo == "them")
                {
                    if (fx.TonTai("Ban.xml", "MaBan", txtMaBan.Text))
                        throw new InvalidOperationException("Mã bàn đã tồn tại. Hãy hủy thao tác và tạo lại mã.");

                    string[] thuTuCot = { "MaBan", "TenBan", "SoGhe", "TrangThai" };
                    var giaTri = new SortedDictionary<string, string>(Comparer<string>.Create((a, b) =>
                        Array.IndexOf(thuTuCot, a).CompareTo(Array.IndexOf(thuTuCot, b))));
                    giaTri.Add("MaBan", txtMaBan.Text);
                    giaTri.Add("TenBan", tenBan);
                    giaTri.Add("SoGhe", nudSoGhe.Value.ToString(CultureInfo.InvariantCulture));
                    giaTri.Add("TrangThai", Convert.ToString(cboTrangThai.SelectedItem));
                    fx.Them("Ban.xml", "Ban", giaTri);
                    ChoDongBo("thêm bàn");
                }
                else
                {
                    var giaTriMoi = new Dictionary<string, string>
                    {
                        { "TenBan", tenBan },
                        { "SoGhe", nudSoGhe.Value.ToString(CultureInfo.InvariantCulture) },
                        { "TrangThai", Convert.ToString(cboTrangThai.SelectedItem) }
                    };
                    if (fx.Sua("Ban.xml", new Dictionary<string, string> { { "MaBan", txtMaBan.Text } }, giaTriMoi) == 0)
                        throw new InvalidOperationException("Không tìm thấy bàn cần sửa trong file XML.");
                    ChoDongBo("sửa bàn");
                }

                ThuDongBo();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể lưu bàn", ex);
            }
        }

        private void ThuDongBo()
        {
            if (!KiemTraQuyenAdmin() || !choDongBo)
                return;
            try
            {
                fx.CapNhatDb("Ban", "Ban.xml");
            }
            catch (Exception ex)
            {
                CapNhatTrangThaiNut();
                MessageBox.Show("XML đã thay đổi nhưng chưa đồng bộ được bàn vào CSDL. Nhấn Lưu để thử đồng bộ lại.\r\n\r\n" + ex.Message,
                    "Lỗi đồng bộ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            choDongBo = false;
            hanhDongChoDongBo = null;
            cheDo = "";
            maBanDangChon = "";
            try
            {
                banSoDo.TaiDanhSachBan();
                CapNhatThongKe();
                HuyChinhSua();
            }
            catch (Exception ex)
            {
                CapNhatTrangThaiNut();
                HienLoi("Bàn đã đồng bộ nhưng không thể tải lại sơ đồ", ex);
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            if (!KiemTraQuyenAdmin() || choDongBo)
                return;
            HuyChinhSua();
        }

        private void HuyChinhSua()
        {
            cheDo = "";
            maBanDangChon = "";
            banSoDo.BoChon();
            txtMaBan.Clear();
            txtTenBan.Clear();
            nudSoGhe.Value = 1;
            cboTrangThai.SelectedItem = null;
            CapNhatTrangThaiNut();
        }

        private void ChoDongBo(string hanhDong)
        {
            choDongBo = true;
            hanhDongChoDongBo = hanhDong;
            CapNhatTrangThaiNut();
        }

        private bool KiemTraQuyenAdmin()
        {
            if (LaAdminHienTai())
                return true;
            MessageBox.Show("Chỉ Admin mới được thêm, sửa hoặc xóa bàn.", "Không đủ quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        private static bool LaAdminHienTai()
        {
            return PhienDangNhap.IsLoggedIn
                && string.Equals(PhienDangNhap.ChucVu, "Admin", StringComparison.Ordinal);
        }

        private void CapNhatTrangThaiNut()
        {
            bool dangChinhSua = !string.IsNullOrEmpty(cheDo);
            bool coQuyen = LaAdminHienTai() && !choDongBo;
            btnThem.Enabled = coQuyen && !dangChinhSua;
            btnSua.Enabled = coQuyen && !dangChinhSua && !string.IsNullOrEmpty(maBanDangChon);
            btnXoa.Enabled = coQuyen && !dangChinhSua && !string.IsNullOrEmpty(maBanDangChon);
            btnLuu.Enabled = LaAdminHienTai() && (dangChinhSua || choDongBo);
            btnHuy.Enabled = coQuyen && dangChinhSua;
            btnLamMoi.Enabled = !choDongBo;
            banSoDo.Enabled = !dangChinhSua && !choDongBo;
            txtTenBan.ReadOnly = !dangChinhSua;
            nudSoGhe.Enabled = dangChinhSua;
            cboTrangThai.Enabled = dangChinhSua;
            btnLuu.Text = choDongBo ? "Đồng bộ" : "Lưu";
            txtMaBan.BackColor = Color.FromArgb(243, 244, 246);
            txtTenBan.BackColor = dangChinhSua ? Color.White : Color.FromArgb(243, 244, 246);
            nudSoGhe.ReadOnly = !dangChinhSua;
            nudSoGhe.BackColor = dangChinhSua ? Color.White : Color.FromArgb(243, 244, 246);
            cboTrangThai.Enabled = dangChinhSua;
            cboTrangThai.BackColor = dangChinhSua ? Color.White : Color.FromArgb(243, 244, 246);
            Text = choDongBo && !string.IsNullOrEmpty(hanhDongChoDongBo)
                ? "Quản lý bàn - chờ đồng bộ " + hanhDongChoDongBo
                : laAdmin ? "Quản lý bàn" : "Sơ đồ bàn";
        }

        private static void HienLoi(string noiDung, Exception ex)
        {
            MessageBox.Show(noiDung + ".\r\n\r\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
