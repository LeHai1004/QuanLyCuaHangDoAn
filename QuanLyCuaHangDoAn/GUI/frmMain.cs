using System;
using System.Windows.Forms;
using QuanLyCuaHangDoAn.Class;

namespace QuanLyCuaHangDoAn.GUI
{
    public partial class frmMain : Form
    {
        private static readonly string[] TatCa = { "Admin", "ThuNgan", "Bep" };
        private static readonly string[] ChiAdmin = { "Admin" };
        private static readonly string[] AdminThuNgan = { "Admin", "ThuNgan" };
        private static readonly string[] AdminBep = { "Admin", "Bep" };

        private MenuStrip menu;
        private StatusStrip thanhTrangThai;

        // true khi người dùng bấm Đăng xuất (Program dùng để quay lại màn đăng nhập)
        public bool DangXuat { get; private set; }

        public frmMain()
        {
            InitializeComponent();
            Text = "Quản lý cửa hàng đồ ăn";
            WindowState = FormWindowState.Maximized;
            Font = new System.Drawing.Font("Segoe UI", 10F);
            Load += frmMain_Load;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (!PhienDangNhap.IsLoggedIn)
            {
                Close();
                return;
            }
            XayDungMenu();
            XayDungThanhTrangThai();
            ApDungQuyen();
        }

        private static bool CoQuyen(string[] dsChucVu)
        {
            return Array.IndexOf(dsChucVu, PhienDangNhap.ChucVu) >= 0;
        }

        private static string TenVaiTro(string chucVu)
        {
            switch (chucVu)
            {
                case "Admin": return "Quản trị viên";
                case "ThuNgan": return "Thu ngân";
                case "Bep": return "Bếp";
                default: return chucVu;
            }
        }

        private ToolStripMenuItem ThemNhom(string ten)
        {
            ToolStripMenuItem nhom = new ToolStripMenuItem(ten);
            menu.Items.Add(nhom);
            return nhom;
        }

        private void ThemMuc(ToolStripMenuItem nhom, string ten, string[] quyen)
        {
            ToolStripMenuItem muc = new ToolStripMenuItem(ten);
            muc.Tag = quyen;
            muc.Click += Muc_Click;
            nhom.DropDownItems.Add(muc);
        }

        private void XayDungMenu()
        {
            menu = new MenuStrip();

            ToolStripMenuItem heThong = ThemNhom("Hệ thống");
            ThemMuc(heThong, "Đổi mật khẩu", TatCa);
            ToolStripMenuItem dangXuat = new ToolStripMenuItem("Đăng xuất");
            dangXuat.Tag = TatCa;
            dangXuat.Click += DangXuat_Click;
            heThong.DropDownItems.Add(dangXuat);

            ToolStripMenuItem banHang = ThemNhom("Bán hàng");
            ThemMuc(banHang, "Sơ đồ bàn", AdminThuNgan);
            ThemMuc(banHang, "Gọi món", AdminThuNgan);
            ThemMuc(banHang, "Hóa đơn", AdminThuNgan);
            ThemMuc(banHang, "Khách hàng", AdminThuNgan);

            ToolStripMenuItem bep = ThemNhom("Bếp");
            ThemMuc(bep, "Màn hình bếp", AdminBep);

            ToolStripMenuItem quanLy = ThemNhom("Quản lý");
            ThemMuc(quanLy, "Nhân viên", ChiAdmin);
            ThemMuc(quanLy, "Danh mục", ChiAdmin);
            ThemMuc(quanLy, "Món ăn", ChiAdmin);
            ThemMuc(quanLy, "Bàn", ChiAdmin);

            ToolStripMenuItem kho = ThemNhom("Kho");
            ThemMuc(kho, "Nguyên liệu", ChiAdmin);
            ThemMuc(kho, "Nhà cung cấp", ChiAdmin);
            ThemMuc(kho, "Công thức món", ChiAdmin);
            ThemMuc(kho, "Phiếu nhập", ChiAdmin);

            ToolStripMenuItem baoCao = ThemNhom("Báo cáo");
            ThemMuc(baoCao, "Thống kê", ChiAdmin);
            ThemMuc(baoCao, "Nhập/xuất XML", ChiAdmin);

            MainMenuStrip = menu;
            Controls.Add(menu);
        }

        private void XayDungThanhTrangThai()
        {
            thanhTrangThai = new StatusStrip();
            ToolStripStatusLabel nhan = new ToolStripStatusLabel();
            nhan.Text = "Người dùng: " + PhienDangNhap.TenNV + "   |   Vai trò: " + TenVaiTro(PhienDangNhap.ChucVu);
            thanhTrangThai.Items.Add(nhan);
            Controls.Add(thanhTrangThai);
        }

        // Ẩn mục không đủ quyền; nhóm không còn mục nào thì ẩn luôn
        private void ApDungQuyen()
        {
            foreach (ToolStripMenuItem nhom in menu.Items)
            {
                bool coMucHienThi = false;
                foreach (ToolStripItem muc in nhom.DropDownItems)
                {
                    bool duocPhep = CoQuyen((string[])muc.Tag);
                    muc.Visible = duocPhep;
                    coMucHienThi = coMucHienThi || duocPhep;
                }
                nhom.Visible = coMucHienThi;
            }
        }

        private void Muc_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem muc = (ToolStripMenuItem)sender;
            // Kiểm tra lại quyền trong code, không chỉ dựa vào việc ẩn menu
            if (!CoQuyen((string[])muc.Tag))
            {
                MessageBox.Show("Bạn không có quyền dùng chức năng này.", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (muc.Text == "Đổi mật khẩu")
            {
                using (frmDoiMatKhau f = new frmDoiMatKhau())
                {
                    f.ShowDialog(this);
                }
                return;
            }
            if (muc.Text == "Nhân viên")
            {
                using (frmNhanVien f = new frmNhanVien())
                {
                    f.ShowDialog(this);
                }
                return;
            }
            MessageBox.Show("Chức năng \"" + muc.Text + "\" đang được phát triển.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DangXuat_Click(object sender, EventArgs e)
        {
            PhienDangNhap.DangXuat();
            DangXuat = true;
            Close();
        }
    }
}