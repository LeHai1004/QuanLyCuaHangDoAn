using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using QuanLyCuaHangDoAn;
using QuanLyCuaHangDoAn.Class;

namespace QuanLyCuaHangDoAn.GUI
{
    public class frmPhieuNhap : Form
    {
        private readonly FileXml fx = new FileXml();
        private readonly TextBox txtMaPN = new TextBox();
        private readonly ComboBox cboNhaCungCap = new ComboBox();
        private readonly ComboBox cboNguyenLieu = new ComboBox();
        private readonly TextBox txtNgayNhap = new TextBox();
        private readonly TextBox txtSoLuong = new TextBox();
        private readonly TextBox txtDonGia = new TextBox();
        private readonly DataGridView dgvChiTietNhap = new DataGridView();
        private readonly DataGridView dgvPhieuNhap = new DataGridView();
        private readonly DataGridView dgvChiTietLichSu = new DataGridView();
        private readonly Label lblTongTien = new Label();
        private readonly Label lblThanhTienDuKien = new Label();
        private readonly ToolTip toolTipPhieuNhap = new ToolTip();
        private readonly Button btnThemDong = GiaoDien.TaoNut("Thêm dòng", GiaoDien.MauChinh);
        private readonly Button btnXoaDong = GiaoDien.TaoNut("Xóa dòng", GiaoDien.MauDo);
        private readonly Button btnLuu = GiaoDien.TaoNut("Lưu phiếu", GiaoDien.MauXanh);
        private readonly Button btnHuy = GiaoDien.TaoNut("Hủy", GiaoDien.MauChinhDam);

        private DataTable nhaCungCap;
        private DataTable nguyenLieu;
        private DataTable chiTietNhap;
        private bool choDongBo;
        private string maPNChoDongBo;

        public frmPhieuNhap()
        {
            KhoiTaoGiaoDien();
            Load += frmPhieuNhap_Load;
            FormClosed += (sender, e) => toolTipPhieuNhap.Dispose();
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Quản lý phiếu nhập hàng";
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 246, 248);
            Font = new Font("Segoe UI", 10F);
            ClientSize = new Size(1280, 720);
            DoubleBuffered = true;

            txtMaPN.Name = "txtMaPN";
            cboNhaCungCap.Name = "cboNhaCungCap";
            cboNguyenLieu.Name = "cboNguyenLieu";
            txtNgayNhap.Name = "txtNgayNhap";
            txtSoLuong.Name = "txtSoLuong";
            txtDonGia.Name = "txtDonGia";
            dgvChiTietNhap.Name = "dgvChiTietNhap";
            dgvPhieuNhap.Name = "dgvPhieuNhap";
            dgvChiTietLichSu.Name = "dgvChiTietLichSu";
            lblTongTien.Name = "lblTongTien";
            btnThemDong.Name = "btnThemDong";
            btnXoaDong.Name = "btnXoaDong";
            btnLuu.Name = "btnLuu";
            btnHuy.Name = "btnHuy";

            txtMaPN.ReadOnly = true;
            txtNgayNhap.ReadOnly = true;
            cboNhaCungCap.DropDownStyle = ComboBoxStyle.DropDownList;
            cboNguyenLieu.DropDownStyle = ComboBoxStyle.DropDownList;

            var pnlHeader = new Panel
            {
                Name = "pnlHeader",
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(232, 101, 26)
            };
            pnlHeader.Controls.Add(new Label
            {
                Name = "lblTieuDe",
                Text = "PHIẾU NHẬP HÀNG",
                Location = new Point(30, 10),
                Size = new Size(500, 27),
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false
            });
            pnlHeader.Controls.Add(new Label
            {
                Name = "lblPhuDe",
                Text = "Nhập nguyên liệu và theo dõi lịch sử",
                Location = new Point(32, 42),
                Size = new Size(500, 18),
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.White,
                AutoSize = false
            });

            var pnlTaoPhieu = new BorderPanel
            {
                Name = "pnlTaoPhieu",
                Location = new Point(20, 85),
                Size = new Size(380, 615),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                BackColor = Color.White
            };
            var pnlDongNhap = new BorderPanel
            {
                Name = "pnlDongNhap",
                Location = new Point(415, 85),
                Size = new Size(845, 300),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.White
            };
            var pnlLichSu = new BorderPanel
            {
                Name = "pnlLichSu",
                Location = new Point(415, 400),
                Size = new Size(845, 300),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackColor = Color.White
            };
            TaoGiaoDienTaoPhieu(pnlTaoPhieu);
            TaoGiaoDienDongNhap(pnlDongNhap);
            TaoGiaoDienLichSu(pnlLichSu);

            Controls.Add(pnlTaoPhieu);
            Controls.Add(pnlDongNhap);
            Controls.Add(pnlLichSu);
            Controls.Add(pnlHeader);
            pnlHeader.BringToFront();

            btnThemDong.Click += btnThemDong_Click;
            btnXoaDong.Click += btnXoaDong_Click;
            btnLuu.Click += btnLuu_Click;
            btnHuy.Click += btnHuy_Click;
            dgvPhieuNhap.SelectionChanged += dgvPhieuNhap_SelectionChanged;
            txtSoLuong.TextChanged += txtGiaTriNhap_TextChanged;
            txtDonGia.TextChanged += txtGiaTriNhap_TextChanged;
            dgvChiTietNhap.CellPainting += dgv_CellPainting;
            dgvPhieuNhap.CellPainting += dgv_CellPainting;
            dgvChiTietLichSu.CellPainting += dgv_CellPainting;
            dgvChiTietNhap.CellToolTipTextNeeded += dgv_CellToolTipTextNeeded;
            dgvPhieuNhap.CellToolTipTextNeeded += dgv_CellToolTipTextNeeded;
            dgvChiTietLichSu.CellToolTipTextNeeded += dgv_CellToolTipTextNeeded;

            CauHinhBang(dgvChiTietNhap);
            CauHinhBang(dgvPhieuNhap);
            CauHinhBang(dgvChiTietLichSu);
            TaoCotBang(dgvChiTietNhap, "MaNL", "Mã NL", 12F, false, false);
            TaoCotBang(dgvChiTietNhap, "TenNL", "Tên nguyên liệu", 30F, false, false);
            TaoCotBang(dgvChiTietNhap, "DVT", "ĐVT", 10F, false, false);
            TaoCotBang(dgvChiTietNhap, "SoLuong", "Số lượng", 14F, true, false);
            TaoCotBang(dgvChiTietNhap, "DonGia", "Đơn giá", 16F, true, true);
            TaoCotBang(dgvChiTietNhap, "ThanhTien", "Thành tiền", 18F, true, true);
            TaoCotBang(dgvPhieuNhap, "MaPN", "Mã phiếu", 16F, false, false);
            TaoCotBang(dgvPhieuNhap, "TenNCC", "Nhà cung cấp", 30F, false, false);
            TaoCotBang(dgvPhieuNhap, "NgayNhap", "Ngày nhập", 24F, false, false);
            TaoCotBang(dgvPhieuNhap, "TongTien", "Tổng tiền", 30F, true, true);
            TaoCotBang(dgvPhieuNhap, "TenNV", "Người lập", 1F, false, false, false);
            TaoCotBang(dgvChiTietLichSu, "TenNL", "Tên nguyên liệu", 34F, false, false);
            TaoCotBang(dgvChiTietLichSu, "SoLuong", "Số lượng", 16F, true, false);
            TaoCotBang(dgvChiTietLichSu, "DonGia", "Đơn giá", 24F, true, true);
            TaoCotBang(dgvChiTietLichSu, "ThanhTien", "Thành tiền", 26F, true, true);
            TaoCotBang(dgvChiTietLichSu, "MaNL", "Mã NL", 1F, false, false, false);
            TaoCotBang(dgvChiTietLichSu, "DVT", "ĐVT", 1F, false, false, false);
        }

        private void TaoGiaoDienTaoPhieu(Panel panel)
        {
            panel.Controls.Add(new Label
            {
                Name = "lblTaoPhieu",
                Text = "TẠO PHIẾU NHẬP",
                Location = new Point(20, 15),
                Size = new Size(340, 22),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(232, 101, 26),
                AutoSize = false
            });

            ThemTruong(panel, "Mã phiếu nhập", txtMaPN, 20, 55, 165, true);
            ThemTruong(panel, "Ngày nhập", txtNgayNhap, 195, 55, 165, true);
            ThemTruong(panel, "Nhà cung cấp (*)", cboNhaCungCap, 20, 115, 340, false);
            panel.Controls.Add(new Panel
            {
                Name = "pnlDuongKe",
                Location = new Point(20, 175),
                Size = new Size(340, 1),
                BackColor = Color.FromArgb(229, 231, 235)
            });
            panel.Controls.Add(new Label
            {
                Name = "lblThemNguyenLieu",
                Text = "THÊM NGUYÊN LIỆU",
                Location = new Point(20, 190),
                Size = new Size(340, 20),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(55, 65, 81),
                AutoSize = false
            });
            ThemTruong(panel, "Nguyên liệu (*)", cboNguyenLieu, 20, 225, 340, false);
            ThemTruong(panel, "Số lượng nhập (*)", txtSoLuong, 20, 285, 165, false);
            ThemTruong(panel, "Đơn giá nhập (*)", txtDonGia, 195, 285, 165, false);

            lblThanhTienDuKien.Name = "lblThanhTienDuKien";
            lblThanhTienDuKien.Text = "Thành tiền dự tính: 0 đ";
            lblThanhTienDuKien.Location = new Point(20, 350);
            lblThanhTienDuKien.Size = new Size(340, 20);
            lblThanhTienDuKien.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblThanhTienDuKien.ForeColor = Color.FromArgb(232, 101, 26);
            panel.Controls.Add(lblThanhTienDuKien);

            btnThemDong.Location = new Point(20, 385);
            btnThemDong.Size = new Size(340, 44);
            CauHinhNut(btnThemDong, Color.FromArgb(232, 101, 26));
            btnThemDong.FlatAppearance.MouseOverBackColor = Color.FromArgb(207, 85, 16);
            panel.Controls.Add(btnThemDong);

            panel.Controls.Add(new Label
            {
                Name = "lblHuongDan",
                Text = "1. Chọn nhà cung cấp\r\n2. Thêm các dòng nguyên liệu\r\n3. Bấm Lưu phiếu để tăng tồn kho",
                Location = new Point(20, 450),
                Size = new Size(340, 70),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize = false
            });
        }

        private void TaoGiaoDienDongNhap(Panel panel)
        {
            panel.Controls.Add(TaoNhan("DÒNG ĐANG NHẬP", "lblDongDangNhap",
                new Point(15, 12), new Size(815, 20)));
            dgvChiTietNhap.Location = new Point(15, 40);
            dgvChiTietNhap.Size = new Size(815, 190);
            dgvChiTietNhap.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel.Controls.Add(dgvChiTietNhap);

            btnXoaDong.Location = new Point(15, 242);
            btnXoaDong.Size = new Size(110, 44);
            btnXoaDong.FlatStyle = FlatStyle.Flat;
            btnXoaDong.FlatAppearance.BorderSize = 1;
            btnXoaDong.FlatAppearance.BorderColor = Color.FromArgb(220, 38, 38);
            btnXoaDong.FlatAppearance.MouseOverBackColor = Color.FromArgb(254, 242, 242);
            btnXoaDong.UseVisualStyleBackColor = false;
            btnXoaDong.BackColor = Color.White;
            btnXoaDong.ForeColor = Color.FromArgb(220, 38, 38);
            btnXoaDong.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnXoaDong.Cursor = Cursors.Hand;
            panel.Controls.Add(btnXoaDong);

            CauHinhNut(btnLuu, Color.FromArgb(22, 163, 74));
            btnLuu.Location = new Point(690, 242);
            btnLuu.Size = new Size(140, 44);
            btnLuu.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            CauHinhNut(btnHuy, Color.FromArgb(107, 114, 128));
            btnHuy.Location = new Point(592, 242);
            btnHuy.Size = new Size(90, 44);
            btnHuy.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            panel.Controls.Add(btnLuu);
            panel.Controls.Add(btnHuy);

            lblTongTien.Text = "Tổng: 0 đ";
            lblTongTien.Location = new Point(275, 242);
            lblTongTien.Size = new Size(309, 44);
            lblTongTien.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblTongTien.TextAlign = ContentAlignment.MiddleRight;
            lblTongTien.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTongTien.ForeColor = Color.FromArgb(232, 101, 26);
            panel.Controls.Add(lblTongTien);
        }

        private void TaoGiaoDienLichSu(Panel panel)
        {
            panel.Controls.Add(TaoNhan("LỊCH SỬ PHIẾU NHẬP", "lblLichSuPhieuNhap",
                new Point(15, 12), new Size(395, 20)));
            panel.Controls.Add(TaoNhan("CHI TIẾT PHIẾU ĐANG CHỌN", "lblChiTietLichSu",
                new Point(425, 12), new Size(405, 20)));
            dgvPhieuNhap.Location = new Point(15, 40);
            dgvPhieuNhap.Size = new Size(395, 245);
            dgvPhieuNhap.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
            dgvChiTietLichSu.Location = new Point(425, 40);
            dgvChiTietLichSu.Size = new Size(405, 245);
            dgvChiTietLichSu.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            panel.Controls.Add(dgvPhieuNhap);
            panel.Controls.Add(dgvChiTietLichSu);
        }

        private static Label TaoNhan(string noiDung, string ten, Point viTri, Size kichThuoc)
        {
            return new Label
            {
                Name = ten,
                Text = noiDung,
                Location = viTri,
                Size = kichThuoc,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(232, 101, 26),
                AutoSize = false
            };
        }

        private void ThemTruong(Panel panel, string nhan, Control control,
            int x, int y, int rong, bool chiDoc)
        {
            panel.Controls.Add(new Label
            {
                Name = "lbl" + control.Name.Substring(3),
                Text = nhan,
                Location = new Point(x, y),
                Size = new Size(rong, 16),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize = false
            });
            control.Location = new Point(x, y + 20);
            control.Size = new Size(rong, 34);
            control.Font = new Font("Segoe UI", 10.5F);
            if (control is TextBox textBox)
            {
                textBox.BorderStyle = BorderStyle.FixedSingle;
                textBox.AutoSize = false;
                textBox.ReadOnly = chiDoc;
                textBox.BackColor = chiDoc ? Color.FromArgb(243, 244, 246) : Color.White;
            }
            panel.Controls.Add(control);
        }

        private void CauHinhBang(DataGridView dgv)
        {
            dgv.BorderStyle = BorderStyle.None;
            dgv.BackgroundColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(243, 244, 246);
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AutoGenerateColumns = false;
            dgv.ColumnHeadersHeight = 38;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.RowTemplate.Height = 36;
            dgv.ShowCellToolTips = true;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 243, 234);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(154, 52, 18);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 4, 0);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 243, 234);
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(154, 52, 18);
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(17, 24, 39);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgv.DefaultCellStyle.Padding = new Padding(8, 0, 4, 0);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 243, 234);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(17, 24, 39);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 243, 234);
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.FromArgb(17, 24, 39);
        }

        private static void TaoCotBang(DataGridView dgv, string ten, string tieuDe,
            float tiLe, bool canPhai, bool laTien, bool hienThi = true)
        {
            var cot = new DataGridViewTextBoxColumn
            {
                Name = ten,
                DataPropertyName = ten,
                HeaderText = tieuDe,
                FillWeight = tiLe,
                ReadOnly = true,
                Visible = hienThi,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle =
                {
                    Alignment = canPhai
                        ? DataGridViewContentAlignment.MiddleRight
                        : DataGridViewContentAlignment.MiddleLeft
                }
            };
            if (laTien)
            {
                cot.DefaultCellStyle.Format = "#,##0\" đ\"";
                cot.DefaultCellStyle.FormatProvider = new CultureInfo("vi-VN");
            }
            dgv.Columns.Add(cot);
            if (canPhai)
                cot.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void frmPhieuNhap_Load(object sender, EventArgs e)
        {
            if (!PhienDangNhap.IsLoggedIn || PhienDangNhap.ChucVu != "Admin")
            {
                MessageBox.Show("Chức năng chỉ dành cho quản trị viên (Admin).", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }

            try
            {
                TaiDuLieu();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể tải dữ liệu phiếu nhập", ex);
            }
        }

        private void TaiDuLieu()
        {
            fx.TaoXml("SELECT * FROM NhaCungCap", "NhaCungCap", "NhaCungCap.xml");
            fx.TaoXml("SELECT * FROM NguyenLieu", "NguyenLieu", "NguyenLieu.xml");
            fx.TaoXml("SELECT * FROM PhieuNhap", "PhieuNhap", "PhieuNhap.xml");
            fx.TaoXml("SELECT * FROM ChiTietPhieuNhap", "ChiTietPhieuNhap", "ChiTietPhieuNhap.xml");
            nhaCungCap = fx.HienThi("NhaCungCap.xml");
            nguyenLieu = fx.HienThi("NguyenLieu.xml");
            cboNhaCungCap.DataSource = TaoBangNhaCungCapHienThi();
            cboNhaCungCap.DisplayMember = "TenHienThi";
            cboNhaCungCap.ValueMember = "MaNCC";
            cboNguyenLieu.DataSource = TaoBangNguyenLieuHienThi();
            cboNguyenLieu.DisplayMember = "TenHienThi";
            cboNguyenLieu.ValueMember = "MaNL";
            TaoChiTietNhap();
            TaoMaPhieuMoi();
            TaiLichSuPhieuNhap();
            CapNhatTrangThaiNut();
        }

        private DataTable TaoBangNhaCungCapHienThi()
        {
            var bang = nhaCungCap.Copy();
            bang.Columns.Add("TenHienThi", typeof(string));
            foreach (DataRow dong in bang.Rows)
                dong["TenHienThi"] = dong["MaNCC"] + " - " + dong["TenNCC"];
            return bang;
        }

        private DataTable TaoBangNguyenLieuHienThi()
        {
            var bang = nguyenLieu.Copy();
            bang.Columns.Add("TenHienThi", typeof(string));
            foreach (DataRow dong in bang.Rows)
                dong["TenHienThi"] = dong["MaNL"] + " - " + dong["TenNL"] + " (" + dong["DVT"] + ")";
            return bang;
        }

        private void TaoChiTietNhap()
        {
            chiTietNhap = new DataTable();
            chiTietNhap.Columns.Add("MaNL", typeof(string));
            chiTietNhap.Columns.Add("TenNL", typeof(string));
            chiTietNhap.Columns.Add("DVT", typeof(string));
            chiTietNhap.Columns.Add("SoLuong", typeof(decimal));
            chiTietNhap.Columns.Add("DonGia", typeof(decimal));
            chiTietNhap.Columns.Add("ThanhTien", typeof(decimal));
            HienThiChiTietNhap();
        }

        private void TaoMaPhieuMoi()
        {
            txtMaPN.Text = fx.TaoMa("PN", "PhieuNhap.xml", "MaPN");
            txtNgayNhap.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            cboNhaCungCap.SelectedIndex = -1;
            cboNguyenLieu.SelectedIndex = -1;
            txtSoLuong.Clear();
            txtDonGia.Clear();
        }

        private void HienThiChiTietNhap()
        {
            dgvChiTietNhap.DataSource = chiTietNhap;
            foreach (DataGridViewRow dong in dgvChiTietNhap.Rows)
                dong.Height = 36;
            if (dgvChiTietNhap.Columns.Contains("SoLuong"))
                dgvChiTietNhap.Columns["SoLuong"].DefaultCellStyle.Format = "#,##0.##";
            GiaoDien.DinhDangTien(dgvChiTietNhap, "DonGia", "ThanhTien");
            CapNhatTongTien();
        }

        private void CapNhatTongTien()
        {
            decimal tong = 0m;
            if (chiTietNhap != null)
                foreach (DataRow dong in chiTietNhap.Rows) tong += Convert.ToDecimal(dong["ThanhTien"]);
            lblTongTien.Text = "Tổng: " + tong.ToString("#,##0", new CultureInfo("vi-VN")) + " đ";
        }

        private void txtGiaTriNhap_TextChanged(object sender, EventArgs e)
        {
            decimal soLuong;
            decimal donGia;
            decimal thanhTien = 0m;
            if (TryLaySo(txtSoLuong.Text, out soLuong)
                && TryLaySo(txtDonGia.Text, out donGia)
                && soLuong > 0m && donGia >= 0m)
            {
                if (donGia == 0m || soLuong <= decimal.MaxValue / donGia)
                    thanhTien = soLuong * donGia;
            }
            lblThanhTienDuKien.Text = "Thành tiền dự tính: "
                + thanhTien.ToString("#,##0", new CultureInfo("vi-VN")) + " đ";
        }

        private void btnThemDong_Click(object sender, EventArgs e)
        {
            if (choDongBo)
                return;
            if (cboNguyenLieu.SelectedValue == null)
            {
                BaoLoi("Vui lòng chọn nguyên liệu.");
                return;
            }
            decimal soLuong;
            decimal donGia;
            if (!TryLaySo(txtSoLuong.Text, out soLuong) || soLuong <= 0m)
            {
                BaoLoi("Số lượng nhập phải là số lớn hơn 0.");
                txtSoLuong.Focus();
                return;
            }
            if (!TryLaySo(txtDonGia.Text, out donGia) || donGia < 0m)
            {
                BaoLoi("Đơn giá nhập phải là số lớn hơn hoặc bằng 0.");
                txtDonGia.Focus();
                return;
            }

            string maNL = cboNguyenLieu.SelectedValue.ToString();
            foreach (DataRow dong in chiTietNhap.Rows)
            {
                if (Convert.ToString(dong["MaNL"]) == maNL)
                {
                    BaoLoi("Nguyên liệu này đã có trong phiếu. Vui lòng xóa dòng cũ rồi thêm lại.");
                    return;
                }
            }

            DataRow nguyenLieuChon = TimNguyenLieu(maNL);
            if (nguyenLieuChon == null)
            {
                BaoLoi("Không tìm thấy nguyên liệu đã chọn.");
                return;
            }
            DataRow dongMoi = chiTietNhap.NewRow();
            dongMoi["MaNL"] = maNL;
            dongMoi["TenNL"] = nguyenLieuChon["TenNL"];
            dongMoi["DVT"] = nguyenLieuChon["DVT"];
            decimal soLuongLuu = decimal.Round(soLuong, 2, MidpointRounding.AwayFromZero);
            decimal donGiaLuu = decimal.Round(donGia, 0, MidpointRounding.AwayFromZero);
            if (soLuongLuu <= 0m)
            {
                BaoLoi("Số lượng sau khi làm tròn theo dữ liệu lưu phải ít nhất là 0,01.");
                txtSoLuong.Focus();
                return;
            }
            dongMoi["SoLuong"] = soLuongLuu;
            dongMoi["DonGia"] = donGiaLuu;
            dongMoi["ThanhTien"] = soLuongLuu * donGiaLuu;
            chiTietNhap.Rows.Add(dongMoi);
            HienThiChiTietNhap();
        }

        private void btnXoaDong_Click(object sender, EventArgs e)
        {
            if (choDongBo)
                return;
            if (dgvChiTietNhap.CurrentRow == null || dgvChiTietNhap.CurrentRow.Index < 0)
            {
                BaoLoi("Vui lòng chọn dòng nguyên liệu cần xóa.");
                return;
            }
            if (MessageBox.Show("Bạn có chắc muốn xóa dòng nguyên liệu này?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            chiTietNhap.Rows.RemoveAt(dgvChiTietNhap.CurrentRow.Index);
            HienThiChiTietNhap();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            if (choDongBo)
                return;
            TaoChiTietNhap();
            TaoMaPhieuMoi();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (choDongBo)
            {
                ThuDongBo();
                return;
            }
            if (cboNhaCungCap.SelectedValue == null)
            {
                BaoLoi("Vui lòng chọn nhà cung cấp.");
                return;
            }
            if (chiTietNhap == null || chiTietNhap.Rows.Count == 0)
            {
                BaoLoi("Phiếu nhập phải có ít nhất một dòng nguyên liệu.");
                return;
            }
            foreach (DataRow dong in chiTietNhap.Rows)
            {
                if (Convert.ToDecimal(dong["SoLuong"]) <= 0m || Convert.ToDecimal(dong["DonGia"]) < 0m)
                {
                    BaoLoi("Số lượng phải lớn hơn 0 và đơn giá không được âm.");
                    return;
                }
            }

            try
            {
                string maPN = txtMaPN.Text.Trim();
                string maNCC = cboNhaCungCap.SelectedValue.ToString();
                DateTime ngayNhap = DateTime.Now;
                txtNgayNhap.Text = ngayNhap.ToString("dd/MM/yyyy HH:mm");
                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            string maNV = LayMaNhanVien();
                            using (var cmd = new SqlCommand(
                                "INSERT INTO PhieuNhap (MaPN, MaNCC, MaNV, NgayNhap) " +
                                "VALUES (@MaPN, @MaNCC, @MaNV, @NgayNhap)", conn, tran))
                            {
                                cmd.Parameters.Add("@MaPN", SqlDbType.NVarChar, 10).Value = maPN;
                                cmd.Parameters.Add("@MaNCC", SqlDbType.NVarChar, 10).Value = maNCC;
                                cmd.Parameters.Add("@MaNV", SqlDbType.NVarChar, 10).Value = maNV;
                                cmd.Parameters.Add("@NgayNhap", SqlDbType.DateTime).Value = ngayNhap;
                                cmd.ExecuteNonQuery();
                            }

                            var maNguyenLieuDaNhap = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                            foreach (DataRow dong in chiTietNhap.Rows)
                            {
                                string maNL = Convert.ToString(dong["MaNL"]);
                                decimal soLuong = Convert.ToDecimal(dong["SoLuong"]);
                                decimal donGia = Convert.ToDecimal(dong["DonGia"]);
                                using (var cmd = new SqlCommand(
                                    "INSERT INTO ChiTietPhieuNhap (MaPN, MaNL, SoLuong, DonGia) " +
                                    "VALUES (@MaPN, @MaNL, @SoLuong, @DonGia)", conn, tran))
                                {
                                    cmd.Parameters.Add("@MaPN", SqlDbType.NVarChar, 10).Value = maPN;
                                    cmd.Parameters.Add("@MaNL", SqlDbType.NVarChar, 10).Value = maNL;
                                    GanDecimal(cmd, "@SoLuong", soLuong, 18, 2);
                                    GanDecimal(cmd, "@DonGia", donGia, 18, 0);
                                    cmd.ExecuteNonQuery();
                                }
                                using (var cmd = new SqlCommand(
                                    "UPDATE NguyenLieu SET TonKho = TonKho + @SoLuong WHERE MaNL = @MaNL", conn, tran))
                                {
                                    GanDecimal(cmd, "@SoLuong", soLuong, 18, 2);
                                    cmd.Parameters.Add("@MaNL", SqlDbType.NVarChar, 10).Value = maNL;
                                    cmd.ExecuteNonQuery();
                                }
                                maNguyenLieuDaNhap.Add(maNL);
                            }

                            Kho.CapNhatTrangThaiMonAn(conn, tran, maNguyenLieuDaNhap);
                            tran.Commit();
                        }
                        catch
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }

                choDongBo = true;
                maPNChoDongBo = maPN;
                CapNhatTrangThaiNut();
                ThuDongBo();
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                HienLoi("Mã phiếu nhập đã tồn tại. Vui lòng tạo lại phiếu mới", ex);
            }
            catch (Exception ex)
            {
                HienLoi("Không thể lưu phiếu nhập", ex);
            }
        }

        private string LayMaNhanVien()
        {
            if (!PhienDangNhap.IsLoggedIn || string.IsNullOrWhiteSpace(PhienDangNhap.MaNV))
                throw new InvalidOperationException("Không xác định được nhân viên đang đăng nhập để lập phiếu nhập.");
            return PhienDangNhap.MaNV.Trim();
        }

        private void DongBoXmlSauKhiLuu()
        {
            fx.TaoXml("SELECT * FROM PhieuNhap", "PhieuNhap", "PhieuNhap.xml");
            fx.TaoXml("SELECT * FROM ChiTietPhieuNhap", "ChiTietPhieuNhap", "ChiTietPhieuNhap.xml");
            fx.TaoXml("SELECT * FROM NguyenLieu", "NguyenLieu", "NguyenLieu.xml");
            fx.TaoXml("SELECT * FROM MonAn", "MonAn", "MonAn.xml");
            fx.CapNhatDb("PhieuNhap", "PhieuNhap.xml");
            fx.CapNhatDb("ChiTietPhieuNhap", "ChiTietPhieuNhap.xml");
        }

        private void ThuDongBo()
        {
            try
            {
                DongBoXmlSauKhiLuu();
            }
            catch (Exception ex)
            {
                CapNhatTrangThaiNut();
                HienLoi("Phiếu " + maPNChoDongBo + " đã ghi vào CSDL nhưng XML chưa đồng bộ. Nhấn Đồng bộ để thử lại", ex);
                return;
            }

            choDongBo = false;
            string maPNDaLuu = maPNChoDongBo;
            maPNChoDongBo = null;
            try
            {
                TaiDuLieu();
            }
            catch (Exception ex)
            {
                HienLoi("Phiếu đã đồng bộ nhưng không thể tải lại dữ liệu", ex);
            }
            CapNhatTrangThaiNut();
            MessageBox.Show("Đã lưu phiếu nhập " + maPNDaLuu + " và cập nhật tồn kho.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CapNhatTrangThaiNut()
        {
            bool choPhepThaoTac = !choDongBo;
            cboNhaCungCap.Enabled = choPhepThaoTac;
            cboNguyenLieu.Enabled = choPhepThaoTac;
            txtNgayNhap.ReadOnly = true;
            txtSoLuong.Enabled = choPhepThaoTac;
            txtDonGia.Enabled = choPhepThaoTac;
            txtMaPN.BackColor = Color.FromArgb(243, 244, 246);
            txtNgayNhap.BackColor = Color.FromArgb(243, 244, 246);
            txtSoLuong.BackColor = Color.White;
            txtDonGia.BackColor = Color.White;
            btnThemDong.Enabled = choPhepThaoTac;
            btnXoaDong.Enabled = choPhepThaoTac;
            btnHuy.Enabled = choPhepThaoTac;
            btnLuu.Enabled = true;
            btnLuu.Text = choDongBo ? "Đồng bộ" : "Lưu phiếu";
        }

        private void TaiLichSuPhieuNhap()
        {
            using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
            using (var da = new SqlDataAdapter(
                "SELECT pn.MaPN, ncc.TenNCC, nv.TenNV, pn.NgayNhap, " +
                "ISNULL(SUM(ct.SoLuong * ct.DonGia), 0) AS TongTien " +
                "FROM PhieuNhap pn INNER JOIN NhaCungCap ncc ON ncc.MaNCC = pn.MaNCC " +
                "INNER JOIN NhanVien nv ON nv.MaNV = pn.MaNV " +
                "LEFT JOIN ChiTietPhieuNhap ct ON ct.MaPN = pn.MaPN " +
                "GROUP BY pn.MaPN, ncc.TenNCC, nv.TenNV, pn.NgayNhap ORDER BY pn.NgayNhap DESC, pn.MaPN DESC", conn))
            {
                var bang = new DataTable();
                da.Fill(bang);
                dgvPhieuNhap.DataSource = bang;
                foreach (DataGridViewRow dong in dgvPhieuNhap.Rows)
                    dong.Height = 36;
                if (dgvPhieuNhap.Columns.Contains("NgayNhap"))
                    dgvPhieuNhap.Columns["NgayNhap"].DefaultCellStyle.Format = "dd/MM/yyyy";
                GiaoDien.DinhDangTien(dgvPhieuNhap, "TongTien");
            }
            TaiChiTietLichSu();
        }

        private void dgvPhieuNhap_SelectionChanged(object sender, EventArgs e)
        {
            TaiChiTietLichSu();
        }

        private void TaiChiTietLichSu()
        {
            string maPN = "";
            if (dgvPhieuNhap.CurrentRow != null && dgvPhieuNhap.CurrentRow.Cells["MaPN"].Value != null)
                maPN = Convert.ToString(dgvPhieuNhap.CurrentRow.Cells["MaPN"].Value);
            var bang = new DataTable();
            if (maPN.Length > 0)
            {
                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                using (var cmd = new SqlCommand(
                    "SELECT ct.MaNL, nl.TenNL, nl.DVT, ct.SoLuong, ct.DonGia, " +
                    "ct.SoLuong * ct.DonGia AS ThanhTien " +
                    "FROM ChiTietPhieuNhap ct INNER JOIN NguyenLieu nl ON nl.MaNL = ct.MaNL " +
                    "WHERE ct.MaPN = @MaPN ORDER BY ct.MaNL", conn))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.Add("@MaPN", SqlDbType.NVarChar, 10).Value = maPN;
                    da.Fill(bang);
                }
            }
            dgvChiTietLichSu.DataSource = bang;
            foreach (DataGridViewRow dong in dgvChiTietLichSu.Rows)
                dong.Height = 36;
            if (dgvChiTietLichSu.Columns.Contains("SoLuong"))
                dgvChiTietLichSu.Columns["SoLuong"].DefaultCellStyle.Format = "#,##0.##";
            GiaoDien.DinhDangTien(dgvChiTietLichSu, "DonGia", "ThanhTien");
        }

        private DataRow TimNguyenLieu(string maNL)
        {
            foreach (DataRow dong in nguyenLieu.Rows)
                if (Convert.ToString(dong["MaNL"]) == maNL) return dong;
            return null;
        }

        private static void CauHinhNut(Button nut, Color mau)
        {
            nut.FlatStyle = FlatStyle.Flat;
            nut.FlatAppearance.BorderSize = 0;
            nut.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(mau, 0.1F);
            nut.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(mau, 0.15F);
            nut.UseVisualStyleBackColor = false;
            nut.BackColor = mau;
            nut.ForeColor = Color.White;
            nut.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            nut.Cursor = Cursors.Hand;
        }

        private void dgv_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            e.PaintBackground(e.CellBounds, true);
            Rectangle vungChu = e.CellBounds;
            vungChu.X += e.CellStyle.Padding.Left;
            vungChu.Width -= e.CellStyle.Padding.Left + e.CellStyle.Padding.Right;
            TextFormatFlags canLe = e.CellStyle.Alignment == DataGridViewContentAlignment.MiddleRight
                ? TextFormatFlags.Right
                : TextFormatFlags.Left;
            TextRenderer.DrawText(e.Graphics, Convert.ToString(e.FormattedValue), e.CellStyle.Font,
                vungChu, e.CellStyle.ForeColor, canLe | TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            e.Paint(e.CellBounds, DataGridViewPaintParts.Border);
            e.Handled = true;
        }

        private void dgv_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            if (dgv != null && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                e.ToolTipText = Convert.ToString(
                    dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue);
            }
        }

        private static bool TryLaySo(string giaTri, out decimal so)
        {
            so = 0m;
            if (string.IsNullOrWhiteSpace(giaTri)) return false;
            string text = giaTri.Trim();
            if (text.Contains(",") && text.Contains(".")) text = text.Replace(".", "").Replace(',', '.');
            else if (text.Contains(",")) text = text.Replace(',', '.');
            return decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out so);
        }

        private static void GanDecimal(SqlCommand cmd, string ten, decimal giaTri, byte precision, byte scale)
        {
            var p = cmd.Parameters.Add(ten, SqlDbType.Decimal);
            p.Precision = precision;
            p.Scale = scale;
            p.Value = giaTri;
        }

        private static void BaoLoi(string noiDung)
        {
            MessageBox.Show(noiDung, "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private static void HienLoi(string noiDung, Exception ex)
        {
            MessageBox.Show(noiDung + ":\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
