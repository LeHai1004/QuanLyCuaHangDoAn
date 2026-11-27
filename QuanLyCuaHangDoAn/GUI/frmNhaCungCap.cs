using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QuanLyCuaHangDoAn.Class;

namespace QuanLyCuaHangDoAn.GUI
{
    public class frmNhaCungCap : Form
    {
        private const string TenFile = "NhaCungCap.xml";
        private const string TenBang = "NhaCungCap";

        private readonly FileXml fx = new FileXml();
        private readonly DataGridView dgvNhaCungCap = new DataGridView();
        private readonly TextBox txtMaNCC = new TextBox();
        private readonly TextBox txtTenNCC = new TextBox();
        private readonly TextBox txtSDT = new TextBox();
        private readonly TextBox txtDiaChi = new TextBox();
        private readonly TextBox txtTim = new TextBox();

        private readonly Button btnThem = GiaoDien.TaoNut("Thêm", GiaoDien.MauChinh);
        private readonly Button btnSua = GiaoDien.TaoNut("Sửa", GiaoDien.MauChinh);
        private readonly Button btnXoa = GiaoDien.TaoNut("Xóa", GiaoDien.MauDo);
        private readonly Button btnLuu = GiaoDien.TaoNut("Lưu", GiaoDien.MauXanh);
        private readonly Button btnHuy = GiaoDien.TaoNut("Hủy", GiaoDien.MauChinhDam);
        private readonly Button btnTim = GiaoDien.TaoNut("Tìm", GiaoDien.MauChinh);
        private readonly ToolTip toolTipNhaCungCap = new ToolTip();

        private DataTable duLieu;
        private Label lblTongNCC;
        private Label lblTongPhieuNhap;
        private Label lblNCCDangChon;
        private AvatarNhaCungCap avatarNCC;
        private string cheDo = "";
        private bool choDongBo;
        private string hanhDongChoDongBo;

        public frmNhaCungCap()
        {
            KhoiTaoGiaoDien();
            Load += frmNhaCungCap_Load;
            FormClosed += (sender, e) => toolTipNhaCungCap.Dispose();
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Quản lý nhà cung cấp";
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 246, 248);
            Font = new Font("Segoe UI", 10F);
            ClientSize = new Size(1100, 680);
            DoubleBuffered = true;

            dgvNhaCungCap.Name = "dgvNhaCungCap";
            txtMaNCC.Name = "txtMaNCC";
            txtTenNCC.Name = "txtTenNCC";
            txtSDT.Name = "txtSDT";
            txtDiaChi.Name = "txtDiaChi";
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
                Text = "QUẢN LÝ NHÀ CUNG CẤP",
                Location = new Point(30, 12),
                Size = new Size(650, 32),
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false
            });
            pnlHeader.Controls.Add(new Label
            {
                Name = "lblPhuDe",
                Text = "Nơi nhập nguyên liệu",
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
                ColumnCount = 3,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Color.White
            };
            tlpThongKe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpThongKe.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 12F));
            tlpThongKe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpThongKe.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpThongKe.Controls.Add(TaoTheThongKe("pnlThongKeNCC", "Tổng nhà cung cấp",
                out lblTongNCC), 0, 0);
            tlpThongKe.Controls.Add(TaoTheThongKe("pnlThongKePhieuNhap", "Tổng phiếu nhập",
                out lblTongPhieuNhap), 2, 0);
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

            dgvNhaCungCap.Location = new Point(20, 145);
            dgvNhaCungCap.Size = new Size(650, 385);
            dgvNhaCungCap.Anchor = AnchorStyles.Top | AnchorStyles.Left |
                AnchorStyles.Right | AnchorStyles.Bottom;
            dgvNhaCungCap.BorderStyle = BorderStyle.None;
            dgvNhaCungCap.BackgroundColor = Color.White;
            dgvNhaCungCap.EnableHeadersVisualStyles = false;
            dgvNhaCungCap.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvNhaCungCap.GridColor = Color.FromArgb(243, 244, 246);
            dgvNhaCungCap.RowHeadersVisible = false;
            dgvNhaCungCap.AllowUserToAddRows = false;
            dgvNhaCungCap.AllowUserToDeleteRows = false;
            dgvNhaCungCap.AllowUserToResizeRows = false;
            dgvNhaCungCap.ReadOnly = true;
            dgvNhaCungCap.MultiSelect = false;
            dgvNhaCungCap.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvNhaCungCap.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvNhaCungCap.AutoGenerateColumns = false;
            dgvNhaCungCap.ColumnHeadersHeight = 38;
            dgvNhaCungCap.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvNhaCungCap.RowTemplate.Height = 44;
            dgvNhaCungCap.ShowCellToolTips = true;
            dgvNhaCungCap.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 243, 234);
            dgvNhaCungCap.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(154, 52, 18);
            dgvNhaCungCap.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvNhaCungCap.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvNhaCungCap.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgvNhaCungCap.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 4, 0);
            dgvNhaCungCap.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 243, 234);
            dgvNhaCungCap.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(154, 52, 18);
            dgvNhaCungCap.DefaultCellStyle.BackColor = Color.White;
            dgvNhaCungCap.DefaultCellStyle.ForeColor = Color.FromArgb(17, 24, 39);
            dgvNhaCungCap.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvNhaCungCap.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgvNhaCungCap.DefaultCellStyle.Padding = new Padding(8, 0, 4, 0);
            dgvNhaCungCap.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 243, 234);
            dgvNhaCungCap.DefaultCellStyle.SelectionForeColor = Color.FromArgb(17, 24, 39);
            dgvNhaCungCap.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            dgvNhaCungCap.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 243, 234);
            dgvNhaCungCap.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.FromArgb(17, 24, 39);
            dgvNhaCungCap.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaNCC",
                DataPropertyName = "MaNCC",
                HeaderText = "Mã NCC",
                FillWeight = 14F,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            dgvNhaCungCap.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenNCC",
                DataPropertyName = "TenNCC",
                HeaderText = "Tên nhà cung cấp",
                FillWeight = 30F,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            dgvNhaCungCap.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SDT",
                DataPropertyName = "SDT",
                HeaderText = "Số điện thoại",
                FillWeight = 20F,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            dgvNhaCungCap.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DiaChi",
                DataPropertyName = "DiaChi",
                HeaderText = "Địa chỉ",
                FillWeight = 36F,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            dgvNhaCungCap.SelectionChanged += dgvNhaCungCap_SelectionChanged;
            dgvNhaCungCap.CellPainting += dgvNhaCungCap_CellPainting;
            dgvNhaCungCap.CellToolTipTextNeeded += dgvNhaCungCap_CellToolTipTextNeeded;
            pnlTrai.Controls.Add(dgvNhaCungCap);

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
                Name = "lblThongTinNhaCungCap",
                Text = "THÔNG TIN NHÀ CUNG CẤP",
                Location = new Point(20, 15),
                Size = new Size(285, 22),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(232, 101, 26),
                AutoSize = false
            });

            avatarNCC = new AvatarNhaCungCap
            {
                Name = "avatarNCC",
                Location = new Point(20, 50),
                Size = new Size(52, 52)
            };
            pnlPhai.Controls.Add(avatarNCC);
            lblNCCDangChon = new Label
            {
                Name = "lblNCCDangChon",
                Text = "Chưa chọn nhà cung cấp",
                Location = new Point(80, 50),
                Size = new Size(205, 52),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                AutoEllipsis = true,
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlPhai.Controls.Add(lblNCCDangChon);

            txtDiaChi.Multiline = true;
            txtDiaChi.MaxLength = 200;
            txtDiaChi.ScrollBars = ScrollBars.None;
            ThemTruong(pnlPhai, "Mã nhà cung cấp", txtMaNCC, true, 120, 34);
            ThemTruong(pnlPhai, "Tên nhà cung cấp (*)", txtTenNCC, false, 180, 34);
            ThemTruong(pnlPhai, "Số điện thoại (*)", txtSDT, false, 240, 34);
            ThemTruong(pnlPhai, "Địa chỉ", txtDiaChi, false, 300, 64);

            txtTenNCC.MaxLength = 100;
            txtSDT.MaxLength = 15;
            txtTenNCC.TextChanged += txtTenNCC_TextChanged;

            var tlpNut = new TableLayoutPanel
            {
                Name = "tlpNut",
                Location = new Point(20, 400),
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
            btnTim.Click += btnTim_Click;
            txtTim.KeyDown += txtTim_KeyDown;

            CapNhatTrangThaiNut();
        }

        private static Panel TaoTheThongKe(string ten, string nhan, out Label giaTri)
        {
            var panel = new Panel
            {
                Name = ten,
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(249, 250, 251),
                Margin = Padding.Empty
            };
            giaTri = new Label
            {
                Name = "lbl" + ten.Substring(10),
                Text = "0",
                Location = new Point(4, 4),
                Size = new Size(311, 27),
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false
            };
            panel.Controls.Add(giaTri);
            panel.Controls.Add(new Label
            {
                Name = "lblNhan" + ten.Substring(10),
                Text = nhan,
                Location = new Point(4, 34),
                Size = new Size(311, 18),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(107, 114, 128),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false
            });
            return panel;
        }

        private static void ThemTruong(Panel panel, string nhan, Control control,
            bool chiDoc, int y, int chieuCao)
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
                textBox.ReadOnly = chiDoc;
                textBox.BackColor = chiDoc ? Color.FromArgb(243, 244, 246) : Color.White;
                if (!textBox.Multiline)
                    textBox.AutoSize = false;
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

        private void frmNhaCungCap_Load(object sender, EventArgs e)
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
                HienLoi("Không thể tải danh sách nhà cung cấp", ex);
            }
        }

        private void TaiDuLieu()
        {
            fx.TaoXml("SELECT * FROM NhaCungCap", TenBang, TenFile);
            duLieu = fx.HienThi(TenFile);
            HienThiDuLieu(duLieu);
        }

        private void HienThiDuLieu(DataTable nguon)
        {
            dgvNhaCungCap.DataSource = nguon != null ? nguon.Copy() : null;
            CapNhatThongKe();
            dgvNhaCungCap_SelectionChanged(dgvNhaCungCap, EventArgs.Empty);
            CapNhatTrangThaiNut();
        }

        private void CapNhatThongKe()
        {
            lblTongNCC.Text = duLieu == null ? "0" : duLieu.Rows.Count.ToString();
            try
            {
                DataTable phieuNhap = fx.HienThi("PhieuNhap.xml");
                lblTongPhieuNhap.Text = phieuNhap == null ? "--" : phieuNhap.Rows.Count.ToString();
            }
            catch (Exception)
            {
                lblTongPhieuNhap.Text = "--";
            }
        }

        private void dgvNhaCungCap_SelectionChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cheDo) || choDongBo)
                return;

            if (dgvNhaCungCap.CurrentRow == null)
            {
                txtMaNCC.Clear();
                txtTenNCC.Clear();
                txtSDT.Clear();
                txtDiaChi.Clear();
                CapNhatThongTinAvatar();
                CapNhatTrangThaiNut();
                return;
            }

            txtMaNCC.Text = Convert.ToString(dgvNhaCungCap.CurrentRow.Cells["MaNCC"].Value);
            txtTenNCC.Text = Convert.ToString(dgvNhaCungCap.CurrentRow.Cells["TenNCC"].Value);
            txtSDT.Text = Convert.ToString(dgvNhaCungCap.CurrentRow.Cells["SDT"].Value);
            txtDiaChi.Text = Convert.ToString(dgvNhaCungCap.CurrentRow.Cells["DiaChi"].Value);
            CapNhatThongTinAvatar();
            CapNhatTrangThaiNut();
        }

        private void txtTenNCC_TextChanged(object sender, EventArgs e)
        {
            CapNhatThongTinAvatar();
        }

        private void CapNhatThongTinAvatar()
        {
            string ten = txtTenNCC.Text.Trim();
            avatarNCC.DatTen(ten);
            lblNCCDangChon.Text = ten.Length == 0 ? "Chưa chọn nhà cung cấp" : ten;
            toolTipNhaCungCap.SetToolTip(lblNCCDangChon, ten);
            toolTipNhaCungCap.SetToolTip(avatarNCC, ten);
        }

        private void dgvNhaCungCap_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            e.PaintBackground(e.CellBounds, true);
            Rectangle vungChu = e.CellBounds;
            vungChu.X += e.CellStyle.Padding.Left;
            vungChu.Width -= e.CellStyle.Padding.Left + e.CellStyle.Padding.Right;
            TextRenderer.DrawText(e.Graphics, Convert.ToString(e.FormattedValue), e.CellStyle.Font,
                vungChu, e.CellStyle.ForeColor, TextFormatFlags.Left |
                TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            e.Paint(e.CellBounds, DataGridViewPaintParts.Border);
            e.Handled = true;
        }

        private void dgvNhaCungCap_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                e.ToolTipText = Convert.ToString(
                    dgvNhaCungCap.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue);
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (choDongBo)
                return;

            try
            {
                fx.TaoXml("SELECT * FROM NhaCungCap", TenBang, TenFile);
                txtMaNCC.Text = fx.TaoMa("NCC", TenFile, "MaNCC");
                txtTenNCC.Clear();
                txtSDT.Clear();
                txtDiaChi.Clear();
                cheDo = "them";
                CapNhatTrangThaiNut();
                txtTenNCC.Focus();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể tạo mã nhà cung cấp", ex);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (choDongBo || dgvNhaCungCap.CurrentRow == null)
                return;

            cheDo = "sua";
            CapNhatTrangThaiNut();
            txtTenNCC.Focus();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (choDongBo || dgvNhaCungCap.CurrentRow == null)
            {
                if (dgvNhaCungCap.CurrentRow == null)
                    MessageBox.Show("Vui lòng chọn nhà cung cấp cần xóa.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string maNCC = Convert.ToString(dgvNhaCungCap.CurrentRow.Cells["MaNCC"].Value);
            string tenNCC = Convert.ToString(dgvNhaCungCap.CurrentRow.Cells["TenNCC"].Value);

            try
            {
                // Kiểm tra ràng buộc với bảng PhieuNhap
                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM PhieuNhap WHERE MaNCC = @MaNCC", conn))
                    {
                        cmd.Parameters.Add("@MaNCC", SqlDbType.NVarChar, 10).Value = maNCC;
                        int countPN = Convert.ToInt32(cmd.ExecuteScalar());
                        if (countPN > 0)
                        {
                            MessageBox.Show(
                                "Không thể xóa nhà cung cấp \"" + maNCC + "\" (" + tenNCC + ") vì đã phát sinh " + countPN + " phiếu nhập liên quan.\n" +
                                "Dữ liệu cần được lưu lại để quản lý lịch sử nhập hàng.",
                                "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }

                if (MessageBox.Show(
                    "Bạn có chắc muốn xóa nhà cung cấp " + maNCC + " (" + tenNCC + ") không?",
                    "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                if (fx.Xoa(TenFile, new Dictionary<string, string> { { "MaNCC", maNCC } }) == 0)
                    throw new InvalidOperationException("Không tìm thấy nhà cung cấp trong file XML.");

                choDongBo = true;
                hanhDongChoDongBo = "xóa nhà cung cấp";
                CapNhatTrangThaiNut();
                ThuDongBo();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể xóa hoặc đồng bộ nhà cung cấp", ex);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (choDongBo)
            {
                ThuDongBo();
                return;
            }

            string tenNCC = txtTenNCC.Text.Trim();
            if (tenNCC.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập tên nhà cung cấp.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenNCC.Focus();
                return;
            }
            if (tenNCC.Length > 100)
            {
                MessageBox.Show("Tên nhà cung cấp không được vượt quá 100 ký tự.", "Thông tin không hợp lệ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenNCC.Focus();
                return;
            }

            string sdt = txtSDT.Text.Trim();
            if (sdt.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập số điện thoại nhà cung cấp.", "Thiếu thông tin",
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

            string diaChi = txtDiaChi.Text.Trim();
            if (diaChi.Length > 200)
            {
                MessageBox.Show("Địa chỉ không được vượt quá 200 ký tự.", "Thông tin không hợp lệ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDiaChi.Focus();
                return;
            }

            string maHienTai = txtMaNCC.Text.Trim();

            try
            {
                if (cheDo == "them")
                {
                    if (fx.TonTai(TenFile, "MaNCC", maHienTai))
                        throw new InvalidOperationException("Mã nhà cung cấp đã tồn tại. Hãy hủy thao tác và tạo lại mã.");

                    string[] thuTuCot = { "MaNCC", "TenNCC", "SDT", "DiaChi" };
                    var giaTri = new SortedDictionary<string, string>(Comparer<string>.Create((a, b) =>
                        Array.IndexOf(thuTuCot, a).CompareTo(Array.IndexOf(thuTuCot, b))));

                    giaTri.Add("MaNCC", maHienTai);
                    giaTri.Add("TenNCC", tenNCC);
                    giaTri.Add("SDT", sdt);
                    giaTri.Add("DiaChi", diaChi);

                    fx.Them(TenFile, TenBang, giaTri);
                    hanhDongChoDongBo = "thêm nhà cung cấp";
                }
                else if (cheDo == "sua")
                {
                    var khoa = new Dictionary<string, string> { { "MaNCC", maHienTai } };
                    var giaTriMoi = new Dictionary<string, string>
                    {
                        { "TenNCC", tenNCC },
                        { "SDT", sdt },
                        { "DiaChi", diaChi }
                    };

                    if (fx.Sua(TenFile, khoa, giaTriMoi) == 0)
                        throw new InvalidOperationException("Không tìm thấy nhà cung cấp cần sửa trong file XML.");

                    hanhDongChoDongBo = "sửa nhà cung cấp";
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
                HienLoi("Không thể lưu nhà cung cấp", ex);
            }
        }

        private void ThuDongBo()
        {
            try
            {
                fx.CapNhatDb(TenBang, TenFile);
            }
            catch (SqlException ex) when (ex.Number == 547 && hanhDongChoDongBo == "xóa nhà cung cấp")
            {
                CapNhatTrangThaiNut();
                MessageBox.Show(
                    "Không thể xóa nhà cung cấp vì vẫn còn phiếu nhập tham chiếu đến nhà cung cấp này. XML vẫn đang chờ đồng bộ; hãy xử lý các tham chiếu rồi nhấn Đồng bộ để thử lại.\r\n\r\n" + ex.Message,
                    "Không thể xóa nhà cung cấp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (Exception ex)
            {
                CapNhatTrangThaiNut();
                MessageBox.Show(
                    "XML đã thay đổi nhưng chưa đồng bộ được nhà cung cấp vào CSDL. Nhấn Lưu để thử đồng bộ lại.\r\n\r\n" + ex.Message,
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
                HienLoi("Nhà cung cấp đã đồng bộ nhưng không thể tải lại danh sách", ex);
            }
            CapNhatTrangThaiNut();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            if (choDongBo)
                return;

            cheDo = "";
            txtMaNCC.Clear();
            txtTenNCC.Clear();
            txtSDT.Clear();
            txtDiaChi.Clear();
            CapNhatTrangThaiNut();

            if (duLieu != null)
            {
                HienThiDuLieu(duLieu);
                dgvNhaCungCap_SelectionChanged(dgvNhaCungCap, EventArgs.Empty);
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
                string ma = Convert.ToString(dong["MaNCC"]);
                string ten = Convert.ToString(dong["TenNCC"]);
                string sdt = Convert.ToString(dong["SDT"]);
                if (ma.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0
                    || ten.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0
                    || sdt.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0)
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
            btnSua.Enabled = binhThuong && dgvNhaCungCap.CurrentRow != null;
            btnXoa.Enabled = binhThuong && dgvNhaCungCap.CurrentRow != null;
            btnTim.Enabled = binhThuong;
            txtTim.Enabled = binhThuong;

            txtMaNCC.ReadOnly = true;
            txtTenNCC.ReadOnly = !dangChinhSua;
            txtSDT.ReadOnly = !dangChinhSua;
            txtDiaChi.ReadOnly = !dangChinhSua;
            txtMaNCC.BackColor = Color.FromArgb(243, 244, 246);
            txtTenNCC.BackColor = txtTenNCC.ReadOnly ? Color.FromArgb(243, 244, 246) : Color.White;
            txtSDT.BackColor = txtSDT.ReadOnly ? Color.FromArgb(243, 244, 246) : Color.White;
            txtDiaChi.BackColor = txtDiaChi.ReadOnly ? Color.FromArgb(243, 244, 246) : Color.White;

            btnLuu.Enabled = dangChinhSua || choDongBo;
            btnHuy.Enabled = dangChinhSua && !choDongBo;
            btnLuu.Text = choDongBo ? "Đồng bộ" : "Lưu";

            if (choDongBo && !string.IsNullOrEmpty(hanhDongChoDongBo))
                Text = "Quản lý nhà cung cấp - chờ đồng bộ " + hanhDongChoDongBo;
            else
                Text = "Quản lý nhà cung cấp";
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

        private sealed class AvatarNhaCungCap : Control
        {
            private string tenNCC = "";

            public AvatarNhaCungCap()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            }

            public void DatTen(string ten)
            {
                tenNCC = ten ?? "";
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                Rectangle hinhTron = new Rectangle(1, 1, Width - 2, Height - 2);
                bool coTen = tenNCC.Trim().Length > 0;
                Color mauNen = coTen
                    ? Color.FromArgb(232, 101, 26)
                    : Color.FromArgb(209, 213, 219);
                using (SolidBrush nen = new SolidBrush(mauNen))
                    e.Graphics.FillEllipse(nen, hinhTron);

                string chuCai = coTen ? tenNCC.Trim().Substring(0, 1).ToUpperInvariant() : "?";
                using (Font font = new Font("Segoe UI", 18F, FontStyle.Bold))
                {
                    TextRenderer.DrawText(e.Graphics, chuCai, font, ClientRectangle,
                        Color.White, TextFormatFlags.HorizontalCenter |
                        TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine |
                        TextFormatFlags.NoPadding);
                }
            }
        }
    }
}
