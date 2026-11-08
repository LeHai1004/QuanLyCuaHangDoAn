using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyCuaHangDoAn
{
    public partial class Form1 : Form
    {
        private DataGridView dgv;

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
            Controls.Add(GiaoDien.TaoTieuDe("DANH SÁCH MÓN ĂN"));
            Load += Form1_Load;
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