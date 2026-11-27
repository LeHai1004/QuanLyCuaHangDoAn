using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyCuaHangDoAn
{
    public class frmMonAn : Form
    {
        private readonly FileXml fx = new FileXml();
        private readonly DataGridView dgv = new DataGridView();
        private readonly TextBox txtMaMon = new TextBox();
        private readonly TextBox txtTenMon = new TextBox();
        private readonly TextBox txtDVT = new TextBox();
        private readonly TextBox txtTim = new TextBox();
        private readonly ComboBox cboDanhMuc = new ComboBox();
        private readonly ComboBox cboTrangThai = new ComboBox();
        private readonly NumericUpDown nudDonGia = new NumericUpDown();
        private readonly Button btnThem = GiaoDien.TaoNut("Thêm", GiaoDien.MauChinh);
        private readonly Button btnSua = GiaoDien.TaoNut("Sửa", GiaoDien.MauChinh);
        private readonly Button btnXoa = GiaoDien.TaoNut("Xóa", GiaoDien.MauDo);
        private readonly Button btnLuu = GiaoDien.TaoNut("Lưu", GiaoDien.MauXanh);
        private readonly Button btnHuy = GiaoDien.TaoNut("Hủy", GiaoDien.MauChinhDam);
        private readonly Button btnTim = GiaoDien.TaoNut("Tìm", GiaoDien.MauChinh);

        private DataTable duLieu;
        private readonly Dictionary<string, string> tenDanhMucTheoMa =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private Label lblTongMon;
        private Label lblConBan;
        private Label lblHet;
        private Label lblGiaTrungBinh;
        private string cheDo = "";
        private bool choDongBo;
        private string hanhDongChoDongBo;

        public frmMonAn()
        {
            KhoiTaoGiaoDien();
            Load += frmMonAn_Load;
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Quản lý món ăn";
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 246, 248);
            Font = new Font("Segoe UI", 10F);
            ClientSize = new Size(1100, 680);
            DoubleBuffered = true;

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
                Text = "QUẢN LÝ MÓN ĂN",
                Location = new Point(30, 12),
                Size = new Size(650, 32),
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false
            });
            pnlHeader.Controls.Add(new Label
            {
                Name = "lblPhuDe",
                Text = "Thực đơn, giá và trạng thái bán",
                Location = new Point(32, 50),
                Size = new Size(500, 20),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.White,
                AutoSize = false
            });

            var pnlTrai = new BorderPanel
            {
                Name = "pnlDanhSach",
                Location = new Point(30, 100),
                Size = new Size(690, 550),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackColor = Color.White
            };
            var tlpThongKe = new TableLayoutPanel
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
                tlpThongKe.ColumnStyles.Add(i % 2 == 0
                    ? new ColumnStyle(SizeType.Percent, 25F)
                    : new ColumnStyle(SizeType.Absolute, 12F));
            tlpThongKe.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpThongKe.Controls.Add(TaoTheThongKe("pnlThongKeTongMon", "Tổng món", "lblTongMon",
                Color.FromArgb(17, 24, 39)), 0, 0);
            tlpThongKe.Controls.Add(TaoTheThongKe("pnlThongKeConBan", "Còn bán", "lblConBan",
                Color.FromArgb(22, 163, 74)), 2, 0);
            tlpThongKe.Controls.Add(TaoTheThongKe("pnlThongKeHet", "Hết", "lblHet",
                Color.FromArgb(220, 38, 38)), 4, 0);
            tlpThongKe.Controls.Add(TaoTheThongKe("pnlThongKeGiaTrungBinh", "Giá trung bình",
                "lblGiaTrungBinh", Color.FromArgb(232, 101, 26)), 6, 0);
            pnlTrai.Controls.Add(tlpThongKe);

            txtTim.Name = "txtTim";
            txtTim.Location = new Point(20, 95);
            txtTim.Size = new Size(545, 36);
            txtTim.AutoSize = false;
            txtTim.BorderStyle = BorderStyle.FixedSingle;
            txtTim.Font = new Font("Segoe UI", 11F);
            txtTim.BackColor = Color.White;
            pnlTrai.Controls.Add(txtTim);

            btnTim.Name = "btnTim";
            CauHinhNut(btnTim, "btnTim", Color.FromArgb(232, 101, 26));
            btnTim.Location = new Point(575, 95);
            btnTim.Size = new Size(95, 36);
            pnlTrai.Controls.Add(btnTim);

            dgv.Name = "dgvMonAn";
            dgv.Location = new Point(20, 145);
            dgv.Size = new Size(650, 385);
            dgv.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
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
            dgv.ColumnHeadersHeight = 38;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.RowTemplate.Height = 44;
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
            dgv.SelectionChanged += dgv_SelectionChanged;
            dgv.CellFormatting += dgv_CellFormatting;
            dgv.CellPainting += dgv_CellPainting;
            dgv.CellToolTipTextNeeded += dgv_CellToolTipTextNeeded;
            pnlTrai.Controls.Add(dgv);

            var pnlPhai = new BorderPanel
            {
                Name = "pnlThongTin",
                Location = new Point(745, 100),
                Size = new Size(325, 550),
                Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom,
                BackColor = Color.White
            };
            pnlPhai.Controls.Add(new Label
            {
                Name = "lblThongTinMonAn",
                Text = "THÔNG TIN MÓN ĂN",
                Location = new Point(20, 15),
                Size = new Size(285, 22),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(232, 101, 26),
                AutoSize = false
            });
            ThemTruong(pnlPhai, "lblMaMon", "Mã món", txtMaMon, 55, true);
            ThemTruong(pnlPhai, "lblTenMon", "Tên món", txtTenMon, 115, false);
            ThemTruong(pnlPhai, "lblDanhMuc", "Danh mục", cboDanhMuc, 175, false);
            ThemTruong(pnlPhai, "lblDonGia", "Đơn giá", nudDonGia, 235, false);
            ThemTruong(pnlPhai, "lblDVT", "Đơn vị tính", txtDVT, 295, false);
            ThemTruong(pnlPhai, "lblTrangThai", "Trạng thái", cboTrangThai, 355, false);

            cboDanhMuc.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDanhMuc.FlatStyle = FlatStyle.Flat;
            cboDanhMuc.Font = new Font("Segoe UI", 10.5F);
            cboDanhMuc.BackColor = Color.White;
            cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTrangThai.FlatStyle = FlatStyle.Flat;
            cboTrangThai.Font = new Font("Segoe UI", 10.5F);
            cboTrangThai.BackColor = Color.White;
            cboTrangThai.Items.AddRange(new object[] { "Còn bán", "Hết" });
            nudDonGia.Minimum = 0;
            nudDonGia.Maximum = 999999999999999999m;
            nudDonGia.DecimalPlaces = 0;
            nudDonGia.ThousandsSeparator = true;
            nudDonGia.Font = new Font("Segoe UI", 10.5F);
            nudDonGia.BorderStyle = BorderStyle.FixedSingle;

            var tlpNut = new TableLayoutPanel
            {
                Name = "tlpNutMonAn",
                Location = new Point(20, 425),
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
            CauHinhNut(btnThem, "btnThem", Color.FromArgb(232, 101, 26));
            CauHinhNut(btnSua, "btnSua", Color.FromArgb(37, 99, 235));
            CauHinhNut(btnXoa, "btnXoa", Color.FromArgb(220, 38, 38));
            CauHinhNut(btnLuu, "btnLuu", Color.FromArgb(22, 163, 74));
            CauHinhNut(btnHuy, "btnHuy", Color.FromArgb(107, 114, 128));
            GanNut(tlpNut, btnThem, 0, 0, 1);
            GanNut(tlpNut, btnSua, 1, 0, 1);
            GanNut(tlpNut, btnXoa, 2, 0, 1);
            GanNut(tlpNut, btnLuu, 0, 2, 2);
            GanNut(tlpNut, btnHuy, 2, 2, 1);
            pnlPhai.Controls.Add(tlpNut);

            cboDanhMuc.MaxDropDownItems = 12;
            Controls.Add(pnlTrai);
            Controls.Add(pnlPhai);
            Controls.Add(pnlHeader);

            btnThem.Click += btnThem_Click;
            btnSua.Click += btnSua_Click;
            btnXoa.Click += btnXoa_Click;
            btnLuu.Click += btnLuu_Click;
            btnHuy.Click += btnHuy_Click;
            btnTim.Click += btnTim_Click;
            txtTim.KeyDown += txtTim_KeyDown;
            CapNhatTrangThaiNut();
        }

        private Panel TaoTheThongKe(string tenPanel, string nhan, string tenGiaTri, Color mauGiaTri)
        {
            var panel = new Panel
            {
                Name = tenPanel,
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                BackColor = Color.FromArgb(249, 250, 251)
            };
            var giaTri = new Label
            {
                Name = tenGiaTri,
                Text = "0",
                Location = new Point(4, 2),
                Size = new Size(145, 32),
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = mauGiaTri,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                AutoEllipsis = true
            };
            var lblNhan = new Label
            {
                Name = "lblNhan" + tenGiaTri.Substring(3),
                Text = nhan,
                Location = new Point(4, 34),
                Size = new Size(145, 20),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(107, 114, 128),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                AutoEllipsis = true
            };
            panel.Controls.Add(giaTri);
            panel.Controls.Add(lblNhan);
            if (tenGiaTri == "lblTongMon")
                lblTongMon = giaTri;
            else if (tenGiaTri == "lblConBan")
                lblConBan = giaTri;
            else if (tenGiaTri == "lblHet")
                lblHet = giaTri;
            else
                lblGiaTrungBinh = giaTri;
            return panel;
        }

        private void ThemTruong(Panel panel, string tenNhan, string nhan, Control control, int y, bool chiDoc)
        {
            panel.Controls.Add(new Label
            {
                Name = tenNhan,
                Text = nhan,
                Location = new Point(20, y),
                Size = new Size(285, 16),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize = false
            });
            if (control == txtMaMon)
                control.Name = "txtMaMon";
            else if (control == txtTenMon)
                control.Name = "txtTenMon";
            else if (control == txtDVT)
                control.Name = "txtDVT";
            else if (control == cboDanhMuc)
                control.Name = "cboDanhMuc";
            else if (control == cboTrangThai)
                control.Name = "cboTrangThai";
            else if (control == nudDonGia)
                control.Name = "nudDonGia";
            control.Location = new Point(20, y + 20);
            control.Size = new Size(285, 34);
            if (control is TextBox)
            {
                TextBox textBox = (TextBox)control;
                textBox.AutoSize = false;
                textBox.BorderStyle = BorderStyle.FixedSingle;
                textBox.Font = new Font("Segoe UI", 10.5F);
                textBox.ReadOnly = chiDoc;
                textBox.BackColor = chiDoc ? Color.FromArgb(243, 244, 246) : Color.White;
            }
            else if (control is ComboBox)
            {
                ((ComboBox)control).DropDownStyle = ComboBoxStyle.DropDownList;
            }
            panel.Controls.Add(control);
        }

        private static void CauHinhNut(Button nut, string ten, Color nen)
        {
            nut.Name = ten;
            nut.Size = new Size(89, 40);
            nut.Margin = new Padding(4, 0, 4, 0);
            nut.BackColor = nen;
            nut.ForeColor = Color.White;
            nut.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            nut.FlatStyle = FlatStyle.Flat;
            nut.FlatAppearance.BorderSize = 0;
            nut.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(nen, 0.1F);
            nut.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(nen, 0.18F);
            nut.Cursor = Cursors.Hand;
            nut.UseVisualStyleBackColor = false;
        }

        private static void GanNut(TableLayoutPanel bangNut, Button nut, int cot, int dong, int soCot)
        {
            nut.Dock = DockStyle.Fill;
            bangNut.Controls.Add(nut, cot, dong);
            if (soCot > 1)
                bangNut.SetColumnSpan(nut, soCot);
        }

        private void frmMonAn_Load(object sender, EventArgs e)
        {
            try
            {
                TaiDuLieu();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể tải danh sách món ăn", ex);
            }
        }

        private void TaiDuLieu()
        {
            fx.TaoXml("SELECT * FROM DanhMuc", "DanhMuc", "DanhMuc.xml");
            fx.TaoXml("SELECT * FROM MonAn", "MonAn", "MonAn.xml");
            DataTable danhMuc = fx.HienThi("DanhMuc.xml");
            duLieu = fx.HienThi("MonAn.xml");
            tenDanhMucTheoMa.Clear();
            foreach (DataRow dong in danhMuc.Rows)
                tenDanhMucTheoMa[Convert.ToString(dong["MaDM"])] = Convert.ToString(dong["TenDM"]);
            cboDanhMuc.DataSource = danhMuc;
            cboDanhMuc.DisplayMember = "TenDM";
            cboDanhMuc.ValueMember = "MaDM";
            HienThiDuLieu(duLieu);
        }

        private void HienThiDuLieu(DataTable nguon)
        {
            dgv.DataSource = nguon.Copy();
            CauHinhCotBang();
            GiaoDien.DinhDangTien(dgv, "DonGia");
            foreach (DataGridViewRow dong in dgv.Rows)
                dong.Height = 44;
            CapNhatThongKe();
            CapNhatTrangThaiNut();
        }

        private void CauHinhCotBang()
        {
            string[] tenCot = { "MaMon", "TenMon", "MaDM", "DonGia", "DVT", "TrangThai" };
            string[] tieuDe = { "Mã món", "Tên món", "Danh mục", "Đơn giá", "ĐVT", "Trạng thái" };
            float[] trongSo = { 13F, 30F, 19F, 16F, 10F, 12F };
            for (int i = 0; i < tenCot.Length; i++)
            {
                if (!dgv.Columns.Contains(tenCot[i]))
                    continue;
                DataGridViewColumn cot = dgv.Columns[tenCot[i]];
                cot.HeaderText = tieuDe[i];
                cot.FillWeight = trongSo[i];
                cot.DisplayIndex = i;
                cot.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            if (dgv.Columns.Contains("TrangThai"))
                dgv.Columns["TrangThai"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            if (dgv.Columns.Contains("DonGia"))
            {
                dgv.Columns["DonGia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns["DonGia"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        private void CapNhatThongKe()
        {
            if (duLieu == null)
                return;

            int conBan = 0;
            int het = 0;
            decimal tongGia = 0;
            foreach (DataRow dong in duLieu.Rows)
            {
                string trangThai = Convert.ToString(dong["TrangThai"]);
                if (trangThai == "Còn bán")
                    conBan++;
                else if (trangThai == "Hết")
                    het++;
                tongGia += Convert.ToDecimal(dong["DonGia"]);
            }

            decimal giaTrungBinh = duLieu.Rows.Count == 0 ? 0 : tongGia / duLieu.Rows.Count;
            lblTongMon.Text = duLieu.Rows.Count.ToString();
            lblConBan.Text = conBan.ToString();
            lblHet.Text = het.ToString();
            lblGiaTrungBinh.Text = giaTrungBinh.ToString("N0", new CultureInfo("vi-VN")) + " đ";
        }

        private void dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string tenCot = dgv.Columns[e.ColumnIndex].Name;
            string trangThai = Convert.ToString(dgv.Rows[e.RowIndex].Cells["TrangThai"].Value);
            if (tenCot != "TrangThai")
            {
                Color mauChu = trangThai == "Hết"
                    ? Color.FromArgb(156, 163, 175)
                    : Color.FromArgb(17, 24, 39);
                e.CellStyle.ForeColor = mauChu;
                e.CellStyle.SelectionForeColor = mauChu;
            }

            if (tenCot == "MaDM")
            {
                string maDM = Convert.ToString(e.Value);
                string tenDM;
                if (tenDanhMucTheoMa.TryGetValue(maDM, out tenDM))
                    e.Value = tenDM;
                e.FormattingApplied = true;
            }
        }

        private void dgv_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dgv.Columns[e.ColumnIndex].Name == "TrangThai")
            {
                string trangThai = Convert.ToString(e.FormattedValue);
                Color mauNen = trangThai == "Còn bán"
                    ? Color.FromArgb(22, 163, 74)
                    : trangThai == "Hết"
                        ? Color.FromArgb(220, 38, 38)
                        : Color.FromArgb(107, 114, 128);

                e.PaintBackground(e.CellBounds, true);
                Rectangle vienThuoc = new Rectangle(
                    e.CellBounds.Left + (e.CellBounds.Width - 70) / 2,
                    e.CellBounds.Top + (e.CellBounds.Height - 22) / 2, 70, 22);
                GraphicsState trangThaiDoHoa = e.Graphics.Save();
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath duongVien = TaoDuongVien(vienThuoc, 22))
                using (SolidBrush brush = new SolidBrush(mauNen))
                    e.Graphics.FillPath(brush, duongVien);
                e.Graphics.Restore(trangThaiDoHoa);
                using (Font font = new Font("Segoe UI", 8.5F, FontStyle.Bold))
                    TextRenderer.DrawText(e.Graphics, trangThai, font, vienThuoc, Color.White,
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
            TextFormatFlags canLe = dgv.Columns[e.ColumnIndex].Name == "DonGia"
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
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string tenCot = dgv.Columns[e.ColumnIndex].Name;
            if (tenCot == "MaDM")
            {
                string maDM = Convert.ToString(dgv.Rows[e.RowIndex].Cells["MaDM"].Value);
                string tenDM;
                e.ToolTipText = tenDanhMucTheoMa.TryGetValue(maDM, out tenDM) ? tenDM : maDM;
            }
            else if (tenCot == "DonGia")
            {
                decimal donGia = Convert.ToDecimal(dgv.Rows[e.RowIndex].Cells["DonGia"].Value);
                e.ToolTipText = donGia.ToString("N0", new CultureInfo("vi-VN")) + " đ";
            }
            else
            {
                e.ToolTipText = Convert.ToString(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
            }
        }

        private static GraphicsPath TaoDuongVien(Rectangle hinh, int duongKinh)
        {
            int cung = Math.Min(duongKinh, Math.Min(hinh.Width, hinh.Height));
            GraphicsPath duongVien = new GraphicsPath();
            duongVien.AddArc(hinh.Left, hinh.Top, cung, cung, 180, 90);
            duongVien.AddArc(hinh.Right - cung, hinh.Top, cung, cung, 270, 90);
            duongVien.AddArc(hinh.Right - cung, hinh.Bottom - cung, cung, cung, 0, 90);
            duongVien.AddArc(hinh.Left, hinh.Bottom - cung, cung, cung, 90, 90);
            duongVien.CloseFigure();
            return duongVien;
        }

        private void ApDungTimKiem()
        {
            if (duLieu == null) return;
            string tuKhoa = txtTim.Text.Trim();
            if (tuKhoa.Length == 0)
            {
                HienThiDuLieu(duLieu);
                return;
            }

            DataTable ketQua = duLieu.Clone();
            foreach (DataRow dong in duLieu.Rows)
            {
                if (Convert.ToString(dong["MaMon"]).IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0
                    || Convert.ToString(dong["TenMon"]).IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0)
                    ketQua.ImportRow(dong);
            }
            HienThiDuLieu(ketQua);
        }

        private void dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cheDo) || choDongBo || dgv.CurrentRow == null) return;
            DataGridViewRow dong = dgv.CurrentRow;
            txtMaMon.Text = Convert.ToString(dong.Cells["MaMon"].Value);
            txtTenMon.Text = Convert.ToString(dong.Cells["TenMon"].Value);
            nudDonGia.Value = Math.Min(nudDonGia.Maximum, Math.Max(nudDonGia.Minimum, Convert.ToDecimal(dong.Cells["DonGia"].Value)));
            txtDVT.Text = Convert.ToString(dong.Cells["DVT"].Value);
            cboTrangThai.SelectedItem = Convert.ToString(dong.Cells["TrangThai"].Value);
            cboDanhMuc.SelectedValue = Convert.ToString(dong.Cells["MaDM"].Value);
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (choDongBo) return;
            if (cboDanhMuc.Items.Count == 0)
            {
                MessageBox.Show("Hãy thêm danh mục trước khi thêm món ăn.", "Thiếu danh mục", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                fx.TaoXml("SELECT * FROM MonAn", "MonAn", "MonAn.xml");
                txtMaMon.Text = fx.TaoMa("MA", "MonAn.xml", "MaMon");
                txtTenMon.Clear();
                txtDVT.Clear();
                nudDonGia.Value = 0;
                cboDanhMuc.SelectedIndex = 0;
                cboTrangThai.SelectedItem = "Còn bán";
                cheDo = "them";
                CapNhatTrangThaiNut();
                txtTenMon.Focus();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể tạo mã món ăn", ex);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (choDongBo || dgv.CurrentRow == null) return;
            cheDo = "sua";
            CapNhatTrangThaiNut();
            txtTenMon.Focus();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (choDongBo || dgv.CurrentRow == null)
            {
                if (dgv.CurrentRow == null) MessageBox.Show("Vui lòng chọn món ăn cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string maMon = Convert.ToString(dgv.CurrentRow.Cells["MaMon"].Value);
            try
            {
                fx.TaoXml("SELECT * FROM CongThuc", "CongThuc", "CongThuc.xml");
                fx.TaoXml("SELECT * FROM ChiTietHoaDon", "ChiTietHoaDon", "ChiTietHoaDon.xml");
                if (fx.HienThi("CongThuc.xml").AsEnumerable().Any(dong => string.Equals(Convert.ToString(dong["MaMon"]), maMon, StringComparison.OrdinalIgnoreCase))
                    || fx.HienThi("ChiTietHoaDon.xml").AsEnumerable().Any(dong => string.Equals(Convert.ToString(dong["MaMon"]), maMon, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("Không thể xóa món đã được dùng trong công thức hoặc hóa đơn.", "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("Bạn có chắc muốn xóa món " + maMon + " không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                if (fx.Xoa("MonAn.xml", new Dictionary<string, string> { { "MaMon", maMon } }) == 0)
                    throw new InvalidOperationException("Không tìm thấy món ăn trong file XML.");

                choDongBo = true;
                hanhDongChoDongBo = "xóa món ăn";
                CapNhatTrangThaiNut();
                ThuDongBo();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể xóa hoặc đồng bộ món ăn", ex);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (choDongBo)
            {
                ThuDongBo();
                return;
            }

            string tenMon = txtTenMon.Text.Trim();
            string dvt = txtDVT.Text.Trim();
            if (tenMon.Length == 0
                || dvt.Length == 0
                || cboDanhMuc.SelectedValue == null
                || cboTrangThai.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng nhập tên món, đơn vị tính, danh mục và trạng thái.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (tenMon.Length > 100 || dvt.Length > 20)
            {
                MessageBox.Show("Tên món tối đa 100 ký tự và đơn vị tính tối đa 20 ký tự.", "Thông tin không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maDM = Convert.ToString(cboDanhMuc.SelectedValue);
            var giaTri = new Dictionary<string, string>
            {
                { "MaMon", txtMaMon.Text.Trim() },
                { "TenMon", tenMon },
                { "MaDM", maDM },
                { "DonGia", nudDonGia.Value.ToString("0", System.Globalization.CultureInfo.InvariantCulture) },
                { "DVT", dvt },
                { "TrangThai", Convert.ToString(cboTrangThai.SelectedItem) }
            };

            try
            {
                if (cheDo == "them")
                {
                    if (fx.TonTai("MonAn.xml", "MaMon", txtMaMon.Text))
                        throw new InvalidOperationException("Mã món đã tồn tại. Hãy hủy thao tác và tạo lại mã.");
                    string[] thuTuCot = { "MaMon", "TenMon", "MaDM", "DonGia", "DVT", "TrangThai" };
                    var giaTriTheoThuTu = new SortedDictionary<string, string>(Comparer<string>.Create((a, b) =>
                        Array.IndexOf(thuTuCot, a).CompareTo(Array.IndexOf(thuTuCot, b))));
                    foreach (string tenCot in thuTuCot)
                        giaTriTheoThuTu.Add(tenCot, giaTri[tenCot]);
                    fx.Them("MonAn.xml", "MonAn", giaTriTheoThuTu);
                    hanhDongChoDongBo = "thêm món ăn";
                }
                else if (cheDo == "sua")
                {
                    var giaTriMoi = new Dictionary<string, string>(giaTri);
                    giaTriMoi.Remove("MaMon");
                    if (fx.Sua("MonAn.xml", new Dictionary<string, string> { { "MaMon", txtMaMon.Text } }, giaTriMoi) == 0)
                        throw new InvalidOperationException("Không tìm thấy món ăn cần sửa trong file XML.");
                    hanhDongChoDongBo = "sửa món ăn";
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn Thêm hoặc Sửa trước khi lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                choDongBo = true;
                CapNhatTrangThaiNut();
                ThuDongBo();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể lưu món ăn", ex);
            }
        }

        private void ThuDongBo()
        {
            try
            {
                fx.CapNhatDb("DanhMuc", "DanhMuc.xml");
                fx.CapNhatDb("MonAn", "MonAn.xml");
            }
            catch (Exception ex)
            {
                CapNhatTrangThaiNut();
                MessageBox.Show("XML đã thay đổi nhưng chưa đồng bộ được danh mục/món ăn vào CSDL. Nhấn Lưu để thử đồng bộ lại.\r\n\r\n" + ex.Message,
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
                HienLoi("Món ăn đã đồng bộ nhưng không thể tải lại danh sách", ex);
            }
            CapNhatTrangThaiNut();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            if (choDongBo) return;
            cheDo = "";
            txtMaMon.Clear();
            txtTenMon.Clear();
            txtDVT.Clear();
            nudDonGia.Value = 0;
            if (cboDanhMuc.Items.Count > 0) cboDanhMuc.SelectedIndex = 0;
            cboTrangThai.SelectedItem = null;
            CapNhatTrangThaiNut();
            if (duLieu != null)
            {
                HienThiDuLieu(duLieu);
                dgv_SelectionChanged(dgv, EventArgs.Empty);
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            if (choDongBo) return;
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

        private void CapNhatTrangThaiNut()
        {
            bool dangChinhSua = !string.IsNullOrEmpty(cheDo);
            bool binhThuong = !dangChinhSua && !choDongBo;
            btnThem.Enabled = binhThuong;
            btnSua.Enabled = binhThuong && dgv.CurrentRow != null;
            btnXoa.Enabled = binhThuong && dgv.CurrentRow != null;
            btnTim.Enabled = binhThuong;
            txtTim.Enabled = binhThuong;
            txtTenMon.ReadOnly = !dangChinhSua;
            txtDVT.ReadOnly = !dangChinhSua;
            txtMaMon.BackColor = Color.FromArgb(243, 244, 246);
            txtTenMon.BackColor = txtTenMon.ReadOnly ? Color.FromArgb(243, 244, 246) : Color.White;
            txtDVT.BackColor = txtDVT.ReadOnly ? Color.FromArgb(243, 244, 246) : Color.White;
            nudDonGia.Enabled = dangChinhSua;
            nudDonGia.BackColor = dangChinhSua ? Color.White : Color.FromArgb(243, 244, 246);
            cboDanhMuc.Enabled = dangChinhSua;
            cboDanhMuc.BackColor = dangChinhSua ? Color.White : Color.FromArgb(243, 244, 246);
            cboTrangThai.Enabled = dangChinhSua;
            cboTrangThai.BackColor = dangChinhSua ? Color.White : Color.FromArgb(243, 244, 246);
            btnLuu.Enabled = dangChinhSua || choDongBo;
            btnHuy.Enabled = dangChinhSua && !choDongBo;
            btnLuu.Text = choDongBo ? "Đồng bộ" : "Lưu";
            Text = choDongBo && !string.IsNullOrEmpty(hanhDongChoDongBo)
                ? "Quản lý món ăn - chờ đồng bộ " + hanhDongChoDongBo
                : "Quản lý món ăn";
        }

        private static void HienLoi(string noiDung, Exception ex)
        {
            MessageBox.Show(noiDung + ".\r\n\r\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
