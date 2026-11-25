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
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = GiaoDien.FontThuong;
            ClientSize = new Size(1160, 640);
            MinimumSize = new Size(950, 540);

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

            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None
            };

            // Bên trái: DataGridView danh sách khách hàng
            dgvKhachHang.Dock = DockStyle.Fill;
            dgvKhachHang.SelectionChanged += dgvKhachHang_SelectionChanged;
            split.Panel1.Controls.Add(dgvKhachHang);

            // Bên phải: Tìm kiếm, thông tin chi tiết và các nút thao tác
            var pnlPhai = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(16, 12, 16, 12)
            };

            // Khu vực tìm kiếm theo SĐT
            var grpTim = new GroupBox
            {
                Text = "Tìm kiếm theo SĐT",
                Dock = DockStyle.Top,
                Height = 85,
                Font = GiaoDien.FontDam,
                ForeColor = GiaoDien.MauChinhDam,
                Padding = new Padding(10)
            };

            var pnlTimKiem = new Panel { Dock = DockStyle.Fill };
            txtTim.Dock = DockStyle.Fill;
            txtTim.Font = GiaoDien.FontThuong;
            txtTim.ForeColor = Color.Black;
            btnTim.Dock = DockStyle.Right;
            btnTim.Width = 90;
            pnlTimKiem.Controls.Add(txtTim);
            pnlTimKiem.Controls.Add(btnTim);
            grpTim.Controls.Add(pnlTimKiem);

            // Các ô nhập thông tin
            var fields = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Padding = new Padding(0, 10, 0, 10),
                ColumnCount = 1,
                RowCount = 0,
                AutoSize = true
            };
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            ThemTruong(fields, "Mã khách hàng", txtMaKH, true);
            ThemTruong(fields, "Tên khách hàng (*)", txtTenKH, false);
            ThemTruong(fields, "Số điện thoại (*)", txtSDT, false);
            ThemTruong(fields, "Điểm tích lũy", txtDiemTichLuy, false);
            ThemTruong(fields, "Hạng thành viên", cboHang, false);

            txtTenKH.MaxLength = 100;
            txtSDT.MaxLength = 15;
            txtDiemTichLuy.MaxLength = 10;
            txtDiemTichLuy.TextChanged += txtDiemTichLuy_TextChanged;

            cboHang.DropDownStyle = ComboBoxStyle.DropDownList;
            cboHang.Items.AddRange(new object[] { "Thường", "Bạc", "Vàng" });
            cboHang.SelectedIndex = 0;

            // Hàng nút chức năng
            var nutPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 90,
                Padding = new Padding(0, 10, 0, 0),
                WrapContents = true
            };
            foreach (Button btn in new[] { btnThem, btnSua, btnXoa, btnLuu, btnHuy })
            {
                btn.Width = 96;
                btn.Height = 38;
                btn.Margin = new Padding(4);
                nutPanel.Controls.Add(btn);
            }

            pnlPhai.Controls.Add(nutPanel);
            pnlPhai.Controls.Add(fields);
            pnlPhai.Controls.Add(grpTim);

            split.Panel2.Controls.Add(pnlPhai);

            Controls.Add(split);
            Controls.Add(GiaoDien.TaoTieuDe("QUẢN LÝ KHÁCH HÀNG"));

            Load += (s, e) =>
            {
                if (split.Width > 850)
                {
                    split.Panel1MinSize = 450;
                    split.Panel2MinSize = 380;
                    split.SplitterDistance = Math.Min(670, split.Width - 380);
                }
            };

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
                Name = "lbl" + control.Name.Substring(3),
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
            GiaoDien.LamDepBang(dgvKhachHang, new Dictionary<string, string>
            {
                { "MaKH", "Mã khách hàng" },
                { "TenKH", "Tên khách hàng" },
                { "SDT", "Số điện thoại" },
                { "DiemTichLuy", "Điểm tích lũy" },
                { "Hang", "Hạng thành viên" }
            });

            if (dgvKhachHang.Columns.Contains("DiemTichLuy"))
            {
                dgvKhachHang.Columns["DiemTichLuy"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvKhachHang.Columns["DiemTichLuy"].DefaultCellStyle.Format = "#,##0";
            }

            CapNhatTrangThaiNut();
        }

        private void dgvKhachHang_SelectionChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cheDo) || choDongBo || dgvKhachHang.CurrentRow == null)
                return;

            txtMaKH.Text = Convert.ToString(dgvKhachHang.CurrentRow.Cells["MaKH"].Value);
            txtTenKH.Text = Convert.ToString(dgvKhachHang.CurrentRow.Cells["TenKH"].Value);
            txtSDT.Text = Convert.ToString(dgvKhachHang.CurrentRow.Cells["SDT"].Value);
            txtDiemTichLuy.Text = Convert.ToString(dgvKhachHang.CurrentRow.Cells["DiemTichLuy"].Value);
            string hang = Convert.ToString(dgvKhachHang.CurrentRow.Cells["Hang"].Value);
            if (!string.IsNullOrEmpty(hang) && cboHang.Items.Contains(hang))
                cboHang.SelectedItem = hang;
            else
                cboHang.SelectedIndex = 0;
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
            txtDiemTichLuy.ReadOnly = !dangChinhSua;
            cboHang.Enabled = dangChinhSua;

            btnLuu.Enabled = dangChinhSua || choDongBo;
            btnHuy.Enabled = dangChinhSua && !choDongBo;
            btnLuu.Text = choDongBo ? "Đồng bộ" : "Lưu";

            if (choDongBo && !string.IsNullOrEmpty(hanhDongChoDongBo))
                Text = "Quản lý khách hàng - chờ đồng bộ " + hanhDongChoDongBo;
            else
                Text = "Quản lý khách hàng";
        }

        private static void HienLoi(string noiDung, Exception ex)
        {
            MessageBox.Show(noiDung + ".\r\n\r\n" + ex.Message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
