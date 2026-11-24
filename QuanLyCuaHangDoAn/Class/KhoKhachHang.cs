using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace QuanLyCuaHangDoAn
{
    /// <summary>Nghiệp vụ khách hàng, tích điểm và hạng thành viên (Duy - Task T9).</summary>
    public static class KhachHang
    {
        /// <summary>
        /// Trả về phần trăm giảm giá theo hạng khách hàng: 0, 5 hoặc 10.
        /// Thường: 0%, Bạc: 5%, Vàng: 10%.
        /// Không ném ngoại lệ nếu maKH null, rỗng hoặc không tồn tại trong DB.
        /// </summary>
        public static decimal PhanTramGiam(string maKH)
        {
            if (string.IsNullOrWhiteSpace(maKH))
                return 0m;

            try
            {
                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                using (var cmd = new SqlCommand("SELECT Hang FROM KhachHang WHERE MaKH = @MaKH", conn))
                {
                    cmd.Parameters.Add("@MaKH", SqlDbType.NVarChar, 10).Value = maKH.Trim();
                    conn.Open();
                    object ketQua = cmd.ExecuteScalar();
                    if (ketQua == null || ketQua == DBNull.Value)
                        return 0m;

                    string hang = ketQua.ToString().Trim();
                    if (string.Equals(hang, "Vàng", StringComparison.OrdinalIgnoreCase))
                        return 10m;
                    if (string.Equals(hang, "Bạc", StringComparison.OrdinalIgnoreCase))
                        return 5m;
                    return 0m;
                }
            }
            catch
            {
                return 0m;
            }
        }

        /// <summary>
        /// Cộng điểm tích lũy (10.000đ = 1 điểm) và tự động xét lại hạng thành viên.
        /// Thường: dưới 100 điểm; Bạc: từ 100 đến dưới 300 điểm; Vàng: từ 300 điểm trở lên.
        /// Cập nhật SQL Server và đồng bộ lại file Data/KhachHang.xml.
        /// </summary>
        public static void CongDiem(string maKH, decimal tongTien)
        {
            if (string.IsNullOrWhiteSpace(maKH) || tongTien <= 0m)
                return;

            int diemCong = (int)Math.Floor(tongTien / 10000m);

            try
            {
                int diemHienTai = 0;
                bool tonTai = false;

                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                {
                    conn.Open();
                    using (var cmdKiemTra = new SqlCommand("SELECT DiemTichLuy FROM KhachHang WHERE MaKH = @MaKH", conn))
                    {
                        cmdKiemTra.Parameters.Add("@MaKH", SqlDbType.NVarChar, 10).Value = maKH.Trim();
                        object res = cmdKiemTra.ExecuteScalar();
                        if (res != null && res != DBNull.Value)
                        {
                            tonTai = true;
                            diemHienTai = Convert.ToInt32(res);
                        }
                    }

                    if (!tonTai)
                        return;

                    int tongDiemMoi = diemHienTai + diemCong;
                    string hangMoi;
                    if (tongDiemMoi >= 300)
                        hangMoi = "Vàng";
                    else if (tongDiemMoi >= 100)
                        hangMoi = "Bạc";
                    else
                        hangMoi = "Thường";

                    using (var cmdCapNhat = new SqlCommand(
                        "UPDATE KhachHang SET DiemTichLuy = @Diem, Hang = @Hang WHERE MaKH = @MaKH", conn))
                    {
                        cmdCapNhat.Parameters.Add("@Diem", SqlDbType.Int).Value = tongDiemMoi;
                        cmdCapNhat.Parameters.Add("@Hang", SqlDbType.NVarChar, 10).Value = hangMoi;
                        cmdCapNhat.Parameters.Add("@MaKH", SqlDbType.NVarChar, 10).Value = maKH.Trim();
                        cmdCapNhat.ExecuteNonQuery();
                    }

                    // Đồng bộ lại file Data/KhachHang.xml
                    try
                    {
                        var fx = new FileXml();
                        string duongDanXml = FileXml.DuongDan("KhachHang.xml");
                        if (File.Exists(duongDanXml))
                        {
                            fx.Sua("KhachHang.xml",
                                new Dictionary<string, string> { { "MaKH", maKH.Trim() } },
                                new Dictionary<string, string>
                                {
                                    { "DiemTichLuy", tongDiemMoi.ToString() },
                                    { "Hang", hangMoi }
                                });
                        }
                        else
                        {
                            fx.TaoXml("SELECT * FROM KhachHang", "KhachHang", "KhachHang.xml");
                        }
                    }
                    catch
                    {
                        // Bỏ qua lỗi đồng bộ XML để không làm gián đoạn luồng gọi
                    }
                }
            }
            catch
            {
                // Thoát hàm an toàn, tuyệt đối không ném ngoại lệ
            }
        }
    }

    /// <summary>KHUNG RỖNG. Duy viết phần ruột ở task T11.</summary>
    public static class Kho
    {
        public static void TruKhoTheoHoaDon(string maHD)
        {
            if (string.IsNullOrWhiteSpace(maHD)) return;
            try
            {
                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            var chiTiet = new DataTable();
                            using (var cmd = new SqlCommand(
                                "SELECT ct.MaMon, ct.SoLuong, c.MaNL, c.SoLuong AS SoLuongCongThuc " +
                                "FROM ChiTietHoaDon ct INNER JOIN CongThuc c ON ct.MaMon = c.MaMon " +
                                "WHERE ct.MaHD = @MaHD", conn, tran))
                            {
                                cmd.Parameters.Add("@MaHD", SqlDbType.NVarChar, 10).Value = maHD.Trim();
                                using (var da = new SqlDataAdapter(cmd)) da.Fill(chiTiet);
                            }
                            if (chiTiet.Rows.Count == 0) { tran.Commit(); return; }

                            foreach (DataRow dong in chiTiet.Rows)
                            {
                                using (var cmd = new SqlCommand(
                                    "UPDATE NguyenLieu SET TonKho = CASE WHEN TonKho >= @Luong " +
                                    "THEN TonKho - @Luong ELSE 0 END WHERE MaNL = @MaNL", conn, tran))
                                {
                                    cmd.Parameters.Add("@Luong", SqlDbType.Decimal).Value =
                                        Convert.ToDecimal(dong["SoLuongCongThuc"]) * Convert.ToInt32(dong["SoLuong"]);
                                    cmd.Parameters.Add("@MaNL", SqlDbType.NVarChar, 10).Value = dong["MaNL"];
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            using (var cmd = new SqlCommand(
                                "UPDATE m SET TrangThai = CASE WHEN EXISTS (" +
                                "SELECT 1 FROM CongThuc c LEFT JOIN NguyenLieu n ON n.MaNL = c.MaNL " +
                                "WHERE c.MaMon = m.MaMon AND ISNULL(n.TonKho, 0) < c.SoLuong " +
                                ") THEN N'Hết' ELSE N'Còn bán' END FROM MonAn m", conn, tran))
                                cmd.ExecuteNonQuery();
                            tran.Commit();
                        }
                        catch
                        {
                            try { tran.Rollback(); } catch { }
                            return;
                        }
                    }
                }
                DongBoXmlKho();
            }
            catch { }
        }

        public static bool DuNguyenLieu(string maMon, int soLuong)
        {
            if (string.IsNullOrWhiteSpace(maMon) || soLuong <= 0) return true;
            try
            {
                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                using (var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM CongThuc c LEFT JOIN NguyenLieu n ON n.MaNL = c.MaNL " +
                    "WHERE c.MaMon = @MaMon AND ISNULL(n.TonKho, 0) < c.SoLuong * @SoLuong", conn))
                {
                    cmd.Parameters.Add("@MaMon", SqlDbType.NVarChar, 10).Value = maMon.Trim();
                    cmd.Parameters.Add("@SoLuong", SqlDbType.Int).Value = soLuong;
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar()) == 0;
                }
            }
            catch { return false; }
        }

        public static DataTable LayCanhBao()
        {
            var ketQua = new DataTable("NguyenLieuCanhBao");
            try
            {
                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                using (var da = new SqlDataAdapter(
                    "SELECT MaNL, TenNL, DVT, TonKho, DinhMuc FROM NguyenLieu " +
                    "WHERE TonKho <= DinhMuc ORDER BY MaNL", conn)) da.Fill(ketQua);
            }
            catch { }
            return ketQua;
        }

        /// <summary>
        /// Cap nhat lai trang thai cac mon dung nguyen lieu vua nhap.
        /// Phuong thuc nay dung lai ket noi va transaction cua nghiep vu nhap kho.
        /// </summary>
        public static void CapNhatTrangThaiMonAn(SqlConnection conn, SqlTransaction tran, IEnumerable<string> maNguyenLieu)
        {
            if (conn == null || tran == null || maNguyenLieu == null) return;

            var maMonCanKiemTra = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string maNL in maNguyenLieu)
            {
                if (string.IsNullOrWhiteSpace(maNL)) continue;
                using (var cmd = new SqlCommand(
                    "SELECT DISTINCT MaMon FROM CongThuc WHERE MaNL = @MaNL", conn, tran))
                {
                    cmd.Parameters.Add("@MaNL", SqlDbType.NVarChar, 10).Value = maNL.Trim();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            maMonCanKiemTra.Add(Convert.ToString(reader["MaMon"]));
                    }
                }
            }

            foreach (string maMon in maMonCanKiemTra)
            {
                using (var cmd = new SqlCommand(
                    "UPDATE MonAn SET TrangThai = N'Còn bán' " +
                    "WHERE MaMon = @MaMon AND TrangThai = N'Hết' AND NOT EXISTS (" +
                    "SELECT 1 FROM CongThuc c LEFT JOIN NguyenLieu n ON n.MaNL = c.MaNL " +
                    "WHERE c.MaMon = @MaMon AND ISNULL(n.TonKho, 0) < c.SoLuong)", conn, tran))
                {
                    cmd.Parameters.Add("@MaMon", SqlDbType.NVarChar, 10).Value = maMon;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void DongBoXmlKho()
        {
            try
            {
                var fx = new FileXml();
                fx.TaoXml("SELECT * FROM NguyenLieu", "NguyenLieu", "NguyenLieu.xml");
                fx.TaoXml("SELECT * FROM MonAn", "MonAn", "MonAn.xml");
            }
            catch { }
        }
    }
}
