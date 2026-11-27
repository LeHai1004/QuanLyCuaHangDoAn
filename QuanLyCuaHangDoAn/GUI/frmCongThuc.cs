using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using QuanLyCuaHangDoAn;
using QuanLyCuaHangDoAn.Class;

namespace QuanLyCuaHangDoAn.GUI
{
    public class frmCongThuc : Form
    {
        private readonly FileXml fx = new FileXml();
        private readonly ComboBox cboMonAn = new ComboBox();
        private readonly ComboBox cboNguyenLieu = new ComboBox();
        private readonly TextBox txtSoLuong = new TextBox();
        private readonly DataGridView dgvCongThuc = new DataGridView();
        private readonly ToolTip toolTipCongThuc = new ToolTip();
        private readonly Button btnThem = GiaoDien.TaoNut("Thêm", GiaoDien.MauChinh);
        private readonly Button btnSua = GiaoDien.TaoNut("Sửa", GiaoDien.MauChinh);
        private readonly Button btnXoa = GiaoDien.TaoNut("Xóa", GiaoDien.MauDo);
        private readonly Button btnLuu = GiaoDien.TaoNut("Lưu", GiaoDien.MauXanh);
        private readonly Button btnHuy = GiaoDien.TaoNut("Hủy", GiaoDien.MauChinhDam);
        private Label lblThongTinMon;
        private Label lblMonDangChon;
        private BadgeControl badgeKho;
        private DataTable monAn;
        private DataTable nguyenLieu;
        private DataTable congThuc;
        private string cheDo = "";
        private string maNLCu = "";
        private bool choDongBo;
        private string hanhDongChoDongBo;

        public frmCongThuc()
        {
            KhoiTaoGiaoDien();
            Load += frmCongThuc_Load;
            FormClosed += (sender, e) => toolTipCongThuc.Dispose();
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Quản lý công thức món";
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 246, 248);
            Font = new Font("Segoe UI", 10F);
            ClientSize = new Size(1100, 680);
            DoubleBuffered = true;

            cboMonAn.Name = "cboMonAn";
            cboNguyenLieu.Name = "cboNguyenLieu";
            txtSoLuong.Name = "txtSoLuong";
            dgvCongThuc.Name = "dgvCongThuc";
            btnThem.Name = "btnThem";
            btnSua.Name = "btnSua";
            btnXoa.Name = "btnXoa";
            btnLuu.Name = "btnLuu";
            btnHuy.Name = "btnHuy";
            cboMonAn.DropDownStyle = ComboBoxStyle.DropDownList;
            cboNguyenLieu.DropDownStyle = ComboBoxStyle.DropDownList;
            cboMonAn.SelectedIndexChanged += cboMonAn_SelectedIndexChanged;
            dgvCongThuc.SelectionChanged += dgvCongThuc_SelectionChanged;
            dgvCongThuc.CellPainting += dgvCongThuc_CellPainting;
            dgvCongThuc.CellFormatting += dgvCongThuc_CellFormatting;
            dgvCongThuc.CellToolTipTextNeeded += dgvCongThuc_CellToolTipTextNeeded;

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
                Text = "CÔNG THỨC MÓN",
                Location = new Point(30, 12),
                Size = new Size(650, 32),
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false
            });
            pnlHeader.Controls.Add(new Label
            {
                Name = "lblPhuDe",
                Text = "Định lượng nguyên liệu cho mỗi phần",
                Location = new Point(32, 50),
                Size = new Size(500, 20),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.White,
                AutoSize = false
            });

            var pnlTrai = new BorderPanel
            {
                Name = "pnlCongThuc",
                Location = new Point(30, 100),
                Size = new Size(690, 550),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackColor = Color.White
            };
            pnlTrai.Controls.Add(new Label
            {
                Name = "lblChonMon",
                Text = "Chọn món",
                Location = new Point(20, 15),
                Size = new Size(650, 16),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize = false
            });
            cboMonAn.Location = new Point(20, 33);
            cboMonAn.Size = new Size(650, 36);
            cboMonAn.Font = new Font("Segoe UI", 11F);
            pnlTrai.Controls.Add(cboMonAn);

            var pnlThongTinMon = new Panel
            {
                Name = "pnlThongTinMon",
                Location = new Point(20, 85),
                Size = new Size(650, 56),
                BackColor = Color.FromArgb(255, 243, 234)
            };
            lblThongTinMon = new Label
            {
                Name = "lblThongTinMon",
                Text = "Danh mục: -- • Đơn giá: --",
                Location = new Point(12, 0),
                Size = new Size(460, 56),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(55, 65, 81),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };
            pnlThongTinMon.Controls.Add(lblThongTinMon);
            badgeKho = new BadgeControl
            {
                Name = "badgeKho",
                Location = new Point(488, 15),
                Size = new Size(150, 26)
            };
            pnlThongTinMon.Controls.Add(badgeKho);
            pnlTrai.Controls.Add(pnlThongTinMon);

            CauHinhBang();
            dgvCongThuc.Location = new Point(20, 155);
            dgvCongThuc.Size = new Size(650, 375);
            dgvCongThuc.Anchor = AnchorStyles.Top | AnchorStyles.Left |
                AnchorStyles.Right | AnchorStyles.Bottom;
            pnlTrai.Controls.Add(dgvCongThuc);

            var pnlPhai = new BorderPanel
            {
                Name = "pnlNhapCongThuc",
                Location = new Point(745, 100),
                Size = new Size(325, 550),
                Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom,
                BackColor = Color.White
            };
            pnlPhai.Controls.Add(new Label
            {
                Name = "lblTieuDeNhap",
                Text = "THÊM / SỬA NGUYÊN LIỆU",
                Location = new Point(20, 15),
                Size = new Size(285, 22),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(232, 101, 26),
                AutoSize = false
            });
            lblMonDangChon = new Label
            {
                Name = "lblMonDangChon",
                Text = "Món đang chọn: --",
                Location = new Point(20, 45),
                Size = new Size(285, 24),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(55, 65, 81),
                AutoEllipsis = true
            };
            pnlPhai.Controls.Add(lblMonDangChon);

            ThemTruong(pnlPhai, "Nguyên liệu", cboNguyenLieu, 85, 34);
            ThemTruong(pnlPhai, "Số lượng / 1 phần (*)", txtSoLuong, 145, 34);

            var tlpNut = new TableLayoutPanel
            {
                Name = "tlpNut",
                Location = new Point(20, 215),
                Size = new Size(285, 96),
                ColumnCount = 3,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Color.White
            };
            for (int i = 0; i < 3; i++)
                tlpNut.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / 3F));
            tlpNut.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tlpNut.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            CauHinhNut(btnThem, Color.FromArgb(232, 101, 26));
            CauHinhNut(btnSua, Color.FromArgb(37, 99, 235));
            CauHinhNut(btnXoa, Color.FromArgb(220, 38, 38));
            CauHinhNut(btnLuu, Color.FromArgb(22, 163, 74));
            CauHinhNut(btnHuy, Color.FromArgb(107, 114, 128));
            foreach (Button btn in new[] { btnThem, btnSua, btnXoa, btnLuu, btnHuy })
            {
                btn.Dock = DockStyle.Fill;
                btn.Margin = new Padding(4);
            }
            tlpNut.Controls.Add(btnThem, 0, 0);
            tlpNut.Controls.Add(btnSua, 1, 0);
            tlpNut.Controls.Add(btnXoa, 2, 0);
            tlpNut.Controls.Add(btnLuu, 0, 1);
            tlpNut.SetColumnSpan(btnLuu, 2);
            tlpNut.Controls.Add(btnHuy, 2, 1);
            pnlPhai.Controls.Add(tlpNut);

            Controls.Add(pnlTrai);
            Controls.Add(pnlPhai);
            Controls.Add(pnlHeader);
            pnlHeader.BringToFront();

            btnThem.Click += btnThem_Click;
            btnSua.Click += btnSua_Click;
            btnXoa.Click += btnXoa_Click;
            btnLuu.Click += btnLuu_Click;
            btnHuy.Click += btnHuy_Click;
            CapNhatTrangThaiNut();
        }

        private void CauHinhBang()
        {
            dgvCongThuc.Name = "dgvCongThuc";
            dgvCongThuc.BorderStyle = BorderStyle.None;
            dgvCongThuc.BackgroundColor = Color.White;
            dgvCongThuc.EnableHeadersVisualStyles = false;
            dgvCongThuc.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvCongThuc.GridColor = Color.FromArgb(243, 244, 246);
            dgvCongThuc.RowHeadersVisible = false;
            dgvCongThuc.AllowUserToAddRows = false;
            dgvCongThuc.AllowUserToDeleteRows = false;
            dgvCongThuc.AllowUserToResizeRows = false;
            dgvCongThuc.ReadOnly = true;
            dgvCongThuc.MultiSelect = false;
            dgvCongThuc.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCongThuc.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCongThuc.AutoGenerateColumns = false;
            dgvCongThuc.ColumnHeadersHeight = 38;
            dgvCongThuc.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvCongThuc.RowTemplate.Height = 44;
            dgvCongThuc.ShowCellToolTips = true;
            dgvCongThuc.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 243, 234);
            dgvCongThuc.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(154, 52, 18);
            dgvCongThuc.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvCongThuc.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgvCongThuc.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 4, 0);
            dgvCongThuc.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 243, 234);
            dgvCongThuc.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(154, 52, 18);
            dgvCongThuc.DefaultCellStyle.BackColor = Color.White;
            dgvCongThuc.DefaultCellStyle.ForeColor = Color.FromArgb(17, 24, 39);
            dgvCongThuc.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvCongThuc.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgvCongThuc.DefaultCellStyle.Padding = new Padding(8, 0, 4, 0);
            dgvCongThuc.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 243, 234);
            dgvCongThuc.DefaultCellStyle.SelectionForeColor = Color.FromArgb(17, 24, 39);
            dgvCongThuc.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            dgvCongThuc.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 243, 234);
            dgvCongThuc.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.FromArgb(17, 24, 39);

            dgvCongThuc.Columns.Add(TaoCot("MaNL", "Mã NL", 14F));
            dgvCongThuc.Columns.Add(TaoCot("TenNL", "Tên nguyên liệu", 30F));
            dgvCongThuc.Columns.Add(TaoCot("DVT", "ĐVT", 12F));
            dgvCongThuc.Columns.Add(TaoCotSo("SoLuong", "SL / phần", 16F, true));
            dgvCongThuc.Columns.Add(TaoCotSo("TonKho", "Tồn kho", 14F, true));
            dgvCongThuc.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "LamDuoc",
                DataPropertyName = "LamDuoc",
                HeaderText = "Làm được",
                FillWeight = 14F,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight }
            });
        }

        private static DataGridViewTextBoxColumn TaoCot(string ten, string tieuDe, float tiLe)
        {
            return new DataGridViewTextBoxColumn
            {
                Name = ten,
                DataPropertyName = ten,
                HeaderText = tieuDe,
                FillWeight = tiLe,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
        }

        private static DataGridViewTextBoxColumn TaoCotSo(string ten, string tieuDe,
            float tiLe, bool canPhai)
        {
            return new DataGridViewTextBoxColumn
            {
                Name = ten,
                DataPropertyName = ten,
                HeaderText = tieuDe,
                FillWeight = tiLe,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle =
                {
                    Alignment = canPhai ? DataGridViewContentAlignment.MiddleRight : DataGridViewContentAlignment.MiddleLeft,
                    Format = "#,0.###",
                    FormatProvider = CultureInfo.GetCultureInfo("vi-VN")
                }
            };
        }

        private static void ThemTruong(Panel panel, string nhan, Control control, int y, int chieuCao)
        {
            panel.Controls.Add(new Label
            {
                Name = "lbl" + control.Name.Substring(3),
                Text = nhan,
                Location = new Point(20, y),
                Size = new Size(285, 16),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize = false
            });

            control.Location = new Point(20, y + 20);
            control.Size = new Size(285, chieuCao);
            control.Font = new Font("Segoe UI", 10.5F);
            if (control is TextBox textBox)
            {
                textBox.BorderStyle = BorderStyle.FixedSingle;
                textBox.AutoSize = false;
                textBox.BackColor = Color.White;
            }
            panel.Controls.Add(control);
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

        private void frmCongThuc_Load(object sender, EventArgs e)
        {
            if (!PhienDangNhap.IsLoggedIn || PhienDangNhap.ChucVu != "Admin")
            {
                MessageBox.Show("Chức năng chỉ dành cho quản trị viên (Admin).", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }
            try { TaiDuLieu(); }
            catch (Exception ex) { HienLoi("Không thể tải công thức món", ex); }
        }

        private void TaiDuLieu()
        {
            fx.TaoXml("SELECT * FROM MonAn", "MonAn", "MonAn.xml");
            fx.TaoXml("SELECT * FROM NguyenLieu", "NguyenLieu", "NguyenLieu.xml");
            fx.TaoXml("SELECT * FROM CongThuc", "CongThuc", "CongThuc.xml");
            monAn = fx.HienThi("MonAn.xml");
            nguyenLieu = fx.HienThi("NguyenLieu.xml");
            congThuc = fx.HienThi("CongThuc.xml");
            cboMonAn.DataSource = monAn;
            cboMonAn.DisplayMember = "TenMon";
            cboMonAn.ValueMember = "MaMon";
            cboNguyenLieu.DataSource = TaoBangNguyenLieuHienThi();
            cboNguyenLieu.DisplayMember = "TenHienThi";
            cboNguyenLieu.ValueMember = "MaNL";
            HienThiCongThuc();
            cheDo = "";
        }

        private DataTable TaoBangNguyenLieuHienThi()
        {
            var bang = nguyenLieu.Copy();
            bang.Columns.Add("TenHienThi", typeof(string));
            foreach (DataRow dong in bang.Rows)
                dong["TenHienThi"] = dong["TenNL"] + " (" + dong["DVT"] + ")";
            return bang;
        }

        private void HienThiCongThuc()
        {
            string maMon = cboMonAn.SelectedValue == null ? "" : cboMonAn.SelectedValue.ToString();
            var hienThi = new DataTable();
            hienThi.Columns.Add("MaNL", typeof(string));
            hienThi.Columns.Add("TenNL", typeof(string));
            hienThi.Columns.Add("DVT", typeof(string));
            hienThi.Columns.Add("SoLuong", typeof(decimal));
            hienThi.Columns.Add("TonKho", typeof(decimal));
            hienThi.Columns.Add("LamDuoc", typeof(decimal));

            foreach (DataRow ct in congThuc.Rows)
            {
                if (Convert.ToString(ct["MaMon"]) != maMon) continue;
                DataRow nl = null;
                foreach (DataRow ung in nguyenLieu.Rows)
                {
                    if (Convert.ToString(ung["MaNL"]) == Convert.ToString(ct["MaNL"]))
                    {
                        nl = ung;
                        break;
                    }
                }

                DataRow dong = hienThi.NewRow();
                dong["MaNL"] = ct["MaNL"];
                dong["TenNL"] = nl == null ? "" : nl["TenNL"];
                dong["DVT"] = nl == null ? "" : nl["DVT"];
                decimal soLuong;
                bool coSoLuong = ThuLaySo(ct["SoLuong"], out soLuong);
                dong["SoLuong"] = coSoLuong ? (object)soLuong : DBNull.Value;
                decimal tonKho = 0m;
                bool coTonKho = nl != null && ThuLaySo(nl["TonKho"], out tonKho);
                dong["TonKho"] = coTonKho ? (object)tonKho : DBNull.Value;
                dong["LamDuoc"] = coSoLuong && soLuong > 0m && coTonKho
                    ? (object)Math.Floor(tonKho / soLuong)
                    : DBNull.Value;
                hienThi.Rows.Add(dong);
            }

            dgvCongThuc.DataSource = hienThi;
            foreach (DataGridViewRow dong in dgvCongThuc.Rows)
                dong.Height = 44;
            if (dgvCongThuc.CurrentRow == null && string.IsNullOrEmpty(cheDo))
            {
                maNLCu = "";
                cboNguyenLieu.SelectedIndex = -1;
                txtSoLuong.Clear();
            }
            CapNhatThongTinMon();
            CapNhatTrangThaiNut();
        }

        private void cboMonAn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (congThuc != null) HienThiCongThuc();
        }

        private void CapNhatThongTinMon()
        {
            DataRowView monDangChon = cboMonAn.SelectedItem as DataRowView;
            if (monDangChon == null)
            {
                lblThongTinMon.Text = "Danh mục: -- • Đơn giá: --";
                lblMonDangChon.Text = "Món đang chọn: --";
                badgeKho.DatTrangThai("Chưa có công thức", Color.FromArgb(107, 114, 128));
                toolTipCongThuc.SetToolTip(lblThongTinMon, "");
                toolTipCongThuc.SetToolTip(lblMonDangChon, "");
                toolTipCongThuc.SetToolTip(badgeKho, "");
                return;
            }

            DataRow mon = monDangChon.Row;
            string maMon = Convert.ToString(mon["MaMon"]);
            string tenMon = Convert.ToString(mon["TenMon"]);
            string maDanhMuc = mon.Table.Columns.Contains("MaDM")
                ? Convert.ToString(mon["MaDM"])
                : "--";
            decimal donGia;
            string giaHienThi = ThuLaySo(mon["DonGia"], out donGia)
                ? donGia.ToString("#,##0", CultureInfo.GetCultureInfo("vi-VN")) + " đ"
                : "--";
            lblThongTinMon.Text = "Danh mục: " + maDanhMuc + " • Đơn giá: " + giaHienThi;
            lblMonDangChon.Text = "Món đang chọn: " + tenMon;
            toolTipCongThuc.SetToolTip(lblThongTinMon, lblThongTinMon.Text);
            toolTipCongThuc.SetToolTip(lblMonDangChon, lblMonDangChon.Text);

            if (dgvCongThuc.Rows.Count == 0)
            {
                badgeKho.DatTrangThai("Chưa có công thức", Color.FromArgb(107, 114, 128));
                toolTipCongThuc.SetToolTip(badgeKho, "");
                return;
            }

            try
            {
                bool duNguyenLieu = Kho.DuNguyenLieu(maMon, 1);
                badgeKho.DatTrangThai(
                    duNguyenLieu ? "Đủ nguyên liệu" : "Thiếu nguyên liệu",
                    duNguyenLieu ? Color.FromArgb(22, 163, 74) : Color.FromArgb(220, 38, 38));
                toolTipCongThuc.SetToolTip(badgeKho, "");
            }
            catch (Exception ex)
            {
                badgeKho.DatTrangThai("Không kiểm tra được", Color.FromArgb(107, 114, 128));
                toolTipCongThuc.SetToolTip(badgeKho, ex.Message);
            }
        }

        private static bool ThuLaySo(object giaTri, out decimal so)
        {
            so = 0m;
            if (giaTri == null || giaTri == DBNull.Value)
                return false;
            if (giaTri is decimal giaTriDecimal)
            {
                so = giaTriDecimal;
                return true;
            }
            return decimal.TryParse(Convert.ToString(giaTri), NumberStyles.Any,
                CultureInfo.InvariantCulture, out so)
                || decimal.TryParse(Convert.ToString(giaTri), NumberStyles.Any,
                    CultureInfo.CurrentCulture, out so);
        }

        private void dgvCongThuc_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0
                || dgvCongThuc.Columns[e.ColumnIndex].Name != "LamDuoc")
                return;

            if (e.Value == null || e.Value == DBNull.Value)
            {
                e.Value = "--";
                e.FormattingApplied = true;
                return;
            }

            decimal soPhan;
            if (!ThuLaySo(e.Value, out soPhan))
                return;
            e.Value = "~" + soPhan.ToString("0", CultureInfo.InvariantCulture) + " phần";
            if (soPhan == 0m)
                e.CellStyle.ForeColor = Color.FromArgb(220, 38, 38);
            e.FormattingApplied = true;
        }

        private void dgvCongThuc_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
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

        private void dgvCongThuc_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                e.ToolTipText = Convert.ToString(
                    dgvCongThuc.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue);
            }
        }

        private void dgvCongThuc_SelectionChanged(object sender, EventArgs e)
        {
            if (cheDo != "" || dgvCongThuc.CurrentRow == null) return;
            maNLCu = Convert.ToString(dgvCongThuc.CurrentRow.Cells["MaNL"].Value);
            cboNguyenLieu.SelectedValue = maNLCu;
            txtSoLuong.Text = Convert.ToString(dgvCongThuc.CurrentRow.Cells["SoLuong"].Value, CultureInfo.InvariantCulture);
            CapNhatTrangThaiNut();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (choDongBo)
                return;
            cheDo = "them";
            maNLCu = "";
            txtSoLuong.Clear();
            cboNguyenLieu.SelectedIndex = -1;
            CapNhatTrangThaiNut();
            txtSoLuong.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (choDongBo)
                return;
            if (dgvCongThuc.CurrentRow == null) { BaoLoi("Vui lòng chọn công thức cần sửa."); return; }
            cheDo = "sua";
            maNLCu = Convert.ToString(dgvCongThuc.CurrentRow.Cells["MaNL"].Value);
            CapNhatTrangThaiNut();
            txtSoLuong.Focus();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (choDongBo)
                return;
            if (dgvCongThuc.CurrentRow == null) { BaoLoi("Vui lòng chọn công thức cần xóa."); return; }
            if (cboMonAn.SelectedValue == null) { BaoLoi("Vui lòng chọn món ăn cần sửa."); return; }
            string maMon = cboMonAn.SelectedValue.ToString();
            string maNL = Convert.ToString(dgvCongThuc.CurrentRow.Cells["MaNL"].Value);
            if (MessageBox.Show("Bạn có chắc muốn xóa nguyên liệu này khỏi công thức?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                if (fx.Xoa("CongThuc.xml", new Dictionary<string, string>
                    {
                        { "MaMon", maMon },
                        { "MaNL", maNL }
                    }) == 0)
                    throw new InvalidOperationException("Không tìm thấy dòng công thức cần xóa trong XML.");
                BatDauDongBo("xóa công thức");
            }
            catch (Exception ex) { HienLoi("Không thể xóa công thức", ex); }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (choDongBo)
            {
                ThuDongBo();
                return;
            }
            if (cheDo == "") { BaoLoi("Vui lòng chọn Thêm hoặc Sửa trước khi lưu."); return; }
            if (cboMonAn.SelectedValue == null || cboNguyenLieu.SelectedValue == null) { BaoLoi("Vui lòng chọn món ăn và nguyên liệu."); return; }
            decimal soLuong;
            string text = txtSoLuong.Text.Trim().Replace(',', '.');
            if (!decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out soLuong) || soLuong <= 0m)
            { BaoLoi("Số lượng định mức phải là số thực lớn hơn 0."); return; }
            string maMon = cboMonAn.SelectedValue.ToString();
            string maNL = cboNguyenLieu.SelectedValue.ToString();
            if (CoNguyenLieu(maMon, maNL, cheDo == "sua" ? maNLCu : null))
            { BaoLoi("Nguyên liệu đã có trong công thức. Hãy dùng nút Sửa để thay đổi số lượng."); return; }
            try
            {
                if (cheDo == "them")
                    fx.Them("CongThuc.xml", "CongThuc", TaoGiaTriCongThuc(maMon, maNL, soLuong));
                else
                {
                    if (fx.Sua("CongThuc.xml",
                        new Dictionary<string, string> { { "MaMon", maMon }, { "MaNL", maNLCu } },
                        new Dictionary<string, string>
                        {
                            { "MaNL", maNL },
                            { "SoLuong", soLuong.ToString(CultureInfo.InvariantCulture) }
                        }) == 0)
                        throw new InvalidOperationException("Không tìm thấy dòng công thức cần sửa trong XML.");
                }
                BatDauDongBo(cheDo == "them" ? "thêm công thức" : "sửa công thức");
            }
            catch (Exception ex) { HienLoi("Không thể lưu công thức", ex); }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            if (choDongBo)
                return;
            cheDo = "";
            maNLCu = "";
            txtSoLuong.Clear();
            HienThiCongThuc();
            CapNhatTrangThaiNut();
        }

        private bool CoNguyenLieu(string maMon, string maNL, string maNLBoQua = null)
        {
            if (congThuc == null)
                return false;
            foreach (DataRow dong in congThuc.Rows)
            {
                if (Convert.ToString(dong["MaMon"]) == maMon
                    && Convert.ToString(dong["MaNL"]) == maNL
                    && Convert.ToString(dong["MaNL"]) != maNLBoQua)
                    return true;
            }
            return false;
        }

        private static IDictionary<string, string> TaoGiaTriCongThuc(string maMon, string maNL, decimal soLuong)
        {
            string[] thuTuCot = { "MaMon", "MaNL", "SoLuong" };
            var giaTri = new SortedDictionary<string, string>(Comparer<string>.Create((a, b) =>
                Array.IndexOf(thuTuCot, a).CompareTo(Array.IndexOf(thuTuCot, b))));
            giaTri.Add("MaMon", maMon);
            giaTri.Add("MaNL", maNL);
            giaTri.Add("SoLuong", soLuong.ToString(CultureInfo.InvariantCulture));
            return giaTri;
        }

        private void BatDauDongBo(string hanhDong)
        {
            choDongBo = true;
            hanhDongChoDongBo = hanhDong;
            CapNhatTrangThaiNut();
            ThuDongBo();
        }

        private void ThuDongBo()
        {
            try
            {
                fx.CapNhatDb("CongThuc", "CongThuc.xml");
            }
            catch (Exception ex)
            {
                CapNhatTrangThaiNut();
                HienLoi("XML đã thay đổi nhưng chưa đồng bộ được công thức vào CSDL. Nhấn Đồng bộ để thử lại", ex);
                return;
            }

            choDongBo = false;
            hanhDongChoDongBo = null;
            cheDo = "";
            maNLCu = "";
            try { TaiDuLieu(); }
            catch (Exception ex) { HienLoi("Công thức đã đồng bộ nhưng không thể tải lại dữ liệu", ex); }
            CapNhatTrangThaiNut();
        }

        private void CapNhatTrangThaiNut()
        {
            bool coTheChinhSua = !choDongBo && cheDo != "";
            bool binhThuong = !choDongBo && cheDo == "";
            cboMonAn.Enabled = binhThuong;
            cboNguyenLieu.Enabled = coTheChinhSua;
            txtSoLuong.ReadOnly = !coTheChinhSua;
            txtSoLuong.BackColor = coTheChinhSua ? Color.White : Color.FromArgb(243, 244, 246);
            dgvCongThuc.Enabled = binhThuong;
            btnThem.Enabled = binhThuong && cboMonAn.SelectedValue != null;
            btnSua.Enabled = binhThuong && dgvCongThuc.CurrentRow != null;
            btnXoa.Enabled = binhThuong && dgvCongThuc.CurrentRow != null;
            btnLuu.Enabled = coTheChinhSua || choDongBo;
            btnHuy.Enabled = coTheChinhSua;
            btnLuu.Text = choDongBo ? "Đồng bộ" : "Lưu";
            Text = choDongBo ? "Quản lý công thức món - chờ đồng bộ " + hanhDongChoDongBo : "Quản lý công thức món";
        }

        private static void BaoLoi(string noiDung) { MessageBox.Show(noiDung, "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        private static void HienLoi(string noiDung, Exception ex) { MessageBox.Show(noiDung + ":\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }

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

        private sealed class BadgeControl : Control
        {
            private Color mauNen = Color.FromArgb(107, 114, 128);

            public BadgeControl()
            {
                Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                ForeColor = Color.White;
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            }

            public void DatTrangThai(string noiDung, Color mau)
            {
                Text = noiDung;
                mauNen = mau;
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                Rectangle hinh = new Rectangle(0, 0, Width - 1, Height - 1);
                using (System.Drawing.Drawing2D.GraphicsPath duongVien = TaoDuongVien(hinh))
                using (SolidBrush nen = new SolidBrush(mauNen))
                    e.Graphics.FillPath(nen, duongVien);
                TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, ForeColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
            }

            private System.Drawing.Drawing2D.GraphicsPath TaoDuongVien(Rectangle hinh)
            {
                int duongKinh = Math.Min(hinh.Height, hinh.Width);
                var duongVien = new System.Drawing.Drawing2D.GraphicsPath();
                duongVien.AddArc(hinh.Left, hinh.Top, duongKinh, duongKinh, 180, 90);
                duongVien.AddArc(hinh.Right - duongKinh, hinh.Top, duongKinh, duongKinh, 270, 90);
                duongVien.AddArc(hinh.Right - duongKinh, hinh.Bottom - duongKinh, duongKinh, duongKinh, 0, 90);
                duongVien.AddArc(hinh.Left, hinh.Bottom - duongKinh, duongKinh, duongKinh, 90, 90);
                duongVien.CloseFigure();
                return duongVien;
            }
        }
    }
}
