using System;
using System.Data;
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
            DungGiaoDien();
            Load += Form1_Load;
        }

        private void DungGiaoDien()
        {
            Text = "Quản lý cửa hàng đồ ăn - Thử nối dữ liệu";
            Font = new Font("Segoe UI", 10F);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            ClientSize = new Size(860, 480);

            var lblTieuDe = new Label
            {
                Text = "DANH SÁCH MÓN ĂN",
                Dock = DockStyle.Top,
                Height = 50,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(230, 100, 20),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold)
            };

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 235, 215);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(120, 50, 0);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 36;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 248, 240);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 100, 20);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;

            Controls.Add(dgv);
            Controls.Add(lblTieuDe);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                var fx = new FileXml();
                fx.TaoXml("SELECT * FROM MonAn", "MonAn", "MonAn.xml");
                dgv.DataSource = fx.HienThi("MonAn.xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}