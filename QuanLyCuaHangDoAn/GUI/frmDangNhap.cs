using System;
using System.Windows.Forms;
using QuanLyCuaHangDoAn.Class;

namespace QuanLyCuaHangDoAn.GUI
{
    public partial class frmDangNhap : Form
    {
        public frmDangNhap()
        {
            InitializeComponent();
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string tenDN = txtTenDN.Text.Trim();
            string matKhau = txtMatKhau.Text;

            if (tenDN.Length == 0 || matKhau.Length == 0)
            {
                lblThongBao.Text = "Vui lòng nhập đủ tên đăng nhập và mật khẩu.";
                return;
            }

            try
            {
                if (TaiKhoan.DangNhap(tenDN, matKhau))
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    lblThongBao.Text = "Sai tên đăng nhập hoặc mật khẩu.";
                    txtMatKhau.Clear();
                    txtMatKhau.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không kết nối được cơ sở dữ liệu:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}