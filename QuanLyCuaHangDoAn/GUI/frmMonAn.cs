using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = GiaoDien.FontThuong;
            ClientSize = new Size(1220, 680);
            MinimumSize = new Size(980, 560);

            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Size = new Size(1220, 624),
                BorderStyle = BorderStyle.None
            };
            split.Panel1MinSize = 460;
            split.Panel2MinSize = 380;
            split.SplitterDistance = 700;

            var timPanel = new Panel { Dock = DockStyle.Top, Height = 48, Padding = new Padding(6) };
            txtTim.Dock = DockStyle.Fill;
            btnTim.Dock = DockStyle.Right;
            btnTim.Width = 100;
            timPanel.Controls.Add(txtTim);
            timPanel.Controls.Add(btnTim);

            dgv.Dock = DockStyle.Fill;
            dgv.SelectionChanged += dgv_SelectionChanged;
            split.Panel1.Controls.Add(dgv);
            split.Panel1.Controls.Add(timPanel);

            var fields = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Padding = new Padding(12),
                ColumnCount = 1,
                RowCount = 0,
                AutoSize = true
            };
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            ThemTruong(fields, "Mã món", txtMaMon, true);
            ThemTruong(fields, "Tên món", txtTenMon, false);
            ThemTruong(fields, "Danh mục", cboDanhMuc, false);
            ThemTruong(fields, "Đơn giá", nudDonGia, false);
            ThemTruong(fields, "Đơn vị tính", txtDVT, false);
            ThemTruong(fields, "Trạng thái", cboTrangThai, false);

            cboDanhMuc.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTrangThai.Items.AddRange(new object[] { "Còn bán", "Hết" });
            nudDonGia.Minimum = 0;
            nudDonGia.Maximum = 999999999999999999m;
            nudDonGia.DecimalPlaces = 0;
            nudDonGia.ThousandsSeparator = true;
            nudDonGia.Dock = DockStyle.Fill;

            var nutPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 100,
                Padding = new Padding(8),
                WrapContents = true
            };
            foreach (Button btn in new[] { btnThem, btnSua, btnXoa, btnLuu, btnHuy })
            {
                btn.Width = 108;
                btn.Margin = new Padding(5);
                nutPanel.Controls.Add(btn);
            }

            split.Panel2.Controls.Add(nutPanel);
            split.Panel2.Controls.Add(fields);
            Controls.Add(split);
            Controls.Add(GiaoDien.TaoTieuDe("QUẢN LÝ MÓN ĂN"));

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
            panel.Controls.Add(new Label { Text = nhan, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, row);
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            control.Dock = DockStyle.Fill;
            if (control is TextBox) ((TextBox)control).ReadOnly = chiDoc;
            panel.Controls.Add(control, 0, row + 1);
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
            cboDanhMuc.DataSource = danhMuc;
            cboDanhMuc.DisplayMember = "TenDM";
            cboDanhMuc.ValueMember = "MaDM";
            HienThiDuLieu(duLieu);
        }

        private void HienThiDuLieu(DataTable nguon)
        {
            dgv.DataSource = nguon.Copy();
            GiaoDien.LamDepBang(dgv, new Dictionary<string, string>
            {
                { "MaMon", "Mã món" }, { "TenMon", "Tên món" }, { "MaDM", "Mã danh mục" },
                { "DonGia", "Đơn giá" }, { "DVT", "Đơn vị tính" }, { "TrangThai", "Trạng thái" }
            });
            GiaoDien.DinhDangTien(dgv, "DonGia");
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
            nudDonGia.Enabled = dangChinhSua;
            cboDanhMuc.Enabled = dangChinhSua;
            cboTrangThai.Enabled = dangChinhSua;
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
    }
}
