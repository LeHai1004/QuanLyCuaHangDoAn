namespace QuanLyCuaHangDoAn.GUI
{
    partial class frmDangNhap
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pnlTrai = new System.Windows.Forms.Panel();
            this.lblBieuTuong = new System.Windows.Forms.Label();
            this.lblTenQuan = new System.Windows.Forms.Label();
            this.lblKhauHieu = new System.Windows.Forms.Label();
            this.pnlDai = new System.Windows.Forms.Panel();
            this.lblNhom = new System.Windows.Forms.Label();
            this.lblTieuDe = new System.Windows.Forms.Label();
            this.lblMoTa = new System.Windows.Forms.Label();
            this.lblTenDN = new System.Windows.Forms.Label();
            this.txtTenDN = new System.Windows.Forms.TextBox();
            this.lblMatKhau = new System.Windows.Forms.Label();
            this.txtMatKhau = new System.Windows.Forms.TextBox();
            this.btnDangNhap = new System.Windows.Forms.Button();
            this.lblThongBao = new System.Windows.Forms.Label();
            this.lblBanQuyen = new System.Windows.Forms.Label();
            this.pnlTrai.SuspendLayout();
            this.pnlDai.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlTrai
            //
            this.pnlTrai.BackColor = System.Drawing.Color.FromArgb(230, 81, 0);
            this.pnlTrai.Controls.Add(this.pnlDai);
            this.pnlTrai.Controls.Add(this.lblKhauHieu);
            this.pnlTrai.Controls.Add(this.lblTenQuan);
            this.pnlTrai.Controls.Add(this.lblBieuTuong);
            this.pnlTrai.Location = new System.Drawing.Point(0, 0);
            this.pnlTrai.Name = "pnlTrai";
            this.pnlTrai.Size = new System.Drawing.Size(330, 480);
            //
            // lblBieuTuong
            //
            this.lblBieuTuong.Font = new System.Drawing.Font("Segoe UI Emoji", 54F);
            this.lblBieuTuong.ForeColor = System.Drawing.Color.White;
            this.lblBieuTuong.Location = new System.Drawing.Point(0, 80);
            this.lblBieuTuong.Name = "lblBieuTuong";
            this.lblBieuTuong.Size = new System.Drawing.Size(330, 110);
            this.lblBieuTuong.Text = "🍜";
            this.lblBieuTuong.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblTenQuan
            //
            this.lblTenQuan.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTenQuan.ForeColor = System.Drawing.Color.White;
            this.lblTenQuan.Location = new System.Drawing.Point(20, 200);
            this.lblTenQuan.Name = "lblTenQuan";
            this.lblTenQuan.Size = new System.Drawing.Size(290, 80);
            this.lblTenQuan.Text = "QUẢN LÝ\r\nCỬA HÀNG ĐỒ ĂN";
            this.lblTenQuan.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblKhauHieu
            //
            this.lblKhauHieu.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Italic);
            this.lblKhauHieu.ForeColor = System.Drawing.Color.FromArgb(255, 224, 178);
            this.lblKhauHieu.Location = new System.Drawing.Point(20, 290);
            this.lblKhauHieu.Name = "lblKhauHieu";
            this.lblKhauHieu.Size = new System.Drawing.Size(290, 60);
            this.lblKhauHieu.Text = "Gọi món nhanh - Bếp rõ ràng\r\nKho gọn - Hóa đơn đẹp";
            this.lblKhauHieu.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // pnlDai
            //
            this.pnlDai.BackColor = System.Drawing.Color.FromArgb(191, 54, 12);
            this.pnlDai.Controls.Add(this.lblNhom);
            this.pnlDai.Location = new System.Drawing.Point(0, 430);
            this.pnlDai.Name = "pnlDai";
            this.pnlDai.Size = new System.Drawing.Size(330, 50);
            //
            // lblNhom
            //
            this.lblNhom.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblNhom.ForeColor = System.Drawing.Color.FromArgb(255, 204, 128);
            this.lblNhom.Location = new System.Drawing.Point(0, 0);
            this.lblNhom.Name = "lblNhom";
            this.lblNhom.Size = new System.Drawing.Size(330, 50);
            this.lblNhom.Text = "Đồ án Công nghệ XML - Lớp 126CNX02";
            this.lblNhom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblTieuDe
            //
            this.lblTieuDe.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblTieuDe.ForeColor = System.Drawing.Color.FromArgb(62, 39, 35);
            this.lblTieuDe.Location = new System.Drawing.Point(395, 70);
            this.lblTieuDe.Name = "lblTieuDe";
            this.lblTieuDe.Size = new System.Drawing.Size(340, 45);
            this.lblTieuDe.Text = "Chào mừng trở lại";
            this.lblTieuDe.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblMoTa
            //
            this.lblMoTa.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblMoTa.ForeColor = System.Drawing.Color.FromArgb(141, 110, 99);
            this.lblMoTa.Location = new System.Drawing.Point(395, 117);
            this.lblMoTa.Name = "lblMoTa";
            this.lblMoTa.Size = new System.Drawing.Size(340, 24);
            this.lblMoTa.Text = "Đăng nhập để bắt đầu ca làm việc";
            this.lblMoTa.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblTenDN
            //
            this.lblTenDN.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblTenDN.ForeColor = System.Drawing.Color.FromArgb(93, 64, 55);
            this.lblTenDN.Location = new System.Drawing.Point(395, 170);
            this.lblTenDN.Name = "lblTenDN";
            this.lblTenDN.Size = new System.Drawing.Size(340, 22);
            this.lblTenDN.Text = "Tên đăng nhập";
            this.lblTenDN.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtTenDN
            //
            this.txtTenDN.BackColor = System.Drawing.Color.White;
            this.txtTenDN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTenDN.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtTenDN.Location = new System.Drawing.Point(395, 195);
            this.txtTenDN.MaxLength = 30;
            this.txtTenDN.Name = "txtTenDN";
            this.txtTenDN.Size = new System.Drawing.Size(340, 29);
            this.txtTenDN.TabIndex = 0;
            //
            // lblMatKhau
            //
            this.lblMatKhau.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblMatKhau.ForeColor = System.Drawing.Color.FromArgb(93, 64, 55);
            this.lblMatKhau.Location = new System.Drawing.Point(395, 242);
            this.lblMatKhau.Name = "lblMatKhau";
            this.lblMatKhau.Size = new System.Drawing.Size(340, 22);
            this.lblMatKhau.Text = "Mật khẩu";
            this.lblMatKhau.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtMatKhau
            //
            this.txtMatKhau.BackColor = System.Drawing.Color.White;
            this.txtMatKhau.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMatKhau.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtMatKhau.Location = new System.Drawing.Point(395, 267);
            this.txtMatKhau.Name = "txtMatKhau";
            this.txtMatKhau.Size = new System.Drawing.Size(340, 29);
            this.txtMatKhau.TabIndex = 1;
            this.txtMatKhau.UseSystemPasswordChar = true;
            //
            // btnDangNhap
            //
            this.btnDangNhap.BackColor = System.Drawing.Color.FromArgb(230, 81, 0);
            this.btnDangNhap.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDangNhap.FlatAppearance.BorderSize = 0;
            this.btnDangNhap.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(191, 54, 12);
            this.btnDangNhap.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(244, 108, 30);
            this.btnDangNhap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDangNhap.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnDangNhap.ForeColor = System.Drawing.Color.White;
            this.btnDangNhap.Location = new System.Drawing.Point(395, 330);
            this.btnDangNhap.Name = "btnDangNhap";
            this.btnDangNhap.Size = new System.Drawing.Size(340, 46);
            this.btnDangNhap.TabIndex = 2;
            this.btnDangNhap.Text = "ĐĂNG NHẬP";
            this.btnDangNhap.UseVisualStyleBackColor = false;
            this.btnDangNhap.Click += new System.EventHandler(this.btnDangNhap_Click);
            //
            // lblThongBao
            //
            this.lblThongBao.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblThongBao.ForeColor = System.Drawing.Color.Firebrick;
            this.lblThongBao.Location = new System.Drawing.Point(395, 385);
            this.lblThongBao.Name = "lblThongBao";
            this.lblThongBao.Size = new System.Drawing.Size(340, 24);
            this.lblThongBao.Text = "";
            this.lblThongBao.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblBanQuyen
            //
            this.lblBanQuyen.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblBanQuyen.ForeColor = System.Drawing.Color.FromArgb(161, 136, 127);
            this.lblBanQuyen.Location = new System.Drawing.Point(395, 440);
            this.lblBanQuyen.Name = "lblBanQuyen";
            this.lblBanQuyen.Size = new System.Drawing.Size(340, 24);
            this.lblBanQuyen.Text = "© 2026 Nhóm Hải - Quốc - Duy";
            this.lblBanQuyen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // frmDangNhap
            //
            this.AcceptButton = this.btnDangNhap;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(255, 248, 240);
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.lblBanQuyen);
            this.Controls.Add(this.lblThongBao);
            this.Controls.Add(this.btnDangNhap);
            this.Controls.Add(this.txtMatKhau);
            this.Controls.Add(this.lblMatKhau);
            this.Controls.Add(this.txtTenDN);
            this.Controls.Add(this.lblTenDN);
            this.Controls.Add(this.lblMoTa);
            this.Controls.Add(this.lblTieuDe);
            this.Controls.Add(this.pnlTrai);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDangNhap";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đăng nhập - Quản lý cửa hàng đồ ăn";
            this.pnlTrai.ResumeLayout(false);
            this.pnlDai.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel pnlTrai;
        private System.Windows.Forms.Label lblBieuTuong;
        private System.Windows.Forms.Label lblTenQuan;
        private System.Windows.Forms.Label lblKhauHieu;
        private System.Windows.Forms.Panel pnlDai;
        private System.Windows.Forms.Label lblNhom;
        private System.Windows.Forms.Label lblTieuDe;
        private System.Windows.Forms.Label lblMoTa;
        private System.Windows.Forms.Label lblTenDN;
        private System.Windows.Forms.TextBox txtTenDN;
        private System.Windows.Forms.Label lblMatKhau;
        private System.Windows.Forms.TextBox txtMatKhau;
        private System.Windows.Forms.Button btnDangNhap;
        private System.Windows.Forms.Label lblThongBao;
        private System.Windows.Forms.Label lblBanQuyen;
    }
}