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
        private CheckBox chkHienMatKhau;
        private Panel pnlSucManh1, pnlSucManh2, pnlSucManh3;
        private Label lblDoManh;

        public frmDoiMatKhau()
        {
            InitializeComponent();
            DoubleBuffered = true;
            XayDungGiaoDien();
        }

        private TextBox ThemO(string nhan, string ten, int y)
        {
            Label lblNhan = new Label
            {
                Name = "lbl" + ten.Substring(3),
                Text = nhan,
                Left = 40,
                Top = y,
                Width = 360,
                Height = 20,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(107, 114, 128)
            };
            Controls.Add(lblNhan);

            TextBox t = new TextBox
            {
                Name = ten,
                Left = 40,
                Top = y + 20,
                Width = 360,
                Height = 28,
                TabIndex = ten == "txtCu" ? 0 : ten == "txtMoi" ? 1 : 2,
                AutoSize = false,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 11F),
                UseSystemPasswordChar = true
            };
            Controls.Add(t);

            Panel duongKe = new Panel
            {
                Name = "pnl" + ten.Substring(3) + "DuongKe",
                Left = 40,
                Top = y + 49,
                Width = 360,
                Height = 2,
                BackColor = Color.FromArgb(209, 213, 219)
            };
            Controls.Add(duongKe);
            t.GotFocus += (sender, e) => duongKe.BackColor = Color.FromArgb(232, 101, 26);
            t.LostFocus += (sender, e) => duongKe.BackColor = Color.FromArgb(209, 213, 219);
            return t;
        }

        private void XayDungGiaoDien()
        {
            Panel pnlTieuDe = new Panel
            {
                Name = "pnlTieuDe",
                Dock = DockStyle.Top,
                Height = 110,
                BackColor = Color.FromArgb(232, 101, 26)
            };
            pnlTieuDe.Paint += pnlTieuDe_Paint;

            Label lblTieuDe = new Label
            {
                Name = "lblTieuDe",
                Text = "ĐỔI MẬT KHẨU",
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Left = 40,
                Top = 76,
                Width = 360,
                Height = 26
            };
            pnlTieuDe.Controls.Add(lblTieuDe);
            Controls.Add(pnlTieuDe);

            txtCu = ThemO("Mật khẩu hiện tại", "txtCu", 135);
            txtMoi = ThemO("Mật khẩu mới (từ 6 ký tự)", "txtMoi", 215);
            txtNhapLai = ThemO("Nhập lại mật khẩu mới", "txtNhapLai", 305);

            pnlSucManh1 = TaoDoanSucManh("pnlSucManh1", 40);
            pnlSucManh2 = TaoDoanSucManh("pnlSucManh2", 165);
            pnlSucManh3 = TaoDoanSucManh("pnlSucManh3", 290);
            Controls.Add(pnlSucManh1);
            Controls.Add(pnlSucManh2);
            Controls.Add(pnlSucManh3);

            lblDoManh = new Label
            {
                Name = "lblDoManh",
                Left = 40,
                Top = 279,
                Width = 360,
                Height = 18,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 38, 38),
                Text = "Yếu"
            };
            Controls.Add(lblDoManh);
            txtMoi.TextChanged += txtMoi_TextChanged;
            txtMoi_TextChanged(txtMoi, EventArgs.Empty);

            chkHienMatKhau = new CheckBox
            {
                Name = "chkHienMatKhau",
                Text = "Hiện mật khẩu",
                Left = 40,
                Top = 385,
                AutoSize = true,
                TabIndex = 3,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(107, 114, 128)
            };
            chkHienMatKhau.CheckedChanged += chkHienMatKhau_CheckedChanged;
            Controls.Add(chkHienMatKhau);

            lblThongBao = new Label
            {
                Name = "lblThongBao",
                Left = 40,
                Top = 500,
                Width = 360,
                Height = 36,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(220, 38, 38),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };
            Controls.Add(lblThongBao);

            Button btnDoi = new Button
            {
                Name = "btnDoi",
                Text = "Đổi mật khẩu",
                Left = 40,
                Top = 440,
                Width = 200,
                Height = 44,
                TabIndex = 4,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(232, 101, 26),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDoi.FlatAppearance.BorderSize = 0;
            btnDoi.FlatAppearance.MouseOverBackColor = Color.FromArgb(207, 85, 16);
            btnDoi.Click += btnDoi_Click;
            Controls.Add(btnDoi);

            Button btnHuy = new Button
            {
                Name = "btnHuy",
                Text = "Hủy",
                Left = 252,
                Top = 440,
                Width = 148,
                Height = 44,
                TabIndex = 5,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(232, 101, 26),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };
            btnHuy.FlatAppearance.BorderSize = 1;
            btnHuy.FlatAppearance.BorderColor = Color.FromArgb(232, 101, 26);
            btnHuy.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 243, 234);
            Controls.Add(btnHuy);

            AcceptButton = btnDoi;
            CancelButton = btnHuy;
            ActiveControl = txtCu;
        }

        private Panel TaoDoanSucManh(string ten, int left)
        {
            return new Panel
            {
                Name = ten,
                Left = left,
                Top = 270,
                Width = 110,
                Height = 4,
                BackColor = Color.FromArgb(229, 231, 235)
            };
        }

        private void pnlTieuDe_Paint(object sender, PaintEventArgs e)
        {
            Panel pnlTieuDe = (Panel)sender;
            int left = (pnlTieuDe.Width - 56) / 2;
            Rectangle vongTron = new Rectangle(left, 14, 56, 56);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (SolidBrush nen = new SolidBrush(Color.White))
            using (Font font = new Font("Segoe UI Emoji", 22F))
            using (SolidBrush chu = new SolidBrush(Color.FromArgb(232, 101, 26)))
            using (StringFormat dinhDang = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                e.Graphics.FillEllipse(nen, vongTron);
                e.Graphics.DrawString("🔒", font, chu, vongTron, dinhDang);
            }
        }

        private void txtMoi_TextChanged(object sender, EventArgs e)
        {
            string matKhau = txtMoi.Text;
            int soDoanSang;
            string nhan;
            Color mau;

            if (matKhau.Length < 6)
            {
                soDoanSang = 1;
                nhan = "Yếu";
                mau = Color.FromArgb(220, 38, 38);
            }
            else
            {
                bool coChu = false;
                bool coSo = false;
                foreach (char kyTu in matKhau)
                {
                    coChu = coChu || char.IsLetter(kyTu);
                    coSo = coSo || char.IsDigit(kyTu);
                }

                if (matKhau.Length >= 8 && coChu && coSo)
                {
                    soDoanSang = 3;
                    nhan = "Mạnh";
                    mau = Color.FromArgb(22, 163, 74);
                }
                else
                {
                    soDoanSang = 2;
                    nhan = "Trung bình";
                    mau = Color.FromArgb(245, 158, 11);
                }
            }

            pnlSucManh1.BackColor = soDoanSang >= 1 ? mau : Color.FromArgb(229, 231, 235);
            pnlSucManh2.BackColor = soDoanSang >= 2 ? mau : Color.FromArgb(229, 231, 235);
            pnlSucManh3.BackColor = soDoanSang >= 3 ? mau : Color.FromArgb(229, 231, 235);
            lblDoManh.Text = nhan;
            lblDoManh.ForeColor = mau;
        }

        private void chkHienMatKhau_CheckedChanged(object sender, EventArgs e)
        {
            bool anMatKhau = !chkHienMatKhau.Checked;
            txtCu.UseSystemPasswordChar = anMatKhau;
            txtMoi.UseSystemPasswordChar = anMatKhau;
            txtNhapLai.UseSystemPasswordChar = anMatKhau;
        }

        private void HienThiLoi(string noiDung)
        {
            lblThongBao.Text = noiDung;
            lblThongBao.Visible = true;
        }

        private void btnDoi_Click(object sender, EventArgs e)
        {
            string cu = txtCu.Text;
            string moi = txtMoi.Text;

            if (cu.Length == 0) { HienThiLoi("Vui lòng nhập mật khẩu hiện tại."); return; }
            if (moi.Length < 6) { HienThiLoi("Mật khẩu mới phải từ 6 ký tự."); return; }
            if (moi != txtNhapLai.Text) { HienThiLoi("Hai lần nhập mật khẩu mới không khớp."); return; }
            if (moi == cu) { HienThiLoi("Mật khẩu mới phải khác mật khẩu hiện tại."); return; }

            try
            {
                string loi = TaiKhoan.DoiMatKhau(PhienDangNhap.MaNV, cu, moi);
                if (loi != null)
                {
                    HienThiLoi(loi);
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