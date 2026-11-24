using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using QuanLyCuaHangDoAn.Class;

namespace QuanLyCuaHangDoAn.GUI
{
    public class frmNhapXml : Form
    {
        private readonly FileXml fx = new FileXml();
        private readonly Button btnXuat = GiaoDien.TaoNut("Xuất thực đơn XML", GiaoDien.MauChinh);
        private readonly Button btnChon = GiaoDien.TaoNut("Chọn và kiểm tra XML", GiaoDien.MauChinh);
        private readonly Button btnNhap = GiaoDien.TaoNut("Nhập vào hệ thống", GiaoDien.MauXanh);
        private readonly Label lblTrangThai = new Label();
        private readonly DataGridView dgvPreview = new DataGridView();
        private string duongDanXmlDaChon = "";
        private DataTable duLieuXemTruoc;

        public frmNhapXml()
        {
            KhoiTaoGiaoDien();
            Load += frmNhapXml_Load;
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Nhập / xuất thực đơn XML";
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = GiaoDien.FontThuong;
            ClientSize = new Size(1100, 680);
            MinimumSize = new Size(900, 560);

            btnXuat.Name = "btnXuat";
            btnChon.Name = "btnChon";
            btnNhap.Name = "btnNhap";
            lblTrangThai.Name = "lblTrangThai";
            dgvPreview.Name = "dgvPreview";
            btnNhap.Enabled = false;
            btnXuat.Height = 44;
            btnChon.Height = 44;
            btnNhap.Height = 44;

            var noiDung = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12),
                ColumnCount = 2,
                RowCount = 1
            };
            noiDung.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
            noiDung.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66));

            noiDung.Controls.Add(TaoKhuVucChucNang(), 0, 0);
            noiDung.Controls.Add(TaoKhuVucXemTruoc(), 1, 0);
            Controls.Add(noiDung);
            Controls.Add(GiaoDien.TaoTieuDe("NHẬP / XUẤT THỰC ĐƠN XML"));

            btnXuat.Click += btnXuat_Click;
            btnChon.Click += btnChon_Click;
            btnNhap.Click += btnNhap_Click;
        }

        private Control TaoKhuVucChucNang()
        {
            var khuVuc = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 8,
                Padding = new Padding(8)
            };
            khuVuc.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            khuVuc.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
            khuVuc.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            khuVuc.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
            khuVuc.RowStyles.Add(new RowStyle(SizeType.Absolute, 82));
            khuVuc.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
            khuVuc.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            khuVuc.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            khuVuc.Controls.Add(TaoNhan("XUẤT THỰC ĐƠN"), 0, 0);
            khuVuc.Controls.Add(btnXuat, 0, 1);
            khuVuc.Controls.Add(TaoNhan("KIỂM TRA FILE XML"), 0, 2);
            khuVuc.Controls.Add(btnChon, 0, 3);
            lblTrangThai.Text = "Chưa chọn file XML.";
            lblTrangThai.Dock = DockStyle.Fill;
            lblTrangThai.TextAlign = ContentAlignment.MiddleLeft;
            lblTrangThai.ForeColor = Color.FromArgb(120, 50, 0);
            khuVuc.Controls.Add(lblTrangThai, 0, 4);
            khuVuc.Controls.Add(btnNhap, 0, 5);
            khuVuc.Controls.Add(new Label
            {
                Text = "Chỉ nhập file đã hợp lệ XSD và có mã danh mục tồn tại trong CSDL.",
                Dock = DockStyle.Bottom,
                AutoSize = false,
                Height = 30,
                ForeColor = Color.Gray
            }, 0, 7);
            return khuVuc;
        }

        private Control TaoKhuVucXemTruoc()
        {
            var khuVuc = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(8)
            };
            khuVuc.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            khuVuc.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            khuVuc.Controls.Add(TaoNhan("XEM TRƯỚC DỮ LIỆU MÓN ĂN"), 0, 0);
            dgvPreview.Dock = DockStyle.Fill;
            dgvPreview.ReadOnly = true;
            dgvPreview.AllowUserToAddRows = false;
            dgvPreview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPreview.BorderStyle = BorderStyle.FixedSingle;
            khuVuc.Controls.Add(dgvPreview, 0, 1);
            return khuVuc;
        }

        private static Label TaoNhan(string noiDung)
        {
            return new Label
            {
                Text = noiDung,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = GiaoDien.FontDam,
                ForeColor = Color.FromArgb(120, 50, 0)
            };
        }

        private void frmNhapXml_Load(object sender, EventArgs e)
        {
            if (!PhienDangNhap.IsLoggedIn || PhienDangNhap.ChucVu != "Admin")
            {
                MessageBox.Show("Chức năng chỉ dành cho quản trị viên (Admin).", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
            }
        }

        private void btnXuat_Click(object sender, EventArgs e)
        {
            try
            {
                fx.TaoXml("SELECT MaMon, TenMon, MaDM, DonGia, DVT, TrangThai FROM MonAn", "MonAn", "MonAn.xml");
                using (var dialog = new SaveFileDialog())
                {
                    dialog.Title = "Lưu bản sao thực đơn XML";
                    dialog.Filter = "Tệp XML (*.xml)|*.xml|Tất cả tệp (*.*)|*.*";
                    dialog.FileName = "MonAn.xml";
                    if (dialog.ShowDialog(this) != DialogResult.OK) return;

                    string nguon = FileXml.DuongDan("MonAn.xml");
                    if (!string.Equals(Path.GetFullPath(nguon), Path.GetFullPath(dialog.FileName),
                        StringComparison.OrdinalIgnoreCase))
                        File.Copy(nguon, dialog.FileName, true);
                    lblTrangThai.Text = "Đã xuất thực đơn: " + dialog.FileName;
                    MessageBox.Show("Đã xuất thực đơn XML thành công.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                HienLoi("Không thể xuất thực đơn XML", ex);
            }
        }

        private void btnChon_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Chọn file thực đơn XML";
                dialog.Filter = "Tệp XML (*.xml)|*.xml|Tất cả tệp (*.*)|*.*";
                if (dialog.ShowDialog(this) != DialogResult.OK) return;

                duongDanXmlDaChon = dialog.FileName;
                duLieuXemTruoc = null;
                dgvPreview.DataSource = null;
                btnNhap.Enabled = false;

                string loi;
                bool hopLe = fx.KiemTraXsd(duongDanXmlDaChon, FileXml.DuongDan("MonAn.xsd"), out loi);
                if (!hopLe)
                {
                    lblTrangThai.Text = "File không hợp lệ XSD.";
                    MessageBox.Show("File XML không hợp lệ:\n\n" + loi, "Kiểm tra XML thất bại",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    duLieuXemTruoc = fx.HienThi(duongDanXmlDaChon);
                    HienThiXemTruoc(duLieuXemTruoc);
                    btnNhap.Enabled = duLieuXemTruoc.Rows.Count > 0;
                    lblTrangThai.Text = "File hợp lệ XSD: " + duongDanXmlDaChon;
                    MessageBox.Show("File XML hợp lệ. Có thể nhấn Nhập vào hệ thống.", "Kiểm tra thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    btnNhap.Enabled = false;
                    HienLoi("File hợp lệ XSD nhưng không thể nạp dữ liệu xem trước", ex);
                }
            }
        }

        private void HienThiXemTruoc(DataTable bang)
        {
            dgvPreview.DataSource = bang;
            GiaoDien.LamDepBang(dgvPreview, new Dictionary<string, string>
            {
                { "MaMon", "Mã món" }, { "TenMon", "Tên món" },
                { "MaDM", "Mã danh mục" }, { "DonGia", "Đơn giá" },
                { "DVT", "Đơn vị tính" }, { "TrangThai", "Trạng thái" }
            });
            GiaoDien.DinhDangTien(dgvPreview, "DonGia");
        }

        private void btnNhap_Click(object sender, EventArgs e)
        {
            if (!PhienDangNhap.IsLoggedIn || PhienDangNhap.ChucVu != "Admin")
            {
                MessageBox.Show("Bạn không có quyền nhập thực đơn XML.", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!btnNhap.Enabled || duLieuXemTruoc == null || string.IsNullOrWhiteSpace(duongDanXmlDaChon))
            {
                BaoLoi("Vui lòng chọn và kiểm tra một file XML hợp lệ trước.");
                return;
            }
            if (!KiemTraDanhMuc(out string loiDanhMuc))
            {
                BaoLoi(loiDanhMuc);
                return;
            }

            try
            {
                fx.CapNhatDb("MonAn", duongDanXmlDaChon);
                fx.TaoXml("SELECT * FROM MonAn", "MonAn", "MonAn.xml");
                btnNhap.Enabled = false;
                MessageBox.Show("Đã nhập thực đơn vào SQL Server và đồng bộ MonAn.xml.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblTrangThai.Text = "Đã nhập thành công: " + duongDanXmlDaChon;
            }
            catch (Exception ex)
            {
                HienLoi("Không thể nhập thực đơn XML vào hệ thống", ex);
            }
        }

        private bool KiemTraDanhMuc(out string loi)
        {
            var maDanhMuc = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow dong in duLieuXemTruoc.Rows)
                maDanhMuc.Add(Convert.ToString(dong["MaDM"]));

            var khongTonTai = new List<string>();
            using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM DanhMuc WHERE MaDM = @MaDM", conn))
            {
                cmd.Parameters.Add("@MaDM", SqlDbType.NVarChar, 10);
                conn.Open();
                foreach (string maDM in maDanhMuc)
                {
                    cmd.Parameters["@MaDM"].Value = maDM;
                    if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                        khongTonTai.Add(maDM);
                }
            }

            if (khongTonTai.Count == 0)
            {
                loi = "";
                return true;
            }
            loi = "Các mã danh mục sau chưa tồn tại trong SQL Server: " + string.Join(", ", khongTonTai) + ".\nKhông thể nhập thực đơn.";
            return false;
        }

        private static void BaoLoi(string noiDung)
        {
            MessageBox.Show(noiDung, "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private static void HienLoi(string noiDung, Exception ex)
        {
            MessageBox.Show(noiDung + ":\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
