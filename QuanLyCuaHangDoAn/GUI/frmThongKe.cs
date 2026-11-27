using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using QuanLyCuaHangDoAn.Class;

namespace QuanLyCuaHangDoAn.GUI
{
    public class frmThongKe : Form
    {
        private readonly DateTimePicker dtpTuNgay = new DateTimePicker();
        private readonly DateTimePicker dtpDenNgay = new DateTimePicker();
        private readonly Button btnThongKe = GiaoDien.TaoNut("Xem thống kê", GiaoDien.MauChinh);
        private readonly Button btnHomNay = GiaoDien.TaoNut("Hôm nay", Color.FromArgb(70, 130, 180));
        private readonly Button btnBayNgay = GiaoDien.TaoNut("7 ngày qua", Color.FromArgb(70, 130, 180));
        private readonly Button btnThangNay = GiaoDien.TaoNut("Tháng này", Color.FromArgb(70, 130, 180));

        private readonly Label lblTongDoanhThu = new Label();
        private readonly Label lblSoHoaDon = new Label();
        private readonly Label lblDoanhThuTB = new Label();

        private readonly DataGridView dgvDoanhThuTheoNgay = new DataGridView();
        private readonly DataGridView dgvTopMon = new DataGridView();
        private readonly Chart chartTopMon = new Chart();
        private Label lblKhongDuLieu;
        private TopMonPanel pnlChiTietTop5;

        private readonly CultureInfo viVn = new CultureInfo("vi-VN");

        public frmThongKe()
        {
            KhoiTaoGiaoDien();
            Load += frmThongKe_Load;
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Thống kê doanh thu và món bán chạy";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(1280, 720);
            BackColor = Color.FromArgb(245, 246, 248);
            Font = new Font("Segoe UI", 10F);
            DoubleBuffered = true;

            var pnlHeader = new Panel
            {
                Name = "pnlHeader",
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(232, 101, 26)
            };
            pnlHeader.Controls.Add(new Label
            {
                Name = "lblTieuDe",
                Text = "THỐNG KÊ",
                Location = new Point(30, 10),
                Size = new Size(500, 27),
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false
            });
            pnlHeader.Controls.Add(new Label
            {
                Name = "lblPhuDe",
                Text = "Doanh thu và món bán chạy",
                Location = new Point(32, 42),
                Size = new Size(500, 18),
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.White,
                AutoSize = false
            });

            var pnlBoLoc = new BorderPanel
            {
                Name = "pnlBoLoc",
                Location = new Point(20, 85),
                Size = new Size(1240, 64),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.White
            };
            pnlBoLoc.Controls.Add(TaoNhanBoLoc("Từ ngày", "lblTuNgay", new Point(15, 24), 42));
            dtpTuNgay.Name = "dtpTuNgay";
            dtpTuNgay.Format = DateTimePickerFormat.Custom;
            dtpTuNgay.CustomFormat = "dd/MM/yyyy";
            dtpTuNgay.Size = new Size(140, 32);
            dtpTuNgay.Location = new Point(62, 16);
            dtpTuNgay.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            pnlBoLoc.Controls.Add(dtpTuNgay);

            pnlBoLoc.Controls.Add(TaoNhanBoLoc("Đến ngày", "lblDenNgay", new Point(222, 24), 50));
            dtpDenNgay.Name = "dtpDenNgay";
            dtpDenNgay.Format = DateTimePickerFormat.Custom;
            dtpDenNgay.CustomFormat = "dd/MM/yyyy";
            dtpDenNgay.Size = new Size(140, 32);
            dtpDenNgay.Location = new Point(278, 16);
            dtpDenNgay.Value = DateTime.Today;
            pnlBoLoc.Controls.Add(dtpDenNgay);

            CauHinhNutChinh(btnThongKe);
            btnThongKe.Name = "btnThongKe";
            btnThongKe.Text = "Xem thống kê";
            btnThongKe.Size = new Size(140, 36);
            btnThongKe.Location = new Point(450, 14);
            btnThongKe.Click += btnThongKe_Click;
            pnlBoLoc.Controls.Add(btnThongKe);

            CauHinhNutLoc(btnHomNay, "btnHomNay", "Hôm nay", new Point(614, 14));
            CauHinhNutLoc(btnBayNgay, "btnBayNgay", "7 ngày qua", new Point(732, 14));
            CauHinhNutLoc(btnThangNay, "btnThangNay", "Tháng này", new Point(850, 14));
            btnHomNay.Click += (s, e) =>
            {
                dtpTuNgay.Value = DateTime.Today;
                dtpDenNgay.Value = DateTime.Today;
                TaiDuLieuThongKe();
            };
            btnBayNgay.Click += (s, e) =>
            {
                dtpTuNgay.Value = DateTime.Today.AddDays(-6);
                dtpDenNgay.Value = DateTime.Today;
                TaiDuLieuThongKe();
            };
            btnThangNay.Click += (s, e) =>
            {
                dtpTuNgay.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                dtpDenNgay.Value = DateTime.Today;
                TaiDuLieuThongKe();
            };
            pnlBoLoc.Controls.Add(btnHomNay);
            pnlBoLoc.Controls.Add(btnBayNgay);
            pnlBoLoc.Controls.Add(btnThangNay);

            Controls.Add(pnlHeader);
            Controls.Add(pnlBoLoc);
            pnlHeader.BringToFront();

            var tblThe = new TableLayoutPanel
            {
                Name = "tlpChiSo",
                Location = new Point(20, 164),
                Size = new Size(1240, 100),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ColumnCount = 5,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            tblThe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            tblThe.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 15F));
            tblThe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            tblThe.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 15F));
            tblThe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.334F));
            tblThe.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tblThe.Controls.Add(TaoTheChiSo("pnlTongDoanhThu", "TỔNG DOANH THU", lblTongDoanhThu,
                Color.FromArgb(232, 101, 26)), 0, 0);
            tblThe.Controls.Add(TaoTheChiSo("pnlSoHoaDon", "SỐ HÓA ĐƠN ĐÃ THANH TOÁN", lblSoHoaDon,
                Color.FromArgb(37, 99, 235)), 2, 0);
            tblThe.Controls.Add(TaoTheChiSo("pnlDoanhThuTrungBinh", "DOANH THU TRUNG BÌNH / HÓA ĐƠN", lblDoanhThuTB,
                Color.FromArgb(22, 163, 74)), 4, 0);
            Controls.Add(tblThe);

            var pnlTopMon = new BorderPanel
            {
                Name = "pnlTopMon",
                Location = new Point(20, 279),
                Size = new Size(740, 421),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackColor = Color.White
            };
            pnlTopMon.Controls.Add(TaoNhan("TOP 5 MÓN BÁN CHẠY (THEO SỐ LƯỢNG)",
                "lblTieuDeTopMon", new Point(15, 12), new Size(710, 20)));
            chartTopMon.Name = "chartTopMon";
            chartTopMon.Location = new Point(15, 45);
            chartTopMon.Size = new Size(710, 360);
            chartTopMon.Anchor = AnchorStyles.Top | AnchorStyles.Left |
                AnchorStyles.Right | AnchorStyles.Bottom;
            chartTopMon.BackColor = Color.White;
            pnlTopMon.Controls.Add(chartTopMon);
            lblKhongDuLieu = new Label
            {
                Name = "lblKhongDuLieu",
                Text = "Không có dữ liệu trong khoảng này",
                Location = new Point(15, 45),
                Size = new Size(710, 360),
                Anchor = AnchorStyles.Top | AnchorStyles.Left |
                    AnchorStyles.Right | AnchorStyles.Bottom,
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(156, 163, 175),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };
            pnlTopMon.Controls.Add(lblKhongDuLieu);

            var pnlChiTiet = new BorderPanel
            {
                Name = "pnlChiTietTop5",
                Location = new Point(775, 279),
                Size = new Size(485, 421),
                Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom,
                BackColor = Color.White
            };
            pnlChiTiet.Controls.Add(TaoNhan("CHI TIẾT TOP 5", "lblTieuDeChiTietTop5",
                new Point(15, 12), new Size(455, 20)));
            pnlChiTietTop5 = new TopMonPanel
            {
                Name = "pnlDanhSachTop5",
                Location = new Point(15, 45),
                Size = new Size(455, 280),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            pnlChiTiet.Controls.Add(pnlChiTietTop5);

            dgvDoanhThuTheoNgay.Visible = false;
            dgvTopMon.Visible = false;
            Controls.Add(pnlTopMon);
            Controls.Add(pnlChiTiet);
        }

        private Panel TaoTheChiSo(string ten, string tieuDe, Label lblGiaTri, Color mauChu)
        {
            var card = new BorderPanel
            {
                Name = ten,
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = Padding.Empty
            };
            card.Controls.Add(new Panel
            {
                Name = "pnlThanhMau",
                Location = new Point(0, 0),
                Size = new Size(4, 100),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                BackColor = mauChu
            });
            card.Controls.Add(new Label
            {
                Name = "lblNhan" + ten.Substring(3),
                Text = tieuDe,
                Location = new Point(20, 14),
                Size = new Size(360, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(107, 114, 128),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false
            });
            lblGiaTri.Text = "0";
            lblGiaTri.Location = new Point(20, 39);
            lblGiaTri.Size = new Size(365, 48);
            lblGiaTri.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblGiaTri.ForeColor = mauChu;
            lblGiaTri.TextAlign = ContentAlignment.MiddleLeft;
            lblGiaTri.AutoSize = false;

            card.Controls.Add(lblGiaTri);
            return card;
        }

        private static Label TaoNhanBoLoc(string chu, string ten, Point viTri, int rong)
        {
            return new Label
            {
                Name = ten,
                Text = chu,
                Location = viTri,
                Size = new Size(rong, 16),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(107, 114, 128),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false
            };
        }

        private static Label TaoNhan(string chu, string ten, Point viTri, Size kichThuoc)
        {
            return new Label
            {
                Name = ten,
                Text = chu,
                Location = viTri,
                Size = kichThuoc,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(232, 101, 26),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false
            };
        }

        private static void CauHinhNutChinh(Button nut)
        {
            nut.FlatStyle = FlatStyle.Flat;
            nut.FlatAppearance.BorderSize = 0;
            nut.FlatAppearance.MouseOverBackColor = Color.FromArgb(207, 85, 16);
            nut.FlatAppearance.MouseDownBackColor = Color.FromArgb(190, 75, 12);
            nut.UseVisualStyleBackColor = false;
            nut.BackColor = Color.FromArgb(232, 101, 26);
            nut.ForeColor = Color.White;
            nut.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            nut.Cursor = Cursors.Hand;
        }

        private static void CauHinhNutLoc(Button nut, string ten, string chu, Point viTri)
        {
            nut.Name = ten;
            nut.Text = chu;
            nut.Location = viTri;
            nut.Size = new Size(110, 36);
            nut.FlatStyle = FlatStyle.Flat;
            nut.FlatAppearance.BorderSize = 1;
            nut.FlatAppearance.BorderColor = Color.FromArgb(232, 101, 26);
            nut.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 243, 234);
            nut.FlatAppearance.MouseDownBackColor = Color.FromArgb(255, 243, 234);
            nut.UseVisualStyleBackColor = false;
            nut.BackColor = Color.White;
            nut.ForeColor = Color.FromArgb(232, 101, 26);
            nut.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            nut.Cursor = Cursors.Hand;
        }

        private void frmThongKe_Load(object sender, EventArgs e)
        {
            if (!KiemTraQuyen())
            {
                Close();
                return;
            }

            TaiDuLieuThongKe();
        }

        private bool KiemTraQuyen()
        {
            if (!PhienDangNhap.IsLoggedIn)
            {
                MessageBox.Show("Vui lòng đăng nhập trước khi xem thống kê.",
                    "Chưa đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!string.Equals(PhienDangNhap.ChucVu, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Chức năng này chỉ dành cho Quản trị viên (Admin).",
                    "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            if (!KiemTraQuyen())
                return;

            TaiDuLieuThongKe();
        }

        private void TaiDuLieuThongKe()
        {
            DateTime tuNgay = dtpTuNgay.Value.Date;
            DateTime denNgay = dtpDenNgay.Value.Date;

            if (tuNgay > denNgay)
            {
                MessageBox.Show("Từ ngày không được lớn hơn đến ngày.", "Khoảng ngày không hợp lệ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Nhớ lấy đến hết ngày cuối (< ngày cuối + 1)
            DateTime denNgayHetGio = denNgay.AddDays(1);

            try
            {
                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                {
                    conn.Open();

                    // 1. Tổng quan doanh thu và số hóa đơn ("Đã thanh toán")
                    TaiTongQuan(conn, tuNgay, denNgayHetGio);

                    // 2. Bảng doanh thu theo ngày
                    TaiDoanhThuTheoNgay(conn, tuNgay, denNgayHetGio);

                    // 3. Top 5 món bán chạy & biểu đồ cột
                    TaiTop5Mon(conn, tuNgay, denNgayHetGio);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu thống kê: " + ex.Message, "Lỗi thống kê",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TaiTongQuan(SqlConnection conn, DateTime tuNgay, DateTime denNgayHetGio)
        {
            string sqlTongQuan = @"
                SELECT 
                    ISNULL(COUNT(MaHD), 0) AS SoHoaDon,
                    ISNULL(SUM(TongTien), 0) AS TongDoanhThu
                FROM HoaDon
                WHERE TrangThai = N'Đã thanh toán'
                  AND NgayLap >= @TuNgay AND NgayLap < @DenNgay";

            using (var cmd = new SqlCommand(sqlTongQuan, conn))
            {
                cmd.Parameters.Add("@TuNgay", SqlDbType.DateTime).Value = tuNgay;
                cmd.Parameters.Add("@DenNgay", SqlDbType.DateTime).Value = denNgayHetGio;

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int soHD = reader["SoHoaDon"] != DBNull.Value ? Convert.ToInt32(reader["SoHoaDon"]) : 0;
                        decimal tongTien = reader["TongDoanhThu"] != DBNull.Value ? Convert.ToDecimal(reader["TongDoanhThu"]) : 0m;
                        decimal trungBinh = soHD > 0 ? (tongTien / soHD) : 0m;

                        lblTongDoanhThu.Text = string.Format(viVn, "{0:#,##0} đ", tongTien);
                        lblSoHoaDon.Text = string.Format(viVn, "{0:#,##0} hóa đơn", soHD);
                        lblDoanhThuTB.Text = string.Format(viVn, "{0:#,##0} đ", trungBinh);
                    }
                }
            }
        }

        private void TaiDoanhThuTheoNgay(SqlConnection conn, DateTime tuNgay, DateTime denNgayHetGio)
        {
            string sqlDoanhThu = @"
                SELECT 
                    CAST(NgayLap AS DATE) AS Ngay,
                    COUNT(MaHD) AS SoHoaDon,
                    SUM(TongTien) AS DoanhThu
                FROM HoaDon
                WHERE TrangThai = N'Đã thanh toán'
                  AND NgayLap >= @TuNgay AND NgayLap < @DenNgay
                GROUP BY CAST(NgayLap AS DATE)
                ORDER BY Ngay ASC";

            using (var cmd = new SqlCommand(sqlDoanhThu, conn))
            {
                cmd.Parameters.Add("@TuNgay", SqlDbType.DateTime).Value = tuNgay;
                cmd.Parameters.Add("@DenNgay", SqlDbType.DateTime).Value = denNgayHetGio;

                using (var da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvDoanhThuTheoNgay.DataSource = dt;
                    GiaoDien.LamDepBang(dgvDoanhThuTheoNgay, new Dictionary<string, string>
                    {
                        { "Ngay", "Ngày" },
                        { "SoHoaDon", "Số hóa đơn" },
                        { "DoanhThu", "Doanh thu" }
                    });

                    if (dgvDoanhThuTheoNgay.Columns.Contains("Ngay"))
                    {
                        dgvDoanhThuTheoNgay.Columns["Ngay"].DefaultCellStyle.Format = "dd/MM/yyyy";
                        dgvDoanhThuTheoNgay.Columns["Ngay"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                    if (dgvDoanhThuTheoNgay.Columns.Contains("SoHoaDon"))
                    {
                        dgvDoanhThuTheoNgay.Columns["SoHoaDon"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                    GiaoDien.DinhDangTien(dgvDoanhThuTheoNgay, "DoanhThu");
                }
            }
        }

        private void TaiTop5Mon(SqlConnection conn, DateTime tuNgay, DateTime denNgayHetGio)
        {
            string sqlTop5 = @"
                SELECT TOP 5
                    m.TenMon,
                    SUM(ct.SoLuong) AS TongSoLuong,
                    SUM(ct.SoLuong * ct.DonGia) AS DoanhThu
                FROM ChiTietHoaDon ct
                INNER JOIN HoaDon hd ON ct.MaHD = hd.MaHD
                INNER JOIN MonAn m ON ct.MaMon = m.MaMon
                WHERE hd.TrangThai = N'Đã thanh toán'
                  AND hd.NgayLap >= @TuNgay AND hd.NgayLap < @DenNgay
                GROUP BY m.MaMon, m.TenMon
                ORDER BY TongSoLuong DESC, DoanhThu DESC";

            using (var cmd = new SqlCommand(sqlTop5, conn))
            {
                cmd.Parameters.Add("@TuNgay", SqlDbType.DateTime).Value = tuNgay;
                cmd.Parameters.Add("@DenNgay", SqlDbType.DateTime).Value = denNgayHetGio;

                using (var da = new SqlDataAdapter(cmd))
                {
                    DataTable dtTop = new DataTable();
                    da.Fill(dtTop);

                    dgvTopMon.DataSource = dtTop;
                    GiaoDien.LamDepBang(dgvTopMon, new Dictionary<string, string>
                    {
                        { "TenMon", "Tên món" },
                        { "TongSoLuong", "Tổng số lượng" },
                        { "DoanhThu", "Doanh thu" }
                    });

                    if (dgvTopMon.Columns.Contains("TongSoLuong"))
                    {
                        dgvTopMon.Columns["TongSoLuong"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                    GiaoDien.DinhDangTien(dgvTopMon, "DoanhThu");

                    // Cập nhật biểu đồ cột
                    VeBieuDoTopMon(dtTop);
                }
            }
        }

        private void VeBieuDoTopMon(DataTable dtTop)
        {
            chartTopMon.Series.Clear();
            chartTopMon.ChartAreas.Clear();
            chartTopMon.Titles.Clear();
            pnlChiTietTop5.DatDuLieu(dtTop);

            if (dtTop.Rows.Count == 0)
            {
                chartTopMon.Visible = false;
                lblKhongDuLieu.Visible = true;
                return;
            }

            var chartArea = new ChartArea("AreaTopMon")
            {
                BackColor = Color.White
            };
            chartArea.Position = new ElementPosition(0, 0, 100, 100);
            chartArea.InnerPlotPosition = new ElementPosition(10, 5, 86, 84);
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(243, 244, 246);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(243, 244, 246);
            chartArea.AxisX.LabelStyle.Font = new Font("Segoe UI", 9F);
            chartArea.AxisY.LabelStyle.Font = new Font("Segoe UI", 9F);
            chartArea.AxisX.Interval = 1;
            chartArea.AxisX.IsLabelAutoFit = true;
            chartArea.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.LabelsAngleStep45;
            chartTopMon.ChartAreas.Add(chartArea);

            var series = new Series("Số lượng bán")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.FromArgb(232, 101, 26),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                LabelForeColor = Color.FromArgb(55, 65, 81),
                IsValueShownAsLabel = true
            };
            series["PointWidth"] = "0.65";
            foreach (DataRow row in dtTop.Rows)
            {
                string tenMon = Convert.ToString(row["TenMon"]);
                decimal soLuong = Convert.ToDecimal(row["TongSoLuong"]);
                series.Points.AddXY(tenMon, soLuong);
            }

            chartTopMon.Series.Add(series);
            chartTopMon.Visible = true;
            lblKhongDuLieu.Visible = false;
        }

        private sealed class BorderPanel : Panel
        {
            public BorderPanel()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw, true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                using (Pen vien = new Pen(Color.FromArgb(229, 231, 235)))
                    e.Graphics.DrawRectangle(vien, 0, 0, Width - 1, Height - 1);
            }
        }

        private sealed class TopMonPanel : Panel
        {
            private DataTable duLieu;

            public TopMonPanel()
            {
                BackColor = Color.White;
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            }

            public void DatDuLieu(DataTable bang)
            {
                duLieu = bang;
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                if (duLieu == null)
                    return;

                CultureInfo viVn = CultureInfo.GetCultureInfo("vi-VN");
                using (Font fontTen = new Font("Segoe UI", 10.5F, FontStyle.Bold))
                using (Font fontSoLuong = new Font("Segoe UI", 10F, FontStyle.Bold))
                using (Font fontDoanhThu = new Font("Segoe UI", 9F))
                using (Font fontHang = new Font("Segoe UI", 10F, FontStyle.Bold))
                using (SolidBrush mauCam = new SolidBrush(Color.FromArgb(232, 101, 26)))
                using (SolidBrush mauTrang = new SolidBrush(Color.White))
                using (Pen duongKe = new Pen(Color.FromArgb(243, 244, 246)))
                {
                    int soDong = Math.Min(5, duLieu.Rows.Count);
                    for (int i = 0; i < soDong; i++)
                    {
                        DataRow dong = duLieu.Rows[i];
                        int y = i * 56;
                        Rectangle huyHieu = new Rectangle(0, y + 12, 32, 32);
                        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        e.Graphics.FillEllipse(mauCam, huyHieu);
                        TextRenderer.DrawText(e.Graphics, (i + 1).ToString(), fontHang, huyHieu,
                            Color.White, TextFormatFlags.HorizontalCenter |
                            TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

                        Rectangle vungTen = new Rectangle(44, y + 5, Math.Max(0, Width - 44 - 185 - 8), 22);
                        TextRenderer.DrawText(e.Graphics, Convert.ToString(dong["TenMon"]), fontTen,
                            vungTen, Color.FromArgb(17, 24, 39), TextFormatFlags.Left |
                            TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine |
                            TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

                        decimal soLuong = Convert.ToDecimal(dong["TongSoLuong"]);
                        Rectangle vungSoLuong = new Rectangle(Width - 185, y + 3, 185, 22);
                        TextRenderer.DrawText(e.Graphics,
                            soLuong.ToString("#,##0.###", viVn) + " phần",
                            fontSoLuong, vungSoLuong, Color.FromArgb(17, 24, 39),
                            TextFormatFlags.Right | TextFormatFlags.VerticalCenter |
                            TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);

                        if (duLieu.Columns.Contains("DoanhThu") && dong["DoanhThu"] != DBNull.Value)
                        {
                            decimal doanhThu = Convert.ToDecimal(dong["DoanhThu"]);
                            Rectangle vungDoanhThu = new Rectangle(Width - 185, y + 25, 185, 18);
                            TextRenderer.DrawText(e.Graphics,
                                doanhThu.ToString("#,##0", viVn) + " đ",
                                fontDoanhThu, vungDoanhThu, Color.FromArgb(107, 114, 128),
                                TextFormatFlags.Right | TextFormatFlags.VerticalCenter |
                                TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
                        }

                        if (i < soDong - 1)
                            e.Graphics.DrawLine(duongKe, 0, y + 55, Width, y + 55);
                    }
                }
            }
        }
    }
}
