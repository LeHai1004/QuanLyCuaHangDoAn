using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using QuanLyCuaHangDoAn.Class;

namespace QuanLyCuaHangDoAn.GUI
{
    public class frmBep : Form
    {
        private const string TrangThaiHoaDonDangPhucVu = "Đang phục vụ";
        private const string TrangThaiBepCho = "Chờ";
        private const string TrangThaiBepDangLam = "Đang làm";
        private const string TrangThaiBepXong = "Xong";
        private const int PhutGiuCotXong = 5;

        private readonly FileXml fx = new FileXml();
        private readonly Timer tmrLamMoi = new Timer();
        private readonly Dictionary<string, DateTime> thoiGianXong = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

        private readonly TableLayoutPanel tblCot = new TableLayoutPanel();
        private readonly FlowLayoutPanel flpCho = new FlowLayoutPanel();
        private readonly FlowLayoutPanel flpDangLam = new FlowLayoutPanel();
        private readonly FlowLayoutPanel flpXong = new FlowLayoutPanel();

        private readonly Label lblTieuDeCho = new Label();
        private readonly Label lblTieuDeDangLam = new Label();
        private readonly Label lblTieuDeXong = new Label();

        private readonly Label lblTrangThai = new Label();
        private readonly Label lblThongKe = new Label();
        private readonly Button btnLamMoi = GiaoDien.TaoNut("Làm mới ngay", GiaoDien.MauChinh);

        private bool dangXuLy;

        public frmBep()
        {
            KhoiTaoGiaoDien();
            Load += frmBep_Load;
            FormClosing += frmBep_FormClosing;
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Màn hình bếp chế biến";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1000, 620);
            BackColor = Color.White;
            Font = GiaoDien.FontThuong;

            // Bật DoubleBuffered chống giật lag
            BatDoubleBuffer(this);

            // Thanh tiêu đề cam phía trên
            Label lblTieuDe = GiaoDien.TaoTieuDe("MÀN HÌNH BẾP");
            Controls.Add(lblTieuDe);

            // Thanh công cụ / thông tin trạng thái
            Panel pnlThanhCongCu = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = GiaoDien.MauNenNhat,
                Padding = new Padding(12, 6, 12, 6)
            };

            lblTrangThai.Text = "Tự động làm mới mỗi 4 giây";
            lblTrangThai.Font = GiaoDien.FontThuong;
            lblTrangThai.ForeColor = Color.FromArgb(90, 90, 90);
            lblTrangThai.AutoSize = true;
            lblTrangThai.Location = new Point(14, 12);

            lblThongKe.Text = "Chờ: 0  |  Đang làm: 0  |  Xong: 0";
            lblThongKe.Font = GiaoDien.FontDam;
            lblThongKe.ForeColor = GiaoDien.MauChinhDam;
            lblThongKe.AutoSize = true;
            lblThongKe.Location = new Point(320, 12);

            btnLamMoi.Size = new Size(130, 32);
            btnLamMoi.Dock = DockStyle.Right;
            btnLamMoi.Font = GiaoDien.FontDam;
            btnLamMoi.Click += btnLamMoi_Click;

            pnlThanhCongCu.Controls.Add(lblTrangThai);
            pnlThanhCongCu.Controls.Add(lblThongKe);
            pnlThanhCongCu.Controls.Add(btnLamMoi);
            Controls.Add(pnlThanhCongCu);

            // 3 cột thẻ: Chờ | Đang làm | Xong
            tblCot.Dock = DockStyle.Fill;
            tblCot.ColumnCount = 3;
            tblCot.RowCount = 1;
            tblCot.Padding = new Padding(8);
            tblCot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            tblCot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34f));
            tblCot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));

            Panel pnlCotCho = TaoPanelCot("CHỜ CHẾ BIẾN", Color.FromArgb(230, 100, 20), lblTieuDeCho, flpCho);
            Panel pnlCotDangLam = TaoPanelCot("ĐANG LÀM", Color.FromArgb(20, 120, 200), lblTieuDeDangLam, flpDangLam);
            Panel pnlCotXong = TaoPanelCot("ĐÃ XONG", GiaoDien.MauXanh, lblTieuDeXong, flpXong);

            tblCot.Controls.Add(pnlCotCho, 0, 0);
            tblCot.Controls.Add(pnlCotDangLam, 1, 0);
            tblCot.Controls.Add(pnlCotXong, 2, 0);
            Controls.Add(tblCot);

            // Cấu hình Timer tự động làm mới mỗi 4 giây (3 đến 5 giây)
            tmrLamMoi.Interval = 4000;
            tmrLamMoi.Tick += tmrLamMoi_Tick;
        }

        private Panel TaoPanelCot(string tieuDe, Color mauTieuDe, Label lblTieuDe, FlowLayoutPanel flp)
        {
            Panel pnl = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(246, 247, 249),
                Margin = new Padding(6)
            };

            lblTieuDe.Text = tieuDe + " (0)";
            lblTieuDe.Dock = DockStyle.Top;
            lblTieuDe.Height = 44;
            lblTieuDe.BackColor = mauTieuDe;
            lblTieuDe.ForeColor = Color.White;
            lblTieuDe.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTieuDe.TextAlign = ContentAlignment.MiddleCenter;

            flp.Dock = DockStyle.Fill;
            flp.AutoScroll = true;
            flp.WrapContents = false;
            flp.FlowDirection = FlowDirection.TopDown;
            flp.Padding = new Padding(8);
            flp.BackColor = Color.FromArgb(246, 247, 249);
            flp.ClientSizeChanged += (s, e) => CapNhatKichThuocTheTrongCot(flp);

            BatDoubleBuffer(flp);

            pnl.Controls.Add(flp);
            pnl.Controls.Add(lblTieuDe);
            return pnl;
        }

        private static void BatDoubleBuffer(Control control)
        {
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { true });
        }

        private void frmBep_Load(object sender, EventArgs e)
        {
            if (!KiemTraQuyenBep())
            {
                Close();
                return;
            }

            TaiVaLamMoiDuLieu(true);
            tmrLamMoi.Start();
        }

        private void frmBep_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmrLamMoi.Stop();
            tmrLamMoi.Dispose();
        }

        private bool KiemTraQuyenBep()
        {
            if (!PhienDangNhap.IsLoggedIn)
            {
                MessageBox.Show("Vui lòng đăng nhập trước khi sử dụng màn hình bếp.",
                    "Chưa đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (PhienDangNhap.ChucVu != "Admin" && PhienDangNhap.ChucVu != "Bep")
            {
                MessageBox.Show("Bạn không có quyền sử dụng chức năng màn hình bếp.",
                    "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            TaiVaLamMoiDuLieu(true);
        }

        private void tmrLamMoi_Tick(object sender, EventArgs e)
        {
            TaiVaLamMoiDuLieu(false);
        }

        /// <summary>
        /// Gọi fx.TaoXml để lấy dữ liệu mới nhất từ CSDL và làm mới giao diện 3 cột thẻ.
        /// </summary>
        private void TaiVaLamMoiDuLieu(bool hienThiThongBaoLoi)
        {
            if (dangXuLy) return;
            dangXuLy = true;

            try
            {
                // Lấy dữ liệu mới từ DB ra file XML
                fx.TaoXml("SELECT * FROM Ban", "Ban", "Ban.xml");
                fx.TaoXml("SELECT * FROM MonAn", "MonAn", "MonAn.xml");
                fx.TaoXml("SELECT * FROM HoaDon", "HoaDon", "HoaDon.xml");
                fx.TaoXml("SELECT * FROM ChiTietHoaDon", "ChiTietHoaDon", "ChiTietHoaDon.xml");

                DataTable dtBan = fx.HienThi("Ban.xml");
                DataTable dtMonAn = fx.HienThi("MonAn.xml");
                DataTable dtHoaDon = fx.HienThi("HoaDon.xml");
                DataTable dtChiTiet = fx.HienThi("ChiTietHoaDon.xml");

                // Bản đồ ánh xạ mã bàn -> tên bàn
                var mapTenBan = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (DataRow r in dtBan.Rows)
                {
                    string maBan = Convert.ToString(r["MaBan"]);
                    string tenBan = Convert.ToString(r["TenBan"]);
                    if (!string.IsNullOrEmpty(maBan))
                        mapTenBan[maBan] = tenBan;
                }

                // Bản đồ ánh xạ mã món -> tên món
                var mapTenMon = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (DataRow r in dtMonAn.Rows)
                {
                    string maMon = Convert.ToString(r["MaMon"]);
                    string tenMon = Convert.ToString(r["TenMon"]);
                    if (!string.IsNullOrEmpty(maMon))
                        mapTenMon[maMon] = tenMon;
                }

                // Chỉ lấy món thuộc hóa đơn "Đang phục vụ"
                var mapHoaDon = new Dictionary<string, DataRow>(StringComparer.OrdinalIgnoreCase);
                foreach (DataRow r in dtHoaDon.Rows)
                {
                    string maHD = Convert.ToString(r["MaHD"]);
                    string trangThaiHD = Convert.ToString(r["TrangThai"]);
                    if (string.Equals(trangThaiHD, TrangThaiHoaDonDangPhucVu, StringComparison.OrdinalIgnoreCase))
                    {
                        mapHoaDon[maHD] = r;
                    }
                }

                var dsCho = new List<MonBepItem>();
                var dsDangLam = new List<MonBepItem>();
                var dsXong = new List<MonBepItem>();

                DateTime bayGio = DateTime.Now;

                foreach (DataRow r in dtChiTiet.Rows)
                {
                    string maHD = Convert.ToString(r["MaHD"]);
                    if (!mapHoaDon.ContainsKey(maHD))
                        continue;

                    string maMon = Convert.ToString(r["MaMon"]);
                    int soLuong = Convert.ToInt32(r["SoLuong"]);
                    string trangThaiBep = Convert.ToString(r["TrangThaiBep"]);

                    DataRow rowHD = mapHoaDon[maHD];
                    string maBan = Convert.ToString(rowHD["MaBan"]);
                    string tenBan = mapTenBan.ContainsKey(maBan) ? mapTenBan[maBan] : maBan;
                    string tenMon = mapTenMon.ContainsKey(maMon) ? mapTenMon[maMon] : maMon;

                    DateTime ngayLap = bayGio;
                    if (rowHD["NgayLap"] != DBNull.Value && DateTime.TryParse(Convert.ToString(rowHD["NgayLap"]), out DateTime dtLap))
                    {
                        ngayLap = dtLap;
                    }

                    var item = new MonBepItem
                    {
                        MaHD = maHD,
                        MaMon = maMon,
                        TenBan = tenBan,
                        TenMon = tenMon,
                        SoLuong = soLuong,
                        NgayLap = ngayLap,
                        TrangThaiBep = trangThaiBep
                    };

                    string khoa = maHD + "#" + maMon;

                    if (string.Equals(trangThaiBep, TrangThaiBepCho, StringComparison.OrdinalIgnoreCase))
                    {
                        dsCho.Add(item);
                    }
                    else if (string.Equals(trangThaiBep, TrangThaiBepDangLam, StringComparison.OrdinalIgnoreCase))
                    {
                        dsDangLam.Add(item);
                    }
                    else if (string.Equals(trangThaiBep, TrangThaiBepXong, StringComparison.OrdinalIgnoreCase))
                    {
                        // Cột Xong chỉ giữ vài phút gần nhất (PhutGiuCotXong = 5 phút)
                        DateTime tgXong;
                        if (!thoiGianXong.TryGetValue(khoa, out tgXong))
                        {
                            if ((bayGio - ngayLap).TotalMinutes <= PhutGiuCotXong)
                            {
                                tgXong = bayGio;
                                thoiGianXong[khoa] = tgXong;
                                dsXong.Add(item);
                            }
                        }
                        else
                        {
                            if ((bayGio - tgXong).TotalMinutes <= PhutGiuCotXong)
                            {
                                dsXong.Add(item);
                            }
                        }
                    }
                }

                // Sắp xếp: Chờ và Đang làm xếp theo giờ gọi trước đến sau (FIFO)
                dsCho = dsCho.OrderBy(x => x.NgayLap).ToList();
                dsDangLam = dsDangLam.OrderBy(x => x.NgayLap).ToList();
                // Xong xếp theo thứ tự mới làm xong gần nhất lên trước
                dsXong = dsXong.OrderByDescending(x =>
                {
                    string k = x.MaHD + "#" + x.MaMon;
                    return thoiGianXong.ContainsKey(k) ? thoiGianXong[k] : x.NgayLap;
                }).ToList();

                // Dọn dẹp cache thời gian xong của các món đã quá 30 phút để tránh tốn bộ nhớ
                var dsKhoaCanXoa = thoiGianXong.Where(kv => (bayGio - kv.Value).TotalMinutes > 30).Select(kv => kv.Key).ToList();
                foreach (string k in dsKhoaCanXoa)
                    thoiGianXong.Remove(k);

                // Hiển thị lên 3 cột thẻ
                HienThiDanhSachThe(flpCho, dsCho, TrangThaiBepCho);
                HienThiDanhSachThe(flpDangLam, dsDangLam, TrangThaiBepDangLam);
                HienThiDanhSachThe(flpXong, dsXong, TrangThaiBepXong);

                // Cập nhật số lượng tiêu đề cột và thanh công cụ
                lblTieuDeCho.Text = string.Format("CHỜ CHẾ BIẾN ({0})", dsCho.Count);
                lblTieuDeDangLam.Text = string.Format("ĐANG LÀM ({0})", dsDangLam.Count);
                lblTieuDeXong.Text = string.Format("ĐÃ XONG ({0})", dsXong.Count);

                lblThongKe.Text = string.Format("Chờ: {0}  |  Đang làm: {1}  |  Xong: {2}",
                    dsCho.Count, dsDangLam.Count, dsXong.Count);
                lblTrangThai.Text = "Tự động làm mới mỗi 4 giây | Cập nhật lúc: " + bayGio.ToString("HH:mm:ss");
            }
            catch (Exception ex)
            {
                lblTrangThai.Text = "Lỗi làm mới lúc: " + DateTime.Now.ToString("HH:mm:ss");
                if (hienThiThongBaoLoi)
                {
                    MessageBox.Show("Không thể tải dữ liệu bếp: " + ex.Message, "Lỗi tải dữ liệu",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            finally
            {
                dangXuLy = false;
            }
        }

        private void HienThiDanhSachThe(FlowLayoutPanel flp, List<MonBepItem> danhSach, string loaiCot)
        {
            flp.SuspendLayout();
            try
            {
                foreach (Control c in flp.Controls.Cast<Control>().ToList())
                {
                    flp.Controls.Remove(c);
                    c.Dispose();
                }

                int chieuRong = Math.Max(flp.ClientSize.Width - 20, 240);

                if (danhSach.Count == 0)
                {
                    var lblTrong = new Label
                    {
                        Text = loaiCot == TrangThaiBepCho
                            ? "Không có món nào đang chờ"
                            : (loaiCot == TrangThaiBepDangLam ? "Không có món đang chế biến" : "Không có món vừa làm xong"),
                        Font = new Font("Segoe UI", 11F, FontStyle.Italic),
                        ForeColor = Color.FromArgb(140, 140, 140),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Height = 60,
                        Width = chieuRong,
                        Margin = new Padding(0, 20, 0, 0)
                    };
                    flp.Controls.Add(lblTrong);
                }
                else
                {
                    foreach (MonBepItem item in danhSach)
                    {
                        Control card = TaoTheMon(item, loaiCot, chieuRong);
                        flp.Controls.Add(card);
                    }
                }
            }
            finally
            {
                flp.ResumeLayout(true);
            }
        }

        private Control TaoTheMon(MonBepItem item, string loaiCot, int chieuRong)
        {
            Panel card = new Panel
            {
                Width = chieuRong,
                Height = 158,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 10),
                Padding = new Padding(12, 10, 12, 10)
            };

            // Vẽ viền và dải màu chỉ thị bên trái
            Color mauDai;
            if (loaiCot == TrangThaiBepCho)
                mauDai = Color.FromArgb(230, 100, 20); // Cam
            else if (loaiCot == TrangThaiBepDangLam)
                mauDai = Color.FromArgb(20, 120, 200); // Xanh dương
            else
                mauDai = GiaoDien.MauXanh; // Xanh lá

            card.Paint += (s, e) =>
            {
                Graphics g = e.Graphics;
                using (var penVien = new Pen(Color.FromArgb(220, 222, 226), 1))
                {
                    g.DrawRectangle(penVien, 0, 0, card.Width - 1, card.Height - 1);
                }
                using (var brushDai = new SolidBrush(mauDai))
                {
                    g.FillRectangle(brushDai, 0, 0, 6, card.Height);
                }
            };

            // Dòng 1: Tên bàn (trái) và Giờ gọi (phải)
            Label lblBan = new Label
            {
                Text = item.TenBan,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                AutoSize = true,
                Location = new Point(14, 10)
            };

            Label lblGioGoi = new Label
            {
                Text = "Gọi lúc: " + item.NgayLap.ToString("HH:mm:ss"),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(110, 110, 110),
                AutoSize = true,
                Location = new Point(card.Width - 145, 12),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // Dòng 2: Tên món ăn (Chữ to, dễ đọc)
            Label lblTenMon = new Label
            {
                Text = item.TenMon,
                Font = new Font("Segoe UI", 13.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 20),
                Location = new Point(14, 38),
                Size = new Size(card.Width - 28, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                AutoEllipsis = true
            };

            // Dòng 3: Số lượng món
            Label lblSoLuong = new Label
            {
                Text = "Số lượng: " + item.SoLuong,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = GiaoDien.MauDo,
                Location = new Point(14, 70),
                AutoSize = true
            };

            // Dòng 4: Nút thao tác đổi trạng thái
            if (loaiCot == TrangThaiBepCho)
            {
                Button btnNhanLam = GiaoDien.TaoNut("Nhận làm", Color.FromArgb(20, 120, 200));
                btnNhanLam.Height = 36;
                btnNhanLam.Width = card.Width - 28;
                btnNhanLam.Location = new Point(14, 108);
                btnNhanLam.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                btnNhanLam.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
                btnNhanLam.Click += (s, e) => DoiTrangThai(item.MaHD, item.MaMon, TrangThaiBepDangLam);
                card.Controls.Add(btnNhanLam);
            }
            else if (loaiCot == TrangThaiBepDangLam)
            {
                Button btnLamXong = GiaoDien.TaoNut("Làm xong", GiaoDien.MauXanh);
                btnLamXong.Height = 36;
                btnLamXong.Width = card.Width - 28;
                btnLamXong.Location = new Point(14, 108);
                btnLamXong.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                btnLamXong.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
                btnLamXong.Click += (s, e) => DoiTrangThai(item.MaHD, item.MaMon, TrangThaiBepXong);
                card.Controls.Add(btnLamXong);
            }
            else
            {
                Label lblXong = new Label
                {
                    Text = "✓ Đã làm xong (chờ phục vụ)",
                    Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                    ForeColor = GiaoDien.MauXanh,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Height = 34,
                    Width = card.Width - 28,
                    Location = new Point(14, 108),
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
                };
                card.Controls.Add(lblXong);
            }

            card.Controls.Add(lblBan);
            card.Controls.Add(lblGioGoi);
            card.Controls.Add(lblTenMon);
            card.Controls.Add(lblSoLuong);

            return card;
        }

        private void CapNhatKichThuocTheTrongCot(FlowLayoutPanel flp)
        {
            int chieuRongMoi = Math.Max(flp.ClientSize.Width - 20, 240);
            flp.SuspendLayout();
            try
            {
                foreach (Control c in flp.Controls)
                {
                    if (c is Panel)
                    {
                        c.Width = chieuRongMoi;
                    }
                }
            }
            finally
            {
                flp.ResumeLayout(true);
            }
        }

        /// <summary>
        /// Cập nhật TrangThaiBep trực tiếp vào DB bằng SqlParameter (khóa MaHD + MaMon) rồi làm mới.
        /// </summary>
        private void DoiTrangThai(string maHD, string maMon, string trangThaiMoi)
        {
            if (!KiemTraQuyenBep())
                return;

            try
            {
                string sql = "UPDATE ChiTietHoaDon SET TrangThaiBep = @TrangThaiBep WHERE MaHD = @MaHD AND MaMon = @MaMon";
                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@TrangThaiBep", SqlDbType.NVarChar, 10).Value = trangThaiMoi;
                    cmd.Parameters.Add("@MaHD", SqlDbType.NVarChar, 10).Value = maHD;
                    cmd.Parameters.Add("@MaMon", SqlDbType.NVarChar, 10).Value = maMon;

                    conn.Open();
                    int soDong = cmd.ExecuteNonQuery();
                    if (soDong == 0)
                    {
                        MessageBox.Show("Không tìm thấy món để cập nhật. Vui lòng làm mới dữ liệu.",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Ghi nhận thời điểm hoàn thành để giữ trong cột Xong vài phút gần nhất
                if (trangThaiMoi == TrangThaiBepXong)
                {
                    string khoa = maHD + "#" + maMon;
                    thoiGianXong[khoa] = DateTime.Now;
                }

                // Làm mới dữ liệu từ CSDL
                TaiVaLamMoiDuLieu(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật trạng thái món: " + ex.Message, "Lỗi cập nhật",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private sealed class MonBepItem
        {
            public string MaHD { get; set; }
            public string MaMon { get; set; }
            public string TenBan { get; set; }
            public string TenMon { get; set; }
            public int SoLuong { get; set; }
            public DateTime NgayLap { get; set; }
            public string TrangThaiBep { get; set; }
        }
    }
}
