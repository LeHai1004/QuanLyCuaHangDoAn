using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using QuanLyCuaHangDoAn.Class;

namespace QuanLyCuaHangDoAn.GUI
{
    public class ucTrangChu : UserControl
    {
        private static readonly Color MauNen = Color.FromArgb(245, 246, 248);
        private static readonly Color MauCam = Color.FromArgb(232, 101, 26);
        private static readonly Color MauXam = Color.FromArgb(107, 114, 128);
        private readonly string tenNguoiDung;
        private readonly string vaiTro;
        private readonly Action<string> moChucNang;
        private readonly ToolTip toolTip = new ToolTip();
        private readonly Timer timerDongHo = new Timer();
        private readonly Timer timerLamMoi = new Timer();
        private readonly Label lblChao = new Label();
        private readonly Label lblNgay = new Label();
        private readonly Label lblDongHo = new Label();
        private readonly TableLayoutPanel bangChiSo = new TableLayoutPanel();
        private readonly MetricCard[] theChiSo;
        private readonly DataGridView dgvHoaDonGanDay = new DataGridView();
        private readonly Label lblHoaDonRong = new Label();
        private readonly TopMonPanel pnlTopMon = new TopMonPanel();
        private readonly QuickActionButton btnMoBep = new QuickActionButton("🍜", "Mở màn hình bếp");
        private bool dangLamMoi;

        public ucTrangChu(string tenNguoiDung, string vaiTro, Action<string> moChucNang)
        {
            this.tenNguoiDung = tenNguoiDung ?? "";
            this.vaiTro = vaiTro ?? "";
            this.moChucNang = moChucNang;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw, true);
            DoubleBuffered = true;
            Dock = DockStyle.Fill;
            BackColor = MauNen;
            Name = "ucTrangChu";

            List<MetricCard> dsThe = TaoDanhSachThe();
            theChiSo = dsThe.ToArray();
            XayDungGiaoDien(dsThe);
            CapNhatBanner();

            timerDongHo.Interval = 1000;
            timerDongHo.Tick += (s, e) => CapNhatBanner();
            timerDongHo.Start();
            timerLamMoi.Interval = 30000;
            timerLamMoi.Tick += (s, e) => LamMoiDuLieu();
            timerLamMoi.Start();
            VisibleChanged += ucTrangChu_VisibleChanged;
            LamMoiDuLieu();
        }

        public void LamMoiDuLieu()
        {
            if (dangLamMoi || IsDisposed)
                return;

            dangLamMoi = true;
            try
            {
                if (vaiTro == "Admin")
                {
                    NapDoanhThuVaHoaDon(theChiSo[0], theChiSo[1]);
                    NapThongKeBan(theChiSo[2], null, true);
                    NapCanhBaoNguyenLieu(theChiSo[3]);
                    NapHoaDonGanDay();
                    NapTopMonBanChay();
                }
                else if (vaiTro == "ThuNgan")
                {
                    NapSoHoaDonHomNay(theChiSo[0]);
                    NapThongKeBan(theChiSo[1], theChiSo[2], false);
                    NapHoaDonDangPhucVu(theChiSo[3]);
                    NapHoaDonGanDay();
                    NapTopMonBanChay();
                }
                else if (vaiTro == "Bep")
                {
                    NapThongKeBep();
                }
            }
            finally
            {
                dangLamMoi = false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                timerDongHo.Stop();
                timerDongHo.Dispose();
                timerLamMoi.Stop();
                timerLamMoi.Dispose();
                toolTip.Dispose();
            }
            base.Dispose(disposing);
        }

        private List<MetricCard> TaoDanhSachThe()
        {
            var ketQua = new List<MetricCard>();
            if (vaiTro == "Admin")
            {
                ketQua.Add(new MetricCard(toolTip, "💰", "Doanh thu hôm nay", Color.FromArgb(232, 101, 26),
                    Color.FromArgb(255, 243, 234)));
                ketQua.Add(new MetricCard(toolTip, "🧾", "Hóa đơn hôm nay", Color.FromArgb(37, 99, 235),
                    Color.FromArgb(239, 246, 255)));
                ketQua.Add(new MetricCard(toolTip, "🪑", "Bàn có khách", Color.FromArgb(220, 38, 38),
                    Color.FromArgb(254, 242, 242)));
                ketQua.Add(new MetricCard(toolTip, "⚠", "Nguyên liệu sắp hết", Color.FromArgb(22, 163, 74),
                    Color.FromArgb(236, 253, 243)));
            }
            else if (vaiTro == "ThuNgan")
            {
                ketQua.Add(new MetricCard(toolTip, "🧾", "Hóa đơn hôm nay", Color.FromArgb(37, 99, 235),
                    Color.FromArgb(239, 246, 255)));
                ketQua.Add(new MetricCard(toolTip, "🪑", "Bàn trống", Color.FromArgb(22, 163, 74),
                    Color.FromArgb(236, 253, 243)));
                ketQua.Add(new MetricCard(toolTip, "🪑", "Bàn có khách", Color.FromArgb(220, 38, 38),
                    Color.FromArgb(254, 242, 242)));
                ketQua.Add(new MetricCard(toolTip, "🧾", "Hóa đơn đang phục vụ", Color.FromArgb(245, 158, 11),
                    Color.FromArgb(255, 247, 237)));
            }
            else
            {
                ketQua.Add(new MetricCard(toolTip, "⏳", "Chờ", Color.FromArgb(245, 158, 11),
                    Color.FromArgb(255, 247, 237)));
                ketQua.Add(new MetricCard(toolTip, "🔥", "Đang làm", Color.FromArgb(37, 99, 235),
                    Color.FromArgb(239, 246, 255)));
                ketQua.Add(new MetricCard(toolTip, "✅", "Xong", Color.FromArgb(22, 163, 74),
                    Color.FromArgb(236, 253, 243)));
            }
            return ketQua;
        }

        private void XayDungGiaoDien(List<MetricCard> dsThe)
        {
            var boCuc = new TableLayoutPanel
            {
                Name = "tlpTrangChu",
                Dock = DockStyle.Fill,
                BackColor = MauNen,
                Padding = new Padding(24),
                ColumnCount = 1,
                RowCount = 5
            };
            boCuc.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            boCuc.RowStyles.Add(new RowStyle(SizeType.Absolute, 120F));
            boCuc.RowStyles.Add(new RowStyle(SizeType.Absolute, 16F));
            boCuc.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F));
            boCuc.RowStyles.Add(new RowStyle(SizeType.Absolute, 16F));
            boCuc.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            boCuc.Controls.Add(TaoBanner(), 0, 0);
            TaoBangChiSo(dsThe);
            boCuc.Controls.Add(bangChiSo, 0, 2);
            boCuc.Controls.Add(TaoVungDuoi(), 0, 4);
            Controls.Add(boCuc);
        }

        private Control TaoBanner()
        {
            var banner = new BannerPanel
            {
                Name = "pnlBanner",
                Dock = DockStyle.Fill,
                Margin = Padding.Empty
            };
            lblChao.Name = "lblChao";
            lblChao.Location = new Point(24, 21);
            lblChao.Size = new Size(650, 34);
            lblChao.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblChao.ForeColor = Color.White;
            lblChao.BackColor = Color.Transparent;
            lblChao.AutoEllipsis = true;
            lblNgay.Name = "lblNgay";
            lblNgay.Location = new Point(27, 68);
            lblNgay.Size = new Size(650, 22);
            lblNgay.Font = new Font("Segoe UI", 10F);
            lblNgay.ForeColor = Color.White;
            lblNgay.BackColor = Color.Transparent;
            lblDongHo.Name = "lblDongHo";
            lblDongHo.Size = new Size(205, 48);
            lblDongHo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblDongHo.TextAlign = ContentAlignment.MiddleRight;
            lblDongHo.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
            lblDongHo.ForeColor = Color.White;
            lblDongHo.BackColor = Color.Transparent;
            banner.Controls.Add(lblChao);
            banner.Controls.Add(lblNgay);
            banner.Controls.Add(lblDongHo);
            banner.SizeChanged += (s, e) =>
            {
                lblChao.Width = Math.Max(350, banner.ClientSize.Width - 390);
                lblNgay.Width = lblChao.Width;
                lblDongHo.Location = new Point(banner.ClientSize.Width - lblDongHo.Width - 28, 35);
            };
            return banner;
        }

        private void TaoBangChiSo(List<MetricCard> dsThe)
        {
            int soCot = dsThe.Count;
            bangChiSo.Name = "tlpChiSo";
            bangChiSo.Dock = DockStyle.Fill;
            bangChiSo.Margin = Padding.Empty;
            bangChiSo.Padding = Padding.Empty;
            bangChiSo.BackColor = MauNen;
            bangChiSo.ColumnCount = soCot * 2 - 1;
            bangChiSo.RowCount = 1;
            bangChiSo.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            for (int i = 0; i < soCot; i++)
            {
                bangChiSo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / soCot));
                MetricCard the = dsThe[i];
                the.Dock = DockStyle.Fill;
                the.Margin = Padding.Empty;
                bangChiSo.Controls.Add(the, i * 2, 0);
                if (i < soCot - 1)
                    bangChiSo.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 16F));
            }
        }

        private Control TaoVungDuoi()
        {
            var boCucDuoi = new TableLayoutPanel
            {
                Name = "tlpNoiDung",
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = MauNen
            };
            boCucDuoi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            boCucDuoi.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 16F));
            boCucDuoi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            boCucDuoi.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            if (vaiTro == "Bep")
                boCucDuoi.Controls.Add(TaoTheBep(), 0, 0);
            else
                boCucDuoi.Controls.Add(TaoTheHoaDonGanDay(), 0, 0);

            var cotPhai = new TableLayoutPanel
            {
                Name = "tlpCotPhai",
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                ColumnCount = 1,
                RowCount = vaiTro == "Bep" ? 1 : 3,
                BackColor = MauNen
            };
            cotPhai.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            if (vaiTro == "Bep")
            {
                cotPhai.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                cotPhai.Controls.Add(TaoTheLoiTat(), 0, 0);
            }
            else
            {
                cotPhai.RowStyles.Add(new RowStyle(SizeType.Percent, 49F));
                cotPhai.RowStyles.Add(new RowStyle(SizeType.Absolute, 16F));
                cotPhai.RowStyles.Add(new RowStyle(SizeType.Percent, 51F));
                cotPhai.Controls.Add(TaoTheLoiTat(), 0, 0);
                cotPhai.Controls.Add(TaoTheTopMon(), 0, 2);
            }
            boCucDuoi.Controls.Add(cotPhai, 2, 0);
            return boCucDuoi;
        }

        private Control TaoTheHoaDonGanDay()
        {
            RoundedPanel the = TaoKhungTieuDe("HÓA ĐƠN GẦN ĐÂY", out Panel noiDung);
            CauHinhBangHoaDon();
            dgvHoaDonGanDay.Dock = DockStyle.Fill;
            lblHoaDonRong.Dock = DockStyle.Fill;
            lblHoaDonRong.TextAlign = ContentAlignment.MiddleCenter;
            lblHoaDonRong.ForeColor = MauXam;
            lblHoaDonRong.Font = new Font("Segoe UI", 10F);
            lblHoaDonRong.Text = "Chưa có dữ liệu";
            noiDung.Controls.Add(dgvHoaDonGanDay);
            noiDung.Controls.Add(lblHoaDonRong);
            lblHoaDonRong.BringToFront();
            return the;
        }

        private void CauHinhBangHoaDon()
        {
            dgvHoaDonGanDay.Name = "dgvHoaDonGanDay";
            dgvHoaDonGanDay.ReadOnly = true;
            dgvHoaDonGanDay.AllowUserToAddRows = false;
            dgvHoaDonGanDay.AllowUserToDeleteRows = false;
            dgvHoaDonGanDay.AllowUserToResizeRows = false;
            dgvHoaDonGanDay.RowHeadersVisible = false;
            dgvHoaDonGanDay.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHoaDonGanDay.MultiSelect = false;
            dgvHoaDonGanDay.AutoGenerateColumns = false;
            dgvHoaDonGanDay.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHoaDonGanDay.BorderStyle = BorderStyle.None;
            dgvHoaDonGanDay.BackgroundColor = Color.White;
            dgvHoaDonGanDay.GridColor = Color.FromArgb(229, 231, 235);
            dgvHoaDonGanDay.EnableHeadersVisualStyles = false;
            dgvHoaDonGanDay.ColumnHeadersHeight = 32;
            dgvHoaDonGanDay.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvHoaDonGanDay.RowTemplate.Height = 32;
            dgvHoaDonGanDay.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 243, 234);
            dgvHoaDonGanDay.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(154, 52, 18);
            dgvHoaDonGanDay.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvHoaDonGanDay.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgvHoaDonGanDay.DefaultCellStyle.ForeColor = Color.FromArgb(55, 65, 81);
            dgvHoaDonGanDay.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 243, 234);
            dgvHoaDonGanDay.DefaultCellStyle.SelectionForeColor = Color.FromArgb(55, 65, 81);
            dgvHoaDonGanDay.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            dgvHoaDonGanDay.Columns.Add(TaoCotHoaDon("MaHD", "Mã HD", 18F));
            dgvHoaDonGanDay.Columns.Add(TaoCotHoaDon("TenBan", "Bàn", 20F));
            dgvHoaDonGanDay.Columns.Add(TaoCotHoaDon("Gio", "Giờ", 14F));
            var cotTien = TaoCotHoaDon("TongTien", "Tổng tiền", 28F);
            cotTien.DefaultCellStyle.Format = "N0";
            cotTien.DefaultCellStyle.FormatProvider = new CultureInfo("vi-VN");
            cotTien.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvHoaDonGanDay.Columns.Add(cotTien);
            dgvHoaDonGanDay.Columns.Add(TaoCotHoaDon("TrangThai", "Trạng thái", 25F));
            dgvHoaDonGanDay.CellFormatting += dgvHoaDonGanDay_CellFormatting;
        }

        private static DataGridViewTextBoxColumn TaoCotHoaDon(string ten, string tieuDe, float tiLe)
        {
            return new DataGridViewTextBoxColumn
            {
                Name = ten,
                DataPropertyName = ten,
                HeaderText = tieuDe,
                FillWeight = tiLe,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
        }

        private void dgvHoaDonGanDay_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvHoaDonGanDay.Columns[e.ColumnIndex].Name != "TrangThai")
                return;
            string trangThai = Convert.ToString(e.Value);
            Color mau = trangThai == "Đã thanh toán"
                ? Color.FromArgb(22, 163, 74)
                : Color.FromArgb(245, 158, 11);
            e.CellStyle.ForeColor = mau;
            e.CellStyle.SelectionForeColor = mau;
        }

        private Control TaoTheLoiTat()
        {
            RoundedPanel the = TaoKhungTieuDe("LỐI TẮT NHANH", out Panel noiDung);
            var danhSach = new List<QuickActionInfo>();
            if (vaiTro == "Admin")
            {
                danhSach.Add(new QuickActionInfo("🍽", "Món ăn", "Món ăn"));
                danhSach.Add(new QuickActionInfo("🪑", "Bàn", "Bàn"));
                danhSach.Add(new QuickActionInfo("📊", "Thống kê", "Thống kê"));
                danhSach.Add(new QuickActionInfo("👥", "Khách hàng", "Khách hàng"));
                danhSach.Add(new QuickActionInfo("📦", "Nguyên liệu", "Nguyên liệu"));
                danhSach.Add(new QuickActionInfo("📥", "Nhập/xuất XML", "Nhập/xuất XML"));
            }
            else if (vaiTro == "ThuNgan")
            {
                danhSach.Add(new QuickActionInfo("📝", "Gọi món", "Gọi món"));
                danhSach.Add(new QuickActionInfo("🪑", "Sơ đồ bàn", "Sơ đồ bàn"));
                danhSach.Add(new QuickActionInfo("💳", "Thanh toán / Hóa đơn", "Hóa đơn"));
                danhSach.Add(new QuickActionInfo("👥", "Khách hàng", "Khách hàng"));
            }
            else
            {
                danhSach.Add(new QuickActionInfo("🍜", "Màn hình bếp", "Màn hình bếp"));
            }

            int soCot = danhSach.Count == 1 ? 1 : 2;
            int soHang = (int)Math.Ceiling(danhSach.Count / (double)soCot);
            var luoi = new TableLayoutPanel
            {
                Name = "tlpLoiTat",
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Color.White,
                ColumnCount = soCot,
                RowCount = soHang
            };
            for (int cot = 0; cot < soCot; cot++)
                luoi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / soCot));
            for (int hang = 0; hang < soHang; hang++)
                luoi.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / soHang));

            for (int i = 0; i < danhSach.Count; i++)
            {
                QuickActionInfo muc = danhSach[i];
                var nut = new QuickActionButton(muc.BieuTuong, muc.TenHienThi)
                {
                    Dock = DockStyle.Fill,
                    Margin = new Padding(4),
                    Name = "btnLoiTat" + i
                };
                string tenMenu = muc.TenMenu;
                nut.Click += (s, e) =>
                {
                    if (moChucNang != null)
                        moChucNang(tenMenu);
                };
                luoi.Controls.Add(nut, i % soCot, i / soCot);
            }
            noiDung.Controls.Add(luoi);
            return the;
        }

        private Control TaoTheTopMon()
        {
            RoundedPanel the = TaoKhungTieuDe("TOP 5 MÓN BÁN CHẠY (7 NGÀY)", out Panel noiDung);
            pnlTopMon.Dock = DockStyle.Fill;
            pnlTopMon.Name = "pnlTopMon";
            noiDung.Controls.Add(pnlTopMon);
            return the;
        }

        private Control TaoTheBep()
        {
            RoundedPanel the = TaoKhungTieuDe("MÀN HÌNH BẾP", out Panel noiDung);
            var boCuc = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.White
            };
            boCuc.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            boCuc.RowStyles.Add(new RowStyle(SizeType.Absolute, 115F));
            boCuc.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
            boCuc.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
            boCuc.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            var lblBieuTuong = new Label
            {
                Name = "lblBieuTuongBep",
                Text = "🍜",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Emoji", 52F),
                ForeColor = Color.FromArgb(232, 101, 26)
            };
            var lblMoTaBep = new Label
            {
                Name = "lblMoTaBep",
                Text = "Theo dõi món chờ chế biến và cập nhật trạng thái món ăn.",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10F),
                ForeColor = MauXam
            };
            btnMoBep.Dock = DockStyle.Fill;
            btnMoBep.Margin = new Padding(32, 4, 32, 4);
            btnMoBep.Click += (s, e) =>
            {
                if (moChucNang != null)
                    moChucNang("Màn hình bếp");
            };
            boCuc.Controls.Add(lblBieuTuong, 0, 0);
            boCuc.Controls.Add(lblMoTaBep, 0, 1);
            boCuc.Controls.Add(btnMoBep, 0, 2);
            noiDung.Controls.Add(boCuc);
            return the;
        }

        private static RoundedPanel TaoKhungTieuDe(string tieuDe, out Panel noiDung)
        {
            var the = new RoundedPanel
            {
                BackColor = Color.White,
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Name = "pnlTheThongTin"
            };
            var lblTieuDe = new Label
            {
                Text = tieuDe,
                Location = new Point(16, 11),
                Size = new Size(400, 24),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = MauCam,
                AutoEllipsis = true
            };
            var panelNoiDung = new Panel
            {
                Location = new Point(16, 43),
                Size = new Size(300, 180),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.White
            };
            noiDung = panelNoiDung;
            the.Controls.Add(panelNoiDung);
            the.Controls.Add(lblTieuDe);
            the.SizeChanged += (s, e) =>
            {
                lblTieuDe.Width = Math.Max(20, the.ClientSize.Width - 32);
                panelNoiDung.Width = Math.Max(20, the.ClientSize.Width - 32);
                panelNoiDung.Height = Math.Max(20, the.ClientSize.Height - 59);
            };
            return the;
        }

        private void CapNhatBanner()
        {
            DateTime now = DateTime.Now;
            string loiChao = now.Hour < 11 ? "Chào buổi sáng" :
                (now.Hour <= 18 ? "Chào buổi chiều" : "Chào buổi tối");
            string ngay = now.ToString("dddd, dd/MM/yyyy", new CultureInfo("vi-VN"));
            ngay = char.ToUpper(ngay[0], new CultureInfo("vi-VN")) + ngay.Substring(1);
            lblChao.Text = loiChao + ", " + tenNguoiDung;
            lblNgay.Text = TenVaiTro(vaiTro) + " • " + ngay;
            lblDongHo.Text = now.ToString("HH:mm:ss");
        }

        private static string TenVaiTro(string chucVu)
        {
            switch (chucVu)
            {
                case "Admin": return "Quản trị viên";
                case "ThuNgan": return "Thu ngân";
                case "Bep": return "Nhân viên bếp";
                default: return chucVu;
            }
        }

        private void ucTrangChu_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
                LamMoiDuLieu();
        }

        private void NapDoanhThuVaHoaDon(MetricCard theDoanhThu, MetricCard theHoaDon)
        {
            try
            {
                DateTime ngay = DateTime.Today;
                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                using (var cmd = new SqlCommand(
                    "SELECT COUNT(*) AS SoHoaDon, ISNULL(SUM(TongTien), 0) AS DoanhThu " +
                    "FROM HoaDon WHERE TrangThai = @TrangThai " +
                    "AND NgayLap >= @TuNgay AND NgayLap < @DenNgay", conn))
                {
                    cmd.Parameters.Add("@TrangThai", SqlDbType.NVarChar, 15).Value = "Đã thanh toán";
                    cmd.Parameters.Add("@TuNgay", SqlDbType.DateTime).Value = ngay;
                    cmd.Parameters.Add("@DenNgay", SqlDbType.DateTime).Value = ngay.AddDays(1);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new InvalidOperationException("Không đọc được số liệu doanh thu.");
                        int soHoaDon = Convert.ToInt32(reader["SoHoaDon"]);
                        decimal doanhThu = Convert.ToDecimal(reader["DoanhThu"]);
                        theDoanhThu.SetGiaTri(doanhThu.ToString("N0", new CultureInfo("vi-VN")) + " đ",
                            soHoaDon == 0 ? "Chưa có dữ liệu" : "Hôm nay", MauCam);
                        theHoaDon.SetGiaTri(soHoaDon.ToString(),
                            soHoaDon == 0 ? "Chưa có dữ liệu" : "Đã thanh toán", Color.FromArgb(37, 99, 235));
                    }
                }
            }
            catch (Exception ex)
            {
                theDoanhThu.SetLoi(ex);
                theHoaDon.SetLoi(ex);
            }
        }

        private void NapSoHoaDonHomNay(MetricCard the)
        {
            try
            {
                DateTime ngay = DateTime.Today;
                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                using (var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM HoaDon WHERE TrangThai = @TrangThai " +
                    "AND NgayLap >= @TuNgay AND NgayLap < @DenNgay", conn))
                {
                    cmd.Parameters.Add("@TrangThai", SqlDbType.NVarChar, 15).Value = "Đã thanh toán";
                    cmd.Parameters.Add("@TuNgay", SqlDbType.DateTime).Value = ngay;
                    cmd.Parameters.Add("@DenNgay", SqlDbType.DateTime).Value = ngay.AddDays(1);
                    conn.Open();
                    int soHoaDon = Convert.ToInt32(cmd.ExecuteScalar());
                    the.SetGiaTri(soHoaDon.ToString(),
                        soHoaDon == 0 ? "Chưa có dữ liệu" : "Đã thanh toán", Color.FromArgb(37, 99, 235));
                }
            }
            catch (Exception ex)
            {
                the.SetLoi(ex);
            }
        }

        private void NapThongKeBan(MetricCard theBanThuNgan, MetricCard theBanCoKhach, bool chiCoBanCoKhach)
        {
            try
            {
                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                using (var cmd = new SqlCommand(
                    "SELECT COUNT(*) AS TongBan, " +
                    "SUM(CASE WHEN TrangThai = @TrangThaiTrong THEN 1 ELSE 0 END) AS BanTrong, " +
                    "SUM(CASE WHEN TrangThai = @TrangThaiKhach THEN 1 ELSE 0 END) AS BanCoKhach " +
                    "FROM Ban", conn))
                {
                    cmd.Parameters.Add("@TrangThaiTrong", SqlDbType.NVarChar, 15).Value = "Trống";
                    cmd.Parameters.Add("@TrangThaiKhach", SqlDbType.NVarChar, 15).Value = "Có khách";
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new InvalidOperationException("Không đọc được số liệu bàn.");
                        int tongBan = Convert.ToInt32(reader["TongBan"]);
                        int banTrong = reader["BanTrong"] == DBNull.Value ? 0 : Convert.ToInt32(reader["BanTrong"]);
                        int banCoKhach = reader["BanCoKhach"] == DBNull.Value ? 0 : Convert.ToInt32(reader["BanCoKhach"]);
                        string ghiChu = tongBan == 0 ? "Chưa có dữ liệu" : "Tổng số bàn";
                        if (chiCoBanCoKhach)
                        {
                            theBanThuNgan.SetGiaTri(banCoKhach + " / " + tongBan, ghiChu,
                                Color.FromArgb(220, 38, 38));
                        }
                        else
                        {
                            theBanThuNgan.SetGiaTri(banTrong.ToString(),
                                tongBan == 0 ? "Chưa có dữ liệu" : "Sẵn sàng phục vụ",
                                Color.FromArgb(22, 163, 74));
                            theBanCoKhach.SetGiaTri(banCoKhach + " / " + tongBan, ghiChu,
                                Color.FromArgb(220, 38, 38));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                theBanThuNgan.SetLoi(ex);
                if (theBanCoKhach != null)
                    theBanCoKhach.SetLoi(ex);
            }
        }

        private void NapCanhBaoNguyenLieu(MetricCard the)
        {
            try
            {
                DataTable canhBao = Kho.LayCanhBao();
                int soLuong = canhBao.Rows.Count;
                the.SetGiaTri(soLuong.ToString(), soLuong == 0 ? "Kho ổn" : "Cần kiểm tra nguyên liệu",
                    soLuong == 0 ? Color.FromArgb(22, 163, 74) : Color.FromArgb(220, 38, 38));
            }
            catch (Exception ex)
            {
                the.SetLoi(ex);
            }
        }

        private void NapHoaDonDangPhucVu(MetricCard the)
        {
            try
            {
                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                using (var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM HoaDon WHERE TrangThai = @TrangThai", conn))
                {
                    cmd.Parameters.Add("@TrangThai", SqlDbType.NVarChar, 15).Value = "Đang phục vụ";
                    conn.Open();
                    int soHoaDon = Convert.ToInt32(cmd.ExecuteScalar());
                    the.SetGiaTri(soHoaDon.ToString(),
                        soHoaDon == 0 ? "Chưa có dữ liệu" : "Đang phục vụ", Color.FromArgb(245, 158, 11));
                }
            }
            catch (Exception ex)
            {
                the.SetLoi(ex);
            }
        }

        private void NapThongKeBep()
        {
            try
            {
                var soLuongTheoTrangThai = new Dictionary<string, int>(StringComparer.Ordinal);
                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                using (var cmd = new SqlCommand(
                    "SELECT ct.TrangThaiBep, COUNT(*) AS SoLuong " +
                    "FROM ChiTietHoaDon ct INNER JOIN HoaDon hd ON hd.MaHD = ct.MaHD " +
                    "WHERE hd.TrangThai = @TrangThai GROUP BY ct.TrangThaiBep", conn))
                {
                    cmd.Parameters.Add("@TrangThai", SqlDbType.NVarChar, 15).Value = "Đang phục vụ";
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            soLuongTheoTrangThai[Convert.ToString(reader["TrangThaiBep"])] =
                                Convert.ToInt32(reader["SoLuong"]);
                    }
                }

                string[] trangThai = { "Chờ", "Đang làm", "Xong" };
                for (int i = 0; i < trangThai.Length; i++)
                {
                    int soLuong;
                    if (!soLuongTheoTrangThai.TryGetValue(trangThai[i], out soLuong))
                        soLuong = 0;
                    theChiSo[i].SetGiaTri(soLuong.ToString(),
                        soLuong == 0 ? "Chưa có dữ liệu" : "Món trong hóa đơn đang phục vụ",
                        theChiSo[i].MauChuSo);
                }
            }
            catch (Exception ex)
            {
                foreach (MetricCard the in theChiSo)
                    the.SetLoi(ex);
            }
        }

        private void NapHoaDonGanDay()
        {
            try
            {
                var bang = new DataTable();
                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                using (var cmd = new SqlCommand(
                    "SELECT TOP 8 hd.MaHD, ISNULL(b.TenBan, hd.MaBan) AS TenBan, " +
                    "CONVERT(varchar(5), hd.NgayLap, 108) AS Gio, hd.TongTien, hd.TrangThai " +
                    "FROM HoaDon hd LEFT JOIN Ban b ON b.MaBan = hd.MaBan " +
                    "ORDER BY hd.NgayLap DESC, hd.MaHD DESC", conn))
                using (var adapter = new SqlDataAdapter(cmd))
                    adapter.Fill(bang);

                dgvHoaDonGanDay.DataSource = bang;
                bool coDuLieu = bang.Rows.Count > 0;
                dgvHoaDonGanDay.Visible = coDuLieu;
                lblHoaDonRong.Text = coDuLieu ? "" : "Chưa có dữ liệu";
                lblHoaDonRong.ForeColor = MauXam;
                lblHoaDonRong.Visible = !coDuLieu;
                toolTip.SetToolTip(lblHoaDonRong, "");
            }
            catch (Exception ex)
            {
                dgvHoaDonGanDay.DataSource = null;
                dgvHoaDonGanDay.Visible = false;
                lblHoaDonRong.Text = "--";
                lblHoaDonRong.ForeColor = Color.FromArgb(220, 38, 38);
                lblHoaDonRong.Visible = true;
                toolTip.SetToolTip(lblHoaDonRong, ex.Message);
            }
        }

        private void NapTopMonBanChay()
        {
            try
            {
                DateTime denNgay = DateTime.Today.AddDays(1);
                DateTime tuNgay = DateTime.Today.AddDays(-6);
                var monBanChay = new List<TopMon>();
                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                using (var cmd = new SqlCommand(
                    "SELECT TOP 5 m.TenMon, SUM(ct.SoLuong) AS SoLuong " +
                    "FROM ChiTietHoaDon ct " +
                    "INNER JOIN HoaDon hd ON hd.MaHD = ct.MaHD " +
                    "INNER JOIN MonAn m ON m.MaMon = ct.MaMon " +
                    "WHERE hd.TrangThai = @TrangThai AND hd.NgayLap >= @TuNgay AND hd.NgayLap < @DenNgay " +
                    "GROUP BY m.MaMon, m.TenMon " +
                    "ORDER BY SUM(ct.SoLuong) DESC, m.TenMon ASC", conn))
                {
                    cmd.Parameters.Add("@TrangThai", SqlDbType.NVarChar, 15).Value = "Đã thanh toán";
                    cmd.Parameters.Add("@TuNgay", SqlDbType.DateTime).Value = tuNgay;
                    cmd.Parameters.Add("@DenNgay", SqlDbType.DateTime).Value = denNgay;
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            monBanChay.Add(new TopMon
                            {
                                TenMon = Convert.ToString(reader["TenMon"]),
                                SoLuong = Convert.ToInt32(reader["SoLuong"])
                            });
                    }
                }
                pnlTopMon.DatDuLieu(monBanChay, false);
            }
            catch (Exception ex)
            {
                pnlTopMon.DatDuLieu(null, true);
                toolTip.SetToolTip(pnlTopMon, ex.Message);
            }
        }

        private static RoundedPath TaoDuongVienBoGoc(Rectangle hinh, int banKinh)
        {
            return new RoundedPath(hinh, banKinh);
        }

        private sealed class RoundedPath : IDisposable
        {
            public GraphicsPath Path { get; private set; }

            public RoundedPath(Rectangle hinh, int banKinh)
            {
                int duongKinh = banKinh * 2;
                Path = new GraphicsPath();
                if (hinh.Width <= 0 || hinh.Height <= 0)
                    return;
                Path.AddArc(hinh.X, hinh.Y, duongKinh, duongKinh, 180, 90);
                Path.AddArc(hinh.Right - duongKinh, hinh.Y, duongKinh, duongKinh, 270, 90);
                Path.AddArc(hinh.Right - duongKinh, hinh.Bottom - duongKinh, duongKinh, duongKinh, 0, 90);
                Path.AddArc(hinh.X, hinh.Bottom - duongKinh, duongKinh, duongKinh, 90, 90);
                Path.CloseFigure();
            }

            public void Dispose()
            {
                if (Path != null)
                    Path.Dispose();
            }
        }

        private sealed class BannerPanel : Panel
        {
            private readonly Color mauTrai = Color.FromArgb(232, 101, 26);
            private readonly Color mauPhai = Color.FromArgb(245, 158, 11);

            public BannerPanel()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            }

            protected override void OnResize(EventArgs eventargs)
            {
                base.OnResize(eventargs);
                using (var hinh = TaoDuongVienBoGoc(new Rectangle(0, 0, Width, Height), 12))
                {
                    Region oldRegion = Region;
                    Region = new Region(hinh.Path);
                    if (oldRegion != null)
                        oldRegion.Dispose();
                }
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var hinh = TaoDuongVienBoGoc(new Rectangle(0, 0, Width - 1, Height - 1), 12))
                using (var brush = new LinearGradientBrush(ClientRectangle, mauTrai, mauPhai, LinearGradientMode.Horizontal))
                    e.Graphics.FillPath(brush, hinh.Path);

                using (var brush = new SolidBrush(Color.FromArgb(48, Color.White)))
                {
                    e.Graphics.FillEllipse(brush, Width - 236, -32, 96, 96);
                    e.Graphics.FillEllipse(brush, Width - 170, 72, 64, 64);
                    e.Graphics.FillEllipse(brush, Width - 74, -22, 88, 88);
                }
                using (var font = new Font("Segoe UI Emoji", 54F))
                using (var brush = new SolidBrush(Color.FromArgb(110, Color.White)))
                    e.Graphics.DrawString("🍜", font, brush, Width - 112, 23);
            }
        }

        private class RoundedPanel : Panel
        {
            public RoundedPanel()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            }

            protected override void OnResize(EventArgs eventargs)
            {
                base.OnResize(eventargs);
                using (var hinh = TaoDuongVienBoGoc(new Rectangle(0, 0, Width, Height), 10))
                {
                    Region oldRegion = Region;
                    Region = new Region(hinh.Path);
                    if (oldRegion != null)
                        oldRegion.Dispose();
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var hinh = TaoDuongVienBoGoc(new Rectangle(0, 0, Width - 1, Height - 1), 10))
                using (var pen = new Pen(Color.FromArgb(229, 231, 235), 1F))
                    e.Graphics.DrawPath(pen, hinh.Path);
            }
        }

        private sealed class IconCircle : Control
        {
            private readonly string bieuTuong;
            private readonly Color mauNenIcon;

            public IconCircle(string bieuTuong, Color mauNenIcon)
            {
                this.bieuTuong = bieuTuong;
                this.mauNenIcon = mauNenIcon;
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw | ControlStyles.UserPaint |
                    ControlStyles.SupportsTransparentBackColor, true);
                BackColor = Color.Transparent;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brushNen = new SolidBrush(mauNenIcon))
                    e.Graphics.FillEllipse(brushNen, 0, 0, Width - 1, Height - 1);
                using (var font = new Font("Segoe UI Emoji", 19F))
                using (var brush = new SolidBrush(Color.FromArgb(55, 65, 81)))
                using (var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                })
                    e.Graphics.DrawString(bieuTuong, font, brush, ClientRectangle, format);
            }
        }

        private sealed class MetricCard : RoundedPanel
        {
            private readonly Label lblGiaTri = new Label();
            private readonly Label lblNhan = new Label();
            private readonly Label lblPhu = new Label();
            private readonly ToolTip toolTip;
            public Color MauChuSo { get; private set; }
            private readonly Color mauThanh;

            public MetricCard(ToolTip toolTip, string bieuTuong, string nhan, Color mauChuSo, Color mauNenIcon)
            {
                this.toolTip = toolTip;
                MauChuSo = mauChuSo;
                mauThanh = mauChuSo;
                BackColor = Color.White;
                var icon = new IconCircle(bieuTuong, mauNenIcon)
                {
                    Location = new Point(18, 32),
                    Size = new Size(44, 44),
                    Anchor = AnchorStyles.Left
                };
                lblGiaTri.Location = new Point(76, 13);
                lblGiaTri.Size = new Size(175, 34);
                lblGiaTri.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                lblGiaTri.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
                lblGiaTri.ForeColor = Color.FromArgb(17, 24, 39);
                lblGiaTri.AutoEllipsis = true;
                lblNhan.Location = new Point(77, 48);
                lblNhan.Size = new Size(175, 18);
                lblNhan.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                lblNhan.Text = nhan;
                lblNhan.Font = new Font("Segoe UI", 9F);
                lblNhan.ForeColor = MauXam;
                lblNhan.AutoEllipsis = true;
                lblPhu.Location = new Point(77, 72);
                lblPhu.Size = new Size(175, 18);
                lblPhu.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                lblPhu.Font = new Font("Segoe UI", 8.5F);
                lblPhu.ForeColor = MauXam;
                lblPhu.AutoEllipsis = true;
                Controls.Add(icon);
                Controls.Add(lblGiaTri);
                Controls.Add(lblNhan);
                Controls.Add(lblPhu);
            }

            public void SetGiaTri(string giaTri, string phu, Color mau)
            {
                lblGiaTri.Text = giaTri;
                lblGiaTri.ForeColor = mau;
                lblPhu.Text = phu;
                lblPhu.ForeColor = MauXam;
                toolTip.SetToolTip(lblGiaTri, "");
            }

            public void SetLoi(Exception ex)
            {
                lblGiaTri.Text = "--";
                lblGiaTri.ForeColor = Color.FromArgb(220, 38, 38);
                lblPhu.Text = "Không tải được dữ liệu";
                lblPhu.ForeColor = MauXam;
                toolTip.SetToolTip(lblGiaTri, ex.Message);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(mauThanh))
                    e.Graphics.FillRectangle(brush, 0, 10, 4, Height - 20);
            }

            protected override void OnSizeChanged(EventArgs e)
            {
                base.OnSizeChanged(e);
                int rong = Math.Max(40, Width - 92);
                lblGiaTri.Width = rong;
                lblNhan.Width = rong;
                lblPhu.Width = rong;
            }
        }

        private sealed class QuickActionButton : Button
        {
            private readonly string bieuTuong;
            private bool dangHover;
            private bool dangNhan;

            public QuickActionButton(string bieuTuong, string noiDung)
            {
                this.bieuTuong = bieuTuong;
                Text = noiDung;
                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 0;
                BackColor = Color.FromArgb(255, 243, 234);
                ForeColor = Color.FromArgb(55, 65, 81);
                Cursor = Cursors.Hand;
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                dangHover = true;
                Invalidate();
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                dangHover = false;
                dangNhan = false;
                Invalidate();
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);
                dangNhan = true;
                Invalidate();
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                base.OnMouseUp(e);
                dangNhan = false;
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Color nen = dangNhan ? Color.FromArgb(255, 228, 204) :
                    (dangHover ? Color.FromArgb(255, 228, 204) : Color.FromArgb(255, 243, 234));
                using (var hinh = TaoDuongVienBoGoc(new Rectangle(0, 0, Width - 1, Height - 1), 8))
                using (var brush = new SolidBrush(nen))
                using (var pen = new Pen(Color.FromArgb(253, 186, 116), 1F))
                {
                    e.Graphics.FillPath(brush, hinh.Path);
                    e.Graphics.DrawPath(pen, hinh.Path);
                }
                using (var fontIcon = new Font("Segoe UI Emoji", 18F))
                using (var brushIcon = new SolidBrush(ForeColor))
                using (var formatIcon = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                })
                    e.Graphics.DrawString(bieuTuong, fontIcon, brushIcon,
                        new RectangleF(4, 3, Width - 8, Math.Max(20, Height / 2 - 2)), formatIcon);
                using (var fontText = new Font("Segoe UI", 9.5F))
                using (var brushText = new SolidBrush(ForeColor))
                {
                    TextRenderer.DrawText(e.Graphics, Text, fontText,
                        new Rectangle(5, Height / 2, Width - 10, Math.Max(18, Height / 2 - 4)),
                        ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                        TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis);
                }
            }
        }

        private sealed class TopMonPanel : Panel
        {
            private readonly List<TopMon> dsMon = new List<TopMon>();
            private bool coLoi;

            public TopMonPanel()
            {
                BackColor = Color.White;
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            }

            public void DatDuLieu(List<TopMon> duLieu, bool coLoi)
            {
                dsMon.Clear();
                if (duLieu != null)
                    dsMon.AddRange(duLieu);
                this.coLoi = coLoi;
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                if (coLoi)
                {
                    TextRenderer.DrawText(e.Graphics, "--", Font, ClientRectangle, MauXam,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                    return;
                }
                if (dsMon.Count == 0)
                {
                    TextRenderer.DrawText(e.Graphics, "Chưa có dữ liệu", Font, ClientRectangle, MauXam,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                    return;
                }

                int le = 4;
                int caoDong = Math.Max(28, Math.Min(34, (Height - 4) / 5));
                int rongTen = Math.Min(130, Math.Max(75, Width / 3));
                int rongSo = 38;
                int xThanh = le + 24 + 8 + rongTen + 8;
                int rongThanh = Math.Max(24, Width - xThanh - rongSo - le - 6);
                int lonNhat = 1;
                foreach (TopMon mon in dsMon)
                    lonNhat = Math.Max(lonNhat, mon.SoLuong);

                using (var fontHang = new Font("Segoe UI", 10F))
                using (var fontSo = new Font("Segoe UI", 9F, FontStyle.Bold))
                using (var fontThuHang = new Font("Segoe UI", 8F, FontStyle.Bold))
                using (var brushCam = new SolidBrush(MauCam))
                using (var brushTrang = new SolidBrush(Color.White))
                using (var brushChu = new SolidBrush(Color.FromArgb(31, 41, 55)))
                using (var brushNenThanh = new SolidBrush(Color.FromArgb(255, 228, 204)))
                using (var brushThanh = new SolidBrush(MauCam))
                using (var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                })
                {
                    for (int i = 0; i < dsMon.Count; i++)
                    {
                        int y = i * caoDong;
                        Rectangle hinhHang = new Rectangle(le, y + (caoDong - 22) / 2, 22, 22);
                        e.Graphics.FillEllipse(brushCam, hinhHang);
                        e.Graphics.DrawString((i + 1).ToString(), fontThuHang, brushTrang, hinhHang, format);

                        Rectangle vungTen = new Rectangle(le + 30, y, rongTen, caoDong);
                        TextRenderer.DrawText(e.Graphics, dsMon[i].TenMon, fontHang, vungTen,
                            brushChu.Color, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis |
                            TextFormatFlags.NoPadding);

                        Rectangle vungThanh = new Rectangle(xThanh, y + (caoDong - 8) / 2, rongThanh, 8);
                        e.Graphics.FillRectangle(brushNenThanh, vungThanh);
                        int rongDaBan = (int)Math.Round(rongThanh * (double)dsMon[i].SoLuong / lonNhat);
                        if (rongDaBan > 0)
                            e.Graphics.FillRectangle(brushThanh,
                                new Rectangle(vungThanh.X, vungThanh.Y, Math.Max(2, rongDaBan), vungThanh.Height));

                        Rectangle vungSo = new Rectangle(Width - rongSo - le, y, rongSo, caoDong);
                        TextRenderer.DrawText(e.Graphics, dsMon[i].SoLuong.ToString(), fontSo, vungSo,
                            Color.FromArgb(17, 24, 39), TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
                    }
                }
            }
        }

        private sealed class QuickActionInfo
        {
            public string BieuTuong { get; private set; }
            public string TenHienThi { get; private set; }
            public string TenMenu { get; private set; }

            public QuickActionInfo(string bieuTuong, string tenHienThi, string tenMenu)
            {
                BieuTuong = bieuTuong;
                TenHienThi = tenHienThi;
                TenMenu = tenMenu;
            }
        }

        private sealed class TopMon
        {
            public string TenMon { get; set; }
            public int SoLuong { get; set; }
        }
    }
}
