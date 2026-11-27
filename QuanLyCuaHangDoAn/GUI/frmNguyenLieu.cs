using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using QuanLyCuaHangDoAn.Class;

namespace QuanLyCuaHangDoAn.GUI
{
    public class frmNguyenLieu : Form
    {
        private const string TenFile = "NguyenLieu.xml";
        private const string TenBang = "NguyenLieu";

        private readonly FileXml fx = new FileXml();
        private readonly DataGridView dgvNguyenLieu = new DataGridView();
        private readonly TextBox txtMaNL = new TextBox();
        private readonly TextBox txtTenNL = new TextBox();
        private readonly TextBox txtDVT = new TextBox();
        private readonly TextBox txtTonKho = new TextBox();
        private readonly TextBox txtDinhMuc = new TextBox();
        private readonly TextBox txtTim = new TextBox();

        private readonly Button btnThem = GiaoDien.TaoNut("Thêm", GiaoDien.MauChinh);
        private readonly Button btnSua = GiaoDien.TaoNut("Sửa", GiaoDien.MauChinh);
        private readonly Button btnXoa = GiaoDien.TaoNut("Xóa", GiaoDien.MauDo);
        private readonly Button btnLuu = GiaoDien.TaoNut("Lưu", GiaoDien.MauXanh);
        private readonly Button btnHuy = GiaoDien.TaoNut("Hủy", GiaoDien.MauChinhDam);
        private readonly Button btnTim = GiaoDien.TaoNut("Tìm", GiaoDien.MauChinh);

        private DataTable duLieu;
        private Label lblTongNguyenLieu;
        private Label lblDuHang;
        private Label lblSapHet;
        private Label lblHetHang;
        private Panel pnlMucTonKho;
        private Label lblMucTonKho;
        private StockProgressBar prgMucTonKho;
        private Label lblChiTietTonKho;
        private string cheDo = "";
        private bool choDongBo;
        private string hanhDongChoDongBo;

        public frmNguyenLieu()
        {
            KhoiTaoGiaoDien();
            Load += frmNguyenLieu_Load;
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Quản lý nguyên liệu";
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 246, 248);
            Font = new Font("Segoe UI", 10F);
            ClientSize = new Size(1100, 680);
            DoubleBuffered = true;

            dgvNguyenLieu.Name = "dgvNguyenLieu";
            txtMaNL.Name = "txtMaNL";
            txtTenNL.Name = "txtTenNL";
            txtDVT.Name = "txtDVT";
            txtTonKho.Name = "txtTonKho";
            txtDinhMuc.Name = "txtDinhMuc";
            txtTim.Name = "txtTim";
            btnThem.Name = "btnThem";
            btnSua.Name = "btnSua";
            btnXoa.Name = "btnXoa";
            btnLuu.Name = "btnLuu";
            btnHuy.Name = "btnHuy";
            btnTim.Name = "btnTim";

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
                Text = "QUẢN LÝ NGUYÊN LIỆU",
                Location = new Point(30, 12),
                Size = new Size(650, 32),
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false
            });
            pnlHeader.Controls.Add(new Label
            {
                Name = "lblPhuDe",
                Text = "Tồn kho và định mức cảnh báo",
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
            tlpThongKe.Controls.Add(TaoTheThongKe("pnlThongKeTong", "Tổng nguyên liệu",
                "lblTongNguyenLieu", Color.FromArgb(17, 24, 39)), 0, 0);
            tlpThongKe.Controls.Add(TaoTheThongKe("pnlThongKeDuHang", "Đủ hàng",
                "lblDuHang", Color.FromArgb(22, 163, 74)), 2, 0);
            tlpThongKe.Controls.Add(TaoTheThongKe("pnlThongKeSapHet", "Sắp hết",
                "lblSapHet", Color.FromArgb(220, 38, 38)), 4, 0);
            tlpThongKe.Controls.Add(TaoTheThongKe("pnlThongKeHetHang", "Hết hàng",
                "lblHetHang", Color.FromArgb(127, 29, 29)), 6, 0);
            pnlTrai.Controls.Add(tlpThongKe);

            txtTim.Location = new Point(20, 95);
            txtTim.Size = new Size(545, 36);
            txtTim.AutoSize = false;
            txtTim.BorderStyle = BorderStyle.FixedSingle;
            txtTim.Font = new Font("Segoe UI", 11F);
            txtTim.BackColor = Color.White;
            pnlTrai.Controls.Add(txtTim);

            CauHinhNut(btnTim, Color.FromArgb(232, 101, 26));
            btnTim.Location = new Point(575, 95);
            btnTim.Size = new Size(95, 36);
            pnlTrai.Controls.Add(btnTim);

            dgvNguyenLieu.Location = new Point(20, 145);
            dgvNguyenLieu.Size = new Size(650, 385);
            dgvNguyenLieu.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            dgvNguyenLieu.BorderStyle = BorderStyle.None;
            dgvNguyenLieu.BackgroundColor = Color.White;
            dgvNguyenLieu.EnableHeadersVisualStyles = false;
            dgvNguyenLieu.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvNguyenLieu.GridColor = Color.FromArgb(243, 244, 246);
            dgvNguyenLieu.RowHeadersVisible = false;
            dgvNguyenLieu.AllowUserToAddRows = false;
            dgvNguyenLieu.AllowUserToDeleteRows = false;
            dgvNguyenLieu.AllowUserToResizeRows = false;
            dgvNguyenLieu.ReadOnly = true;
            dgvNguyenLieu.MultiSelect = false;
            dgvNguyenLieu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvNguyenLieu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvNguyenLieu.AutoGenerateColumns = false;
            dgvNguyenLieu.ColumnHeadersHeight = 38;
            dgvNguyenLieu.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvNguyenLieu.RowTemplate.Height = 44;
            dgvNguyenLieu.ShowCellToolTips = true;
            dgvNguyenLieu.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 243, 234);
            dgvNguyenLieu.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(154, 52, 18);
            dgvNguyenLieu.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvNguyenLieu.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvNguyenLieu.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgvNguyenLieu.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 4, 0);
            dgvNguyenLieu.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 243, 234);
            dgvNguyenLieu.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(154, 52, 18);
            dgvNguyenLieu.DefaultCellStyle.BackColor = Color.White;
            dgvNguyenLieu.DefaultCellStyle.ForeColor = Color.FromArgb(17, 24, 39);
            dgvNguyenLieu.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvNguyenLieu.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgvNguyenLieu.DefaultCellStyle.Padding = new Padding(8, 0, 4, 0);
            dgvNguyenLieu.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 243, 234);
            dgvNguyenLieu.DefaultCellStyle.SelectionForeColor = Color.FromArgb(17, 24, 39);
            dgvNguyenLieu.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            dgvNguyenLieu.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 243, 234);
            dgvNguyenLieu.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.FromArgb(17, 24, 39);
            dgvNguyenLieu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaNL",
                DataPropertyName = "MaNL",
                HeaderText = "Mã NL",
                FillWeight = 12F,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            dgvNguyenLieu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenNL",
                DataPropertyName = "TenNL",
                HeaderText = "Tên nguyên liệu",
                FillWeight = 28F,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            dgvNguyenLieu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DVT",
                DataPropertyName = "DVT",
                HeaderText = "ĐVT",
                FillWeight = 10F,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            var cotTonKho = TaoCotSo("TonKho", "Tồn kho", 14F);
            var cotDinhMuc = TaoCotSo("DinhMuc", "Định mức", 14F);
            dgvNguyenLieu.Columns.Add(cotTonKho);
            dgvNguyenLieu.Columns.Add(cotDinhMuc);
            var cotTinhTrang = new DataGridViewTextBoxColumn
            {
                Name = "TinhTrang",
                HeaderText = "Tình trạng",
                FillWeight = 22F,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            dgvNguyenLieu.Columns.Add(cotTinhTrang);
            dgvNguyenLieu.SelectionChanged += dgvNguyenLieu_SelectionChanged;
            dgvNguyenLieu.RowPrePaint += dgvNguyenLieu_RowPrePaint;
            dgvNguyenLieu.CellPainting += dgvNguyenLieu_CellPainting;
            dgvNguyenLieu.CellToolTipTextNeeded += dgvNguyenLieu_CellToolTipTextNeeded;
            pnlTrai.Controls.Add(dgvNguyenLieu);

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
                Name = "lblThongTinNguyenLieu",
                Text = "THÔNG TIN NGUYÊN LIỆU",
                Location = new Point(20, 15),
                Size = new Size(285, 22),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(232, 101, 26),
                AutoSize = false
            });
            ThemTruong(pnlPhai, "Mã nguyên liệu", txtMaNL, true, 55);
            ThemTruong(pnlPhai, "Tên nguyên liệu (*)", txtTenNL, false, 115);
            ThemTruong(pnlPhai, "Đơn vị tính (*)", txtDVT, false, 175);
            ThemTruong(pnlPhai, "Tồn kho (*)", txtTonKho, false, 235);
            ThemTruong(pnlPhai, "Định mức tối thiểu (*)", txtDinhMuc, false, 295);

            txtTenNL.MaxLength = 100;
            txtDVT.MaxLength = 20;

            var tlpNut = new TableLayoutPanel
            {
                Name = "tlpNutNguyenLieu",
                Location = new Point(20, 365),
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
            CauHinhNut(btnThem, Color.FromArgb(232, 101, 26));
            CauHinhNut(btnSua, Color.FromArgb(37, 99, 235));
            CauHinhNut(btnXoa, Color.FromArgb(220, 38, 38));
            CauHinhNut(btnLuu, Color.FromArgb(22, 163, 74));
            CauHinhNut(btnHuy, Color.FromArgb(107, 114, 128));
            GanNut(tlpNut, btnThem, 0, 0, 1);
            GanNut(tlpNut, btnSua, 1, 0, 1);
            GanNut(tlpNut, btnXoa, 2, 0, 1);
            GanNut(tlpNut, btnLuu, 0, 2, 2);
            GanNut(tlpNut, btnHuy, 2, 2, 1);
            pnlPhai.Controls.Add(tlpNut);

            pnlMucTonKho = new Panel
            {
                Name = "pnlMucTonKho",
                Location = new Point(20, 467),
                Size = new Size(285, 70),
                BackColor = Color.White,
                Visible = false
            };
            lblMucTonKho = new Label
            {
                Name = "lblMucTonKho",
                Text = "Mức tồn kho",
                Location = new Point(0, 0),
                Size = new Size(285, 16),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize = false
            };
            prgMucTonKho = new StockProgressBar
            {
                Name = "prgMucTonKho",
                Location = new Point(0, 21),
                Size = new Size(285, 10)
            };
            lblChiTietTonKho = new Label
            {
                Name = "lblChiTietTonKho",
                Text = "",
                Location = new Point(0, 36),
                Size = new Size(285, 30),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize = false,
                AutoEllipsis = true
            };
            pnlMucTonKho.Controls.Add(lblMucTonKho);
            pnlMucTonKho.Controls.Add(prgMucTonKho);
            pnlMucTonKho.Controls.Add(lblChiTietTonKho);
            pnlPhai.Controls.Add(pnlMucTonKho);

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
            panel.Controls.Add(giaTri);
            panel.Controls.Add(new Label
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
            });
            if (tenGiaTri == "lblTongNguyenLieu")
                lblTongNguyenLieu = giaTri;
            else if (tenGiaTri == "lblDuHang")
                lblDuHang = giaTri;
            else if (tenGiaTri == "lblSapHet")
                lblSapHet = giaTri;
            else
                lblHetHang = giaTri;
            return panel;
        }

        private static DataGridViewTextBoxColumn TaoCotSo(string ten, string tieuDe, float trongSo)
        {
            return new DataGridViewTextBoxColumn
            {
                Name = ten,
                DataPropertyName = ten,
                HeaderText = tieuDe,
                FillWeight = trongSo,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle =
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "#,##0.##",
                    FormatProvider = CultureInfo.GetCultureInfo("vi-VN")
                },
                HeaderCell =
                {
                    Style = { Alignment = DataGridViewContentAlignment.MiddleRight }
                }
            };
        }

        private static void ThemTruong(Panel panel, string nhan, Control control, bool chiDoc, int y)
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
            panel.Controls.Add(control);
        }

        private static void CauHinhNut(Button nut, Color nen)
        {
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

        private void frmNguyenLieu_Load(object sender, EventArgs e)
        {
            try
            {
                if (!PhienDangNhap.IsLoggedIn || PhienDangNhap.ChucVu != "Admin")
                {
                    MessageBox.Show("Chức năng chỉ dành cho quản trị viên (Admin).", "Từ chối",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Close();
                    return;
                }

                TaiDuLieu();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể tải danh sách nguyên liệu", ex);
            }
        }

        private void TaiDuLieu()
        {
            fx.TaoXml("SELECT * FROM NguyenLieu", TenBang, TenFile);
            duLieu = fx.HienThi(TenFile);
            HienThiDuLieu(duLieu);
        }

        private void HienThiDuLieu(DataTable nguon)
        {
            dgvNguyenLieu.DataSource = nguon != null ? nguon.Copy() : null;
            foreach (DataGridViewRow dong in dgvNguyenLieu.Rows)
            {
                dong.Height = 44;
                decimal tonKho;
                decimal dinhMuc;
                dong.Cells["TinhTrang"].Value =
                    ThuLaySo(dong.Cells["TonKho"].Value, out tonKho)
                    && ThuLaySo(dong.Cells["DinhMuc"].Value, out dinhMuc)
                        ? LayTinhTrang(tonKho, dinhMuc)
                        : "--";
            }

            CapNhatThongKe();
            CapNhatThanhTonKho();
            CapNhatTrangThaiNut();
        }

        private static string LayTinhTrang(decimal tonKho, decimal dinhMuc)
        {
            if (tonKho <= 0)
                return "Hết hàng";
            if (tonKho <= dinhMuc)
                return "Sắp hết";
            return "Đủ hàng";
        }

        private void CapNhatThongKe()
        {
            if (duLieu == null)
                return;

            int duHang = 0;
            int sapHet = 0;
            int hetHang = 0;
            foreach (DataRow dong in duLieu.Rows)
            {
                decimal tonKho;
                decimal dinhMuc;
                if (!ThuLaySo(dong["TonKho"], out tonKho)
                    || !ThuLaySo(dong["DinhMuc"], out dinhMuc))
                    continue;

                if (tonKho > dinhMuc)
                    duHang++;
                if (tonKho <= dinhMuc)
                    sapHet++;
                if (tonKho <= 0)
                    hetHang++;
            }

            lblTongNguyenLieu.Text = duLieu.Rows.Count.ToString();
            lblDuHang.Text = duHang.ToString();
            lblSapHet.Text = sapHet.ToString();
            lblHetHang.Text = hetHang.ToString();
        }

        private void CapNhatThanhTonKho()
        {
            DataGridViewRow dong = dgvNguyenLieu.CurrentRow;
            if (dong == null || dgvNguyenLieu.SelectedRows.Count == 0 || cheDo == "them")
            {
                pnlMucTonKho.Visible = false;
                return;
            }

            decimal tonKho;
            decimal dinhMuc;
            if (!ThuLaySo(dong.Cells["TonKho"].Value, out tonKho)
                || !ThuLaySo(dong.Cells["DinhMuc"].Value, out dinhMuc))
            {
                pnlMucTonKho.Visible = false;
                return;
            }

            prgMucTonKho.DatMucTon(tonKho, dinhMuc);
            string dvt = Convert.ToString(dong.Cells["DVT"].Value);
            lblChiTietTonKho.Text = "Còn " + DinhDangSo(tonKho) + " " + dvt
                + " • định mức " + DinhDangSo(dinhMuc);
            pnlMucTonKho.Visible = true;
        }

        private static string DinhDangSo(decimal giaTri)
        {
            return giaTri.ToString("#,##0.##", CultureInfo.GetCultureInfo("vi-VN"));
        }

        private void dgvNguyenLieu_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dgvNguyenLieu.Columns[e.ColumnIndex].Name == "TinhTrang")
            {
                string tinhTrang = Convert.ToString(e.FormattedValue);
                Color mauNen = tinhTrang == "Đủ hàng"
                    ? Color.FromArgb(22, 163, 74)
                    : tinhTrang == "Sắp hết"
                        ? Color.FromArgb(220, 38, 38)
                        : tinhTrang == "Hết hàng"
                            ? Color.FromArgb(127, 29, 29)
                            : Color.FromArgb(107, 114, 128);

                e.PaintBackground(e.CellBounds, true);
                Rectangle vienThuoc = new Rectangle(
                    e.CellBounds.Left + (e.CellBounds.Width - 80) / 2,
                    e.CellBounds.Top + (e.CellBounds.Height - 22) / 2, 80, 22);
                GraphicsState trangThaiDoHoa = e.Graphics.Save();
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath duongVien = TaoDuongVien(vienThuoc, 22))
                using (SolidBrush brush = new SolidBrush(mauNen))
                    e.Graphics.FillPath(brush, duongVien);
                e.Graphics.Restore(trangThaiDoHoa);
                using (Font font = new Font("Segoe UI", 8.5F, FontStyle.Bold))
                    TextRenderer.DrawText(e.Graphics, tinhTrang, font, vienThuoc, Color.White,
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
            TextFormatFlags canLe = dgvNguyenLieu.Columns[e.ColumnIndex].Name == "TonKho"
                || dgvNguyenLieu.Columns[e.ColumnIndex].Name == "DinhMuc"
                    ? TextFormatFlags.Right
                    : TextFormatFlags.Left;
            TextRenderer.DrawText(e.Graphics, Convert.ToString(e.FormattedValue), e.CellStyle.Font,
                vungChu, e.CellStyle.ForeColor, canLe | TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            e.Paint(e.CellBounds, DataGridViewPaintParts.Border);
            e.Handled = true;
        }

        private void dgvNguyenLieu_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                e.ToolTipText = Convert.ToString(dgvNguyenLieu.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue);
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

        private void dgvNguyenLieu_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvNguyenLieu.Rows.Count)
                return;

            DataGridViewRow dong = dgvNguyenLieu.Rows[e.RowIndex];
            object valTonKho = dong.Cells["TonKho"].Value;
            object valDinhMuc = dong.Cells["DinhMuc"].Value;

            bool canhBao = ThuLaySo(valTonKho, out decimal tonKho)
                && ThuLaySo(valDinhMuc, out decimal dinhMuc)
                && tonKho <= dinhMuc;
            if (!canhBao)
            {
                dong.DefaultCellStyle.BackColor = Color.Empty;
                dong.DefaultCellStyle.ForeColor = Color.Empty;
                dong.DefaultCellStyle.Font = null;
                dong.DefaultCellStyle.SelectionBackColor = Color.Empty;
                dong.DefaultCellStyle.SelectionForeColor = Color.Empty;
                return;
            }

            dong.DefaultCellStyle.BackColor = Color.FromArgb(254, 242, 242);
            dong.DefaultCellStyle.ForeColor = Color.FromArgb(153, 27, 27);
            dong.DefaultCellStyle.Font = GiaoDien.FontDam;
            dong.DefaultCellStyle.SelectionBackColor = Color.FromArgb(254, 242, 242);
            dong.DefaultCellStyle.SelectionForeColor = Color.FromArgb(153, 27, 27);
        }

        private static bool ThuLaySo(object giaTri, out decimal so)
        {
            so = 0m;
            if (giaTri == null || giaTri == DBNull.Value)
                return false;

            if (giaTri is decimal d) { so = d; return true; }
            if (giaTri is double db) { so = (decimal)db; return true; }
            if (giaTri is int i) { so = i; return true; }

            string s = Convert.ToString(giaTri).Trim();
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out so))
                return true;
            return decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out so);
        }

        private void dgvNguyenLieu_SelectionChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cheDo) || choDongBo)
                return;
            if (dgvNguyenLieu.CurrentRow == null)
            {
                CapNhatThanhTonKho();
                return;
            }

            txtMaNL.Text = Convert.ToString(dgvNguyenLieu.CurrentRow.Cells["MaNL"].Value);
            txtTenNL.Text = Convert.ToString(dgvNguyenLieu.CurrentRow.Cells["TenNL"].Value);
            txtDVT.Text = Convert.ToString(dgvNguyenLieu.CurrentRow.Cells["DVT"].Value);
            txtTonKho.Text = Convert.ToString(dgvNguyenLieu.CurrentRow.Cells["TonKho"].Value);
            txtDinhMuc.Text = Convert.ToString(dgvNguyenLieu.CurrentRow.Cells["DinhMuc"].Value);
            CapNhatThanhTonKho();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (choDongBo)
                return;

            try
            {
                fx.TaoXml("SELECT * FROM NguyenLieu", TenBang, TenFile);
                txtMaNL.Text = fx.TaoMa("NL", TenFile, "MaNL");
                txtTenNL.Clear();
                txtDVT.Clear();
                txtTonKho.Text = "0";
                txtDinhMuc.Text = "0";
                cheDo = "them";
                CapNhatThanhTonKho();
                CapNhatTrangThaiNut();
                txtTenNL.Focus();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể tạo mã nguyên liệu", ex);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (choDongBo || dgvNguyenLieu.CurrentRow == null)
                return;

            cheDo = "sua";
            CapNhatTrangThaiNut();
            txtTenNL.Focus();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (choDongBo || dgvNguyenLieu.CurrentRow == null)
            {
                if (dgvNguyenLieu.CurrentRow == null)
                    MessageBox.Show("Vui lòng chọn nguyên liệu cần xóa.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string maNL = Convert.ToString(dgvNguyenLieu.CurrentRow.Cells["MaNL"].Value);
            string tenNL = Convert.ToString(dgvNguyenLieu.CurrentRow.Cells["TenNL"].Value);

            try
            {
                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                {
                    conn.Open();

                    // Kiểm tra bảng CongThuc
                    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM CongThuc WHERE MaNL = @MaNL", conn))
                    {
                        cmd.Parameters.Add("@MaNL", SqlDbType.NVarChar, 10).Value = maNL;
                        int countCT = Convert.ToInt32(cmd.ExecuteScalar());
                        if (countCT > 0)
                        {
                            MessageBox.Show(
                                "Không thể xóa nguyên liệu \"" + maNL + "\" (" + tenNL + ") vì đang được sử dụng trong " + countCT + " công thức món ăn.\n" +
                                "Vui lòng gỡ nguyên liệu này khỏi các công thức trước.",
                                "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Kiểm tra bảng ChiTietPhieuNhap
                    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM ChiTietPhieuNhap WHERE MaNL = @MaNL", conn))
                    {
                        cmd.Parameters.Add("@MaNL", SqlDbType.NVarChar, 10).Value = maNL;
                        int countCTPN = Convert.ToInt32(cmd.ExecuteScalar());
                        if (countCTPN > 0)
                        {
                            MessageBox.Show(
                                "Không thể xóa nguyên liệu \"" + maNL + "\" (" + tenNL + ") vì đã phát sinh trong " + countCTPN + " chi tiết phiếu nhập.\n" +
                                "Dữ liệu cần được lưu lại để đối chiếu sổ sách nhập hàng.",
                                "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }

                if (MessageBox.Show(
                    "Bạn có chắc muốn xóa nguyên liệu " + maNL + " (" + tenNL + ") không?",
                    "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                if (fx.Xoa(TenFile, new Dictionary<string, string> { { "MaNL", maNL } }) == 0)
                    throw new InvalidOperationException("Không tìm thấy nguyên liệu trong file XML.");

                choDongBo = true;
                hanhDongChoDongBo = "xóa nguyên liệu";
                CapNhatTrangThaiNut();
                ThuDongBo();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể xóa hoặc đồng bộ nguyên liệu", ex);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (choDongBo)
            {
                ThuDongBo();
                return;
            }

            string tenNL = txtTenNL.Text.Trim();
            if (tenNL.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập tên nguyên liệu.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenNL.Focus();
                return;
            }
            if (tenNL.Length > 100)
            {
                MessageBox.Show("Tên nguyên liệu không được vượt quá 100 ký tự.", "Thông tin không hợp lệ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenNL.Focus();
                return;
            }

            string dvt = txtDVT.Text.Trim();
            if (dvt.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập đơn vị tính.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDVT.Focus();
                return;
            }
            if (dvt.Length > 20)
            {
                MessageBox.Show("Đơn vị tính không được vượt quá 20 ký tự.", "Thông tin không hợp lệ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDVT.Focus();
                return;
            }

            if (!ThuLaySo(txtTonKho.Text, out decimal tonKho) || tonKho < 0)
            {
                MessageBox.Show("Tồn kho phải là số thập phân không âm (>= 0).", "Thông tin không hợp lệ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTonKho.Focus();
                return;
            }

            if (!ThuLaySo(txtDinhMuc.Text, out decimal dinhMuc) || dinhMuc < 0)
            {
                MessageBox.Show("Định mức phải là số thập phân không âm (>= 0).", "Thông tin không hợp lệ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDinhMuc.Focus();
                return;
            }

            string maHienTai = txtMaNL.Text.Trim();

            try
            {
                if (cheDo == "them")
                {
                    if (fx.TonTai(TenFile, "MaNL", maHienTai))
                        throw new InvalidOperationException("Mã nguyên liệu đã tồn tại. Hãy hủy thao tác và tạo lại mã.");

                    string[] thuTuCot = { "MaNL", "TenNL", "DVT", "TonKho", "DinhMuc" };
                    var giaTri = new SortedDictionary<string, string>(Comparer<string>.Create((a, b) =>
                        Array.IndexOf(thuTuCot, a).CompareTo(Array.IndexOf(thuTuCot, b))));

                    giaTri.Add("MaNL", maHienTai);
                    giaTri.Add("TenNL", tenNL);
                    giaTri.Add("DVT", dvt);
                    giaTri.Add("TonKho", tonKho.ToString("0.00", CultureInfo.InvariantCulture));
                    giaTri.Add("DinhMuc", dinhMuc.ToString("0.00", CultureInfo.InvariantCulture));

                    fx.Them(TenFile, TenBang, giaTri);
                    hanhDongChoDongBo = "thêm nguyên liệu";
                }
                else if (cheDo == "sua")
                {
                    var khoa = new Dictionary<string, string> { { "MaNL", maHienTai } };
                    var giaTriMoi = new Dictionary<string, string>
                    {
                        { "TenNL", tenNL },
                        { "DVT", dvt },
                        { "TonKho", tonKho.ToString("0.00", CultureInfo.InvariantCulture) },
                        { "DinhMuc", dinhMuc.ToString("0.00", CultureInfo.InvariantCulture) }
                    };

                    if (fx.Sua(TenFile, khoa, giaTriMoi) == 0)
                        throw new InvalidOperationException("Không tìm thấy nguyên liệu cần sửa trong file XML.");

                    hanhDongChoDongBo = "sửa nguyên liệu";
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
                HienLoi("Không thể lưu nguyên liệu", ex);
            }
        }

        private void ThuDongBo()
        {
            try
            {
                fx.CapNhatDb(TenBang, TenFile);
            }
            catch (SqlException ex) when (ex.Number == 547 && hanhDongChoDongBo == "xóa nguyên liệu")
            {
                CapNhatTrangThaiNut();
                MessageBox.Show(
                    "Không thể xóa nguyên liệu vì đang được tham chiếu trong công thức món ăn hoặc chi tiết phiếu nhập. XML vẫn đang chờ đồng bộ; hãy xử lý các tham chiếu rồi nhấn Đồng bộ để thử lại.\r\n\r\n" + ex.Message,
                    "Không thể xóa nguyên liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (Exception ex)
            {
                CapNhatTrangThaiNut();
                MessageBox.Show(
                    "XML đã thay đổi nhưng chưa đồng bộ được nguyên liệu vào CSDL. Nhấn Lưu để thử đồng bộ lại.\r\n\r\n" + ex.Message,
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
                HienLoi("Nguyên liệu đã đồng bộ nhưng không thể tải lại danh sách", ex);
            }
            CapNhatTrangThaiNut();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            if (choDongBo)
                return;

            cheDo = "";
            txtMaNL.Clear();
            txtTenNL.Clear();
            txtDVT.Clear();
            txtTonKho.Clear();
            txtDinhMuc.Clear();
            CapNhatTrangThaiNut();

            if (duLieu != null)
            {
                HienThiDuLieu(duLieu);
                dgvNguyenLieu_SelectionChanged(dgvNguyenLieu, EventArgs.Empty);
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
                string ma = Convert.ToString(dong["MaNL"]);
                string ten = Convert.ToString(dong["TenNL"]);
                if (ma.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0
                    || ten.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0)
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
            btnSua.Enabled = binhThuong && dgvNguyenLieu.CurrentRow != null;
            btnXoa.Enabled = binhThuong && dgvNguyenLieu.CurrentRow != null;
            btnTim.Enabled = binhThuong;
            txtTim.Enabled = binhThuong;

            txtMaNL.ReadOnly = true;
            txtTenNL.ReadOnly = !dangChinhSua;
            txtDVT.ReadOnly = !dangChinhSua;
            txtTonKho.ReadOnly = !dangChinhSua;
            txtDinhMuc.ReadOnly = !dangChinhSua;
            txtMaNL.BackColor = Color.FromArgb(243, 244, 246);
            txtTenNL.BackColor = txtTenNL.ReadOnly ? Color.FromArgb(243, 244, 246) : Color.White;
            txtDVT.BackColor = txtDVT.ReadOnly ? Color.FromArgb(243, 244, 246) : Color.White;
            txtTonKho.BackColor = txtTonKho.ReadOnly ? Color.FromArgb(243, 244, 246) : Color.White;
            txtDinhMuc.BackColor = txtDinhMuc.ReadOnly ? Color.FromArgb(243, 244, 246) : Color.White;

            btnLuu.Enabled = dangChinhSua || choDongBo;
            btnHuy.Enabled = dangChinhSua && !choDongBo;
            btnLuu.Text = choDongBo ? "Đồng bộ" : "Lưu";

            if (choDongBo && !string.IsNullOrEmpty(hanhDongChoDongBo))
                Text = "Quản lý nguyên liệu - chờ đồng bộ " + hanhDongChoDongBo;
            else
                Text = "Quản lý nguyên liệu";
        }

        private static void HienLoi(string noiDung, Exception ex)
        {
            MessageBox.Show(noiDung + ".\r\n\r\n" + ex.Message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private sealed class StockProgressBar : Control
        {
            private double tiLe;
            private Color mauThanh = Color.FromArgb(22, 163, 74);

            public StockProgressBar()
            {
                BackColor = Color.FromArgb(229, 231, 235);
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            }

            public void DatMucTon(decimal tonKho, decimal dinhMuc)
            {
                mauThanh = tonKho <= dinhMuc
                    ? Color.FromArgb(220, 38, 38)
                    : Color.FromArgb(22, 163, 74);
                if (dinhMuc <= 0)
                    tiLe = tonKho > 0 ? 1 : 0;
                else
                    tiLe = Math.Max(0, Math.Min(1, (double)tonKho / ((double)dinhMuc * 3d)));
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle nen = new Rectangle(0, 0, Width, Height);
                using (GraphicsPath duongNen = TaoDuongVien(nen, Height))
                using (SolidBrush brushNen = new SolidBrush(BackColor))
                    e.Graphics.FillPath(brushNen, duongNen);

                int rongThanh = (int)Math.Round(Width * tiLe);
                if (rongThanh <= 0)
                    return;
                Rectangle thanh = new Rectangle(0, 0, Math.Min(Width, rongThanh), Height);
                using (GraphicsPath duongThanh = TaoDuongVien(thanh, Height))
                using (SolidBrush brushThanh = new SolidBrush(mauThanh))
                    e.Graphics.FillPath(brushThanh, duongThanh);
            }
        }
    }
}
