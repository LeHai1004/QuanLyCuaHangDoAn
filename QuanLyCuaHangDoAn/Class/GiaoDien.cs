using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace QuanLyCuaHangDoAn
{
    /// <summary>Bang mau, font va ham lam dep dung chung. Hai so huu; nguoi khac chi goi ham.</summary>
    public static class GiaoDien
    {
        public static readonly Color MauChinh = Color.FromArgb(230, 100, 20);
        public static readonly Color MauChinhDam = Color.FromArgb(190, 80, 10);
        public static readonly Color MauNenNhat = Color.FromArgb(255, 248, 240);
        public static readonly Color MauTieuDeBang = Color.FromArgb(255, 235, 215);
        public static readonly Color MauXanh = Color.FromArgb(40, 160, 90);
        public static readonly Color MauDo = Color.FromArgb(210, 60, 50);

        public static readonly Font FontThuong = new Font("Segoe UI", 10F);
        public static readonly Font FontDam = new Font("Segoe UI", 10F, FontStyle.Bold);
        public static readonly Font FontTieuDe = new Font("Segoe UI", 15F, FontStyle.Bold);

        private static readonly CultureInfo ViVN = new CultureInfo("vi-VN");

        /// <summary>Thanh tieu de mau cam phia tren form.</summary>
        public static Label TaoTieuDe(string chu)
        {
            return new Label
            {
                Text = chu,
                Dock = DockStyle.Top,
                Height = 56,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = MauChinh,
                ForeColor = Color.White,
                Font = FontTieuDe
            };
        }

        /// <summary>Nut phang, bo vien, doi mau khi re chuot vao.</summary>
        public static Button TaoNut(string chu, Color mau)
        {
            var b = new Button
            {
                Text = chu,
                BackColor = mau,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = FontDam,
                Height = 38,
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(mau, 0.1f);
            return b;
        }

        /// <summary>Lam dep bang va doi tieu de cot sang tieng Viet co dau. Goi SAU khi gan DataSource.</summary>
        public static void LamDepBang(DataGridView dgv, Dictionary<string, string> tieuDeCot = null)
        {
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgv.GridColor = Color.FromArgb(225, 205, 190);
            dgv.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersHeight = 40;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = MauTieuDeBang;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(120, 50, 0);
            dgv.ColumnHeadersDefaultCellStyle.Font = FontDam;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = MauTieuDeBang;
            dgv.RowTemplate.Height = 34;
            dgv.DefaultCellStyle.Font = FontThuong;
            dgv.DefaultCellStyle.SelectionBackColor = MauChinh;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = MauNenNhat;

            if (tieuDeCot == null) return;
            foreach (var kv in tieuDeCot)
                if (dgv.Columns.Contains(kv.Key)) dgv.Columns[kv.Key].HeaderText = kv.Value;
        }

        /// <summary>Hien cot tien dang 55.000 d, can phai.</summary>
        public static void DinhDangTien(DataGridView dgv, params string[] cot)
        {
            foreach (string c in cot)
            {
                if (!dgv.Columns.Contains(c)) continue;
                var s = dgv.Columns[c].DefaultCellStyle;
                s.FormatProvider = ViVN;
                s.Format = "#,##0\" đ\"";
                s.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns[c].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }
    }
}