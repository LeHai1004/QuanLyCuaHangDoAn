using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyCuaHangDoAn
{
    public class frmDanhMuc : Form
    {
        private readonly FileXml fx = new FileXml();
        private readonly DataGridView dgv = new DataGridView();
        private readonly TextBox txtMaDM = new TextBox();
        private readonly TextBox txtTenDM = new TextBox();
        private readonly TextBox txtTim = new TextBox();
        private readonly Button btnThem = GiaoDien.TaoNut("Thêm", GiaoDien.MauChinh);
        private readonly Button btnSua = GiaoDien.TaoNut("Sửa", GiaoDien.MauChinh);
        private readonly Button btnXoa = GiaoDien.TaoNut("Xóa", GiaoDien.MauDo);
        private readonly Button btnLuu = GiaoDien.TaoNut("Lưu", GiaoDien.MauXanh);
        private readonly Button btnHuy = GiaoDien.TaoNut("Hủy", GiaoDien.MauChinhDam);
        private readonly Button btnTim = GiaoDien.TaoNut("Tìm", GiaoDien.MauChinh);

        private DataTable duLieu;
        private readonly Dictionary<string, int> soMonTheoDanhMuc =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private int? tongSoMonAn;
        private Label lblTongDanhMuc;
        private Label lblTongMonAn;
        private Label lblDanhMucTrong;
        private Label lblSoMonDangChon;
        private string cheDo = "";
        private bool choDongBo;
        private string hanhDongChoDongBo;

        public frmDanhMuc()
        {
            KhoiTaoGiaoDien();
            Load += frmDanhMuc_Load;
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Quản lý danh mục";
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
                Text = "QUẢN LÝ DANH MỤC",
                Location = new Point(30, 12),
                Size = new Size(650, 32),
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false
            });
            pnlHeader.Controls.Add(new Label
            {
                Name = "lblPhuDe",
                Text = "Nhóm món trong thực đơn",
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
                ColumnCount = 5,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Color.White
            };
            tlpThongKe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333F));
            tlpThongKe.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 12F));
            tlpThongKe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333F));
            tlpThongKe.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 12F));
            tlpThongKe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333F));
            tlpThongKe.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpThongKe.Controls.Add(TaoTheThongKe("pnlThongKeDanhMuc", "Tổng danh mục",
                "lblTongDanhMuc", out lblTongDanhMuc), 0, 0);
            tlpThongKe.Controls.Add(TaoTheThongKe("pnlThongKeMonAn", "Tổng món ăn",
                "lblTongMonAn", out lblTongMonAn), 2, 0);
            tlpThongKe.Controls.Add(TaoTheThongKe("pnlThongKeDanhMucTrong", "Danh mục chưa có món",
                "lblDanhMucTrong", out lblDanhMucTrong), 4, 0);
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

            dgv.Name = "dgvDanhMuc";
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
            dgv.AutoGenerateColumns = false;
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
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaDM",
                DataPropertyName = "MaDM",
                HeaderText = "Mã DM",
                FillWeight = 20F,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenDM",
                DataPropertyName = "TenDM",
                HeaderText = "Tên danh mục",
                FillWeight = 55F,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            var cotSoMon = new DataGridViewTextBoxColumn
            {
                Name = "SoMon",
                HeaderText = "Số món",
                FillWeight = 25F,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                ValueType = typeof(string)
            };
            cotSoMon.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cotSoMon.DefaultCellStyle.Padding = Padding.Empty;
            cotSoMon.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns.Add(cotSoMon);
            dgv.SelectionChanged += dgv_SelectionChanged;
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
                Name = "lblThongTinDanhMuc",
                Text = "THÔNG TIN DANH MỤC",
                Location = new Point(20, 15),
                Size = new Size(285, 22),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(232, 101, 26),
                AutoSize = false
            });
            pnlPhai.Controls.Add(TaoNhanTruong("lblMaDM", "Mã danh mục", 55));
            txtMaDM.Name = "txtMaDM";
            txtMaDM.Location = new Point(20, 75);
            txtMaDM.Size = new Size(285, 34);
            txtMaDM.AutoSize = false;
            txtMaDM.BorderStyle = BorderStyle.FixedSingle;
            txtMaDM.Font = new Font("Segoe UI", 10.5F);
            txtMaDM.ReadOnly = true;
            txtMaDM.BackColor = Color.FromArgb(243, 244, 246);
            pnlPhai.Controls.Add(txtMaDM);
            pnlPhai.Controls.Add(TaoNhanTruong("lblTenDM", "Tên danh mục", 115));
            txtTenDM.Name = "txtTenDM";
            txtTenDM.Location = new Point(20, 135);
            txtTenDM.Size = new Size(285, 34);
            txtTenDM.AutoSize = false;
            txtTenDM.BorderStyle = BorderStyle.FixedSingle;
            txtTenDM.Font = new Font("Segoe UI", 10.5F);
            txtTenDM.BackColor = Color.White;
            pnlPhai.Controls.Add(txtTenDM);

            var tlpNut = new TableLayoutPanel
            {
                Name = "tlpNutDanhMuc",
                Location = new Point(20, 185),
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

            var pnlThongKeDanhMuc = new Panel
            {
                Name = "pnlSoMonDanhMuc",
                Location = new Point(20, 301),
                Size = new Size(285, 90),
                BackColor = Color.FromArgb(255, 243, 234)
            };
            lblSoMonDangChon = new Label
            {
                Name = "lblSoMonDangChon",
                Text = "--",
                Location = new Point(8, 7),
                Size = new Size(269, 48),
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = Color.FromArgb(232, 101, 26),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false
            };
            pnlThongKeDanhMuc.Controls.Add(lblSoMonDangChon);
            pnlThongKeDanhMuc.Controls.Add(new Label
            {
                Name = "lblMoTaSoMonDanhMuc",
                Text = "món thuộc danh mục này",
                Location = new Point(8, 58),
                Size = new Size(269, 20),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(107, 114, 128),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false
            });
            pnlPhai.Controls.Add(pnlThongKeDanhMuc);

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

        private static Panel TaoTheThongKe(string tenPanel, string nhan, string tenGiaTri, out Label giaTri)
        {
            var panel = new Panel
            {
                Name = tenPanel,
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                BackColor = Color.FromArgb(249, 250, 251)
            };
            giaTri = new Label
            {
                Name = tenGiaTri,
                Text = "0",
                Location = new Point(4, 2),
                Size = new Size(195, 32),
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                AutoEllipsis = true
            };
            panel.Controls.Add(giaTri);
            panel.Controls.Add(new Label
            {
                Name = "lblNhan" + tenGiaTri.Substring(3),
                Text = nhan,
                Location = new Point(4, 34),
                Size = new Size(195, 20),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(107, 114, 128),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                AutoEllipsis = true
            });
            return panel;
        }

        private static Label TaoNhanTruong(string ten, string noiDung, int y)
        {
            return new Label
            {
                Name = ten,
                Text = noiDung,
                Location = new Point(20, y),
                Size = new Size(285, 16),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize = false
            };
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
            nut.FlatAppearance.MouseOverBackColor = MauToiHon(nen, 0.9);
            nut.FlatAppearance.MouseDownBackColor = MauToiHon(nen, 0.82);
            nut.Cursor = Cursors.Hand;
            nut.UseVisualStyleBackColor = false;
        }

        private static Color MauToiHon(Color mau, double heSo)
        {
            return Color.FromArgb((int)(mau.R * heSo), (int)(mau.G * heSo), (int)(mau.B * heSo));
        }

        private static void GanNut(TableLayoutPanel bangNut, Button nut, int cot, int dong, int soCot)
        {
            nut.Dock = DockStyle.Fill;
            bangNut.Controls.Add(nut, cot, dong);
            if (soCot > 1)
                bangNut.SetColumnSpan(nut, soCot);
        }

        private void CapNhatDuLieuMonAn()
        {
            soMonTheoDanhMuc.Clear();
            tongSoMonAn = null;
            try
            {
                fx.TaoXml("SELECT * FROM MonAn", "MonAn", "MonAn.xml");
                DataTable monAn = fx.HienThi("MonAn.xml");
                foreach (DataRow dong in monAn.Rows)
                {
                    string maDM = Convert.ToString(dong["MaDM"]);
                    if (string.IsNullOrWhiteSpace(maDM))
                        continue;

                    if (soMonTheoDanhMuc.ContainsKey(maDM))
                        soMonTheoDanhMuc[maDM]++;
                    else
                        soMonTheoDanhMuc.Add(maDM, 1);
                }
                tongSoMonAn = monAn.Rows.Count;
            }
            catch (Exception)
            {
                soMonTheoDanhMuc.Clear();
                tongSoMonAn = null;
            }
        }

        private void CapNhatThongKe()
        {
            if (duLieu == null)
                return;

            lblTongDanhMuc.Text = duLieu.Rows.Count.ToString();
            if (!tongSoMonAn.HasValue)
            {
                lblTongMonAn.Text = "--";
                lblDanhMucTrong.Text = "--";
                return;
            }

            lblTongMonAn.Text = tongSoMonAn.Value.ToString();
            int danhMucTrong = 0;
            foreach (DataRow dong in duLieu.Rows)
            {
                int soLuong;
                string maDM = Convert.ToString(dong["MaDM"]);
                if (!soMonTheoDanhMuc.TryGetValue(maDM, out soLuong) || soLuong == 0)
                    danhMucTrong++;
            }
            lblDanhMucTrong.Text = danhMucTrong.ToString();
        }

        private void CapNhatSoMonDangChon()
        {
            if (lblSoMonDangChon == null)
                return;
            if (cheDo == "them" || dgv.CurrentRow == null || !tongSoMonAn.HasValue)
            {
                lblSoMonDangChon.Text = "--";
                return;
            }

            string maDM = Convert.ToString(dgv.CurrentRow.Cells["MaDM"].Value);
            int soLuong;
            lblSoMonDangChon.Text = soMonTheoDanhMuc.TryGetValue(maDM, out soLuong)
                ? soLuong.ToString()
                : "0";
        }

        private void dgv_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            e.PaintBackground(e.CellBounds, true);
            Rectangle vungChu = e.CellBounds;
            vungChu.X += e.CellStyle.Padding.Left;
            vungChu.Width -= e.CellStyle.Padding.Left + e.CellStyle.Padding.Right;
            TextFormatFlags canLe = dgv.Columns[e.ColumnIndex].Name == "SoMon"
                ? TextFormatFlags.HorizontalCenter
                : TextFormatFlags.Left;
            TextRenderer.DrawText(e.Graphics, Convert.ToString(e.FormattedValue), e.CellStyle.Font,
                vungChu, e.CellStyle.ForeColor, canLe | TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            e.Paint(e.CellBounds, DataGridViewPaintParts.Border);
            e.Handled = true;
        }

        private void dgv_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                e.ToolTipText = Convert.ToString(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
        }

        private void frmDanhMuc_Load(object sender, EventArgs e)
        {
            try
            {
                TaiDuLieu();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể tải danh sách danh mục", ex);
            }
        }

        private void TaiDuLieu()
        {
            fx.TaoXml("SELECT * FROM DanhMuc", "DanhMuc", "DanhMuc.xml");
            duLieu = fx.HienThi("DanhMuc.xml");
            CapNhatDuLieuMonAn();
            HienThiDuLieu(duLieu);
        }

        private void HienThiDuLieu(DataTable nguon)
        {
            dgv.DataSource = nguon.Copy();
            foreach (DataGridViewRow dong in dgv.Rows)
            {
                dong.Height = 44;
                string maDM = Convert.ToString(dong.Cells["MaDM"].Value);
                int soLuong;
                dong.Cells["SoMon"].Value = tongSoMonAn.HasValue
                    ? (soMonTheoDanhMuc.TryGetValue(maDM, out soLuong) ? soLuong.ToString() : "0")
                    : "--";
            }
            CapNhatThongKe();
            CapNhatSoMonDangChon();
            CapNhatTrangThaiNut();
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
                if (Convert.ToString(dong["MaDM"]).IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0
                    || Convert.ToString(dong["TenDM"]).IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0)
                    ketQua.ImportRow(dong);
            }
            HienThiDuLieu(ketQua);
        }

        private void dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cheDo) || choDongBo) return;
            if (dgv.CurrentRow == null)
            {
                CapNhatSoMonDangChon();
                return;
            }
            txtMaDM.Text = Convert.ToString(dgv.CurrentRow.Cells["MaDM"].Value);
            txtTenDM.Text = Convert.ToString(dgv.CurrentRow.Cells["TenDM"].Value);
            CapNhatSoMonDangChon();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (choDongBo) return;
            try
            {
                fx.TaoXml("SELECT * FROM DanhMuc", "DanhMuc", "DanhMuc.xml");
                txtMaDM.Text = fx.TaoMa("DM", "DanhMuc.xml", "MaDM");
                txtTenDM.Clear();
                cheDo = "them";
                CapNhatSoMonDangChon();
                CapNhatTrangThaiNut();
                txtTenDM.Focus();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể tạo mã danh mục", ex);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (choDongBo || dgv.CurrentRow == null) return;
            cheDo = "sua";
            CapNhatTrangThaiNut();
            txtTenDM.Focus();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (choDongBo || dgv.CurrentRow == null)
            {
                if (dgv.CurrentRow == null) MessageBox.Show("Vui lòng chọn danh mục cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string maDM = Convert.ToString(dgv.CurrentRow.Cells["MaDM"].Value);
            try
            {
                fx.TaoXml("SELECT * FROM MonAn", "MonAn", "MonAn.xml");
                DataTable monAn = fx.HienThi("MonAn.xml");
                if (monAn.AsEnumerable().Any(dong => string.Equals(Convert.ToString(dong["MaDM"]), maDM, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("Không thể xóa danh mục đang có món ăn. Hãy chuyển hoặc xóa các món thuộc danh mục này trước.", "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("Bạn có chắc muốn xóa danh mục " + maDM + " không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                if (fx.Xoa("DanhMuc.xml", new Dictionary<string, string> { { "MaDM", maDM } }) == 0)
                    throw new InvalidOperationException("Không tìm thấy danh mục trong file XML.");

                choDongBo = true;
                hanhDongChoDongBo = "xóa danh mục";
                CapNhatTrangThaiNut();
                ThuDongBo();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể xóa hoặc đồng bộ danh mục", ex);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (choDongBo)
            {
                ThuDongBo();
                return;
            }

            string tenDM = txtTenDM.Text.Trim();
            if (tenDM.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập tên danh mục.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenDM.Focus();
                return;
            }
            if (tenDM.Length > 50)
            {
                MessageBox.Show("Tên danh mục không được vượt quá 50 ký tự.", "Thông tin không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenDM.Focus();
                return;
            }

            try
            {
                if (cheDo == "them")
                {
                    if (fx.TonTai("DanhMuc.xml", "MaDM", txtMaDM.Text))
                        throw new InvalidOperationException("Mã danh mục đã tồn tại. Hãy hủy thao tác và tạo lại mã.");
                    string[] thuTuCot = { "MaDM", "TenDM" };
                    var giaTri = new SortedDictionary<string, string>(Comparer<string>.Create((a, b) =>
                        Array.IndexOf(thuTuCot, a).CompareTo(Array.IndexOf(thuTuCot, b))));
                    giaTri.Add("MaDM", txtMaDM.Text.Trim());
                    giaTri.Add("TenDM", tenDM);
                    fx.Them("DanhMuc.xml", "DanhMuc", giaTri);
                    hanhDongChoDongBo = "thêm danh mục";
                }
                else if (cheDo == "sua")
                {
                    if (fx.Sua("DanhMuc.xml", new Dictionary<string, string> { { "MaDM", txtMaDM.Text } },
                        new Dictionary<string, string> { { "TenDM", tenDM } }) == 0)
                        throw new InvalidOperationException("Không tìm thấy danh mục cần sửa trong file XML.");
                    hanhDongChoDongBo = "sửa danh mục";
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
                HienLoi("Không thể lưu danh mục", ex);
            }
        }

        private void ThuDongBo()
        {
            try
            {
                fx.CapNhatDb("DanhMuc", "DanhMuc.xml");
            }
            catch (Exception ex)
            {
                CapNhatTrangThaiNut();
                MessageBox.Show("XML đã thay đổi nhưng chưa đồng bộ được danh mục vào CSDL. Nhấn Lưu để thử đồng bộ lại.\r\n\r\n" + ex.Message,
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
                HienLoi("Danh mục đã đồng bộ nhưng không thể tải lại danh sách", ex);
            }
            CapNhatTrangThaiNut();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            if (choDongBo) return;
            cheDo = "";
            txtMaDM.Clear();
            txtTenDM.Clear();
            CapNhatTrangThaiNut();
            if (duLieu != null)
            {
                HienThiDuLieu(duLieu);
                dgv_SelectionChanged(dgv, EventArgs.Empty);
            }
            else
            {
                CapNhatSoMonDangChon();
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
            txtTenDM.ReadOnly = !dangChinhSua;
            txtTenDM.BackColor = txtTenDM.ReadOnly
                ? Color.FromArgb(243, 244, 246)
                : Color.White;
            btnLuu.Enabled = dangChinhSua || choDongBo;
            btnHuy.Enabled = dangChinhSua && !choDongBo;
            btnLuu.Text = choDongBo ? "Đồng bộ" : "Lưu";
            if (choDongBo && !string.IsNullOrEmpty(hanhDongChoDongBo))
                Text = "Quản lý danh mục - chờ đồng bộ " + hanhDongChoDongBo;
            else
                Text = "Quản lý danh mục";
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
