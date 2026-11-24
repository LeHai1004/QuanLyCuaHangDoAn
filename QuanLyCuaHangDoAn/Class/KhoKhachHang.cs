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
        /// <summary>Trừ kho theo công thức của các món trong hóa đơn.</summary>
        public static void TruKhoTheoHoaDon(string maHD)
        {
            // TODO(Duy, T11)
        }

        /// <summary>Kho còn đủ nguyên liệu để làm soLuong phần của món này không.</summary>
        public static bool DuNguyenLieu(string maMon, int soLuong)
        {
            return true; // TODO(Duy, T11)
        }
    }
}
