using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyCuaHangDoAn
{
    public sealed class BanDuocChonEventArgs : EventArgs
    {
        public string MaBan { get; private set; }
        public string TenBan { get; private set; }
        public int SoGhe { get; private set; }
        public string TrangThai { get; private set; }

        internal BanDuocChonEventArgs(string maBan, string tenBan, int soGhe, string trangThai)
        {
            MaBan = maBan;
            TenBan = tenBan;
            SoGhe = soGhe;
            TrangThai = trangThai;
        }
    }

    public sealed class BanSoDoControl : UserControl
    {
        private readonly FileXml fx = new FileXml();
        private readonly FlowLayoutPanel flpDanhSachBan = new FlowLayoutPanel();
        private readonly ToolTip toolTip = new ToolTip();

        public event EventHandler<BanDuocChonEventArgs> BanDuocChon;

        public BanSoDoControl()
        {
            BackColor = Color.White;
            Dock = DockStyle.Fill;
            flpDanhSachBan.Dock = DockStyle.Fill;
            flpDanhSachBan.AutoScroll = true;
            flpDanhSachBan.Padding = new Padding(12);
            flpDanhSachBan.WrapContents = true;
            Controls.Add(flpDanhSachBan);
        }

        public void TaiDanhSachBan()
        {
            fx.TaoXml("SELECT * FROM Ban", "Ban", "Ban.xml");
            DataTable duLieu = fx.HienThi("Ban.xml");
            flpDanhSachBan.SuspendLayout();
            try
            {
                foreach (Control control in flpDanhSachBan.Controls.Cast<Control>().ToList())
                {
                    flpDanhSachBan.Controls.Remove(control);
                    control.Dispose();
                }
                foreach (DataRow dong in duLieu.Rows)
                {
                    string maBan = Convert.ToString(dong["MaBan"]);
                    string tenBan = Convert.ToString(dong["TenBan"]);
                    int soGhe = Convert.ToInt32(dong["SoGhe"], CultureInfo.InvariantCulture);
                    string trangThai = Convert.ToString(dong["TrangThai"]);
                    Color mau;

                    if (trangThai == "Trống")
                        mau = GiaoDien.MauXanh;
                    else if (trangThai == "Có khách")
                        mau = GiaoDien.MauDo;
                    else
                        throw new InvalidOperationException("Bàn " + maBan + " có trạng thái không hợp lệ: " + trangThai);

                    var btnBan = GiaoDien.TaoNut(tenBan + Environment.NewLine + soGhe + " ghế", mau);
                    btnBan.Width = 150;
                    btnBan.Height = 78;
                    btnBan.Margin = new Padding(8);
                    btnBan.Tag = new BanDuocChonEventArgs(maBan, tenBan, soGhe, trangThai);
                    toolTip.SetToolTip(btnBan, maBan + " - " + trangThai);
                    btnBan.Click += btnBan_Click;
                    flpDanhSachBan.Controls.Add(btnBan);
                }
            }
            finally
            {
                flpDanhSachBan.ResumeLayout();
            }
        }

        private void btnBan_Click(object sender, EventArgs e)
        {
            var btnBan = sender as Button;
            var ban = btnBan != null ? btnBan.Tag as BanDuocChonEventArgs : null;
            if (ban != null)
                BanDuocChon?.Invoke(this, ban);
        }
    }

    public class frmBan : Form
    {
        private readonly FileXml fx = new FileXml();
        private readonly bool laAdmin;
        private readonly BanSoDoControl banSoDo = new BanSoDoControl();
        private readonly TextBox txtMaBan = new TextBox();
        private readonly TextBox txtTenBan = new TextBox();
        private readonly NumericUpDown nudSoGhe = new NumericUpDown();
        private readonly ComboBox cboTrangThai = new ComboBox();
        private readonly Button btnThem = GiaoDien.TaoNut("Thêm", GiaoDien.MauChinh);
        private readonly Button btnSua = GiaoDien.TaoNut("Sửa", GiaoDien.MauChinh);
        private readonly Button btnXoa = GiaoDien.TaoNut("Xóa", GiaoDien.MauDo);
        private readonly Button btnLuu = GiaoDien.TaoNut("Lưu", GiaoDien.MauXanh);
        private readonly Button btnHuy = GiaoDien.TaoNut("Hủy", GiaoDien.MauChinhDam);
        private readonly Button btnLamMoi = GiaoDien.TaoNut("Làm mới", GiaoDien.MauChinh);

        private string cheDo = "";
        private bool choDongBo;
        private string hanhDongChoDongBo;
        private string maBanDangChon = "";

        public frmBan() : this("ThuNgan")
        {
        }

        public frmBan(string chucVu)
        {
            laAdmin = string.Equals(chucVu, "Admin", StringComparison.Ordinal);
            KhoiTaoGiaoDien();
            Load += frmBan_Load;
        }

        private void KhoiTaoGiaoDien()
        {
            Text = laAdmin ? "Quản lý bàn" : "Sơ đồ bàn";
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = GiaoDien.FontThuong;
            ClientSize = new Size(1080, 650);
            MinimumSize = new Size(900, 520);

            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Size = new Size(1080, 594),
                BorderStyle = BorderStyle.None
            };
            split.Panel1MinSize = 400;
            split.Panel2MinSize = 330;
            split.SplitterDistance = 650;

            var legend = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 42,
                Padding = new Padding(12, 8, 8, 4),
                WrapContents = false
            };
            legend.Controls.Add(TaoChuGiai(GiaoDien.MauXanh, "Trống"));
            legend.Controls.Add(TaoChuGiai(GiaoDien.MauDo, "Có khách"));

            split.Panel1.Controls.Add(banSoDo);
            split.Panel1.Controls.Add(legend);

            if (laAdmin)
                TaoKhungCrud(split.Panel2);
            else
            {
                split.Panel2MinSize = 0;
                split.Panel2Collapsed = true;
            }

            var nutLamMoi = new Panel { Dock = DockStyle.Bottom, Height = 52, Padding = new Padding(8) };
            btnLamMoi.Width = 120;
            btnLamMoi.Dock = DockStyle.Right;
            nutLamMoi.Controls.Add(btnLamMoi);
            split.Panel1.Controls.Add(nutLamMoi);

            Controls.Add(split);
            Controls.Add(GiaoDien.TaoTieuDe(laAdmin ? "QUẢN LÝ BÀN" : "SƠ ĐỒ BÀN"));

            banSoDo.BanDuocChon += banSoDo_BanDuocChon;
            btnThem.Click += btnThem_Click;
            btnSua.Click += btnSua_Click;
            btnXoa.Click += btnXoa_Click;
            btnLuu.Click += btnLuu_Click;
            btnHuy.Click += btnHuy_Click;
            btnLamMoi.Click += btnLamMoi_Click;
            CapNhatTrangThaiNut();
        }

        private static Label TaoChuGiai(Color mau, string noiDung)
        {
            return new Label
            {
                AutoSize = true,
                Text = "● " + noiDung,
                ForeColor = mau,
                Font = GiaoDien.FontDam,
                Margin = new Padding(0, 2, 20, 0)
            };
        }

        private void TaoKhungCrud(Control cha)
        {
            var fields = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Padding = new Padding(12),
                ColumnCount = 1,
                RowCount = 0,
                AutoSize = true
            };
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            ThemTruong(fields, "Mã bàn", txtMaBan, true);
            ThemTruong(fields, "Tên bàn", txtTenBan, false);
            ThemTruong(fields, "Số ghế", nudSoGhe, false);
            ThemTruong(fields, "Trạng thái", cboTrangThai, false);

            nudSoGhe.Minimum = 1;
            nudSoGhe.Maximum = int.MaxValue;
            nudSoGhe.DecimalPlaces = 0;
            nudSoGhe.ThousandsSeparator = true;
            nudSoGhe.Dock = DockStyle.Fill;
            cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTrangThai.Items.AddRange(new object[] { "Trống", "Có khách" });

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

            cha.Controls.Add(nutPanel);
            cha.Controls.Add(fields);
        }

        private static void ThemTruong(TableLayoutPanel panel, string nhan, Control control, bool chiDoc)
        {
            int row = panel.RowCount;
            panel.RowCount += 2;
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
            panel.Controls.Add(new Label { Text = nhan, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, row);
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            control.Dock = DockStyle.Fill;
            if (control is TextBox)
                ((TextBox)control).ReadOnly = chiDoc;
            panel.Controls.Add(control, 0, row + 1);
        }

        private void frmBan_Load(object sender, EventArgs e)
        {
            try
            {
                banSoDo.TaiDanhSachBan();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể tải sơ đồ bàn", ex);
            }
        }

        private void banSoDo_BanDuocChon(object sender, BanDuocChonEventArgs e)
        {
            maBanDangChon = e.MaBan;
            txtMaBan.Text = e.MaBan;
            txtTenBan.Text = e.TenBan;
            nudSoGhe.Value = Math.Min(nudSoGhe.Maximum, Math.Max(nudSoGhe.Minimum, e.SoGhe));
            cboTrangThai.SelectedItem = e.TrangThai;
            CapNhatTrangThaiNut();
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            try
            {
                banSoDo.TaiDanhSachBan();
                HuyChinhSua();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể làm mới sơ đồ bàn", ex);
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!KiemTraQuyenAdmin() || choDongBo)
                return;
            try
            {
                fx.TaoXml("SELECT * FROM Ban", "Ban", "Ban.xml");
                maBanDangChon = "";
                txtMaBan.Text = fx.TaoMa("BA", "Ban.xml", "MaBan");
                txtTenBan.Clear();
                nudSoGhe.Value = 1;
                cboTrangThai.SelectedItem = "Trống";
                cheDo = "them";
                CapNhatTrangThaiNut();
                txtTenBan.Focus();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể tạo mã bàn", ex);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (!KiemTraQuyenAdmin() || choDongBo)
                return;
            if (string.IsNullOrEmpty(maBanDangChon))
            {
                MessageBox.Show("Vui lòng chọn bàn cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            cheDo = "sua";
            CapNhatTrangThaiNut();
            txtTenBan.Focus();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (!KiemTraQuyenAdmin() || choDongBo)
                return;
            if (string.IsNullOrEmpty(maBanDangChon))
            {
                MessageBox.Show("Vui lòng chọn bàn cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                fx.TaoXml("SELECT * FROM HoaDon", "HoaDon", "HoaDon.xml");
                DataTable hoaDon = fx.HienThi("HoaDon.xml");
                if (hoaDon.AsEnumerable().Any(dong =>
                    string.Equals(Convert.ToString(dong["MaBan"]), maBanDangChon, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("Không thể xóa bàn đã được ghi nhận trong hóa đơn.", "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("Bạn có chắc muốn xóa bàn " + txtTenBan.Text + " không?", "Xác nhận xóa",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                if (fx.Xoa("Ban.xml", new Dictionary<string, string> { { "MaBan", maBanDangChon } }) == 0)
                    throw new InvalidOperationException("Không tìm thấy bàn cần xóa trong file XML.");

                ChoDongBo("xóa bàn");
                ThuDongBo();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể xóa hoặc đồng bộ bàn", ex);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (!KiemTraQuyenAdmin())
                return;
            if (choDongBo)
            {
                ThuDongBo();
                return;
            }
            if (string.IsNullOrEmpty(cheDo))
            {
                MessageBox.Show("Vui lòng chọn Thêm hoặc Sửa trước khi lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string tenBan = txtTenBan.Text.Trim();
            if (tenBan.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập tên bàn.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenBan.Focus();
                return;
            }
            if (tenBan.Length > 30)
            {
                MessageBox.Show("Tên bàn không được vượt quá 30 ký tự.", "Thông tin không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenBan.Focus();
                return;
            }
            if (cboTrangThai.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn trạng thái bàn.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (cheDo == "them")
                {
                    if (fx.TonTai("Ban.xml", "MaBan", txtMaBan.Text))
                        throw new InvalidOperationException("Mã bàn đã tồn tại. Hãy hủy thao tác và tạo lại mã.");

                    string[] thuTuCot = { "MaBan", "TenBan", "SoGhe", "TrangThai" };
                    var giaTri = new SortedDictionary<string, string>(Comparer<string>.Create((a, b) =>
                        Array.IndexOf(thuTuCot, a).CompareTo(Array.IndexOf(thuTuCot, b))));
                    giaTri.Add("MaBan", txtMaBan.Text);
                    giaTri.Add("TenBan", tenBan);
                    giaTri.Add("SoGhe", nudSoGhe.Value.ToString(CultureInfo.InvariantCulture));
                    giaTri.Add("TrangThai", Convert.ToString(cboTrangThai.SelectedItem));
                    fx.Them("Ban.xml", "Ban", giaTri);
                    ChoDongBo("thêm bàn");
                }
                else
                {
                    var giaTriMoi = new Dictionary<string, string>
                    {
                        { "TenBan", tenBan },
                        { "SoGhe", nudSoGhe.Value.ToString(CultureInfo.InvariantCulture) },
                        { "TrangThai", Convert.ToString(cboTrangThai.SelectedItem) }
                    };
                    if (fx.Sua("Ban.xml", new Dictionary<string, string> { { "MaBan", txtMaBan.Text } }, giaTriMoi) == 0)
                        throw new InvalidOperationException("Không tìm thấy bàn cần sửa trong file XML.");
                    ChoDongBo("sửa bàn");
                }

                ThuDongBo();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể lưu bàn", ex);
            }
        }

        private void ThuDongBo()
        {
            if (!KiemTraQuyenAdmin() || !choDongBo)
                return;
            try
            {
                fx.CapNhatDb("Ban", "Ban.xml");
            }
            catch (Exception ex)
            {
                CapNhatTrangThaiNut();
                MessageBox.Show("XML đã thay đổi nhưng chưa đồng bộ được bàn vào CSDL. Nhấn Lưu để thử đồng bộ lại.\r\n\r\n" + ex.Message,
                    "Lỗi đồng bộ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            choDongBo = false;
            hanhDongChoDongBo = null;
            cheDo = "";
            maBanDangChon = "";
            try
            {
                banSoDo.TaiDanhSachBan();
                HuyChinhSua();
            }
            catch (Exception ex)
            {
                CapNhatTrangThaiNut();
                HienLoi("Bàn đã đồng bộ nhưng không thể tải lại sơ đồ", ex);
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            if (!KiemTraQuyenAdmin() || choDongBo)
                return;
            HuyChinhSua();
        }

        private void HuyChinhSua()
        {
            cheDo = "";
            maBanDangChon = "";
            txtMaBan.Clear();
            txtTenBan.Clear();
            nudSoGhe.Value = 1;
            cboTrangThai.SelectedItem = null;
            CapNhatTrangThaiNut();
        }

        private void ChoDongBo(string hanhDong)
        {
            choDongBo = true;
            hanhDongChoDongBo = hanhDong;
            CapNhatTrangThaiNut();
        }

        private bool KiemTraQuyenAdmin()
        {
            if (laAdmin)
                return true;
            MessageBox.Show("Chỉ Admin mới được thêm, sửa hoặc xóa bàn.", "Không đủ quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        private void CapNhatTrangThaiNut()
        {
            bool dangChinhSua = !string.IsNullOrEmpty(cheDo);
            bool coQuyen = laAdmin && !choDongBo;
            btnThem.Enabled = coQuyen && !dangChinhSua;
            btnSua.Enabled = coQuyen && !dangChinhSua && !string.IsNullOrEmpty(maBanDangChon);
            btnXoa.Enabled = coQuyen && !dangChinhSua && !string.IsNullOrEmpty(maBanDangChon);
            btnLuu.Enabled = laAdmin && (dangChinhSua || choDongBo);
            btnHuy.Enabled = coQuyen && dangChinhSua;
            btnLamMoi.Enabled = !choDongBo;
            txtTenBan.ReadOnly = !dangChinhSua;
            nudSoGhe.Enabled = dangChinhSua;
            cboTrangThai.Enabled = dangChinhSua;
            btnLuu.Text = choDongBo ? "Đồng bộ" : "Lưu";
            Text = choDongBo && !string.IsNullOrEmpty(hanhDongChoDongBo)
                ? "Quản lý bàn - chờ đồng bộ " + hanhDongChoDongBo
                : laAdmin ? "Quản lý bàn" : "Sơ đồ bàn";
        }

        private static void HienLoi(string noiDung, Exception ex)
        {
            MessageBox.Show(noiDung + ".\r\n\r\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
