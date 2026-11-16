using System;
using System.Windows.Forms;
using QuanLyCuaHangDoAn.GUI;

namespace QuanLyCuaHangDoAn
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Đăng nhập -> vào frmMain; bấm Đăng xuất thì quay lại đăng nhập
            while (true)
            {
                using (frmDangNhap dangNhap = new frmDangNhap())
                {
                    if (dangNhap.ShowDialog() != DialogResult.OK)
                    {
                        break;
                    }
                }

                using (frmMain chinh = new frmMain())
                {
                    Application.Run(chinh);
                    if (!chinh.DangXuat)
                    {
                        break;
                    }
                }
            }
        }
    }
}