using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using QuanLyCuaHangDoAn.Class;

namespace QuanLyCuaHangDoAn.GUI
{
    public partial class frmNhanVien : Form
    {
        private const string TenFile = "NhanVien.xml";
        private const string TenBang = "NhanVien";
        private static readonly string[] DsChucVu = { "Admin", "ThuNgan", "Bep" };

        private readonly FileXml fx = new FileXml();
        private DataTable bang;
        private bool dangThem;

        private DataGridView dgvNhanVien;
        private TextBox txtMaNV, txtTenNV, txtSDT, txtTenDN, txtMatKhau, txtTim;
        private ComboBox cboChucVu;
        private Button btnThem, btnSua, btnXoa, btnLuu, btnHuy, btnTim;
        private Label lblTongNhanVien, lblSoAdmin, lblSoThuNgan, lblSoBep;

        public frmNhanVien()
        {
            InitializeComponent();
            XayDungGiaoDien();
            Load += frmNhanVien_Load;
        }

        // ---------- Dựng giao diện ----------
        private TextBox ThemO(Panel cha, string tenControl, string nhan, int y, bool readOnly = false)
        {
            Label l = new Label
            {
                Name = "lbl" + tenControl.Substring(3),
                Text = nhan,
                Location = new Point(20, y),
                Size = new Size(285, 16),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(107, 114, 128)
            };
            TextBox t = new TextBox
            {
                Name = tenControl,
                Location = new Point(20, y + 20),
                Size = new Size(285, 34),
                AutoSize = false,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10.5F),
                ReadOnly = readOnly,
                BackColor = readOnly ? Color.FromArgb(243, 244, 246) : Color.White
            };
            cha.Controls.Add(l);
            cha.Controls.Add(t);
            return t;
        }

        private static Label TaoNhanThongKe(string ten, string noiDung, Font font, Color mau, Point viTri, Size kichThuoc)
        {
            return new Label
            {
                Name = ten,
                Text = noiDung,
                Location = viTri,
                Size = kichThuoc,
                Font = font,
                ForeColor = mau,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                AutoEllipsis = true
            };
        }

        private static Panel TaoTheThongKe(string ten, string nhan, out Label lblGiaTri)
        {
            Panel the = new Panel
            {
                Name = ten,
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(249, 250, 251),
                Margin = Padding.Empty
            };
            lblGiaTri = TaoNhanThongKe("lbl" + ten.Substring(3), "0",
                new Font("Segoe UI", 18F, FontStyle.Bold), Color.FromArgb(17, 24, 39),
                new Point(4, 3), new Size(145, 29));
            Label lblNhan = TaoNhanThongKe("lblNhan" + ten.Substring(3), nhan,
                new Font("Segoe UI", 9F), Color.FromArgb(107, 114, 128),
                new Point(4, 34), new Size(145, 20));
            the.Controls.Add(lblGiaTri);
            the.Controls.Add(lblNhan);
            return the;
        }

        private Button ThemNut(TableLayoutPanel cha, string tenControl, string chu, int cot, int dong,
            int soCot, Color nen, EventHandler suKien)
        {
            Button b = new Button
            {
                Name = tenControl,
                Text = chu,
                Dock = DockStyle.Fill,
                Margin = new Padding(4, 0, 4, 0),
                BackColor = nen,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = MauToiHon(nen, 0.9);
            b.FlatAppearance.MouseDownBackColor = MauToiHon(nen, 0.82);
            b.Click += suKien;
            cha.Controls.Add(b, cot, dong);
            if (soCot > 1)
                cha.SetColumnSpan(b, soCot);
            return b;
        }

        private static Color MauToiHon(Color mau, double heSo)
        {
            return Color.FromArgb((int)(mau.R * heSo), (int)(mau.G * heSo), (int)(mau.B * heSo));
        }

        private void XayDungGiaoDien()
        {
            Text = "Quản lý nhân viên";
            ClientSize = new Size(1100, 680);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 10F);
            BackColor = Color.FromArgb(245, 246, 248);
            DoubleBuffered = true;

            Panel pnlTop = new Panel
            {
                Name = "pnlHeader",
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(232, 101, 26)
            };
            Label lblTieuDe = new Label
            {
                Name = "lblTieuDe",
                Text = "QUẢN LÝ NHÂN VIÊN",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(30, 12),
                Size = new Size(650, 32),
                AutoSize = false,
                AutoEllipsis = true
            };
            Label lblPhuDe = new Label
            {
                Name = "lblPhuDe",
                Text = "Tài khoản và phân quyền",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.White,
                Location = new Point(32, 50),
                Size = new Size(500, 20),
                AutoSize = false
            };
            pnlTop.Controls.Add(lblTieuDe);
            pnlTop.Controls.Add(lblPhuDe);

            BorderPanel pnlTrai = new BorderPanel
            {
                Name = "pnlDanhSach",
                Location = new Point(30, 100),
                Size = new Size(690, 550),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackColor = Color.White
            };
            TableLayoutPanel tlpThongKe = new TableLayoutPanel
            {
                Name = "tlpThongKe",
                Location = new Point(20, 20),
                Size = new Size(650, 60),
                ColumnCount = 7,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Color.White
            };
            for (int i = 0; i < 7; i++)
            {
                if (i % 2 == 0)
                    tlpThongKe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
                else
                    tlpThongKe.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 12F));
            }
            tlpThongKe.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpThongKe.Controls.Add(TaoTheThongKe("pnlThongKeTong", "Tổng nhân viên", out lblTongNhanVien), 0, 0);
            tlpThongKe.Controls.Add(TaoTheThongKe("pnlThongKeAdmin", "Admin", out lblSoAdmin), 2, 0);
            tlpThongKe.Controls.Add(TaoTheThongKe("pnlThongKeThuNgan", "Thu ngân", out lblSoThuNgan), 4, 0);
            tlpThongKe.Controls.Add(TaoTheThongKe("pnlThongKeBep", "Bếp", out lblSoBep), 6, 0);
            pnlTrai.Controls.Add(tlpThongKe);

            txtTim = new TextBox
            {
                Name = "txtTim",
                Location = new Point(20, 95),
                Size = new Size(545, 36),
                AutoSize = false,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 11F)
            };
            txtTim.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    btnTim.PerformClick();
                }
            };
            pnlTrai.Controls.Add(txtTim);

            btnTim = new Button
            {
                Name = "btnTim",
                Text = "Tìm",
                Location = new Point(575, 95),
                Size = new Size(95, 36),
                BackColor = Color.FromArgb(232, 101, 26),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
            btnTim.FlatAppearance.BorderSize = 0;
            btnTim.FlatAppearance.MouseOverBackColor = MauToiHon(Color.FromArgb(232, 101, 26), 0.9);
            btnTim.Click += btnTim_Click;
            pnlTrai.Controls.Add(btnTim);

            dgvNhanVien = new DataGridView
            {
                Name = "dgvNhanVien",
                Location = new Point(20, 145),
                Size = new Size(650, 385),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BorderStyle = BorderStyle.None,
                BackgroundColor = Color.White,
                EnableHeadersVisualStyles = false,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(243, 244, 246),
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeight = 38,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                RowTemplate = { Height = 44 },
                ShowCellToolTips = true
            };
            dgvNhanVien.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 243, 234);
            dgvNhanVien.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(154, 52, 18);
            dgvNhanVien.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvNhanVien.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvNhanVien.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgvNhanVien.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 4, 0);
            dgvNhanVien.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 243, 234);
            dgvNhanVien.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(154, 52, 18);
            dgvNhanVien.DefaultCellStyle.BackColor = Color.White;
            dgvNhanVien.DefaultCellStyle.ForeColor = Color.FromArgb(17, 24, 39);
            dgvNhanVien.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvNhanVien.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgvNhanVien.DefaultCellStyle.Padding = new Padding(8, 0, 4, 0);
            dgvNhanVien.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 243, 234);
            dgvNhanVien.DefaultCellStyle.SelectionForeColor = Color.FromArgb(17, 24, 39);
            dgvNhanVien.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            dgvNhanVien.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 243, 234);
            dgvNhanVien.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.FromArgb(17, 24, 39);
            dgvNhanVien.SelectionChanged += dgvNhanVien_SelectionChanged;
            dgvNhanVien.CellPainting += dgvNhanVien_CellPainting;
            dgvNhanVien.CellToolTipTextNeeded += dgvNhanVien_CellToolTipTextNeeded;
            pnlTrai.Controls.Add(dgvNhanVien);

            BorderPanel pnlPhai = new BorderPanel
            {
                Name = "pnlThongTin",
                Location = new Point(745, 100),
                Size = new Size(325, 550),
                Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom,
                BackColor = Color.White
            };
            Label lblThongTin = new Label
            {
                Name = "lblThongTinNhanVien",
                Text = "THÔNG TIN NHÂN VIÊN",
                Location = new Point(20, 15),
                Size = new Size(285, 22),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(232, 101, 26),
                AutoSize = false,
                AutoEllipsis = true
            };
            pnlPhai.Controls.Add(lblThongTin);

            txtMaNV = ThemO(pnlPhai, "txtMaNV", "Mã nhân viên", 55, true);
            txtTenNV = ThemO(pnlPhai, "txtTenNV", "Họ tên", 115);
            txtTenNV.MaxLength = 100;
            txtSDT = ThemO(pnlPhai, "txtSDT", "Số điện thoại", 175);
            txtSDT.MaxLength = 15;

            Label lblChucVu = new Label
            {
                Name = "lblChucVu",
                Text = "Chức vụ",
                Location = new Point(20, 235),
                Size = new Size(285, 16),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(107, 114, 128)
            };
            cboChucVu = new ComboBox
            {
                Name = "cboChucVu",
                Location = new Point(20, 255),
                Size = new Size(285, 34),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10.5F),
                BackColor = Color.White
            };
            cboChucVu.Items.AddRange(DsChucVu);
            pnlPhai.Controls.Add(lblChucVu);
            pnlPhai.Controls.Add(cboChucVu);

            txtTenDN = ThemO(pnlPhai, "txtTenDN", "Tên đăng nhập", 295);
            txtTenDN.MaxLength = 30;
            txtMatKhau = ThemO(pnlPhai, "txtMatKhau", "Mật khẩu", 355);
            txtMatKhau.UseSystemPasswordChar = true;
            pnlPhai.Controls.Add(new Label
            {
                Name = "lblGhiChuMatKhau",
                Text = "Khi sửa: để trống = giữ mật khẩu cũ",
                Location = new Point(20, 413),
                Size = new Size(285, 18),
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize = false,
                AutoEllipsis = true
            });

            TableLayoutPanel tlpNut = new TableLayoutPanel
            {
                Name = "tlpNutNhanVien",
                Location = new Point(20, 443),
                Size = new Size(285, 88),
                ColumnCount = 3,
                RowCount = 3,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Color.White
            };
            for (int i = 0; i < 3; i++)
                tlpNut.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333F));
            tlpNut.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tlpNut.RowStyles.Add(new RowStyle(SizeType.Absolute, 8F));
            tlpNut.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            btnThem = ThemNut(tlpNut, "btnThem", "Thêm", 0, 0, 1,
                Color.FromArgb(232, 101, 26), btnThem_Click);
            btnSua = ThemNut(tlpNut, "btnSua", "Sửa", 1, 0, 1,
                Color.FromArgb(37, 99, 235), btnSua_Click);
            btnXoa = ThemNut(tlpNut, "btnXoa", "Xóa", 2, 0, 1,
                Color.FromArgb(220, 38, 38), btnXoa_Click);
            btnLuu = ThemNut(tlpNut, "btnLuu", "Lưu", 0, 2, 2,
                Color.FromArgb(22, 163, 74), btnLuu_Click);
            btnHuy = ThemNut(tlpNut, "btnHuy", "Hủy", 2, 2, 1,
                Color.FromArgb(107, 114, 128), btnHuy_Click);
            pnlPhai.Controls.Add(tlpNut);

            Controls.Add(pnlTrai);
            Controls.Add(pnlPhai);
            Controls.Add(pnlTop);
        }

        private void CauHinhCotBang()
        {
            string[] tenCot = { "MaNV", "TenNV", "SDT", "ChucVu", "TenDN" };
            string[] tieuDe = { "Mã NV", "Họ tên", "Điện thoại", "Chức vụ", "Tên đăng nhập" };
            float[] trongSo = { 14F, 28F, 20F, 18F, 20F };
            for (int i = 0; i < tenCot.Length; i++)
            {
                if (!dgvNhanVien.Columns.Contains(tenCot[i]))
                    continue;
                DataGridViewColumn cot = dgvNhanVien.Columns[tenCot[i]];
                cot.HeaderText = tieuDe[i];
                cot.FillWeight = trongSo[i];
                cot.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            if (dgvNhanVien.Columns.Contains("MatKhau"))
                dgvNhanVien.Columns["MatKhau"].Visible = false;
        }

        private void CapNhatThongKe()
        {
            if (bang == null || lblTongNhanVien == null)
                return;

            int admin = 0;
            int thuNgan = 0;
            int bep = 0;
            foreach (DataRow dong in bang.Rows)
            {
                string chucVu = Convert.ToString(dong["ChucVu"]);
                if (chucVu == "Admin")
                    admin++;
                else if (chucVu == "ThuNgan")
                    thuNgan++;
                else if (chucVu == "Bep")
                    bep++;
            }

            lblTongNhanVien.Text = bang.Rows.Count.ToString();
            lblSoAdmin.Text = admin.ToString();
            lblSoThuNgan.Text = thuNgan.ToString();
            lblSoBep.Text = bep.ToString();
        }

        private void dgvNhanVien_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string tenCot = dgvNhanVien.Columns[e.ColumnIndex].Name;
            if (tenCot == "ChucVu")
            {
                string chucVu = Convert.ToString(e.FormattedValue);
                string hienThi = chucVu;
                Color mauNen = Color.FromArgb(107, 114, 128);
                if (chucVu == "Admin")
                {
                    hienThi = "Admin";
                    mauNen = Color.FromArgb(124, 58, 237);
                }
                else if (chucVu == "ThuNgan")
                {
                    hienThi = "Thu ngân";
                    mauNen = Color.FromArgb(37, 99, 235);
                }
                else if (chucVu == "Bep")
                {
                    hienThi = "Bếp";
                    mauNen = Color.FromArgb(22, 163, 74);
                }

                e.PaintBackground(e.CellBounds, true);
                Rectangle vienThuoc = new Rectangle(
                    e.CellBounds.Left + (e.CellBounds.Width - 80) / 2,
                    e.CellBounds.Top + (e.CellBounds.Height - 22) / 2, 80, 22);
                GraphicsState trangThai = e.Graphics.Save();
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath duongVien = TaoDuongVien(vienThuoc, 22))
                using (SolidBrush brush = new SolidBrush(mauNen))
                    e.Graphics.FillPath(brush, duongVien);
                e.Graphics.Restore(trangThai);
                using (Font font = new Font("Segoe UI", 8.5F, FontStyle.Bold))
                    TextRenderer.DrawText(e.Graphics, hienThi, font, vienThuoc, Color.White,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                        TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
                e.Paint(e.CellBounds, DataGridViewPaintParts.Border);
                e.Handled = true;
                return;
            }

            e.PaintBackground(e.CellBounds, true);
            Rectangle vungChu = e.CellBounds;
            vungChu.X += e.CellStyle.Padding.Left;
            vungChu.Width -= e.CellStyle.Padding.Left + e.CellStyle.Padding.Right;
            TextRenderer.DrawText(e.Graphics, Convert.ToString(e.FormattedValue), e.CellStyle.Font,
                vungChu, e.CellStyle.ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            e.Paint(e.CellBounds, DataGridViewPaintParts.Border);
            e.Handled = true;
        }

        private void dgvNhanVien_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                e.ToolTipText = Convert.ToString(dgvNhanVien.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
        }

        private static GraphicsPath TaoDuongVien(Rectangle hinh, int duongKinh)
        {
            int kichThuoc = Math.Min(duongKinh, Math.Min(hinh.Width, hinh.Height));
            int cung = kichThuoc;
            GraphicsPath duongVien = new GraphicsPath();
            duongVien.AddArc(hinh.Left, hinh.Top, cung, cung, 180, 90);
            duongVien.AddArc(hinh.Right - cung, hinh.Top, cung, cung, 270, 90);
            duongVien.AddArc(hinh.Right - cung, hinh.Bottom - cung, cung, cung, 0, 90);
            duongVien.AddArc(hinh.Left, hinh.Bottom - cung, cung, cung, 90, 90);
            duongVien.CloseFigure();
            return duongVien;
        }

        // ---------- Nạp dữ liệu ----------
        private void frmNhanVien_Load(object sender, EventArgs e)
        {
            // Kiểm tra quyền trong code, không chỉ ẩn menu
            if (PhienDangNhap.ChucVu != "Admin")
            {
                MessageBox.Show("Chỉ Admin mới được quản lý nhân viên.", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }
            try
            {
                LamMoi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải được danh sách nhân viên:\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        // Tạo lại XML từ CSDL rồi đưa lên bảng
        private void LamMoi()
        {
            fx.TaoXml("SELECT MaNV, TenNV, SDT, ChucVu, TenDN, MatKhau FROM NhanVien ORDER BY MaNV",
                TenBang, TenFile);
            bang = fx.HienThi(TenFile);
            dgvNhanVien.DataSource = bang;

            CauHinhCotBang();
            CapNhatThongKe();

            dangThem = false;
            DatCheDo(false);
            HienChiTiet();
        }

        private static string Chuoi(DataGridViewRow dong, string cot)
        {
            return Convert.ToString(dong.Cells[cot].Value);
        }

        private void HienChiTiet()
        {
            DataGridViewRow r = dgvNhanVien.CurrentRow;
            if (r == null)
            {
                XoaO();
                return;
            }
            txtMaNV.Text = Chuoi(r, "MaNV");
            txtTenNV.Text = Chuoi(r, "TenNV");
            txtSDT.Text = Chuoi(r, "SDT");
            txtTenDN.Text = Chuoi(r, "TenDN");
            txtMatKhau.Clear();
            cboChucVu.SelectedItem = Chuoi(r, "ChucVu");
        }

        private void XoaO()
        {
            txtMaNV.Clear();
            txtTenNV.Clear();
            txtSDT.Clear();
            txtTenDN.Clear();
            txtMatKhau.Clear();
            cboChucVu.SelectedIndex = -1;
        }

        // nhapLieu = true: đang thêm/sửa
        private void DatCheDo(bool nhapLieu)
        {
            txtTenNV.Enabled = nhapLieu;
            txtSDT.Enabled = nhapLieu;
            cboChucVu.Enabled = nhapLieu;
            txtMatKhau.Enabled = nhapLieu;
            txtTenDN.Enabled = nhapLieu && dangThem; // đổi tên đăng nhập sẽ làm sai mã băm mật khẩu
            btnLuu.Enabled = nhapLieu;
            btnHuy.Enabled = nhapLieu;
            btnThem.Enabled = !nhapLieu;
            btnSua.Enabled = !nhapLieu;
            btnXoa.Enabled = !nhapLieu;
            dgvNhanVien.Enabled = !nhapLieu;
        }

        private void dgvNhanVien_SelectionChanged(object sender, EventArgs e)
        {
            if (!btnLuu.Enabled)
            {
                HienChiTiet();
            }
        }

        // ---------- Kiểm tra hợp lệ ----------
        private static bool TatCaLaSo(string s)
        {
            foreach (char c in s)
            {
                if (c < '0' || c > '9') return false;
            }
            return true;
        }

        private static bool ChiChuSoKhongDau(string s)
        {
            foreach (char c in s)
            {
                if (c > 127 || !char.IsLetterOrDigit(c)) return false;
            }
            return true;
        }

        private string KiemTra(string ten, string sdt, string tenDN, string mk)
        {
            if (ten.Length == 0) return "Vui lòng nhập họ tên nhân viên.";
            if (sdt.Length > 0 && (sdt.Length < 9 || !TatCaLaSo(sdt)))
                return "Số điện thoại phải gồm 9 đến 15 chữ số.";
            if (cboChucVu.SelectedItem == null) return "Vui lòng chọn chức vụ.";
            if (dangThem)
            {
                if (tenDN.Length < 3) return "Tên đăng nhập phải từ 3 ký tự.";
                if (!ChiChuSoKhongDau(tenDN)) return "Tên đăng nhập chỉ gồm chữ không dấu và số, không có khoảng trắng.";
                if (fx.TonTai(TenFile, "TenDN", tenDN)) return "Tên đăng nhập này đã tồn tại.";
                if (mk.Length < 6) return "Mật khẩu phải từ 6 ký tự.";
            }
            else if (mk.Length > 0 && mk.Length < 6)
            {
                return "Mật khẩu mới phải từ 6 ký tự.";
            }
            return null;
        }

        // ---------- Các nút ----------
        private void btnThem_Click(object sender, EventArgs e)
        {
            dangThem = true;
            XoaO();
            txtMaNV.Text = fx.TaoMa("NV", TenFile, "MaNV");
            cboChucVu.SelectedItem = "ThuNgan";
            DatCheDo(true);
            txtTenNV.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (txtMaNV.Text.Length == 0)
            {
                MessageBox.Show("Hãy chọn một nhân viên trong bảng.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            dangThem = false;
            DatCheDo(true);
            txtTenNV.Focus();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            dangThem = false;
            DatCheDo(false);
            HienChiTiet();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            string ma = txtMaNV.Text;
            string ten = txtTenNV.Text.Trim();
            string sdt = txtSDT.Text.Trim();
            string tenDN = txtTenDN.Text.Trim();
            string mk = txtMatKhau.Text;

            string loi = KiemTra(ten, sdt, tenDN, mk);
            if (loi != null)
            {
                MessageBox.Show(loi, "Dữ liệu chưa hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string chucVu = cboChucVu.SelectedItem.ToString();
            if (!dangThem && ma == PhienDangNhap.MaNV && chucVu != "Admin")
            {
                MessageBox.Show("Không thể tự hạ quyền của tài khoản đang đăng nhập.", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (dangThem)
                {
                    Dictionary<string, string> moi = new Dictionary<string, string>
                    {
                        { "MaNV", ma }, { "TenNV", ten }, { "SDT", sdt },
                        { "ChucVu", chucVu }, { "TenDN", tenDN },
                        { "MatKhau", TaiKhoan.BamMatKhau(tenDN, mk) }
                    };
                    fx.Them(TenFile, TenBang, moi);
                }
                else
                {
                    Dictionary<string, string> khoa = new Dictionary<string, string> { { "MaNV", ma } };
                    Dictionary<string, string> doi = new Dictionary<string, string>
                    {
                        { "TenNV", ten }, { "SDT", sdt }, { "ChucVu", chucVu }
                    };
                    if (mk.Length > 0)
                    {
                        doi["MatKhau"] = TaiKhoan.BamMatKhau(tenDN, mk);
                    }
                    fx.Sua(TenFile, khoa, doi);
                }
                fx.CapNhatDb(TenBang, TenFile);
                LamMoi();
                MessageBox.Show("Đã lưu nhân viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không lưu được:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LamMoi(); // đồng bộ lại XML theo CSDL
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string ma = txtMaNV.Text;
            if (ma.Length == 0)
            {
                MessageBox.Show("Hãy chọn một nhân viên trong bảng.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (ma == PhienDangNhap.MaNV)
            {
                MessageBox.Show("Không thể xóa tài khoản đang đăng nhập.", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("Xóa nhân viên " + txtTenNV.Text + " (" + ma + ")?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                fx.Xoa(TenFile, new Dictionary<string, string> { { "MaNV", ma } });
                fx.CapNhatDb(TenBang, TenFile);
                LamMoi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không xóa được (nhân viên có thể đã lập hóa đơn hoặc phiếu nhập).\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LamMoi();
            }
        }

        private static string ThoatLike(string s)
        {
            return s.Replace("'", "''").Replace("[", "[[]").Replace("%", "[%]").Replace("*", "[*]");
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string tu = txtTim.Text.Trim();
            if (tu.Length == 0)
            {
                bang.DefaultView.RowFilter = "";
                return;
            }
            string t = ThoatLike(tu);
            bang.DefaultView.RowFilter = "TenNV LIKE '%" + t + "%' OR TenDN LIKE '%" + t + "%' OR MaNV LIKE '%" + t + "%'";
        }

        private sealed class BorderPanel : Panel
        {
            public BorderPanel()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw, true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                using (Pen vien = new Pen(Color.FromArgb(229, 231, 235)))
                    e.Graphics.DrawRectangle(vien, 0, 0, Width - 1, Height - 1);
            }
        }
    }
}