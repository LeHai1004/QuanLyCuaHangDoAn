using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = GiaoDien.FontThuong;
            ClientSize = new Size(1120, 620);
            MinimumSize = new Size(900, 520);

            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Size = new Size(1120, 564),
                BorderStyle = BorderStyle.None
            };
            split.Panel1MinSize = 420;
            split.Panel2MinSize = 350;
            split.SplitterDistance = 650;

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
                RowCount = 4,
                AutoSize = true
            };
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
            fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
            fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            fields.Controls.Add(new Label { Text = "Mã danh mục", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
            txtMaDM.Dock = DockStyle.Fill;
            txtMaDM.ReadOnly = true;
            fields.Controls.Add(txtMaDM, 0, 1);
            fields.Controls.Add(new Label { Text = "Tên danh mục", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
            txtTenDM.Dock = DockStyle.Fill;
            fields.Controls.Add(txtTenDM, 0, 3);

            var nutPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 100,
                Padding = new Padding(8),
                WrapContents = true
            };
            foreach (Button btn in new[] { btnThem, btnSua, btnXoa, btnLuu, btnHuy })
            {
                btn.Width = 100;
                btn.Margin = new Padding(5);
                nutPanel.Controls.Add(btn);
            }

            split.Panel2.Controls.Add(nutPanel);
            split.Panel2.Controls.Add(fields);
            Controls.Add(split);
            Controls.Add(GiaoDien.TaoTieuDe("QUẢN LÝ DANH MỤC"));

            btnThem.Click += btnThem_Click;
            btnSua.Click += btnSua_Click;
            btnXoa.Click += btnXoa_Click;
            btnLuu.Click += btnLuu_Click;
            btnHuy.Click += btnHuy_Click;
            btnTim.Click += btnTim_Click;
            txtTim.KeyDown += txtTim_KeyDown;
            CapNhatTrangThaiNut();
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
            HienThiDuLieu(duLieu);
        }

        private void HienThiDuLieu(DataTable nguon)
        {
            dgv.DataSource = nguon.Copy();
            GiaoDien.LamDepBang(dgv, new Dictionary<string, string>
            {
                { "MaDM", "Mã danh mục" }, { "TenDM", "Tên danh mục" }
            });
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
            if (!string.IsNullOrEmpty(cheDo) || choDongBo || dgv.CurrentRow == null) return;
            txtMaDM.Text = Convert.ToString(dgv.CurrentRow.Cells["MaDM"].Value);
            txtTenDM.Text = Convert.ToString(dgv.CurrentRow.Cells["TenDM"].Value);
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
    }
}
