using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyCuaHangDoAn.Class
{
    public static class TaiKhoan
    {
        // SHA-256 của "TenDN:matKhauGoc", UTF-8, 64 ký tự hex thường
        public static string BamMatKhau(string tenDN, string matKhauGoc)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] kq = sha.ComputeHash(Encoding.UTF8.GetBytes(tenDN + ":" + matKhauGoc));
                StringBuilder sb = new StringBuilder(64);
                foreach (byte b in kq)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        // Đúng tài khoản: điền PhienDangNhap và trả về true
        public static bool DangNhap(string tenDN, string matKhauGoc)
        {
            string chuoiNoi = ConfigurationManager.ConnectionStrings["QLCuaHangDoAn"].ConnectionString;
            const string sql = "SELECT MaNV, TenNV, ChucVu FROM NhanVien WHERE TenDN = @tenDN AND MatKhau = @matKhau";

            using (SqlConnection conn = new SqlConnection(chuoiNoi))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@tenDN", SqlDbType.VarChar, 30).Value = tenDN;
                cmd.Parameters.Add("@matKhau", SqlDbType.VarChar, 64).Value = BamMatKhau(tenDN, matKhauGoc);
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (!rd.Read())
                    {
                        return false;
                    }
                    PhienDangNhap.MaNV = rd.GetString(0);
                    PhienDangNhap.TenNV = rd.GetString(1);
                    PhienDangNhap.ChucVu = rd.GetString(2);
                    return true;
                }
            }
        }
    }
}