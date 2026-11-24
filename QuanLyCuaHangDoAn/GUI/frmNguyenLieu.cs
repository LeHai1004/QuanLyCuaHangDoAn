using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
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
        private readonly SplitContainer split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None
        };

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
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = GiaoDien.FontThuong;
            ClientSize = new Size(1160, 640);
            MinimumSize = new Size(900, 540);

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

            // Panel tìm kiếm phía trên bảng
            var timPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 48,
                Padding = new Padding(6)
            };
            txtTim.Dock = DockStyle.Fill;
            txtTim.Font = GiaoDien.FontThuong;
            btnTim.Dock = DockStyle.Right;
            btnTim.Width = 90;
            timPanel.Controls.Add(txtTim);
            timPanel.Controls.Add(btnTim);

            dgvNguyenLieu.Dock = DockStyle.Fill;
            dgvNguyenLieu.SelectionChanged += dgvNguyenLieu_SelectionChanged;
            dgvNguyenLieu.RowPrePaint += dgvNguyenLieu_RowPrePaint;

            split.Panel1.Controls.Add(dgvNguyenLieu);
            split.Panel1.Controls.Add(timPanel);

            // Bảng nhập liệu bên phải
            var fields = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Padding = new Padding(12),
                ColumnCount = 1,
                RowCount = 0,
                AutoSize = true
            };
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            ThemTruong(fields, "Mã nguyên liệu", txtMaNL, true);
            ThemTruong(fields, "Tên nguyên liệu (*)", txtTenNL, false);
            ThemTruong(fields, "Đơn vị tính (*)", txtDVT, false);
            ThemTruong(fields, "Tồn kho (*)", txtTonKho, false);
            ThemTruong(fields, "Định mức tối thiểu (*)", txtDinhMuc, false);

            txtTenNL.MaxLength = 100;
            txtDVT.MaxLength = 20;

            // Hàng nút thao tác
            var nutPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 90,
                Padding = new Padding(8),
                WrapContents = true
            };
            foreach (Button btn in new[] { btnThem, btnSua, btnXoa, btnLuu, btnHuy })
            {
                btn.Width = 100;
                btn.Height = 38;
                btn.Margin = new Padding(5);
                nutPanel.Controls.Add(btn);
            }

            split.Panel2.Controls.Add(nutPanel);
            split.Panel2.Controls.Add(fields);

            // Controls.Add split vào Form trước khi thiết lập kích thước chi tiết trong Load
            Controls.Add(split);
            Controls.Add(GiaoDien.TaoTieuDe("QUẢN LÝ NGUYÊN LIỆU"));

            btnThem.Click += btnThem_Click;
            btnSua.Click += btnSua_Click;
            btnXoa.Click += btnXoa_Click;
            btnLuu.Click += btnLuu_Click;
            btnHuy.Click += btnHuy_Click;
            btnTim.Click += btnTim_Click;
            txtTim.KeyDown += txtTim_KeyDown;

            CapNhatTrangThaiNut();
        }

        private static void ThemTruong(TableLayoutPanel panel, string nhan, Control control, bool chiDoc)
        {
            int row = panel.RowCount;
            panel.RowCount += 2;
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
            panel.Controls.Add(new Label
            {
                Text = nhan,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = GiaoDien.FontDam,
                ForeColor = Color.FromArgb(60, 60, 60)
            }, 0, row);

            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            control.Dock = DockStyle.Fill;
            control.Font = GiaoDien.FontThuong;
            if (control is TextBox)
            {
                ((TextBox)control).ReadOnly = chiDoc;
            }
            panel.Controls.Add(control, 0, row + 1);
        }

        private void frmNguyenLieu_Load(object sender, EventArgs e)
        {
            // Thiết lập kích thước SplitContainer an toàn trên màn hình scale cao
            if (split.Width > 850)
            {
                split.Panel1MinSize = 420;
                split.Panel2MinSize = 350;
                split.SplitterDistance = 650;
            }

            try
            {
                if (PhienDangNhap.IsLoggedIn && PhienDangNhap.ChucVu != "Admin")
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
            GiaoDien.LamDepBang(dgvNguyenLieu, new Dictionary<string, string>
            {
                { "MaNL", "Mã NL" },
                { "TenNL", "Tên nguyên liệu" },
                { "DVT", "Đơn vị tính" },
                { "TonKho", "Tồn kho" },
                { "DinhMuc", "Định mức" }
            });

            if (dgvNguyenLieu.Columns.Contains("TonKho"))
            {
                dgvNguyenLieu.Columns["TonKho"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvNguyenLieu.Columns["TonKho"].DefaultCellStyle.Format = "#,##0.##";
            }
            if (dgvNguyenLieu.Columns.Contains("DinhMuc"))
            {
                dgvNguyenLieu.Columns["DinhMuc"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvNguyenLieu.Columns["DinhMuc"].DefaultCellStyle.Format = "#,##0.##";
            }

            CapNhatTrangThaiNut();
        }

        private void dgvNguyenLieu_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvNguyenLieu.Rows.Count)
                return;

            DataGridViewRow dong = dgvNguyenLieu.Rows[e.RowIndex];
            object valTonKho = dong.Cells["TonKho"].Value;
            object valDinhMuc = dong.Cells["DinhMuc"].Value;

            if (ThuLaySo(valTonKho, out decimal tonKho) && ThuLaySo(valDinhMuc, out decimal dinhMuc))
            {
                if (tonKho <= dinhMuc)
                {
                    // Cảnh báo sắp hết: nền đỏ nhạt, chữ đậm màu đỏ
                    dong.DefaultCellStyle.BackColor = Color.FromArgb(255, 230, 230);
                    dong.DefaultCellStyle.ForeColor = GiaoDien.MauDo;
                    dong.DefaultCellStyle.Font = GiaoDien.FontDam;
                    dong.DefaultCellStyle.SelectionBackColor = Color.FromArgb(235, 90, 80);
                    dong.DefaultCellStyle.SelectionForeColor = Color.White;
                }
            }
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
            if (!string.IsNullOrEmpty(cheDo) || choDongBo || dgvNguyenLieu.CurrentRow == null)
                return;

            txtMaNL.Text = Convert.ToString(dgvNguyenLieu.CurrentRow.Cells["MaNL"].Value);
            txtTenNL.Text = Convert.ToString(dgvNguyenLieu.CurrentRow.Cells["TenNL"].Value);
            txtDVT.Text = Convert.ToString(dgvNguyenLieu.CurrentRow.Cells["DVT"].Value);
            txtTonKho.Text = Convert.ToString(dgvNguyenLieu.CurrentRow.Cells["TonKho"].Value);
            txtDinhMuc.Text = Convert.ToString(dgvNguyenLieu.CurrentRow.Cells["DinhMuc"].Value);
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
    }
}
