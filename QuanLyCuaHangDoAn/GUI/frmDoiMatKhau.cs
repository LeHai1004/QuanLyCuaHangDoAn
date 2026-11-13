using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyCuaHangDoAn.Class;

namespace QuanLyCuaHangDoAn.GUI
{
    public partial class frmDoiMatKhau : Form
    {
        private TextBox txtCu, txtMoi, txtNhapLai;
        private Label lblThongBao;

        public frmDoiMatKhau()
        {
            InitializeComponent();
            XayDungGiaoDien();
        }

        private TextBox ThemO(string nhan, int y)
        {
            Controls.Add(new Label { Text = nhan, Left = 25, Top = y, Width = 330, Height = 22, Font = GiaoDien.FontDam });
            TextBox t = new TextBox { Left = 25, Top = y + 24, Width = 330, UseSystemPasswordChar = true };
            Controls.Add(t);
            return t;
        }

        private void XayDungGiaoDien()
        {
            Text = "Đổi mật khẩu";
            ClientSize = new Size(380, 360);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Font = GiaoDien.FontThuong;
            BackColor = GiaoDien.MauNenNhat;

            Controls.Add(new Label
            {
                Text = "ĐỔI MẬT KHẨU",
                Font = GiaoDien.FontTieuDe,
                ForeColor = GiaoDien.MauChinh,
                AutoSize = true,
                Left = 25,
                Top = 15
            });

            txtCu = ThemO("Mật khẩu hiện tại", 60);
            txtMoi = ThemO("Mật khẩu mới (từ 6 ký tự)", 120);
            txtNhapLai = ThemO("Nhập lại mật khẩu mới", 180);

            lblThongBao = new Label
            {
                Left = 25,
                Top = 240,
                Width = 330,
                Height = 22,
                ForeColor = GiaoDien.MauDo,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(lblThongBao);

            Button btnDoi = new Button
            {
                Text = "Đổi mật khẩu",
                Left = 25,
                Top = 275,
                Width = 160,
                Height = 38,
                BackColor = GiaoDien.MauChinh,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDoi.FlatAppearance.BorderSize = 0;
            btnDoi.Click += btnDoi_Click;
            Controls.Add(btnDoi);

            Button btnHuy = new Button
            {
                Text = "Hủy",
                Left = 195,
                Top = 275,
                Width = 160,
                Height = 38,
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };
            btnHuy.FlatAppearance.BorderSize = 0;
            Controls.Add(btnHuy);

            AcceptButton = btnDoi;
            CancelButton = btnHuy;
        }

        private void btnDoi_Click(object sender, EventArgs e)
        {
            string cu = txtCu.Text;
            string moi = txtMoi.Text;

            if (cu.Length == 0) { lblThongBao.Text = "Vui lòng nhập mật khẩu hiện tại."; return; }
            if (moi.Length < 6) { lblThongBao.Text = "Mật khẩu mới phải từ 6 ký tự."; return; }
            if (moi != txtNhapLai.Text) { lblThongBao.Text = "Hai lần nhập mật khẩu mới không khớp."; return; }
            if (moi == cu) { lblThongBao.Text = "Mật khẩu mới phải khác mật khẩu hiện tại."; return; }

            try
            {
                string loi = TaiKhoan.DoiMatKhau(PhienDangNhap.MaNV, cu, moi);
                if (loi != null)
                {
                    lblThongBao.Text = loi;
                    txtCu.Clear();
                    txtCu.Focus();
                    return;
                }
                MessageBox.Show("Đổi mật khẩu thành công.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không đổi được mật khẩu:\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}