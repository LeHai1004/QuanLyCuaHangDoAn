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
    public class frmCongThuc : Form
    {
        private readonly FileXml fx = new FileXml();
        private readonly SplitContainer split = new SplitContainer { Dock = DockStyle.Fill, BorderStyle = BorderStyle.None };
        private readonly ComboBox cboMonAn = new ComboBox();
        private readonly ComboBox cboNguyenLieu = new ComboBox();
        private readonly TextBox txtSoLuong = new TextBox();
        private readonly DataGridView dgvCongThuc = new DataGridView();
        private readonly Button btnThem = GiaoDien.TaoNut("Thêm", GiaoDien.MauChinh);
        private readonly Button btnSua = GiaoDien.TaoNut("Sửa", GiaoDien.MauChinh);
        private readonly Button btnXoa = GiaoDien.TaoNut("Xóa", GiaoDien.MauDo);
        private readonly Button btnLuu = GiaoDien.TaoNut("Lưu", GiaoDien.MauXanh);
        private readonly Button btnHuy = GiaoDien.TaoNut("Hủy", GiaoDien.MauChinhDam);
        private DataTable monAn;
        private DataTable nguyenLieu;
        private DataTable congThuc;
        private string cheDo = "";
        private string maNLCu = "";

        public frmCongThuc()
        {
            KhoiTaoGiaoDien();
            Load += frmCongThuc_Load;
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Quản lý công thức món";
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = GiaoDien.FontThuong;
            ClientSize = new Size(1120, 650);
            MinimumSize = new Size(900, 540);

            cboMonAn.Name = "cboMonAn";
            cboNguyenLieu.Name = "cboNguyenLieu";
            txtSoLuong.Name = "txtSoLuong";
            dgvCongThuc.Name = "dgvCongThuc";
            cboMonAn.DropDownStyle = ComboBoxStyle.DropDownList;
            cboNguyenLieu.DropDownStyle = ComboBoxStyle.DropDownList;
            cboMonAn.Dock = DockStyle.Fill;
            cboNguyenLieu.Dock = DockStyle.Fill;
            txtSoLuong.Dock = DockStyle.Fill;
            cboMonAn.SelectedIndexChanged += cboMonAn_SelectedIndexChanged;
            dgvCongThuc.SelectionChanged += dgvCongThuc_SelectionChanged;

            dgvCongThuc.Dock = DockStyle.Fill;
            dgvCongThuc.ReadOnly = true;
            dgvCongThuc.AutoGenerateColumns = true;
            dgvCongThuc.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            var trai = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };
            trai.Controls.Add(dgvCongThuc);
            split.Panel1.Controls.Add(trai);

            var phai = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, Padding = new Padding(16), ColumnCount = 1,
                RowCount = 8, AutoSize = false
            };
            phai.RowStyles.Add(new RowStyle(SizeType.Absolute, 74));
            phai.RowStyles.Add(new RowStyle(SizeType.Absolute, 74));
            phai.RowStyles.Add(new RowStyle(SizeType.Absolute, 74));
            phai.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
            phai.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
            phai.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
            phai.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
            phai.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            ThemTruong(phai, "Món ăn", cboMonAn, 0);
            ThemTruong(phai, "Nguyên liệu", cboNguyenLieu, 1);
            ThemTruong(phai, "Số lượng / 1 phần (*)", txtSoLuong, 2);

            var nut = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, Padding = new Padding(0, 6, 0, 0) };
foreach (Button btn in new[] { btnThem, btnSua, btnXoa, btnLuu, btnHuy })
{
    btn.Width = 76;
    btn.Height = 36;
    btn.Margin = new Padding(0, 0, 6, 0);
    nut.Controls.Add(btn);
}
            phai.Controls.Add(nut, 0, 3);
            btnThem.Click += btnThem_Click; btnSua.Click += btnSua_Click;
            btnXoa.Click += btnXoa_Click; btnLuu.Click += btnLuu_Click; btnHuy.Click += btnHuy_Click;
            split.Panel2.Controls.Add(phai);

            Controls.Add(split);
            Controls.Add(GiaoDien.TaoTieuDe("CÔNG THỨC MÓN"));
        }

        private static void ThemTruong(TableLayoutPanel bang, string nhan, Control control, int dong)
        {
            var panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = new Padding(0) };
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            control.Margin = new Padding(0, 2, 0, 6);
            panel.Controls.Add(new Label { Text = nhan, Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft }, 0, 0);
            panel.Controls.Add(control, 0, 1);
            bang.Controls.Add(panel, 0, dong);
        }

        private void frmCongThuc_Load(object sender, EventArgs e)
        {
            if (split.Width > 850)
            {
                split.Panel1MinSize = 420;
                split.Panel2MinSize = 350;
                split.SplitterDistance = 650;
            }
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
            hienThi.Columns.Add("MaNL"); hienThi.Columns.Add("TenNL");
            hienThi.Columns.Add("DVT"); hienThi.Columns.Add("SoLuong", typeof(decimal));
            foreach (DataRow ct in congThuc.Rows)
            {
                if (Convert.ToString(ct["MaMon"]) != maMon) continue;
                DataRow nl = null;
                foreach (DataRow ung in nguyenLieu.Rows)
                    if (Convert.ToString(ung["MaNL"]) == Convert.ToString(ct["MaNL"])) { nl = ung; break; }
                DataRow dong = hienThi.NewRow();
                dong["MaNL"] = ct["MaNL"];
                dong["TenNL"] = nl == null ? "" : nl["TenNL"];
                dong["DVT"] = nl == null ? "" : nl["DVT"];
                dong["SoLuong"] = ct["SoLuong"];
                hienThi.Rows.Add(dong);
            }
            dgvCongThuc.DataSource = hienThi;
            GiaoDien.LamDepBang(dgvCongThuc, new Dictionary<string, string>
            {
                { "MaNL", "Mã NL" }, { "TenNL", "Tên nguyên liệu" },
                { "DVT", "Đơn vị tính" }, { "SoLuong", "Số lượng / phần" }
            });
            dgvCongThuc.BorderStyle = BorderStyle.FixedSingle;
        }

        private void cboMonAn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (congThuc != null) HienThiCongThuc();
        }

        private void dgvCongThuc_SelectionChanged(object sender, EventArgs e)
        {
            if (cheDo != "" || dgvCongThuc.CurrentRow == null) return;
            maNLCu = Convert.ToString(dgvCongThuc.CurrentRow.Cells["MaNL"].Value);
            cboNguyenLieu.SelectedValue = maNLCu;
            txtSoLuong.Text = Convert.ToString(dgvCongThuc.CurrentRow.Cells["SoLuong"].Value, CultureInfo.InvariantCulture);
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            cheDo = "them"; maNLCu = ""; txtSoLuong.Clear(); cboNguyenLieu.SelectedIndex = -1; txtSoLuong.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvCongThuc.CurrentRow == null) { BaoLoi("Vui lòng chọn công thức cần sửa."); return; }
            cheDo = "sua"; maNLCu = Convert.ToString(dgvCongThuc.CurrentRow.Cells["MaNL"].Value); txtSoLuong.Focus();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvCongThuc.CurrentRow == null) { BaoLoi("Vui lòng chọn công thức cần xóa."); return; }
            string maMon = cboMonAn.SelectedValue.ToString();
            string maNL = Convert.ToString(dgvCongThuc.CurrentRow.Cells["MaNL"].Value);
            if (MessageBox.Show("Bạn có chắc muốn xóa nguyên liệu này khỏi công thức?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                ThucThi("DELETE FROM CongThuc WHERE MaMon = @MaMon AND MaNL = @MaNL",
                    new Action<SqlCommand>(cmd => { cmd.Parameters.Add("@MaMon", SqlDbType.NVarChar, 10).Value = maMon; cmd.Parameters.Add("@MaNL", SqlDbType.NVarChar, 10).Value = maNL; }));
                TaiDuLieu();
            }
            catch (Exception ex) { HienLoi("Không thể xóa công thức", ex); }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (cheDo == "") { BaoLoi("Vui lòng chọn Thêm hoặc Sửa trước khi lưu."); return; }
            if (cboMonAn.SelectedValue == null || cboNguyenLieu.SelectedValue == null) { BaoLoi("Vui lòng chọn món ăn và nguyên liệu."); return; }
            decimal soLuong;
            string text = txtSoLuong.Text.Trim().Replace(',', '.');
            if (!decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out soLuong) || soLuong <= 0m)
            { BaoLoi("Số lượng định mức phải là số thực lớn hơn 0."); return; }
            string maMon = cboMonAn.SelectedValue.ToString();
            string maNL = cboNguyenLieu.SelectedValue.ToString();
            if (cheDo == "them" && CoNguyenLieu(maMon, maNL)) { BaoLoi("Nguyên liệu đã có trong công thức. Hãy dùng nút Sửa để thay đổi số lượng."); return; }
            try
            {
                if (cheDo == "them")
                    ThucThi("INSERT INTO CongThuc (MaMon, MaNL, SoLuong) VALUES (@MaMon, @MaNL, @SoLuong)", cmd => GanThamSo(cmd, maMon, maNL, soLuong));
                else
                    ThucThi("UPDATE CongThuc SET MaNL = @MaNLMoi, SoLuong = @SoLuong WHERE MaMon = @MaMon AND MaNL = @MaNLCu", cmd =>
                    {
                        cmd.Parameters.Add("@MaMon", SqlDbType.NVarChar, 10).Value = maMon;
                        cmd.Parameters.Add("@MaNLCu", SqlDbType.NVarChar, 10).Value = maNLCu;
                        cmd.Parameters.Add("@MaNLMoi", SqlDbType.NVarChar, 10).Value = maNL;
                        GanDecimal(cmd, soLuong);
                    });
                TaiDuLieu();
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601) { BaoLoi("Nguyên liệu đã có trong công thức. Hãy dùng nút Sửa."); }
            catch (Exception ex) { HienLoi("Không thể lưu công thức", ex); }
        }

        private void btnHuy_Click(object sender, EventArgs e) { cheDo = ""; maNLCu = ""; HienThiCongThuc(); }
        private bool CoNguyenLieu(string maMon, string maNL) { return congThuc.Select("MaMon = '" + maMon + "' AND MaNL = '" + maNL + "'").Length > 0; }
        private static void GanThamSo(SqlCommand cmd, string maMon, string maNL, decimal soLuong) { cmd.Parameters.Add("@MaMon", SqlDbType.NVarChar, 10).Value = maMon; cmd.Parameters.Add("@MaNL", SqlDbType.NVarChar, 10).Value = maNL; GanDecimal(cmd, soLuong); }
        private static void GanDecimal(SqlCommand cmd, decimal value) { var p = cmd.Parameters.Add("@SoLuong", SqlDbType.Decimal); p.Precision = 18; p.Scale = 3; p.Value = value; }
        private static void ThucThi(string sql, Action<SqlCommand> ganThamSo)
        {
            using (var conn = new SqlConnection(FileXml.ChuoiKetNoi)) using (var cmd = new SqlCommand(sql, conn)) { ganThamSo(cmd); conn.Open(); cmd.ExecuteNonQuery(); }
        }
        private static void BaoLoi(string noiDung) { MessageBox.Show(noiDung, "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        private static void HienLoi(string noiDung, Exception ex) { MessageBox.Show(noiDung + ":\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
