using System;

namespace QuanLyCuaHangDoAn.Class
{
    public static class PhienDangNhap
    {
        public static string MaNV { get; set; }
        public static string TenNV { get; set; }
        public static string ChucVu { get; set; }

        public static void DangXuat()
        {
            MaNV = null;
            TenNV = null;
            ChucVu = null;
        }

        public static bool IsLoggedIn => !string.IsNullOrEmpty(MaNV);
    }
}