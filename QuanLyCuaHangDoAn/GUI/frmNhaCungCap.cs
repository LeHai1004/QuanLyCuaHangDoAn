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
        private readonly SplitContainer split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None
        };

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

        private DataTable duLieu;
        private string cheDo = "";
        private bool choDongBo;
        private string hanhDongChoDongBo;

        public frmNhaCungCap()
        {
            KhoiTaoGiaoDien();
            Load += frmNhaCungCap_Load;
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Quản lý nhà cung cấp";
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = GiaoDien.FontThuong;
            ClientSize = new Size(1160, 640);
            MinimumSize = new Size(900, 540);

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

            dgvNhaCungCap.Dock = DockStyle.Fill;
            dgvNhaCungCap.SelectionChanged += dgvNhaCungCap_SelectionChanged;

            split.Panel1.Controls.Add(dgvNhaCungCap);
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

            ThemTruong(fields, "Mã nhà cung cấp", txtMaNCC, true);
            ThemTruong(fields, "Tên nhà cung cấp (*)", txtTenNCC, false);
            ThemTruong(fields, "Số điện thoại (*)", txtSDT, false);
            ThemTruong(fields, "Địa chỉ", txtDiaChi, false);

            txtTenNCC.MaxLength = 100;
            txtSDT.MaxLength = 15;
            txtDiaChi.MaxLength = 200;

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
            Controls.Add(GiaoDien.TaoTieuDe("QUẢN LÝ NHÀ CUNG CẤP"));

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

        private void frmNhaCungCap_Load(object sender, EventArgs e)
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
            GiaoDien.LamDepBang(dgvNhaCungCap, new Dictionary<string, string>
            {
                { "MaNCC", "Mã NCC" },
                { "TenNCC", "Tên nhà cung cấp" },
                { "SDT", "Số điện thoại" },
                { "DiaChi", "Địa chỉ" }
            });

            CapNhatTrangThaiNut();
        }

        private void dgvNhaCungCap_SelectionChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cheDo) || choDongBo || dgvNhaCungCap.CurrentRow == null)
                return;

            txtMaNCC.Text = Convert.ToString(dgvNhaCungCap.CurrentRow.Cells["MaNCC"].Value);
            txtTenNCC.Text = Convert.ToString(dgvNhaCungCap.CurrentRow.Cells["TenNCC"].Value);
            txtSDT.Text = Convert.ToString(dgvNhaCungCap.CurrentRow.Cells["SDT"].Value);
            txtDiaChi.Text = Convert.ToString(dgvNhaCungCap.CurrentRow.Cells["DiaChi"].Value);
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
    }
}
