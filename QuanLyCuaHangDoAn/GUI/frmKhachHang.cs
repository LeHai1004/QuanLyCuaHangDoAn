using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using QuanLyCuaHangDoAn.Class;

namespace QuanLyCuaHangDoAn.GUI
{
    public partial class frmKhachHang : Form
    {
        private const string TenFile = "KhachHang.xml";
        private const string TenBang = "KhachHang";

        private readonly FileXml fx = new FileXml();
        private readonly DataGridView dgvKhachHang = new DataGridView();
        private readonly TextBox txtMaKH = new TextBox();
        private readonly TextBox txtTenKH = new TextBox();
        private readonly TextBox txtSDT = new TextBox();
        private readonly TextBox txtDiemTichLuy = new TextBox();
        private readonly ComboBox cboHang = new ComboBox();
        private readonly TextBox txtTim = new TextBox();

        private readonly Button btnThem = GiaoDien.TaoNut("Thêm", GiaoDien.MauChinh);
        private readonly Button btnSua = GiaoDien.TaoNut("Sửa", GiaoDien.MauChinh);
        private readonly Button btnXoa = GiaoDien.TaoNut("Xóa", GiaoDien.MauDo);
        private readonly Button btnLuu = GiaoDien.TaoNut("Lưu", GiaoDien.MauXanh);
        private readonly Button btnHuy = GiaoDien.TaoNut("Hủy", GiaoDien.MauChinhDam);
        private readonly Button btnTim = GiaoDien.TaoNut("Tìm", GiaoDien.MauChinh);
        private readonly Label[] lblSoLuongHang = new Label[4];
        private readonly Label lblTenKhach = new Label();
        private readonly RankBadge lblHangKhach = new RankBadge();
        private readonly AvatarKhachHang avatarKhach = new AvatarKhachHang();
        private readonly ProgressHang progressHang = new ProgressHang();
        private readonly Label lblTienDoHang = new Label();
        private Panel pnlThongTin;
        private Panel pnlDanhSach;
        private ToolTip toolTip;

        private DataTable duLieu;
        private string cheDo = "";
        private bool choDongBo;
        private string hanhDongChoDongBo;

        public frmKhachHang()
        {
            InitializeComponent();
            KhoiTaoGiaoDien();
            Load += frmKhachHang_Load;
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Quản lý khách hàng";
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 246, 248);
            Font = new Font("Segoe UI", 10F);
            ClientSize = new Size(1100, 680);
            MinimumSize = new Size(1116, 719);
            DoubleBuffered = true;
            toolTip = new ToolTip(components);

            dgvKhachHang.Name = "dgvKhachHang";
            txtMaKH.Name = "txtMaKH";
            txtTenKH.Name = "txtTenKH";
            txtSDT.Name = "txtSDT";
            txtDiemTichLuy.Name = "txtDiemTichLuy";
            cboHang.Name = "cboHang";
            txtTim.Name = "txtTim";
            btnThem.Name = "btnThem";
            btnSua.Name = "btnSua";
            btnXoa.Name = "btnXoa";
            btnLuu.Name = "btnLuu";
            btnHuy.Name = "btnHuy";
            btnTim.Name = "btnTim";

            CauHinhBang();
            CauHinhNut(btnThem, Color.FromArgb(232, 101, 26), Color.White);
            CauHinhNut(btnSua, Color.FromArgb(37, 99, 235), Color.White);
            CauHinhNut(btnXoa, Color.FromArgb(220, 38, 38), Color.White);
            CauHinhNut(btnLuu, Color.FromArgb(22, 163, 74), Color.White);
            CauHinhNut(btnHuy, Color.FromArgb(107, 114, 128), Color.White);
            CauHinhNut(btnTim, Color.FromArgb(232, 101, 26), Color.White);
            CauHinhTruong();

            var pnlHeader = new Panel
            {
                Name = "pnlHeader",
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(232, 101, 26)
            };
            pnlHeader.Controls.Add(new Label
            {
                Name = "lblTieuDe",
                Text = "QUẢN LÝ KHÁCH HÀNG",
                Location = new Point(30, 12),
                Size = new Size(650, 32),
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            });
            pnlHeader.Controls.Add(new Label
            {
                Name = "lblMoTa",
                Text = "Tích điểm và hạng thành viên",
                Location = new Point(32, 50),
                Size = new Size(600, 20),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            });

            pnlDanhSach = TaoThe("pnlDanhSach", new Point(30, 100), new Size(690, 550));
            pnlDanhSach.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            TaoKhuVucDanhSach();

            pnlThongTin = TaoThe("pnlThongTin", new Point(745, 100), new Size(325, 550));
            pnlThongTin.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            TaoKhuVucThongTin();

            Controls.Add(pnlDanhSach);
            Controls.Add(pnlThongTin);
            Controls.Add(pnlHeader);
            pnlHeader.BringToFront();

            txtTenKH.MaxLength = 100;
            txtSDT.MaxLength = 15;
            txtDiemTichLuy.MaxLength = 10;
            txtDiemTichLuy.TextChanged += txtDiemTichLuy_TextChanged;
            txtTenKH.TextChanged += txtTenKH_TextChanged_UI;
            cboHang.SelectedIndexChanged += cboHang_SelectedIndexChanged_UI;
            cboHang.DropDownStyle = ComboBoxStyle.DropDownList;
            cboHang.Items.AddRange(new object[] { "Thường", "Bạc", "Vàng" });
            cboHang.SelectedIndex = 0;
            dgvKhachHang.SelectionChanged += dgvKhachHang_SelectionChanged;
            dgvKhachHang.CellPainting += dgvKhachHang_CellPainting;

            btnThem.Click += btnThem_Click;
            btnSua.Click += btnSua_Click;
            btnXoa.Click += btnXoa_Click;
            btnLuu.Click += btnLuu_Click;
            btnHuy.Click += btnHuy_Click;
            btnTim.Click += btnTim_Click;
            txtTim.KeyDown += txtTim_KeyDown;
            CapNhatTrangThaiNut();
        }

        private void TaoKhuVucDanhSach()
        {
            TaoThongKe();

            pnlDanhSach.Controls.Add(TaoNhan("Tìm theo SĐT", "lblTim", new Point(20, 84),
                new Size(650, 18), 9F, false, Color.FromArgb(107, 114, 128)));
            txtTim.Location = new Point(20, 105);
            txtTim.Size = new Size(545, 36);
            txtTim.BorderStyle = BorderStyle.FixedSingle;
            txtTim.Font = new Font("Segoe UI", 11F);
            txtTim.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnTim.Location = new Point(575, 105);
            btnTim.Size = new Size(95, 36);
            btnTim.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pnlDanhSach.Controls.Add(txtTim);
            pnlDanhSach.Controls.Add(btnTim);

            dgvKhachHang.Location = new Point(20, 145);
            dgvKhachHang.Size = new Size(650, 390);
            dgvKhachHang.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            pnlDanhSach.Controls.Add(dgvKhachHang);
        }

        private void TaoThongKe()
        {
            string[] nhan = { "Tổng khách", "Thường", "Bạc", "Vàng" };
            Color[] mau = {
                Color.FromArgb(17, 24, 39),
                Color.FromArgb(107, 114, 128),
                Color.FromArgb(100, 116, 139),
                Color.FromArgb(217, 119, 6)
            };
            for (int i = 0; i < nhan.Length; i++)
            {
                var the = new MetricCustomerCard(nhan[i], mau[i])
                {
                    Name = "pnlThongKe" + i,
                    Location = new Point(20 + i * 166, 20),
                    Size = new Size(152, 60),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left
                };
                lblSoLuongHang[i] = the.GiaTriLabel;
                pnlDanhSach.Controls.Add(the);
            }
        }

        private void TaoKhuVucThongTin()
        {
            pnlThongTin.Controls.Add(TaoNhan("THÔNG TIN KHÁCH", "lblTieuDeThongTin",
                new Point(20, 15), new Size(285, 24), 12F, true, Color.FromArgb(232, 101, 26)));

            avatarKhach.Name = "avatarKhach";
            avatarKhach.Location = new Point(20, 50);
            avatarKhach.Size = new Size(52, 52);
            lblTenKhach.Name = "lblTenKhach";
            lblTenKhach.Location = new Point(80, 51);
            lblTenKhach.Size = new Size(205, 24);
            lblTenKhach.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTenKhach.ForeColor = Color.FromArgb(17, 24, 39);
            lblTenKhach.AutoEllipsis = true;
            lblTenKhach.Text = "Chưa chọn khách";
            lblHangKhach.Name = "lblHangKhach";
            lblHangKhach.Location = new Point(80, 80);
            lblHangKhach.Size = new Size(70, 22);
            lblHangKhach.Text = "";
            lblHangKhach.Visible = false;
            pnlThongTin.Controls.Add(avatarKhach);
            pnlThongTin.Controls.Add(lblTenKhach);
            pnlThongTin.Controls.Add(lblHangKhach);

            lblTienDoHang.Name = "lblTienDoHang";
            lblTienDoHang.Location = new Point(20, 115);
            lblTienDoHang.Size = new Size(285, 18);
            lblTienDoHang.Font = new Font("Segoe UI", 9F);
            lblTienDoHang.ForeColor = Color.FromArgb(107, 114, 128);
            lblTienDoHang.AutoEllipsis = true;
            progressHang.Name = "progressHang";
            progressHang.Location = new Point(20, 137);
            progressHang.Size = new Size(285, 8);
            pnlThongTin.Controls.Add(lblTienDoHang);
            pnlThongTin.Controls.Add(progressHang);

            pnlThongTin.Controls.Add(TaoNhan("Mã khách hàng", "lblMaKH", new Point(20, 165),
                new Size(120, 18), 9F, false, Color.FromArgb(107, 114, 128)));
            pnlThongTin.Controls.Add(TaoNhan("Điểm tích lũy", "lblDiemTichLuy", new Point(170, 165),
                new Size(135, 18), 9F, false, Color.FromArgb(107, 114, 128)));
            TaoTextBox(txtMaKH, new Point(20, 187), new Size(135, 34), true);
            TaoTextBox(txtDiemTichLuy, new Point(170, 187), new Size(135, 34), true);

            ThemTruongThongTin("Tên khách hàng (*)", "lblTenKH", txtTenKH, 225, 285, false);
            ThemTruongThongTin("Số điện thoại (*)", "lblSDT", txtSDT, 285, 285, false);
            pnlThongTin.Controls.Add(TaoNhan("Hạng thành viên", "lblHang", new Point(20, 345),
                new Size(285, 18), 9F, false, Color.FromArgb(107, 114, 128)));
            cboHang.Location = new Point(20, 367);
            cboHang.Size = new Size(285, 34);
            cboHang.Font = new Font("Segoe UI", 10.5F);
            cboHang.FlatStyle = FlatStyle.Flat;
            pnlThongTin.Controls.Add(cboHang);

            TaoLuoiNut();
            CapNhatTheThongTin(null);
        }

        private void ThemTruongThongTin(string nhan, string tenNhan, Control control, int y, int rong, bool chiDoc)
        {
            pnlThongTin.Controls.Add(TaoNhan(nhan, tenNhan, new Point(20, y),
                new Size(rong, 18), 9F, false, Color.FromArgb(107, 114, 128)));
            TaoTextBox(control, new Point(20, y + 22), new Size(rong, 34), chiDoc);
        }

        private static void TaoTextBox(Control control, Point viTri, Size kichThuoc, bool chiDoc)
        {
            control.Location = viTri;
            control.Size = kichThuoc;
            control.Font = new Font("Segoe UI", 10.5F);
            if (control is TextBox)
            {
                TextBox textBox = (TextBox)control;
                textBox.BorderStyle = BorderStyle.FixedSingle;
                textBox.ReadOnly = chiDoc;
            }
            control.BackColor = chiDoc ? Color.FromArgb(243, 244, 246) : Color.White;
        }

        private void TaoLuoiNut()
        {
            var luoi = new TableLayoutPanel
            {
                Name = "tlpNut",
                Location = new Point(20, 410),
                Size = new Size(285, 88),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ColumnCount = 3,
                RowCount = 2,
                BackColor = Color.White
            };
            for (int i = 0; i < 3; i++)
                luoi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            luoi.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            luoi.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            foreach (Button nut in new[] { btnThem, btnSua, btnXoa, btnLuu, btnHuy })
            {
                nut.Dock = DockStyle.Fill;
                nut.Margin = new Padding(4);
            }
            luoi.Controls.Add(btnThem, 0, 0);
            luoi.Controls.Add(btnSua, 1, 0);
            luoi.Controls.Add(btnXoa, 2, 0);
            luoi.Controls.Add(btnLuu, 0, 1);
            luoi.SetColumnSpan(btnLuu, 2);
            luoi.Controls.Add(btnHuy, 2, 1);
            pnlThongTin.Controls.Add(luoi);
        }

        private void CauHinhBang()
        {
            dgvKhachHang.ReadOnly = true;
            dgvKhachHang.AllowUserToAddRows = false;
            dgvKhachHang.AllowUserToDeleteRows = false;
            dgvKhachHang.AllowUserToResizeRows = false;
            dgvKhachHang.RowHeadersVisible = false;
            dgvKhachHang.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvKhachHang.MultiSelect = false;
            dgvKhachHang.AutoGenerateColumns = false;
            dgvKhachHang.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvKhachHang.BorderStyle = BorderStyle.None;
            dgvKhachHang.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvKhachHang.BackgroundColor = Color.White;
            dgvKhachHang.GridColor = Color.FromArgb(243, 244, 246);
            dgvKhachHang.EnableHeadersVisualStyles = false;
            dgvKhachHang.ColumnHeadersHeight = 38;
            dgvKhachHang.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvKhachHang.RowTemplate.Height = 44;
            dgvKhachHang.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 243, 234);
            dgvKhachHang.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(154, 52, 18);
            dgvKhachHang.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvKhachHang.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgvKhachHang.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 243, 234);
            dgvKhachHang.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(154, 52, 18);
            dgvKhachHang.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvKhachHang.DefaultCellStyle.ForeColor = Color.FromArgb(17, 24, 39);
            dgvKhachHang.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 243, 234);
            dgvKhachHang.DefaultCellStyle.SelectionForeColor = Color.FromArgb(17, 24, 39);
            dgvKhachHang.DefaultCellStyle.Padding = new Padding(8, 0, 4, 0);
            dgvKhachHang.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            dgvKhachHang.Columns.Clear();
            dgvKhachHang.Columns.Add(TaoCot("MaKH", "Mã KH", 15F));
            dgvKhachHang.Columns.Add(TaoCot("TenKH", "Tên khách hàng", 35F));
            dgvKhachHang.Columns.Add(TaoCot("SDT", "Số điện thoại", 22F));
            DataGridViewTextBoxColumn cotDiem = TaoCot("DiemTichLuy", "Điểm", 13F);
            cotDiem.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            cotDiem.DefaultCellStyle.Format = "#,##0";
            dgvKhachHang.Columns.Add(cotDiem);
            DataGridViewTextBoxColumn cotHang = TaoCot("Hang", "Hạng", 15F);
            cotHang.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvKhachHang.Columns.Add(cotHang);
        }

        private static DataGridViewTextBoxColumn TaoCot(string ten, string tieuDe, float tiLe)
        {
            return new DataGridViewTextBoxColumn
            {
                Name = ten,
                DataPropertyName = ten,
                HeaderText = tieuDe,
                FillWeight = tiLe,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
        }

        private static Panel TaoThe(string ten, Point viTri, Size kichThuoc)
        {
            var the = new Panel
            {
                Name = ten,
                Location = viTri,
                Size = kichThuoc,
                BackColor = Color.White
            };
            the.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(229, 231, 235)))
                    e.Graphics.DrawRectangle(pen, 0, 0, the.Width - 1, the.Height - 1);
            };
            return the;
        }

        private static Label TaoNhan(string noiDung, string ten, Point viTri, Size kichThuoc,
            float coChu, bool dam, Color mau)
        {
            return new Label
            {
                Name = ten,
                Text = noiDung,
                Location = viTri,
                Size = kichThuoc,
                Font = new Font("Segoe UI", coChu, dam ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = mau,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };
        }

        private static void CauHinhNut(Button nut, Color nen, Color chu)
        {
            Color hover = Color.FromArgb(
                Math.Max(0, (int)(nen.R * 0.9)),
                Math.Max(0, (int)(nen.G * 0.9)),
                Math.Max(0, (int)(nen.B * 0.9)));
            nut.FlatStyle = FlatStyle.Flat;
            nut.FlatAppearance.BorderSize = 0;
            nut.FlatAppearance.MouseOverBackColor = hover;
            nut.FlatAppearance.MouseDownBackColor = hover;
            nut.BackColor = nen;
            nut.ForeColor = chu;
            nut.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            nut.Cursor = Cursors.Hand;
        }

        private void CauHinhTruong()
        {
            foreach (TextBox textBox in new[] { txtMaKH, txtTenKH, txtSDT, txtDiemTichLuy })
                textBox.BorderStyle = BorderStyle.FixedSingle;
            txtMaKH.ReadOnly = true;
            txtDiemTichLuy.ReadOnly = true;
        }

        private void frmKhachHang_Load(object sender, EventArgs e)
        {
            try
            {
                if (PhienDangNhap.IsLoggedIn
                    && PhienDangNhap.ChucVu != "Admin"
                    && PhienDangNhap.ChucVu != "ThuNgan")
                {
                    MessageBox.Show("Bạn không có quyền truy cập chức năng này.", "Từ chối",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Close();
                    return;
                }

                TaiDuLieu();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể tải danh sách khách hàng", ex);
            }
        }

        private void TaiDuLieu()
        {
            fx.TaoXml("SELECT * FROM KhachHang", TenBang, TenFile);
            duLieu = fx.HienThi(TenFile);
            HienThiDuLieu(duLieu);
        }

        private void HienThiDuLieu(DataTable nguon)
        {
            dgvKhachHang.DataSource = nguon != null ? nguon.Copy() : null;
            CapNhatThongKe();
            CapNhatTrangThaiNut();
        }

        private void dgvKhachHang_SelectionChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cheDo) || choDongBo)
                return;

            if (dgvKhachHang.CurrentRow == null)
            {
                txtMaKH.Clear();
                txtTenKH.Clear();
                txtSDT.Clear();
                txtDiemTichLuy.Clear();
                cboHang.SelectedIndex = 0;
                CapNhatTheThongTin(null);
                CapNhatTrangThaiNut();
                return;
            }

            txtMaKH.Text = Convert.ToString(dgvKhachHang.CurrentRow.Cells["MaKH"].Value);
            txtTenKH.Text = Convert.ToString(dgvKhachHang.CurrentRow.Cells["TenKH"].Value);
            txtSDT.Text = Convert.ToString(dgvKhachHang.CurrentRow.Cells["SDT"].Value);
            txtDiemTichLuy.Text = Convert.ToString(dgvKhachHang.CurrentRow.Cells["DiemTichLuy"].Value);
            string hang = Convert.ToString(dgvKhachHang.CurrentRow.Cells["Hang"].Value);
            if (!string.IsNullOrEmpty(hang) && cboHang.Items.Contains(hang))
                cboHang.SelectedItem = hang;
            else
                cboHang.SelectedIndex = 0;
            CapNhatTheThongTin(hang);
            CapNhatTrangThaiNut();
        }

        private void dgvKhachHang_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || dgvKhachHang.Columns[e.ColumnIndex].Name != "Hang")
                return;

            e.PaintBackground(e.CellBounds, true);
            string hang = Convert.ToString(e.FormattedValue);
            Color mau = MauTheoHang(hang);
            int rong = 70;
            int cao = 22;
            var hinh = new Rectangle(
                e.CellBounds.X + (e.CellBounds.Width - rong) / 2,
                e.CellBounds.Y + (e.CellBounds.Height - cao) / 2,
                rong, cao);
            using (var path = TaoDuongVienBoGoc(hinh, 11))
            using (var brush = new SolidBrush(mau))
            using (var font = new Font("Segoe UI", 8.5F, FontStyle.Bold))
            using (var brushChu = new SolidBrush(Color.White))
            using (var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawString(hang, font, brushChu, hinh, format);
            }
            e.Handled = true;
        }

        private void CapNhatThongKe()
        {
            int[] dem = new int[4];
            if (duLieu != null)
            {
                foreach (DataRow dong in duLieu.Rows)
                {
                    dem[0]++;
                    string hang = Convert.ToString(dong["Hang"]);
                    if (hang == "Thường")
                        dem[1]++;
                    else if (hang == "Bạc")
                        dem[2]++;
                    else if (hang == "Vàng")
                        dem[3]++;
                }
            }

            for (int i = 0; i < lblSoLuongHang.Length; i++)
            {
                bool khongCoDuLieu = dem[0] == 0;
                lblSoLuongHang[i].Text = khongCoDuLieu ? "Chưa có dữ liệu" : dem[i].ToString();
                lblSoLuongHang[i].ForeColor = khongCoDuLieu
                    ? Color.FromArgb(107, 114, 128)
                    : ((MetricCustomerCard)lblSoLuongHang[i].Parent).MauSo;
                lblSoLuongHang[i].Font = new Font("Segoe UI", khongCoDuLieu ? 8.5F : 18F, FontStyle.Bold);
            }
        }

        private void CapNhatTheThongTin(string hang)
        {
            string ten = txtTenKH.Text.Trim();
            bool coKhach = !string.IsNullOrWhiteSpace(txtMaKH.Text);
            if (!coKhach)
            {
                lblTenKhach.Text = "Chưa chọn khách";
                lblHangKhach.Text = "";
                lblHangKhach.Visible = false;
                lblTienDoHang.Text = "";
                lblTienDoHang.Visible = false;
                progressHang.Visible = false;
                avatarKhach.DatDuLieu("", null);
                return;
            }

            if (string.IsNullOrWhiteSpace(hang))
                hang = cboHang.SelectedItem == null ? "Thường" : cboHang.SelectedItem.ToString();
            if (string.IsNullOrWhiteSpace(ten))
                ten = "Khách hàng";

            lblTenKhach.Text = ten;
            lblHangKhach.Text = hang;
            lblHangKhach.Visible = true;
            avatarKhach.DatDuLieu(ten, hang);
            progressHang.Visible = true;
            lblTienDoHang.Visible = true;

            int diem;
            int.TryParse(txtDiemTichLuy.Text.Trim(), out diem);
            diem = Math.Max(0, diem);
            if (hang == "Vàng")
            {
                progressHang.TiLe = 1F;
                lblTienDoHang.Text = "Hạng cao nhất";
            }
            else if (hang == "Bạc")
            {
                progressHang.TiLe = Math.Min(1F, Math.Max(0F, (diem - 100) / 200F));
                lblTienDoHang.Text = "Còn " + Math.Max(0, 300 - diem) + " điểm để lên hạng Vàng";
            }
            else
            {
                progressHang.TiLe = Math.Min(1F, Math.Max(0F, diem / 100F));
                lblTienDoHang.Text = "Còn " + Math.Max(0, 100 - diem) + " điểm để lên hạng Bạc";
            }
        }

        private void txtDiemTichLuy_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cheDo))
                return;

            if (int.TryParse(txtDiemTichLuy.Text.Trim(), out int diem) && diem >= 0)
            {
                if (diem >= 300)
                    cboHang.SelectedItem = "Vàng";
                else if (diem >= 100)
                    cboHang.SelectedItem = "Bạc";
                else
                    cboHang.SelectedItem = "Thường";
            }
        }

        private void txtTenKH_TextChanged_UI(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cheDo))
                CapNhatTheThongTin(cboHang.SelectedItem == null ? "Thường" : cboHang.SelectedItem.ToString());
        }

        private void cboHang_SelectedIndexChanged_UI(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cheDo))
                CapNhatTheThongTin(cboHang.SelectedItem == null ? "Thường" : cboHang.SelectedItem.ToString());
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (choDongBo)
                return;

            try
            {
                fx.TaoXml("SELECT * FROM KhachHang", TenBang, TenFile);
                txtMaKH.Text = fx.TaoMa("KH", TenFile, "MaKH");
                txtTenKH.Clear();
                txtSDT.Clear();
                txtDiemTichLuy.Text = "0";
                cboHang.SelectedItem = "Thường";
                cheDo = "them";
                CapNhatTheThongTin("Thường");
                CapNhatTrangThaiNut();
                txtTenKH.Focus();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể tạo mã khách hàng mới", ex);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (choDongBo || dgvKhachHang.CurrentRow == null)
                return;

            cheDo = "sua";
            CapNhatTrangThaiNut();
            txtTenKH.Focus();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (choDongBo || dgvKhachHang.CurrentRow == null)
            {
                if (dgvKhachHang.CurrentRow == null)
                    MessageBox.Show("Vui lòng chọn khách hàng cần xóa.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string maKH = Convert.ToString(dgvKhachHang.CurrentRow.Cells["MaKH"].Value);
            string tenKH = Convert.ToString(dgvKhachHang.CurrentRow.Cells["TenKH"].Value);

            try
            {
                // Kiểm tra xem khách hàng đã có hóa đơn nào chưa (chống lỗi khóa ngoại)
                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM HoaDon WHERE MaKH = @MaKH", conn))
                {
                    cmd.Parameters.Add("@MaKH", SqlDbType.NVarChar, 10).Value = maKH;
                    conn.Open();
                    int soHoaDon = Convert.ToInt32(cmd.ExecuteScalar());
                    if (soHoaDon > 0)
                    {
                        MessageBox.Show(
                            "Không thể xóa khách hàng " + maKH + " (" + tenKH + ") vì đã phát sinh " + soHoaDon + " hóa đơn.\n" +
                            "Vui lòng giữ lại để bảo đảm toàn vẹn dữ liệu bán hàng.",
                            "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                if (MessageBox.Show(
                    "Bạn có chắc muốn xóa khách hàng " + maKH + " (" + tenKH + ") không?",
                    "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                if (fx.Xoa(TenFile, new Dictionary<string, string> { { "MaKH", maKH } }) == 0)
                    throw new InvalidOperationException("Không tìm thấy khách hàng trong file XML.");

                choDongBo = true;
                hanhDongChoDongBo = "xóa khách hàng";
                CapNhatTrangThaiNut();
                ThuDongBo();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể xóa khách hàng", ex);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (choDongBo)
            {
                ThuDongBo();
                return;
            }

            string tenKH = txtTenKH.Text.Trim();
            if (string.IsNullOrEmpty(tenKH))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenKH.Focus();
                return;
            }
            if (tenKH.Length > 100)
            {
                MessageBox.Show("Tên khách hàng không được vượt quá 100 ký tự.", "Thông tin không hợp lệ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenKH.Focus();
                return;
            }

            string sdt = txtSDT.Text.Trim();
            if (string.IsNullOrEmpty(sdt))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại khách hàng.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSDT.Focus();
                return;
            }
            if (sdt.Length > 15)
            {
                MessageBox.Show("Số điện thoại không được vượt quá 15 ký tự.", "Thông tin không hợp lệ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSDT.Focus();
                return;
            }

            int diem = 0;
            if (!string.IsNullOrWhiteSpace(txtDiemTichLuy.Text))
            {
                if (!int.TryParse(txtDiemTichLuy.Text.Trim(), out diem) || diem < 0)
                {
                    MessageBox.Show("Điểm tích lũy phải là số nguyên không âm (>= 0).", "Thông tin không hợp lệ",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDiemTichLuy.Focus();
                    return;
                }
            }

            string hang = cboHang.SelectedItem != null ? cboHang.SelectedItem.ToString() : "Thường";

            string maHienTai = txtMaKH.Text.Trim();
            if (KiemTraTrungSDT(sdt, cheDo == "sua" ? maHienTai : null))
            {
                MessageBox.Show(
                    "Số điện thoại \"" + sdt + "\" đã được đăng ký cho khách hàng khác.\n" +
                    "Vui lòng kiểm tra lại số điện thoại.",
                    "Trùng số điện thoại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSDT.Focus();
                return;
            }

            try
            {
                if (cheDo == "them")
                {
                    if (fx.TonTai(TenFile, "MaKH", maHienTai))
                        throw new InvalidOperationException("Mã khách hàng đã tồn tại. Hãy hủy thao tác và tạo lại mã mới.");

                    string[] thuTuCot = { "MaKH", "TenKH", "SDT", "DiemTichLuy", "Hang" };
                    var giaTri = new SortedDictionary<string, string>(Comparer<string>.Create((a, b) =>
                        Array.IndexOf(thuTuCot, a).CompareTo(Array.IndexOf(thuTuCot, b))));

                    giaTri.Add("MaKH", maHienTai);
                    giaTri.Add("TenKH", tenKH);
                    giaTri.Add("SDT", sdt);
                    giaTri.Add("DiemTichLuy", diem.ToString());
                    giaTri.Add("Hang", hang);

                    fx.Them(TenFile, TenBang, giaTri);
                    hanhDongChoDongBo = "thêm khách hàng";
                }
                else if (cheDo == "sua")
                {
                    var khoa = new Dictionary<string, string> { { "MaKH", maHienTai } };
                    var giaTriMoi = new Dictionary<string, string>
                    {
                        { "TenKH", tenKH },
                        { "SDT", sdt },
                        { "DiemTichLuy", diem.ToString() },
                        { "Hang", hang }
                    };

                    if (fx.Sua(TenFile, khoa, giaTriMoi) == 0)
                        throw new InvalidOperationException("Không tìm thấy khách hàng cần sửa trong file XML.");

                    hanhDongChoDongBo = "sửa khách hàng";
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn Thêm hoặc Sửa trước khi lưu.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                choDongBo = true;
                CapNhatTrangThaiNut();
                ThuDongBo();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể lưu khách hàng", ex);
            }
        }

        private bool KiemTraTrungSDT(string sdt, string maKHLoaiTru = null)
        {
            try
            {
                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                {
                    string sql = "SELECT COUNT(*) FROM KhachHang WHERE SDT = @SDT";
                    if (!string.IsNullOrEmpty(maKHLoaiTru))
                        sql += " AND MaKH <> @MaKH";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@SDT", SqlDbType.VarChar, 15).Value = sdt;
                        if (!string.IsNullOrEmpty(maKHLoaiTru))
                            cmd.Parameters.Add("@MaKH", SqlDbType.NVarChar, 10).Value = maKHLoaiTru;

                        conn.Open();
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count > 0)
                            return true;
                    }
                }
            }
            catch
            {
                // Nếu DB không phản hồi, đối chiếu trong file XML
                try
                {
                    string p = FileXml.DuongDan(TenFile);
                    if (File.Exists(p))
                    {
                        XDocument doc = XDocument.Load(p);
                        return doc.Root.Elements().Any(r =>
                            string.Equals((string)r.Element("SDT"), sdt, StringComparison.OrdinalIgnoreCase)
                            && (string.IsNullOrEmpty(maKHLoaiTru) || !string.Equals((string)r.Element("MaKH"), maKHLoaiTru, StringComparison.OrdinalIgnoreCase)));
                    }
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        private void ThuDongBo()
        {
            try
            {
                fx.CapNhatDb(TenBang, TenFile);
            }
            catch (Exception ex)
            {
                CapNhatTrangThaiNut();
                MessageBox.Show(
                    "Dữ liệu XML đã lưu nhưng chưa đồng bộ được sang CSDL SQL Server.\n" +
                    "Nhấn \"Đồng bộ\" để thử lại.\r\n\r\nChi tiết: " + ex.Message,
                    "Lỗi đồng bộ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            choDongBo = false;
            hanhDongChoDongBo = null;
            cheDo = "";
            try
            {
                TaiDuLieu();
            }
            catch (Exception ex)
            {
                HienLoi("Khách hàng đã đồng bộ nhưng không thể tải lại danh sách", ex);
            }
            CapNhatTrangThaiNut();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            if (choDongBo)
                return;

            cheDo = "";
            txtMaKH.Clear();
            txtTenKH.Clear();
            txtSDT.Clear();
            txtDiemTichLuy.Clear();
            cboHang.SelectedIndex = 0;
            CapNhatTrangThaiNut();

            if (duLieu != null)
            {
                HienThiDuLieu(duLieu);
                dgvKhachHang_SelectionChanged(dgvKhachHang, EventArgs.Empty);
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            if (choDongBo)
                return;
            ApDungTimKiem();
        }

        private void txtTim_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ApDungTimKiem();
            }
        }

        private void ApDungTimKiem()
        {
            if (duLieu == null)
                return;

            string tuKhoa = txtTim.Text.Trim();
            if (tuKhoa.Length == 0)
            {
                HienThiDuLieu(duLieu);
                return;
            }

            DataTable ketQua = duLieu.Clone();
            foreach (DataRow dong in duLieu.Rows)
            {
                string sdt = Convert.ToString(dong["SDT"]);
                string ten = Convert.ToString(dong["TenKH"]);
                string ma = Convert.ToString(dong["MaKH"]);
                if (sdt.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0
                    || ten.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0
                    || ma.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    ketQua.ImportRow(dong);
                }
            }

            HienThiDuLieu(ketQua);
        }

        private void CapNhatTrangThaiNut()
        {
            bool dangChinhSua = !string.IsNullOrEmpty(cheDo);
            bool binhThuong = !dangChinhSua && !choDongBo;

            btnThem.Enabled = binhThuong;
            btnSua.Enabled = binhThuong && dgvKhachHang.CurrentRow != null;
            btnXoa.Enabled = binhThuong && dgvKhachHang.CurrentRow != null;
            btnTim.Enabled = binhThuong;
            txtTim.Enabled = binhThuong;

            txtMaKH.ReadOnly = true;
            txtTenKH.ReadOnly = !dangChinhSua;
            txtSDT.ReadOnly = !dangChinhSua;
            txtDiemTichLuy.ReadOnly = true;
            txtMaKH.BackColor = Color.FromArgb(243, 244, 246);
            txtDiemTichLuy.BackColor = Color.FromArgb(243, 244, 246);
            txtTenKH.BackColor = txtTenKH.ReadOnly ? Color.FromArgb(243, 244, 246) : Color.White;
            txtSDT.BackColor = txtSDT.ReadOnly ? Color.FromArgb(243, 244, 246) : Color.White;
            cboHang.Enabled = dangChinhSua;
            cboHang.BackColor = dangChinhSua ? Color.White : Color.FromArgb(243, 244, 246);

            btnLuu.Enabled = dangChinhSua || choDongBo;
            btnHuy.Enabled = dangChinhSua && !choDongBo;
            btnLuu.Text = choDongBo ? "Đồng bộ" : "Lưu";

            if (choDongBo && !string.IsNullOrEmpty(hanhDongChoDongBo))
                Text = "Quản lý khách hàng - chờ đồng bộ " + hanhDongChoDongBo;
            else
                Text = "Quản lý khách hàng";
        }

        private static Color MauTheoHang(string hang)
        {
            switch (hang)
            {
                case "Thường": return Color.FromArgb(107, 114, 128);
                case "Bạc": return Color.FromArgb(148, 163, 184);
                case "Vàng": return Color.FromArgb(217, 119, 6);
                default: return Color.FromArgb(209, 213, 219);
            }
        }

        private static System.Drawing.Drawing2D.GraphicsPath TaoDuongVienBoGoc(Rectangle hinh, int banKinh)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            int duongKinh = banKinh * 2;
            if (hinh.Width <= 0 || hinh.Height <= 0)
                return path;
            duongKinh = Math.Min(duongKinh, Math.Min(hinh.Width, hinh.Height));
            path.AddArc(hinh.X, hinh.Y, duongKinh, duongKinh, 180, 90);
            path.AddArc(hinh.Right - duongKinh, hinh.Y, duongKinh, duongKinh, 270, 90);
            path.AddArc(hinh.Right - duongKinh, hinh.Bottom - duongKinh, duongKinh, duongKinh, 0, 90);
            path.AddArc(hinh.X, hinh.Bottom - duongKinh, duongKinh, duongKinh, 90, 90);
            path.CloseFigure();
            return path;
        }

        private sealed class MetricCustomerCard : Panel
        {
            public Label GiaTriLabel { get; private set; }
            public Color MauSo { get; private set; }

            public MetricCustomerCard(string nhan, Color mauSo)
            {
                MauSo = mauSo;
                BackColor = Color.FromArgb(249, 250, 251);
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw, true);
                GiaTriLabel = new Label
                {
                    Name = "lblGiaTri",
                    Text = "0",
                    Location = new Point(2, 4),
                    Size = new Size(148, 32),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                    ForeColor = mauSo,
                    BackColor = Color.Transparent
                };
                var lblNhan = new Label
                {
                    Name = "lblNhan",
                    Text = nhan,
                    Location = new Point(2, 37),
                    Size = new Size(148, 18),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 9F),
                    ForeColor = Color.FromArgb(107, 114, 128),
                    BackColor = Color.Transparent
                };
                Controls.Add(lblNhan);
                Controls.Add(GiaTriLabel);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                using (var pen = new Pen(Color.FromArgb(229, 231, 235)))
                    e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            }
        }

        private sealed class RankBadge : Control
        {
            public RankBadge()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (string.IsNullOrWhiteSpace(Text))
                    return;
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var path = TaoDuongVienBoGoc(new Rectangle(0, 0, Width - 1, Height - 1), Height / 2))
                using (var brush = new SolidBrush(MauTheoHang(Text)))
                using (var font = new Font("Segoe UI", 8.5F, FontStyle.Bold))
                using (var brushChu = new SolidBrush(Color.White))
                using (var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                })
                {
                    e.Graphics.FillPath(brush, path);
                    e.Graphics.DrawString(Text, font, brushChu, ClientRectangle, format);
                }
            }
        }

        private sealed class AvatarKhachHang : Control
        {
            private string ten = "";
            private string hang;

            public AvatarKhachHang()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            }

            public void DatDuLieu(string ten, string hang)
            {
                this.ten = ten ?? "";
                this.hang = hang;
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                Color nen = string.IsNullOrEmpty(hang)
                    ? Color.FromArgb(209, 213, 219)
                    : MauTheoHang(hang);
                using (var brush = new SolidBrush(nen))
                using (var font = new Font("Segoe UI", 18F, FontStyle.Bold))
                using (var brushChu = new SolidBrush(Color.White))
                using (var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                })
                {
                    e.Graphics.FillEllipse(brush, 0, 0, Width - 1, Height - 1);
                    string chuCai = string.IsNullOrWhiteSpace(ten) ? "?" : ten.Trim().Substring(0, 1).ToUpperInvariant();
                    e.Graphics.DrawString(chuCai, font, brushChu, ClientRectangle, format);
                }
            }
        }

        private sealed class ProgressHang : Control
        {
            private float tiLe;

            public float TiLe
            {
                get { return tiLe; }
                set
                {
                    tiLe = Math.Max(0F, Math.Min(1F, value));
                    Invalidate();
                }
            }

            public ProgressHang()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                Rectangle hinhNen = new Rectangle(0, 0, Width - 1, Height - 1);
                using (var pathNen = TaoDuongVienBoGoc(hinhNen, Height / 2))
                using (var brushNen = new SolidBrush(Color.FromArgb(229, 231, 235)))
                {
                    e.Graphics.FillPath(brushNen, pathNen);
                }

                int rong = (int)Math.Round((Width - 1) * tiLe);
                if (rong > 0)
                {
                    using (var path = TaoDuongVienBoGoc(new Rectangle(0, 0, rong, Height - 1), Height / 2))
                    using (var brush = new SolidBrush(Color.FromArgb(232, 101, 26)))
                        e.Graphics.FillPath(brush, path);
                }
            }
        }

        private static void HienLoi(string noiDung, Exception ex)
        {
            MessageBox.Show(noiDung + ".\r\n\r\n" + ex.Message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
