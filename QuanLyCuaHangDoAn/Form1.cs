using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyCuaHangDoAn
{
    public partial class Form1 : Form
    {
        private DataGridView dgv;
        private frmDanhMuc formDanhMuc;
        private frmMonAn formMonAn;

        public Form1()
        {
            InitializeComponent();
            Text = "Quản lý cửa hàng đồ ăn";
            Font = GiaoDien.FontThuong;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            ClientSize = new Size(900, 500);

            dgv = new DataGridView { Dock = DockStyle.Fill };
            Controls.Add(dgv);

            var thanhNut = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(8, 6, 8, 6),
                WrapContents = false
            };
            var btnDanhMuc = GiaoDien.TaoNut("Quản lý danh mục", GiaoDien.MauChinh);
            var btnMonAn = GiaoDien.TaoNut("Quản lý món ăn", GiaoDien.MauChinh);
            btnDanhMuc.Width = 170;
            btnMonAn.Width = 170;
            btnDanhMuc.Click += btnDanhMuc_Click;
            btnMonAn.Click += btnMonAn_Click;
            thanhNut.Controls.Add(btnDanhMuc);
            thanhNut.Controls.Add(btnMonAn);
            Controls.Add(thanhNut);
            Controls.Add(GiaoDien.TaoTieuDe("DANH SÁCH MÓN ĂN"));
            Load += Form1_Load;
        }

        private void btnDanhMuc_Click(object sender, EventArgs e)
        {
            if (formDanhMuc == null || formDanhMuc.IsDisposed)
                formDanhMuc = new frmDanhMuc();

            if (!formDanhMuc.Visible)
                formDanhMuc.Show(this);
            else
                formDanhMuc.Activate();
        }

        private void btnMonAn_Click(object sender, EventArgs e)
        {
            if (formMonAn == null || formMonAn.IsDisposed)
                formMonAn = new frmMonAn();

            if (!formMonAn.Visible)
                formMonAn.Show(this);
            else
                formMonAn.Activate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                var fx = new FileXml();
                fx.TaoXml("SELECT * FROM MonAn", "MonAn", "MonAn.xml");
                dgv.DataSource = fx.HienThi("MonAn.xml");
                GiaoDien.LamDepBang(dgv, new Dictionary<string, string>
                {
                    { "MaMon", "Mã món" }, { "TenMon", "Tên món" }, { "MaDM", "Danh mục" },
                    { "DonGia", "Đơn giá" }, { "DVT", "ĐVT" }, { "TrangThai", "Trạng thái" }
                });
                GiaoDien.DinhDangTien(dgv, "DonGia");
                dgv.Columns["TenMon"].FillWeight = 200;
                dgv.Columns["DVT"].FillWeight = 60;
                dgv.Columns["MaMon"].FillWeight = 80;
                dgv.Columns["MaDM"].FillWeight = 80;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}