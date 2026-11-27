using System;
using System.Drawing;
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
        private Panel pnlVungNoiDung;
        private ucTrangChu trangChu;

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
            XayDungTrangChu();
            Activated += frmMain_Activated;
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
            ToolStripMenuItem nhom = new ToolStripMenuItem(ten)
            {
                ForeColor = Color.FromArgb(55, 65, 81),
                Font = new Font("Segoe UI", 10F)
            };
            menu.Items.Add(nhom);
            return nhom;
        }

        private void ThemMuc(ToolStripMenuItem nhom, string ten, string[] quyen)
        {
            ToolStripMenuItem muc = new ToolStripMenuItem(ten)
            {
                ForeColor = Color.FromArgb(55, 65, 81),
                Font = new Font("Segoe UI", 10F)
            };
            muc.Tag = quyen;
            muc.Click += Muc_Click;
            nhom.DropDownItems.Add(muc);
        }

        private void XayDungMenu()
        {
            menu = new MenuStrip
            {
                Dock = DockStyle.Top,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(55, 65, 81),
                Font = new Font("Segoe UI", 10F),
                Renderer = new ToolStripProfessionalRenderer(new DashboardColorTable())
            };

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
            thanhTrangThai = new StatusStrip
            {
                Dock = DockStyle.Bottom,
                BackColor = Color.FromArgb(255, 243, 234),
                ForeColor = Color.FromArgb(55, 65, 81),
                Font = new Font("Segoe UI", 9F),
                SizingGrip = false,
                Renderer = new ToolStripProfessionalRenderer(new DashboardColorTable())
            };
            ToolStripStatusLabel nhan = new ToolStripStatusLabel
            {
                ForeColor = Color.FromArgb(55, 65, 81),
                BackColor = Color.FromArgb(255, 243, 234)
            };
            nhan.Text = "Người dùng: " + PhienDangNhap.TenNV + "   |   Vai trò: " + TenVaiTro(PhienDangNhap.ChucVu);
            thanhTrangThai.Items.Add(nhan);
            Controls.Add(thanhTrangThai);
        }

        private void XayDungTrangChu()
        {
            pnlVungNoiDung = new Panel
            {
                Name = "pnlVungNoiDung",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 246, 248)
            };
            trangChu = new ucTrangChu(PhienDangNhap.TenNV, PhienDangNhap.ChucVu, MoChucNangTrangChu)
            {
                Dock = DockStyle.Fill
            };
            pnlVungNoiDung.Controls.Add(trangChu);
            Controls.Add(pnlVungNoiDung);
            pnlVungNoiDung.SendToBack();
            menu.BringToFront();
            thanhTrangThai.BringToFront();
        }

        private void frmMain_Activated(object sender, EventArgs e)
        {
            if (trangChu != null && trangChu.Visible)
                trangChu.LamMoiDuLieu();
        }

        private void MoChucNangTrangChu(string tenMuc)
        {
            foreach (ToolStripMenuItem nhom in menu.Items)
            {
                foreach (ToolStripItem muc in nhom.DropDownItems)
                {
                    if (string.Equals(muc.Text, tenMuc, StringComparison.Ordinal))
                    {
                        Muc_Click(muc, EventArgs.Empty);
                        return;
                    }
                }
            }

            MessageBox.Show("Chức năng \"" + tenMuc + "\" đang được phát triển.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (muc.Text == "Sơ đồ bàn")
            {
                MoForm(new frmBan());
                return;
            }
            if (muc.Text == "Hóa đơn")
            {
                MoForm(new frmHoaDon());
                return;
            }
            if (muc.Text == "Gọi món")
            {
                MoForm(new frmOrder());
                return;
            }
            if (muc.Text == "Khách hàng")
            {
                MoForm(new frmKhachHang());
                return;
            }
            if (muc.Text == "Màn hình bếp")
            {
                MoForm(new frmBep());
                return;
            }
            if (muc.Text == "Danh mục")
            {
                MoForm(new frmDanhMuc());
                return;
            }
            if (muc.Text == "Món ăn")
            {
                MoForm(new frmMonAn());
                return;
            }
            if (muc.Text == "Bàn")
            {
                MoForm(new frmBan());
                return;
            }
            if (muc.Text == "Nguyên liệu")
            {
                MoForm(new frmNguyenLieu());
                return;
            }
            if (muc.Text == "Nhà cung cấp")
            {
                MoForm(new frmNhaCungCap());
                return;
            }
            if (muc.Text == "Công thức món")
            {
                MoForm(new frmCongThuc());
                return;
            }
            if (muc.Text == "Phiếu nhập")
            {
                MoForm(new frmPhieuNhap());
                return;
            }
            if (muc.Text == "Thống kê")
            {
                MoForm(new frmThongKe());
                return;
            }
            if (muc.Text == "Nhập/xuất XML")
            {
                MoForm(new frmNhapXml());
                return;
            }
            MessageBox.Show("Chức năng \"" + muc.Text + "\" đang được phát triển.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void MoForm(Form f)
        {
            using (f)
            {
                f.ShowDialog(this);
            }
        }

        private void DangXuat_Click(object sender, EventArgs e)
        {
            PhienDangNhap.DangXuat();
            DangXuat = true;
            Close();
        }

        private sealed class DashboardColorTable : ProfessionalColorTable
        {
            private static readonly Color MauMenu = Color.White;
            private static readonly Color MauHover = Color.FromArgb(255, 243, 234);
            private static readonly Color MauThanhTrangThai = Color.FromArgb(255, 243, 234);

            public override Color MenuStripGradientBegin { get { return MauMenu; } }
            public override Color MenuStripGradientEnd { get { return MauMenu; } }
            public override Color ToolStripDropDownBackground { get { return MauMenu; } }
            public override Color MenuItemSelected { get { return MauHover; } }
            public override Color MenuItemSelectedGradientBegin { get { return MauHover; } }
            public override Color MenuItemSelectedGradientEnd { get { return MauHover; } }
            public override Color MenuItemPressedGradientBegin { get { return MauHover; } }
            public override Color MenuItemPressedGradientMiddle { get { return MauHover; } }
            public override Color MenuItemPressedGradientEnd { get { return MauHover; } }
            public override Color StatusStripGradientBegin { get { return MauThanhTrangThai; } }
            public override Color StatusStripGradientEnd { get { return MauThanhTrangThai; } }
        }
    }
}