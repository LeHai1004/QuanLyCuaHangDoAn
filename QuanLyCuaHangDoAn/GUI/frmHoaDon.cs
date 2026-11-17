using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using QuanLyCuaHangDoAn;
using QuanLyCuaHangDoAn.Class;

namespace QuanLyCuaHangDoAn.GUI
{
    public class frmHoaDon : Form
    {
        private const string TrangThaiHoaDonDaThanhToan = "Đã thanh toán";

        private readonly FileXml fx = new FileXml();
        private readonly DateTimePicker dtpTuNgay = new DateTimePicker();
        private readonly DateTimePicker dtpDenNgay = new DateTimePicker();
        private readonly Button btnXem = GiaoDien.TaoNut("Xem", GiaoDien.MauChinh);
        private readonly Button btnLamMoi = GiaoDien.TaoNut("Làm mới", GiaoDien.MauChinhDam);
        private readonly Button btnInHoaDon = GiaoDien.TaoNut("In hóa đơn", GiaoDien.MauXanh);
        private readonly SplitContainer splHoaDonChiTiet = new SplitContainer();
        private readonly DataGridView dgvHoaDon = new DataGridView();
        private readonly DataGridView dgvChiTiet = new DataGridView();
        private readonly Label lblSoHoaDon = new Label();
        private readonly Label lblDoanhThu = new Label();

        private DataTable hoaDon;
        private DataTable ban;
        private DataTable nhanVien;
        private DataTable khachHang;
        private DataTable chiTietHoaDon;
        private DataTable monAn;
        private DataTable hoaDonDangHienThi;
        private DataTable chiTietDangHienThi;
        private bool dangNap;

        public frmHoaDon()
        {
            KhoiTaoGiaoDien();
            Load += frmHoaDon_Load;
            Shown += frmHoaDon_Shown;
            btnXem.Click += btnXem_Click;
            btnLamMoi.Click += btnLamMoi_Click;
            btnInHoaDon.Click += btnInHoaDon_Click;
            dgvHoaDon.SelectionChanged += dgvHoaDon_SelectionChanged;
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Lịch sử hóa đơn";
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = GiaoDien.FontThuong;
            ClientSize = new Size(1180, 760);
            MinimumSize = new Size(900, 600);

            var noiDung = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(12)
            };
            noiDung.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            noiDung.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            noiDung.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));

            var thanhLoc = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 8, 0, 0),
                Margin = Padding.Empty
            };
            thanhLoc.Controls.Add(TaoNhanNgay("Từ ngày:", "lblTuNgay"));
            dtpTuNgay.Name = "dtpTuNgay";
            CauHinhNgay(dtpTuNgay);
            thanhLoc.Controls.Add(dtpTuNgay);
            thanhLoc.Controls.Add(TaoNhanNgay("Đến ngày:", "lblDenNgay"));
            dtpDenNgay.Name = "dtpDenNgay";
            CauHinhNgay(dtpDenNgay);
            thanhLoc.Controls.Add(dtpDenNgay);

            btnXem.Name = "btnXem";
            btnXem.Width = 90;
            btnXem.Margin = new Padding(12, 0, 6, 0);
            btnLamMoi.Name = "btnLamMoi";
            btnLamMoi.Width = 110;
            btnLamMoi.Margin = new Padding(6, 0, 6, 0);
            btnInHoaDon.Name = "btnInHoaDon";
            btnInHoaDon.Width = 120;
            btnInHoaDon.Enabled = false;
            btnInHoaDon.Margin = new Padding(6, 0, 0, 0);
            thanhLoc.Controls.Add(btnXem);
            thanhLoc.Controls.Add(btnLamMoi);
            thanhLoc.Controls.Add(btnInHoaDon);

            splHoaDonChiTiet.Name = "splHoaDonChiTiet";
            splHoaDonChiTiet.Dock = DockStyle.Fill;
            splHoaDonChiTiet.Orientation = Orientation.Horizontal;
            splHoaDonChiTiet.FixedPanel = FixedPanel.None;
            splHoaDonChiTiet.Margin = Padding.Empty;
            dgvHoaDon.Name = "dgvHoaDon";
            dgvHoaDon.Dock = DockStyle.Fill;
            dgvHoaDon.AutoGenerateColumns = true;
            dgvChiTiet.Name = "dgvChiTiet";
            dgvChiTiet.Dock = DockStyle.Fill;
            dgvChiTiet.AutoGenerateColumns = true;
            splHoaDonChiTiet.Panel1.Controls.Add(dgvHoaDon);
            splHoaDonChiTiet.Panel2.Controls.Add(dgvChiTiet);

            var thongKe = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 8, 0, 0),
                Margin = Padding.Empty
            };
            CauHinhThongKe(lblSoHoaDon, "lblSoHoaDon");
            CauHinhThongKe(lblDoanhThu, "lblDoanhThu");
            thongKe.Controls.Add(lblSoHoaDon);
            thongKe.Controls.Add(lblDoanhThu);

            noiDung.Controls.Add(thanhLoc, 0, 0);
            noiDung.Controls.Add(splHoaDonChiTiet, 0, 1);
            noiDung.Controls.Add(thongKe, 0, 2);
            Controls.Add(noiDung);
            Controls.Add(GiaoDien.TaoTieuDe("LỊCH SỬ HÓA ĐƠN"));

            GiaoDien.LamDepBang(dgvHoaDon, new Dictionary<string, string>
            {
                { "MaHD", "Mã hóa đơn" },
                { "Ban", "Bàn" },
                { "NhanVien", "Nhân viên" },
                { "Khach", "Khách" },
                { "NgayLap", "Ngày lập" },
                { "TrangThai", "Trạng thái" },
                { "Giam", "Giảm (%)" },
                { "TongTien", "Tổng tiền" }
            });
            GiaoDien.LamDepBang(dgvChiTiet, new Dictionary<string, string>
            {
                { "TenMon", "Tên món" },
                { "SoLuong", "SL" },
                { "DonGia", "Đơn giá" },
                { "ThanhTien", "Thành tiền" },
                { "TrangThaiBep", "Trạng thái bếp" }
            });
            GiaoDien.DinhDangTien(dgvHoaDon, "TongTien");
            GiaoDien.DinhDangTien(dgvChiTiet, "DonGia", "ThanhTien");
            dgvChiTiet.DataSource = TaoBangChiTietTrong();
            GiaoDien.LamDepBang(dgvChiTiet, new Dictionary<string, string>
            {
                { "TenMon", "Tên món" },
                { "SoLuong", "SL" },
                { "DonGia", "Đơn giá" },
                { "ThanhTien", "Thành tiền" },
                { "TrangThaiBep", "Trạng thái bếp" }
            });
            GiaoDien.DinhDangTien(dgvChiTiet, "DonGia", "ThanhTien");
            CapNhatThongKe(0, 0m);
        }

        private static Label TaoNhanNgay(string text, string name)
        {
            return new Label
            {
                Name = name,
                Text = text,
                AutoSize = true,
                Height = 38,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 0, 6, 0),
                Padding = new Padding(0, 8, 0, 0)
            };
        }

        private static void CauHinhNgay(DateTimePicker dtp)
        {
            dtp.Format = DateTimePickerFormat.Short;
            dtp.Width = 125;
            dtp.Margin = new Padding(0, 0, 16, 0);
        }

        private static void CauHinhThongKe(Label lbl, string name)
        {
            lbl.Name = name;
            lbl.AutoSize = true;
            lbl.Font = GiaoDien.FontDam;
            lbl.Margin = new Padding(0, 0, 32, 0);
        }

        private void frmHoaDon_Load(object sender, EventArgs e)
        {
            if (!PhienDangNhap.IsLoggedIn
                || (PhienDangNhap.ChucVu != "Admin" && PhienDangNhap.ChucVu != "ThuNgan"))
            {
                MessageBox.Show(this, "Bạn không có quyền xem lịch sử hóa đơn.",
                    "Không có quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }

            DateTime ngayDen = DateTime.Today;
            dtpTuNgay.Value = ngayDen.AddDays(-6);
            dtpDenNgay.Value = ngayDen;
            LamMoiDuLieu();
        }

        private void frmHoaDon_Shown(object sender, EventArgs e)
        {
            if (splHoaDonChiTiet.Height > 300)
                splHoaDonChiTiet.SplitterDistance = Math.Min(
                    350, splHoaDonChiTiet.Height - splHoaDonChiTiet.Panel2MinSize);
        }

        private void btnXem_Click(object sender, EventArgs e)
        {
            HienThiTheoKhoangNgay();
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LamMoiDuLieu();
        }

        private void LamMoiDuLieu()
        {
            try
            {
                fx.TaoXml("SELECT * FROM HoaDon", "HoaDon", "HoaDon.xml");
                fx.TaoXml("SELECT * FROM Ban", "Ban", "Ban.xml");
                fx.TaoXml("SELECT * FROM NhanVien", "NhanVien", "NhanVien.xml");
                fx.TaoXml("SELECT * FROM KhachHang", "KhachHang", "KhachHang.xml");
                fx.TaoXml("SELECT * FROM ChiTietHoaDon", "ChiTietHoaDon", "ChiTietHoaDon.xml");
                fx.TaoXml("SELECT * FROM MonAn", "MonAn", "MonAn.xml");

                hoaDon = fx.HienThi("HoaDon.xml");
                ban = fx.HienThi("Ban.xml");
                nhanVien = fx.HienThi("NhanVien.xml");
                khachHang = fx.HienThi("KhachHang.xml");
                chiTietHoaDon = fx.HienThi("ChiTietHoaDon.xml");
                monAn = fx.HienThi("MonAn.xml");
                HienThiTheoKhoangNgay();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Không thể tải dữ liệu lịch sử hóa đơn.\n" + ex.Message,
                    "Lỗi tải dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HienThiTheoKhoangNgay()
        {
            if (hoaDon == null || ban == null || nhanVien == null
                || khachHang == null || chiTietHoaDon == null || monAn == null)
                return;

            DateTime tuNgay = dtpTuNgay.Value.Date;
            DateTime denNgay = dtpDenNgay.Value.Date;
            if (tuNgay > denNgay)
            {
                MessageBox.Show(this, "Ngày bắt đầu không thể sau ngày kết thúc.",
                    "Khoảng ngày không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                dangNap = true;
                dgvHoaDon.DataSource = null;
                dgvChiTiet.DataSource = TaoBangChiTietTrong();
                hoaDonDangHienThi = TaoBangHoaDonTrong();
                chiTietDangHienThi = TaoBangChiTietTrong();
                decimal doanhThu = 0m;
                int soHoaDonDaThanhToan = 0;
                DateTime denNgayChuaBaoGom = denNgay.AddDays(1);

                foreach (DataRow dong in hoaDon.Rows)
                {
                    DateTime ngayLap = Convert.ToDateTime(dong["NgayLap"]);
                    if (ngayLap < tuNgay || ngayLap >= denNgayChuaBaoGom)
                        continue;

                    string maHoaDon = Convert.ToString(dong["MaHD"]);
                    DataRow hienThi = hoaDonDangHienThi.NewRow();
                    hienThi["MaHD"] = maHoaDon;
                    hienThi["Ban"] = TimTen(ban, "MaBan", Convert.ToString(dong["MaBan"]), "TenBan");
                    hienThi["NhanVien"] = TimTen(nhanVien, "MaNV", Convert.ToString(dong["MaNV"]), "TenNV");
                    hienThi["Khach"] = dong.IsNull("MaKH")
                        ? ""
                        : TimTen(khachHang, "MaKH", Convert.ToString(dong["MaKH"]), "TenKH");
                    hienThi["NgayLap"] = ngayLap;
                    hienThi["TrangThai"] = Convert.ToString(dong["TrangThai"]);
                    hienThi["Giam"] = Convert.ToDecimal(dong["PhanTramGiam"]);
                    hienThi["TongTien"] = Convert.ToDecimal(dong["TongTien"]);
                    hoaDonDangHienThi.Rows.Add(hienThi);

                    if (Convert.ToString(dong["TrangThai"]) == TrangThaiHoaDonDaThanhToan)
                    {
                        soHoaDonDaThanhToan++;
                        doanhThu += Convert.ToDecimal(dong["TongTien"]);
                    }

                }

                DataView view = hoaDonDangHienThi.DefaultView;
                view.Sort = "NgayLap DESC, MaHD DESC";
                dgvHoaDon.DataSource = view;
                GiaoDien.LamDepBang(dgvHoaDon, new Dictionary<string, string>
                {
                    { "MaHD", "Mã hóa đơn" },
                    { "Ban", "Bàn" },
                    { "NhanVien", "Nhân viên" },
                    { "Khach", "Khách" },
                    { "NgayLap", "Ngày lập" },
                    { "TrangThai", "Trạng thái" },
                    { "Giam", "Giảm (%)" },
                    { "TongTien", "Tổng tiền" }
                });
                dgvHoaDon.Columns["NgayLap"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                GiaoDien.DinhDangTien(dgvHoaDon, "TongTien");
                CapNhatThongKe(soHoaDonDaThanhToan, doanhThu);
                dgvHoaDon.ClearSelection();
                dgvChiTiet.DataSource = chiTietDangHienThi;
                CapNhatNutIn();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Không thể lọc hoặc hiển thị hóa đơn.\n" + ex.Message,
                    "Lỗi hiển thị hóa đơn", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dangNap = false;
            }
        }

        private static string TimTen(DataTable bang, string cotMa, string ma, string cotTen)
        {
            DataRow dong = bang.AsEnumerable().FirstOrDefault(r =>
                string.Equals(Convert.ToString(r[cotMa]), ma, StringComparison.OrdinalIgnoreCase));
            return dong == null || dong.IsNull(cotTen) ? "" : Convert.ToString(dong[cotTen]);
        }

        private void dgvHoaDon_SelectionChanged(object sender, EventArgs e)
        {
            if (dangNap)
                return;

            try
            {
                if (dgvHoaDon.SelectedRows.Count != 1)
                {
                    dgvChiTiet.DataSource = TaoBangChiTietTrong();
                    CapNhatNutIn();
                    return;
                }

                DataGridViewRow hoaDonChon = dgvHoaDon.SelectedRows[0];
                string maHoaDon = Convert.ToString(hoaDonChon.Cells["MaHD"].Value);
                chiTietDangHienThi = TaoChiTietHoaDon(maHoaDon);
                dgvChiTiet.DataSource = chiTietDangHienThi;
                GiaoDien.LamDepBang(dgvChiTiet, new Dictionary<string, string>
                {
                    { "TenMon", "Tên món" },
                    { "SoLuong", "SL" },
                    { "DonGia", "Đơn giá" },
                    { "ThanhTien", "Thành tiền" },
                    { "TrangThaiBep", "Trạng thái bếp" }
                });
                GiaoDien.DinhDangTien(dgvChiTiet, "DonGia", "ThanhTien");
                CapNhatNutIn();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Không thể hiển thị chi tiết hóa đơn.\n" + ex.Message,
                    "Lỗi chi tiết hóa đơn", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable TaoChiTietHoaDon(string maHoaDon)
        {
            DataTable ketQua = TaoBangChiTietTrong();
            foreach (DataRow dong in chiTietHoaDon.Rows)
            {
                if (!string.Equals(Convert.ToString(dong["MaHD"]), maHoaDon,
                    StringComparison.OrdinalIgnoreCase))
                    continue;

                int soLuong = Convert.ToInt32(dong["SoLuong"]);
                decimal donGia = Convert.ToDecimal(dong["DonGia"]);
                DataRow chiTiet = ketQua.NewRow();
                chiTiet["TenMon"] = TimTen(monAn, "MaMon", Convert.ToString(dong["MaMon"]), "TenMon");
                chiTiet["SoLuong"] = soLuong;
                chiTiet["DonGia"] = donGia;
                chiTiet["ThanhTien"] = soLuong * donGia;
                chiTiet["TrangThaiBep"] = Convert.ToString(dong["TrangThaiBep"]);
                ketQua.Rows.Add(chiTiet);
            }
            return ketQua;
        }

        private void CapNhatNutIn()
        {
            btnInHoaDon.Enabled = dgvHoaDon.SelectedRows.Count == 1
                && Convert.ToString(dgvHoaDon.SelectedRows[0].Cells["TrangThai"].Value)
                    == TrangThaiHoaDonDaThanhToan;
        }

        private void btnInHoaDon_Click(object sender, EventArgs e)
        {
            if (!btnInHoaDon.Enabled)
                return;

            MessageBox.Show(this, "Chức năng in hóa đơn sẽ làm ở T8.",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CapNhatThongKe(int soHoaDon, decimal doanhThu)
        {
            lblSoHoaDon.Text = "Số hóa đơn đã thanh toán: " + soHoaDon;
            lblDoanhThu.Text = "Tổng doanh thu: " + doanhThu.ToString("#,##0", new CultureInfo("vi-VN")) + " đ";
        }

        private static DataTable TaoBangHoaDonTrong()
        {
            var bang = new DataTable("HoaDonHienThi");
            bang.Columns.Add("MaHD", typeof(string));
            bang.Columns.Add("Ban", typeof(string));
            bang.Columns.Add("NhanVien", typeof(string));
            bang.Columns.Add("Khach", typeof(string));
            bang.Columns.Add("NgayLap", typeof(DateTime));
            bang.Columns.Add("TrangThai", typeof(string));
            bang.Columns.Add("Giam", typeof(decimal));
            bang.Columns.Add("TongTien", typeof(decimal));
            return bang;
        }

        private static DataTable TaoBangChiTietTrong()
        {
            var bang = new DataTable("ChiTietHienThi");
            bang.Columns.Add("TenMon", typeof(string));
            bang.Columns.Add("SoLuong", typeof(int));
            bang.Columns.Add("DonGia", typeof(decimal));
            bang.Columns.Add("ThanhTien", typeof(decimal));
            bang.Columns.Add("TrangThaiBep", typeof(string));
            return bang;
        }
    }
}
