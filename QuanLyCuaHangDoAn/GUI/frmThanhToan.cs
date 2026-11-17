using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Transactions;
using System.Windows.Forms;
using QuanLyCuaHangDoAn;
using QuanLyCuaHangDoAn.Class;

namespace QuanLyCuaHangDoAn.GUI
{
    public class frmThanhToan : Form
    {
        private const string TrangThaiBanTrong = "Trống";
        private const string TrangThaiBanCoKhach = "Có khách";
        private const string TrangThaiHoaDonDangPhucVu = "Đang phục vụ";
        private const string TrangThaiHoaDonDaThanhToan = "Đã thanh toán";

        private readonly string maHD;
        private readonly FileXml fx = new FileXml();
        private readonly Label lblBan = new Label();
        private readonly Label lblTamTinh = new Label();
        private readonly Label lblKhachHang = new Label();
        private readonly Label lblHang = new Label();
        private readonly Label lblGiam = new Label();
        private readonly Label lblTongTien = new Label();
        private readonly TextBox txtSDT = new TextBox();
        private readonly Button btnTimKhach = GiaoDien.TaoNut("Tìm khách", GiaoDien.MauChinh);
        private readonly Button btnThanhToan = GiaoDien.TaoNut("Thanh toán", GiaoDien.MauXanh);
        private readonly Button btnHuy = GiaoDien.TaoNut("Hủy", GiaoDien.MauChinhDam);

        private string maBan;
        private string maKH;
        private decimal tamTinh;
        private decimal phanTram;
        private bool daNapDuLieu;

        public frmThanhToan(string maHD)
        {
            if (string.IsNullOrWhiteSpace(maHD))
                throw new ArgumentException("Mã hóa đơn không được để trống.", "maHD");

            this.maHD = maHD;
            KhoiTaoGiaoDien();
            Load += frmThanhToan_Load;
            btnTimKhach.Click += btnTimKhach_Click;
            btnThanhToan.Click += btnThanhToan_Click;
            btnHuy.Click += btnHuy_Click;
            txtSDT.TextChanged += txtSDT_TextChanged;
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Thanh toán";
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = GiaoDien.FontThuong;
            ClientSize = new Size(540, 500);
            MinimumSize = new Size(540, 500);
            MaximizeBox = false;
            MinimizeBox = false;

            var noiDung = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 8,
                Padding = new Padding(24)
            };
            int[] chieuCaoHang = { 42, 42, 52, 42, 42, 42, 52, 54 };
            foreach (int chieuCao in chieuCaoHang)
                noiDung.RowStyles.Add(new RowStyle(SizeType.Absolute, chieuCao));

            lblBan.Name = "lblBan";
            lblTamTinh.Name = "lblTamTinh";
            lblKhachHang.Name = "lblKhachHang";
            lblHang.Name = "lblHang";
            lblGiam.Name = "lblGiam";
            lblTongTien.Name = "lblTongTien";
            CauHinhNhan(lblBan, GiaoDien.FontDam);
            CauHinhNhan(lblTamTinh, GiaoDien.FontThuong);
            CauHinhNhan(lblKhachHang, GiaoDien.FontThuong);
            CauHinhNhan(lblHang, GiaoDien.FontThuong);
            CauHinhNhan(lblGiam, GiaoDien.FontThuong);
            CauHinhNhan(lblTongTien, GiaoDien.FontDam);

            var hangSDT = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                Margin = Padding.Empty
            };
            var lblSDT = new Label
            {
                Name = "lblSDT",
                Text = "SĐT:",
                Width = 48,
                Height = 38,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = GiaoDien.FontThuong,
                Margin = new Padding(0, 0, 4, 0)
            };
            txtSDT.Name = "txtSDT";
            txtSDT.Width = 170;
            txtSDT.MaxLength = 15;
            txtSDT.Margin = new Padding(0, 5, 8, 0);
            btnTimKhach.Name = "btnTimKhach";
            btnTimKhach.Width = 130;
            btnTimKhach.Height = 38;
            btnTimKhach.Margin = Padding.Empty;
            hangSDT.Controls.Add(lblSDT);
            hangSDT.Controls.Add(txtSDT);
            hangSDT.Controls.Add(btnTimKhach);

            var cacNut = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                Margin = Padding.Empty
            };
            btnThanhToan.Name = "btnThanhToan";
            btnThanhToan.Width = 140;
            btnThanhToan.Height = 40;
            btnThanhToan.Enabled = false;
            btnHuy.Name = "btnHuy";
            btnHuy.Width = 110;
            btnHuy.Height = 40;
            cacNut.Controls.Add(btnHuy);
            cacNut.Controls.Add(btnThanhToan);

            noiDung.Controls.Add(lblBan, 0, 0);
            noiDung.Controls.Add(lblTamTinh, 0, 1);
            noiDung.Controls.Add(hangSDT, 0, 2);
            noiDung.Controls.Add(lblKhachHang, 0, 3);
            noiDung.Controls.Add(lblHang, 0, 4);
            noiDung.Controls.Add(lblGiam, 0, 5);
            noiDung.Controls.Add(lblTongTien, 0, 6);
            noiDung.Controls.Add(cacNut, 0, 7);

            Controls.Add(noiDung);
            Controls.Add(GiaoDien.TaoTieuDe("THANH TOÁN"));
            CapNhatNhanKhachHang();
        }

        private static void CauHinhNhan(Label lbl, Font font)
        {
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            lbl.Font = font;
            lbl.Margin = Padding.Empty;
        }

        private void frmThanhToan_Load(object sender, EventArgs e)
        {
            try
            {
                if (!PhienDangNhap.IsLoggedIn
                    || (PhienDangNhap.ChucVu != "ThuNgan" && PhienDangNhap.ChucVu != "Admin"))
                    throw new InvalidOperationException("Bạn không có quyền thanh toán hóa đơn.");

                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                using (var cmd = new SqlCommand(
                    "SELECT b.MaBan, b.TenBan, " +
                    "COALESCE(SUM(CONVERT(decimal(18, 2), ct.SoLuong) * ct.DonGia), 0) AS TamTinh " +
                    "FROM HoaDon hd INNER JOIN Ban b ON hd.MaBan = b.MaBan " +
                    "LEFT JOIN ChiTietHoaDon ct ON ct.MaHD = hd.MaHD " +
                    "WHERE hd.MaHD = @MaHD AND hd.TrangThai = @TrangThai " +
                    "GROUP BY b.MaBan, b.TenBan;", conn))
                {
                    cmd.Parameters.Add("@MaHD", SqlDbType.NVarChar, 10).Value = maHD;
                    cmd.Parameters.Add("@TrangThai", SqlDbType.NVarChar, 15).Value = TrangThaiHoaDonDangPhucVu;
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new InvalidOperationException("Không tìm thấy hóa đơn đang phục vụ.");

                        maBan = Convert.ToString(reader["MaBan"]);
                        lblBan.Text = "Bàn: " + Convert.ToString(reader["TenBan"]);
                        tamTinh = Convert.ToDecimal(reader["TamTinh"]);
                    }
                }

                daNapDuLieu = true;
                CapNhatNhanTien();
                btnThanhToan.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Không thể tải thông tin thanh toán.\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void btnTimKhach_Click(object sender, EventArgs e)
        {
            if (!daNapDuLieu)
                return;

            maKH = null;
            phanTram = 0m;
            string sdt = txtSDT.Text.Trim();

            if (sdt.Length == 0)
            {
                CapNhatNhanKhachHang();
                CapNhatNhanTien();
                return;
            }

            try
            {
                string tenKH;
                string hang;
                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                using (var cmd = new SqlCommand(
                    "SELECT MaKH, TenKH, Hang FROM KhachHang WHERE SDT = @SDT;", conn))
                {
                    cmd.Parameters.Add("@SDT", SqlDbType.VarChar, 15).Value = sdt;
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            CapNhatNhanKhachHang();
                            CapNhatNhanTien();
                            MessageBox.Show(this, "Không tìm thấy khách hàng có số điện thoại này.",
                                "Không tìm thấy khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        maKH = Convert.ToString(reader["MaKH"]);
                        tenKH = Convert.ToString(reader["TenKH"]);
                        hang = Convert.ToString(reader["Hang"]);
                    }
                }

                phanTram = KhachHang.PhanTramGiam(maKH);
                lblKhachHang.Text = "Khách hàng: " + tenKH;
                lblHang.Text = "Hạng: " + hang;
                CapNhatNhanKhachHang();
                CapNhatNhanTien();
            }
            catch (Exception ex)
            {
                maKH = null;
                phanTram = 0m;
                CapNhatNhanKhachHang();
                CapNhatNhanTien();
                MessageBox.Show(this, "Không thể tìm khách hàng.\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSDT_TextChanged(object sender, EventArgs e)
        {
            if (!daNapDuLieu)
                return;

            maKH = null;
            phanTram = 0m;
            CapNhatNhanKhachHang();
            CapNhatNhanTien();
        }

        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            if (!daNapDuLieu)
                return;

            try
            {
                phanTram = string.IsNullOrEmpty(maKH) ? 0m : KhachHang.PhanTramGiam(maKH);
                using (var scope = new TransactionScope(
                    TransactionScopeOption.Required,
                    new TransactionOptions
                    {
                        IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                        Timeout = TimeSpan.FromSeconds(60)
                    }))
                {
                    using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                    {
                        conn.Open();
                        tamTinh = LayTamTinh(conn);
                        decimal tongTien = TinhTongTien(tamTinh, phanTram);

                        using (var cmdHoaDon = new SqlCommand(
                            "UPDATE HoaDon SET TrangThai = @TrangThaiMoi, MaKH = @MaKH, " +
                            "PhanTramGiam = @PhanTramGiam, TongTien = @TongTien " +
                            "WHERE MaHD = @MaHD AND TrangThai = @TrangThaiHienTai;", conn))
                        {
                            cmdHoaDon.Parameters.Add("@TrangThaiMoi", SqlDbType.NVarChar, 15).Value =
                                TrangThaiHoaDonDaThanhToan;
                            SqlParameter pMaKH = cmdHoaDon.Parameters.Add("@MaKH", SqlDbType.NVarChar, 10);
                            pMaKH.Value = string.IsNullOrEmpty(maKH) ? (object)DBNull.Value : maKH;
                            SqlParameter pPhanTram = cmdHoaDon.Parameters.Add("@PhanTramGiam", SqlDbType.Decimal);
                            pPhanTram.Precision = 5;
                            pPhanTram.Scale = 2;
                            pPhanTram.Value = phanTram;
                            SqlParameter pTongTien = cmdHoaDon.Parameters.Add("@TongTien", SqlDbType.Decimal);
                            pTongTien.Precision = 18;
                            pTongTien.Scale = 0;
                            pTongTien.Value = tongTien;
                            cmdHoaDon.Parameters.Add("@MaHD", SqlDbType.NVarChar, 10).Value = maHD;
                            cmdHoaDon.Parameters.Add("@TrangThaiHienTai", SqlDbType.NVarChar, 15).Value =
                                TrangThaiHoaDonDangPhucVu;

                            if (cmdHoaDon.ExecuteNonQuery() != 1)
                                throw new InvalidOperationException("Hóa đơn không còn ở trạng thái đang phục vụ.");
                        }

                        using (var cmdBan = new SqlCommand(
                            "UPDATE Ban SET TrangThai = @TrangThaiMoi " +
                            "WHERE MaBan = @MaBan AND TrangThai = @TrangThaiHienTai;", conn))
                        {
                            cmdBan.Parameters.Add("@TrangThaiMoi", SqlDbType.NVarChar, 15).Value = TrangThaiBanTrong;
                            cmdBan.Parameters.Add("@MaBan", SqlDbType.NVarChar, 10).Value = maBan;
                            cmdBan.Parameters.Add("@TrangThaiHienTai", SqlDbType.NVarChar, 15).Value =
                                TrangThaiBanCoKhach;

                            if (cmdBan.ExecuteNonQuery() != 1)
                                throw new InvalidOperationException("Bàn không còn ở trạng thái có khách.");
                        }
                    }

                    decimal tongTienSauGiam = TinhTongTien(tamTinh, phanTram);
                    if (!string.IsNullOrEmpty(maKH))
                        KhachHang.CongDiem(maKH, tongTienSauGiam);
                    Kho.TruKhoTheoHoaDon(maHD);

                    scope.Complete();
                }

                CapNhatNhanTien();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Không thể thanh toán. Dữ liệu chưa được xác nhận.\n" + ex.Message,
                    "Lỗi thanh toán", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                fx.TaoXml("SELECT * FROM HoaDon", "HoaDon", "HoaDon.xml");
                fx.TaoXml("SELECT * FROM Ban", "Ban", "Ban.xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    "Thanh toán đã hoàn tất nhưng không thể đồng bộ file XML.\n" + ex.Message,
                    "Lỗi đồng bộ XML", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private static decimal LayTamTinh(SqlConnection conn, string maHD)
        {
            using (var cmd = new SqlCommand(
                "SELECT COALESCE(SUM(CONVERT(decimal(18, 2), SoLuong) * DonGia), 0) " +
                "FROM ChiTietHoaDon WHERE MaHD = @MaHD;", conn))
            {
                cmd.Parameters.Add("@MaHD", SqlDbType.NVarChar, 10).Value = maHD;
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        private decimal LayTamTinh(SqlConnection conn)
        {
            return LayTamTinh(conn, maHD);
        }

        private static decimal TinhTongTien(decimal tamTinh, decimal phanTramGiam)
        {
            return decimal.Round(
                tamTinh * (100m - phanTramGiam) / 100m,
                0,
                MidpointRounding.AwayFromZero);
        }

        private void CapNhatNhanKhachHang()
        {
            if (string.IsNullOrEmpty(maKH))
            {
                lblKhachHang.Text = "Khách hàng: Khách lẻ";
                lblHang.Text = "Hạng: -";
                lblGiam.Text = "Giảm giá: 0%";
            }
            else
            {
                lblGiam.Text = "Giảm giá: " + phanTram.ToString("0.##", CultureInfo.InvariantCulture) + "%";
            }
        }

        private void CapNhatNhanTien()
        {
            lblTamTinh.Text = "Tạm tính: " + DinhDangTien(tamTinh);
            lblTongTien.Text = "Tổng tiền: " + DinhDangTien(TinhTongTien(tamTinh, phanTram));
            if (!string.IsNullOrEmpty(maKH))
                lblGiam.Text = "Giảm giá: " + phanTram.ToString("0.##", CultureInfo.InvariantCulture) + "%";
        }

        private static string DinhDangTien(decimal soTien)
        {
            return soTien.ToString("#,##0", new CultureInfo("vi-VN")) + " đ";
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
