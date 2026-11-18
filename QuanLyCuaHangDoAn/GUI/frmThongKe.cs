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

        private readonly CultureInfo viVn = new CultureInfo("vi-VN");

        public frmThongKe()
        {
            KhoiTaoGiaoDien();
            Load += frmThongKe_Load;
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Thống kê doanh thu và món bán chạy";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1280, 780);
            MinimumSize = new Size(1050, 650);
            BackColor = Color.White;
            Font = GiaoDien.FontThuong;

            // Tiêu đề form
            Label lblTieuDe = GiaoDien.TaoTieuDe("THỐNG KÊ DOANH THU & TOP 5 MÓN BÁN CHẠY");
            Controls.Add(lblTieuDe);

            // Khung chọn thời gian (Bộ lọc)
            Panel pnlBoLoc = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = GiaoDien.MauNenNhat,
                Padding = new Padding(16, 12, 16, 12)
            };

            Label lblTu = new Label
            {
                Text = "Từ ngày:",
                Font = GiaoDien.FontDam,
                AutoSize = true,
                Location = new Point(16, 18)
            };

            dtpTuNgay.Format = DateTimePickerFormat.Custom;
            dtpTuNgay.CustomFormat = "dd/MM/yyyy";
            dtpTuNgay.Size = new Size(130, 30);
            dtpTuNgay.Location = new Point(85, 14);
            dtpTuNgay.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            Label lblDen = new Label
            {
                Text = "Đến ngày:",
                Font = GiaoDien.FontDam,
                AutoSize = true,
                Location = new Point(230, 18)
            };

            dtpDenNgay.Format = DateTimePickerFormat.Custom;
            dtpDenNgay.CustomFormat = "dd/MM/yyyy";
            dtpDenNgay.Size = new Size(130, 30);
            dtpDenNgay.Location = new Point(305, 14);
            dtpDenNgay.Value = DateTime.Today;

            btnThongKe.Size = new Size(130, 34);
            btnThongKe.Location = new Point(455, 12);
            btnThongKe.Click += btnThongKe_Click;

            btnHomNay.Size = new Size(90, 34);
            btnHomNay.Location = new Point(600, 12);
            btnHomNay.Click += (s, e) =>
            {
                dtpTuNgay.Value = DateTime.Today;
                dtpDenNgay.Value = DateTime.Today;
                TaiDuLieuThongKe();
            };

            btnBayNgay.Size = new Size(100, 34);
            btnBayNgay.Location = new Point(700, 12);
            btnBayNgay.Click += (s, e) =>
            {
                dtpTuNgay.Value = DateTime.Today.AddDays(-6);
                dtpDenNgay.Value = DateTime.Today;
                TaiDuLieuThongKe();
            };

            btnThangNay.Size = new Size(95, 34);
            btnThangNay.Location = new Point(810, 12);
            btnThangNay.Click += (s, e) =>
            {
                dtpTuNgay.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                dtpDenNgay.Value = DateTime.Today;
                TaiDuLieuThongKe();
            };

            pnlBoLoc.Controls.Add(lblTu);
            pnlBoLoc.Controls.Add(dtpTuNgay);
            pnlBoLoc.Controls.Add(lblDen);
            pnlBoLoc.Controls.Add(dtpDenNgay);
            pnlBoLoc.Controls.Add(btnThongKe);
            pnlBoLoc.Controls.Add(btnHomNay);
            pnlBoLoc.Controls.Add(btnBayNgay);
            pnlBoLoc.Controls.Add(btnThangNay);
            Controls.Add(pnlBoLoc);

            // Khung tổng quan (Thẻ số liệu tóm tắt)
            Panel pnlTongQuan = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.White,
                Padding = new Padding(16, 8, 16, 8)
            };

            var tblThe = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1
            };
            tblThe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            tblThe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34f));
            tblThe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));

            Panel theDoanhThu = TaoTheChiSo("TỔNG DOANH THU", lblTongDoanhThu, GiaoDien.MauChinhDam);
            Panel theHoaDon = TaoTheChiSo("SỐ HÓA ĐƠN ĐÃ THANH TOÁN", lblSoHoaDon, Color.FromArgb(20, 120, 200));
            Panel theTrungBinh = TaoTheChiSo("DOANH THU TRUNG BÌNH / HÓA ĐƠN", lblDoanhThuTB, GiaoDien.MauXanh);

            tblThe.Controls.Add(theDoanhThu, 0, 0);
            tblThe.Controls.Add(theHoaDon, 1, 0);
            tblThe.Controls.Add(theTrungBinh, 2, 0);
            pnlTongQuan.Controls.Add(tblThe);
            Controls.Add(pnlTongQuan);

            // Khu vực nội dung chính: Bên trái bảng Doanh thu theo ngày, Bên phải Top 5 món + Biểu đồ
            var tblNoiDung = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(16, 8, 16, 16)
            };
            tblNoiDung.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42f));
            tblNoiDung.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58f));

            // Cột trái: Doanh thu theo ngày
            Panel pnlTrai = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 8, 0) };
            Label lblTieuDeBangNgay = new Label
            {
                Text = "DOANH THU THEO TỪNG NGÀY",
                Dock = DockStyle.Top,
                Height = 36,
                BackColor = GiaoDien.MauTieuDeBang,
                ForeColor = Color.FromArgb(120, 50, 0),
                Font = GiaoDien.FontDam,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0)
            };
            dgvDoanhThuTheoNgay.Dock = DockStyle.Fill;
            pnlTrai.Controls.Add(dgvDoanhThuTheoNgay);
            pnlTrai.Controls.Add(lblTieuDeBangNgay);

            // Cột phải: Top 5 món bán chạy (bảng phía trên, biểu đồ phía dưới)
            Panel pnlPhai = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8, 0, 0, 0) };
            var tblPhai = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            tblPhai.RowStyles.Add(new RowStyle(SizeType.Percent, 48f));
            tblPhai.RowStyles.Add(new RowStyle(SizeType.Percent, 52f));

            Panel pnlTopMon = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 0, 6) };
            Label lblTieuDeTopMon = new Label
            {
                Text = "TOP 5 MÓN ĂN BÁN CHẠY NHẤT",
                Dock = DockStyle.Top,
                Height = 36,
                BackColor = GiaoDien.MauTieuDeBang,
                ForeColor = Color.FromArgb(120, 50, 0),
                Font = GiaoDien.FontDam,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0)
            };
            dgvTopMon.Dock = DockStyle.Fill;
            pnlTopMon.Controls.Add(dgvTopMon);
            pnlTopMon.Controls.Add(lblTieuDeTopMon);

            Panel pnlBieuDo = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 6, 0, 0) };
            chartTopMon.Dock = DockStyle.Fill;
            chartTopMon.BackColor = Color.White;
            pnlBieuDo.Controls.Add(chartTopMon);

            tblPhai.Controls.Add(pnlTopMon, 0, 0);
            tblPhai.Controls.Add(pnlBieuDo, 0, 1);
            pnlPhai.Controls.Add(tblPhai);

            tblNoiDung.Controls.Add(pnlTrai, 0, 0);
            tblNoiDung.Controls.Add(pnlPhai, 1, 0);
            Controls.Add(tblNoiDung);
        }

        private Panel TaoTheChiSo(string tieuDe, Label lblGiaTri, Color mauChu)
        {
            Panel card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(250, 250, 252),
                Margin = new Padding(6, 0, 6, 0),
                Padding = new Padding(12, 8, 12, 8)
            };

            card.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(225, 227, 230), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
                }
                using (var brush = new SolidBrush(mauChu))
                {
                    e.Graphics.FillRectangle(brush, 0, 0, 5, card.Height);
                }
            };

            Label lblTieuDe = new Label
            {
                Text = tieuDe,
                Dock = DockStyle.Top,
                Height = 22,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 100, 100),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblGiaTri.Text = "0";
            lblGiaTri.Dock = DockStyle.Fill;
            lblGiaTri.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblGiaTri.ForeColor = mauChu;
            lblGiaTri.TextAlign = ContentAlignment.MiddleLeft;

            card.Controls.Add(lblGiaTri);
            card.Controls.Add(lblTieuDe);
            return card;
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

            var chartArea = new ChartArea("AreaTopMon")
            {
                BackColor = Color.White
            };
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(235, 235, 235);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(235, 235, 235);
            chartArea.AxisX.LabelStyle.Font = new Font("Segoe UI", 9F);
            chartArea.AxisY.LabelStyle.Font = new Font("Segoe UI", 9F);
            chartArea.AxisX.Interval = 1;
            chartArea.AxisX.IsLabelAutoFit = true;
            chartArea.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.LabelsAngleStep45;
            chartTopMon.ChartAreas.Add(chartArea);

            var title = new Title("Biểu đồ top 5 món theo số lượng bán", Docking.Top,
                new Font("Segoe UI", 11F, FontStyle.Bold), Color.FromArgb(80, 80, 80));
            chartTopMon.Titles.Add(title);

            var series = new Series("Số lượng bán")
            {
                ChartType = SeriesChartType.Column,
                Color = GiaoDien.MauChinh,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                IsValueShownAsLabel = true
            };

            if (dtTop.Rows.Count == 0)
            {
                title.Text = "Không có dữ liệu bán trong khoảng thời gian đã chọn";
                title.ForeColor = Color.FromArgb(160, 160, 160);
            }
            else
            {
                foreach (DataRow row in dtTop.Rows)
                {
                    string tenMon = Convert.ToString(row["TenMon"]);
                    int soLuong = Convert.ToInt32(row["TongSoLuong"]);
                    series.Points.AddXY(tenMon, soLuong);
                }
            }

            chartTopMon.Series.Add(series);
        }
    }
}
