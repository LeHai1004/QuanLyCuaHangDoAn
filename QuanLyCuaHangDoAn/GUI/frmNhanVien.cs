using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCuaHangDoAn.Class;

namespace QuanLyCuaHangDoAn.GUI
{
    public partial class frmNhanVien : Form
    {
        private const string TenFile = "NhanVien.xml";
        private const string TenBang = "NhanVien";
        private static readonly string[] DsChucVu = { "Admin", "ThuNgan", "Bep" };

        private readonly FileXml fx = new FileXml();
        private DataTable bang;
        private bool dangThem;

        private DataGridView dgvNhanVien;
        private TextBox txtMaNV, txtTenNV, txtSDT, txtTenDN, txtMatKhau, txtTim;
        private ComboBox cboChucVu;
        private Button btnThem, btnSua, btnXoa, btnLuu, btnHuy, btnTim;

        public frmNhanVien()
        {
            InitializeComponent();
            XayDungGiaoDien();
            Load += frmNhanVien_Load;
        }

        // ---------- Dựng giao diện ----------
        private TextBox ThemO(Panel cha, string nhan, int y)
        {
            Label l = new Label { Text = nhan, Left = 15, Top = y, Width = 300, Height = 22, Font = GiaoDien.FontDam };
            TextBox t = new TextBox { Left = 15, Top = y + 24, Width = 300 };
            cha.Controls.Add(l);
            cha.Controls.Add(t);
            return t;
        }

        private Button ThemNut(Panel cha, string chu, int x, int y, Color nen, EventHandler suKien)
        {
            Button b = new Button
            {
                Text = chu,
                Left = x,
                Top = y,
                Width = 95,
                Height = 34,
                BackColor = nen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            b.Click += suKien;
            cha.Controls.Add(b);
            return b;
        }

        private void XayDungGiaoDien()
        {
            Text = "Quản lý nhân viên";
            ClientSize = new Size(1000, 620);
            MinimumSize = new Size(900, 560);
            StartPosition = FormStartPosition.CenterParent;
            Font = GiaoDien.FontThuong;
            BackColor = GiaoDien.MauNenNhat;

            // Thanh trên: tiêu đề + ô tìm
            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, Width = ClientSize.Width };
            Label lblTieuDe = new Label
            {
                Text = "QUẢN LÝ NHÂN VIÊN",
                Font = GiaoDien.FontTieuDe,
                ForeColor = GiaoDien.MauChinh,
                AutoSize = true,
                Left = 15,
                Top = 14
            };
            pnlTop.Controls.Add(lblTieuDe);
            txtTim = new TextBox
            {
                Width = 230,
                Top = 17,
                Left = ClientSize.Width - 360,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            txtTim.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    btnTim.PerformClick();
                }
            };
            pnlTop.Controls.Add(txtTim);
            btnTim = ThemNut(pnlTop, "Tìm", ClientSize.Width - 120, 14, GiaoDien.MauChinh, btnTim_Click);
            btnTim.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            // Khung nhập bên phải
            Panel pnlPhai = new Panel { Dock = DockStyle.Right, Width = 340, BackColor = Color.White };
            txtMaNV = ThemO(pnlPhai, "Mã nhân viên", 10);
            txtMaNV.ReadOnly = true;
            txtTenNV = ThemO(pnlPhai, "Họ tên", 70);
            txtTenNV.MaxLength = 100;
            txtSDT = ThemO(pnlPhai, "Số điện thoại", 130);
            txtSDT.MaxLength = 15;

            pnlPhai.Controls.Add(new Label { Text = "Chức vụ", Left = 15, Top = 190, Width = 300, Height = 22, Font = GiaoDien.FontDam });
            cboChucVu = new ComboBox { Left = 15, Top = 214, Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            cboChucVu.Items.AddRange(DsChucVu);
            pnlPhai.Controls.Add(cboChucVu);

            txtTenDN = ThemO(pnlPhai, "Tên đăng nhập", 250);
            txtTenDN.MaxLength = 30;
            txtMatKhau = ThemO(pnlPhai, "Mật khẩu", 310);
            txtMatKhau.UseSystemPasswordChar = true;
            pnlPhai.Controls.Add(new Label
            {
                Text = "Khi sửa: để trống = giữ mật khẩu cũ",
                Left = 15,
                Top = 372,
                Width = 300,
                Height = 20,
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.Gray
            });

            btnThem = ThemNut(pnlPhai, "Thêm", 15, 410, GiaoDien.MauChinh, btnThem_Click);
            btnSua = ThemNut(pnlPhai, "Sửa", 120, 410, GiaoDien.MauChinhDam, btnSua_Click);
            btnXoa = ThemNut(pnlPhai, "Xóa", 225, 410, GiaoDien.MauDo, btnXoa_Click);
            btnLuu = ThemNut(pnlPhai, "Lưu", 15, 452, GiaoDien.MauXanh, btnLuu_Click);
            btnHuy = ThemNut(pnlPhai, "Hủy", 120, 452, Color.Gray, btnHuy_Click);

            // Bảng bên trái
            dgvNhanVien = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White
            };
            dgvNhanVien.SelectionChanged += dgvNhanVien_SelectionChanged;

            Controls.Add(pnlPhai);
            Controls.Add(pnlTop);
            Controls.Add(dgvNhanVien);
            dgvNhanVien.BringToFront();
        }

        // ---------- Nạp dữ liệu ----------
        private void frmNhanVien_Load(object sender, EventArgs e)
        {
            // Kiểm tra quyền trong code, không chỉ ẩn menu
            if (PhienDangNhap.ChucVu != "Admin")
            {
                MessageBox.Show("Chỉ Admin mới được quản lý nhân viên.", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }
            try
            {
                LamMoi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải được danh sách nhân viên:\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        // Tạo lại XML từ CSDL rồi đưa lên bảng
        private void LamMoi()
        {
            fx.TaoXml("SELECT MaNV, TenNV, SDT, ChucVu, TenDN, MatKhau FROM NhanVien ORDER BY MaNV",
                TenBang, TenFile);
            bang = fx.HienThi(TenFile);
            dgvNhanVien.DataSource = bang;

            Dictionary<string, string> tieuDe = new Dictionary<string, string>
            {
                { "MaNV", "Mã NV" }, { "TenNV", "Họ tên" }, { "SDT", "Điện thoại" },
                { "ChucVu", "Chức vụ" }, { "TenDN", "Tên đăng nhập" }
            };
            GiaoDien.LamDepBang(dgvNhanVien, tieuDe);
            if (dgvNhanVien.Columns.Contains("MatKhau"))
            {
                dgvNhanVien.Columns["MatKhau"].Visible = false; // không hiện mã băm
            }

            dangThem = false;
            DatCheDo(false);
            HienChiTiet();
        }

        private static string Chuoi(DataGridViewRow dong, string cot)
        {
            return Convert.ToString(dong.Cells[cot].Value);
        }

        private void HienChiTiet()
        {
            DataGridViewRow r = dgvNhanVien.CurrentRow;
            if (r == null)
            {
                XoaO();
                return;
            }
            txtMaNV.Text = Chuoi(r, "MaNV");
            txtTenNV.Text = Chuoi(r, "TenNV");
            txtSDT.Text = Chuoi(r, "SDT");
            txtTenDN.Text = Chuoi(r, "TenDN");
            txtMatKhau.Clear();
            cboChucVu.SelectedItem = Chuoi(r, "ChucVu");
        }

        private void XoaO()
        {
            txtMaNV.Clear();
            txtTenNV.Clear();
            txtSDT.Clear();
            txtTenDN.Clear();
            txtMatKhau.Clear();
            cboChucVu.SelectedIndex = -1;
        }

        // nhapLieu = true: đang thêm/sửa
        private void DatCheDo(bool nhapLieu)
        {
            txtTenNV.Enabled = nhapLieu;
            txtSDT.Enabled = nhapLieu;
            cboChucVu.Enabled = nhapLieu;
            txtMatKhau.Enabled = nhapLieu;
            txtTenDN.Enabled = nhapLieu && dangThem; // đổi tên đăng nhập sẽ làm sai mã băm mật khẩu
            btnLuu.Enabled = nhapLieu;
            btnHuy.Enabled = nhapLieu;
            btnThem.Enabled = !nhapLieu;
            btnSua.Enabled = !nhapLieu;
            btnXoa.Enabled = !nhapLieu;
            dgvNhanVien.Enabled = !nhapLieu;
        }

        private void dgvNhanVien_SelectionChanged(object sender, EventArgs e)
        {
            if (!btnLuu.Enabled)
            {
                HienChiTiet();
            }
        }

        // ---------- Kiểm tra hợp lệ ----------
        private static bool TatCaLaSo(string s)
        {
            foreach (char c in s)
            {
                if (c < '0' || c > '9') return false;
            }
            return true;
        }

        private static bool ChiChuSoKhongDau(string s)
        {
            foreach (char c in s)
            {
                if (c > 127 || !char.IsLetterOrDigit(c)) return false;
            }
            return true;
        }

        private string KiemTra(string ten, string sdt, string tenDN, string mk)
        {
            if (ten.Length == 0) return "Vui lòng nhập họ tên nhân viên.";
            if (sdt.Length > 0 && (sdt.Length < 9 || !TatCaLaSo(sdt)))
                return "Số điện thoại phải gồm 9 đến 15 chữ số.";
            if (cboChucVu.SelectedItem == null) return "Vui lòng chọn chức vụ.";
            if (dangThem)
            {
                if (tenDN.Length < 3) return "Tên đăng nhập phải từ 3 ký tự.";
                if (!ChiChuSoKhongDau(tenDN)) return "Tên đăng nhập chỉ gồm chữ không dấu và số, không có khoảng trắng.";
                if (fx.TonTai(TenFile, "TenDN", tenDN)) return "Tên đăng nhập này đã tồn tại.";
                if (mk.Length < 6) return "Mật khẩu phải từ 6 ký tự.";
            }
            else if (mk.Length > 0 && mk.Length < 6)
            {
                return "Mật khẩu mới phải từ 6 ký tự.";
            }
            return null;
        }

        // ---------- Các nút ----------
        private void btnThem_Click(object sender, EventArgs e)
        {
            dangThem = true;
            XoaO();
            txtMaNV.Text = fx.TaoMa("NV", TenFile, "MaNV");
            cboChucVu.SelectedItem = "ThuNgan";
            DatCheDo(true);
            txtTenNV.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (txtMaNV.Text.Length == 0)
            {
                MessageBox.Show("Hãy chọn một nhân viên trong bảng.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            dangThem = false;
            DatCheDo(true);
            txtTenNV.Focus();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            dangThem = false;
            DatCheDo(false);
            HienChiTiet();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            string ma = txtMaNV.Text;
            string ten = txtTenNV.Text.Trim();
            string sdt = txtSDT.Text.Trim();
            string tenDN = txtTenDN.Text.Trim();
            string mk = txtMatKhau.Text;

            string loi = KiemTra(ten, sdt, tenDN, mk);
            if (loi != null)
            {
                MessageBox.Show(loi, "Dữ liệu chưa hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string chucVu = cboChucVu.SelectedItem.ToString();
            if (!dangThem && ma == PhienDangNhap.MaNV && chucVu != "Admin")
            {
                MessageBox.Show("Không thể tự hạ quyền của tài khoản đang đăng nhập.", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (dangThem)
                {
                    Dictionary<string, string> moi = new Dictionary<string, string>
                    {
                        { "MaNV", ma }, { "TenNV", ten }, { "SDT", sdt },
                        { "ChucVu", chucVu }, { "TenDN", tenDN },
                        { "MatKhau", TaiKhoan.BamMatKhau(tenDN, mk) }
                    };
                    fx.Them(TenFile, TenBang, moi);
                }
                else
                {
                    Dictionary<string, string> khoa = new Dictionary<string, string> { { "MaNV", ma } };
                    Dictionary<string, string> doi = new Dictionary<string, string>
                    {
                        { "TenNV", ten }, { "SDT", sdt }, { "ChucVu", chucVu }
                    };
                    if (mk.Length > 0)
                    {
                        doi["MatKhau"] = TaiKhoan.BamMatKhau(tenDN, mk);
                    }
                    fx.Sua(TenFile, khoa, doi);
                }
                fx.CapNhatDb(TenBang, TenFile);
                LamMoi();
                MessageBox.Show("Đã lưu nhân viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không lưu được:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LamMoi(); // đồng bộ lại XML theo CSDL
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string ma = txtMaNV.Text;
            if (ma.Length == 0)
            {
                MessageBox.Show("Hãy chọn một nhân viên trong bảng.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (ma == PhienDangNhap.MaNV)
            {
                MessageBox.Show("Không thể xóa tài khoản đang đăng nhập.", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("Xóa nhân viên " + txtTenNV.Text + " (" + ma + ")?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                fx.Xoa(TenFile, new Dictionary<string, string> { { "MaNV", ma } });
                fx.CapNhatDb(TenBang, TenFile);
                LamMoi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không xóa được (nhân viên có thể đã lập hóa đơn hoặc phiếu nhập).\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LamMoi();
            }
        }

        private static string ThoatLike(string s)
        {
            return s.Replace("'", "''").Replace("[", "[[]").Replace("%", "[%]").Replace("*", "[*]");
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string tu = txtTim.Text.Trim();
            if (tu.Length == 0)
            {
                bang.DefaultView.RowFilter = "";
                return;
            }
            string t = ThoatLike(tu);
            bang.DefaultView.RowFilter = "TenNV LIKE '%" + t + "%' OR TenDN LIKE '%" + t + "%' OR MaNV LIKE '%" + t + "%'";
        }
    }
}