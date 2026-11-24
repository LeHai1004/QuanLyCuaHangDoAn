using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using QuanLyCuaHangDoAn;
using QuanLyCuaHangDoAn.Class;

namespace QuanLyCuaHangDoAn.GUI
{
    public class frmPhieuNhap : Form
    {
        private readonly FileXml fx = new FileXml();
        private readonly SplitContainer split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None
        };

        private readonly TextBox txtMaPN = new TextBox();
        private readonly ComboBox cboNhaCungCap = new ComboBox();
        private readonly ComboBox cboNguyenLieu = new ComboBox();
        private readonly DateTimePicker dtpNgayNhap = new DateTimePicker();
        private readonly TextBox txtSoLuong = new TextBox();
        private readonly TextBox txtDonGia = new TextBox();
        private readonly DataGridView dgvChiTietNhap = new DataGridView();
        private readonly DataGridView dgvPhieuNhap = new DataGridView();
        private readonly DataGridView dgvChiTietLichSu = new DataGridView();
        private readonly Label lblTongTien = new Label();
        private readonly Button btnThemDong = GiaoDien.TaoNut("Thêm dòng", GiaoDien.MauChinh);
        private readonly Button btnXoaDong = GiaoDien.TaoNut("Xóa dòng", GiaoDien.MauDo);
        private readonly Button btnLuu = GiaoDien.TaoNut("Lưu phiếu", GiaoDien.MauXanh);
        private readonly Button btnHuy = GiaoDien.TaoNut("Hủy", GiaoDien.MauChinhDam);

        private DataTable nhaCungCap;
        private DataTable nguyenLieu;
        private DataTable chiTietNhap;

        public frmPhieuNhap()
        {
            KhoiTaoGiaoDien();
            Load += frmPhieuNhap_Load;
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Quản lý phiếu nhập hàng";
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = GiaoDien.FontThuong;
            ClientSize = new Size(1280, 760);
            MinimumSize = new Size(1050, 620);

            txtMaPN.Name = "txtMaPN";
            cboNhaCungCap.Name = "cboNhaCungCap";
            cboNguyenLieu.Name = "cboNguyenLieu";
            dtpNgayNhap.Name = "dtpNgayNhap";
            txtSoLuong.Name = "txtSoLuong";
            txtDonGia.Name = "txtDonGia";
            dgvChiTietNhap.Name = "dgvChiTietNhap";
            dgvPhieuNhap.Name = "dgvPhieuNhap";
            dgvChiTietLichSu.Name = "dgvChiTietLichSu";
            lblTongTien.Name = "lblTongTien";

            txtMaPN.ReadOnly = true;
            cboNhaCungCap.DropDownStyle = ComboBoxStyle.DropDownList;
            cboNguyenLieu.DropDownStyle = ComboBoxStyle.DropDownList;
            dtpNgayNhap.Format = DateTimePickerFormat.Custom;
            dtpNgayNhap.CustomFormat = "dd/MM/yyyy HH:mm";
            dtpNgayNhap.Value = DateTime.Now;

            split.Panel1.Controls.Add(TaoKhuVucLapPhieu());
            split.Panel2.Controls.Add(TaoKhuVucLichSu());
            Controls.Add(split);
            Controls.Add(GiaoDien.TaoTieuDe("PHIẾU NHẬP HÀNG"));

            btnThemDong.Click += btnThemDong_Click;
            btnXoaDong.Click += btnXoaDong_Click;
            btnLuu.Click += btnLuu_Click;
            btnHuy.Click += btnHuy_Click;
            dgvPhieuNhap.SelectionChanged += dgvPhieuNhap_SelectionChanged;
        }

        private Control TaoKhuVucLapPhieu()
        {
            var khuVuc = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                ColumnCount = 1,
                RowCount = 9
            };
            for (int i = 0; i < 6; i++)
                khuVuc.RowStyles.Add(new RowStyle(SizeType.Absolute, 74));
            khuVuc.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            khuVuc.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            khuVuc.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));

            ThemTruong(khuVuc, "Mã phiếu nhập", txtMaPN, 0);
            ThemTruong(khuVuc, "Nhà cung cấp (*)", cboNhaCungCap, 1);
            ThemTruong(khuVuc, "Ngày nhập", dtpNgayNhap, 2);
            ThemTruong(khuVuc, "Nguyên liệu (*)", cboNguyenLieu, 3);
            ThemTruong(khuVuc, "Số lượng nhập (*)", txtSoLuong, 4);
            ThemTruong(khuVuc, "Đơn giá nhập (*)", txtDonGia, 5);

            var nutDong = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = false,
                Padding = new Padding(0, 5, 0, 0)
            };
            btnThemDong.Width = 110;
            btnXoaDong.Width = 105;
            nutDong.Controls.Add(btnThemDong);
            nutDong.Controls.Add(btnXoaDong);
            khuVuc.Controls.Add(nutDong, 0, 6);

            dgvChiTietNhap.Dock = DockStyle.Fill;
            dgvChiTietNhap.AllowUserToAddRows = false;
            dgvChiTietNhap.ReadOnly = true;
            dgvChiTietNhap.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            khuVuc.Controls.Add(dgvChiTietNhap, 0, 7);

            var tongVaNut = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(0, 6, 0, 0)
            };
            tongVaNut.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tongVaNut.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));
            lblTongTien.Text = "Tổng tiền nhập: 0 đ";
            lblTongTien.Dock = DockStyle.Fill;
            lblTongTien.TextAlign = ContentAlignment.MiddleLeft;
            lblTongTien.Font = GiaoDien.FontDam;
            tongVaNut.Controls.Add(lblTongTien, 0, 0);
            var nutLuu = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
            btnLuu.Width = 105;
            btnHuy.Width = 85;
            nutLuu.Controls.Add(btnLuu);
            nutLuu.Controls.Add(btnHuy);
            tongVaNut.Controls.Add(nutLuu, 1, 0);
            khuVuc.Controls.Add(tongVaNut, 0, 8);
            return khuVuc;
        }

        private Control TaoKhuVucLichSu()
        {
            var khuVuc = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                ColumnCount = 1,
                RowCount = 4
            };
            khuVuc.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            khuVuc.RowStyles.Add(new RowStyle(SizeType.Percent, 48));
            khuVuc.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            khuVuc.RowStyles.Add(new RowStyle(SizeType.Percent, 52));

            khuVuc.Controls.Add(TaoNhan("LỊCH SỬ PHIẾU NHẬP"), 0, 0);
            dgvPhieuNhap.Dock = DockStyle.Fill;
            dgvPhieuNhap.ReadOnly = true;
            dgvPhieuNhap.AllowUserToAddRows = false;
            dgvPhieuNhap.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            khuVuc.Controls.Add(dgvPhieuNhap, 0, 1);
            khuVuc.Controls.Add(TaoNhan("CHI TIẾT PHIẾU ĐANG CHỌN"), 0, 2);
            dgvChiTietLichSu.Dock = DockStyle.Fill;
            dgvChiTietLichSu.ReadOnly = true;
            dgvChiTietLichSu.AllowUserToAddRows = false;
            dgvChiTietLichSu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            khuVuc.Controls.Add(dgvChiTietLichSu, 0, 3);
            return khuVuc;
        }

        private static Label TaoNhan(string noiDung)
        {
            return new Label
            {
                Text = noiDung,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = GiaoDien.FontDam,
                ForeColor = Color.FromArgb(120, 50, 0)
            };
        }

        private static void ThemTruong(TableLayoutPanel bang, string nhan, Control control, int dong)
        {
            var cum = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = new Padding(0)
            };
            cum.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
            cum.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            control.Dock = DockStyle.Fill;
            control.Margin = new Padding(0, 2, 0, 6);
            cum.Controls.Add(new Label
            {
                Text = nhan,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft
            }, 0, 0);
            cum.Controls.Add(control, 0, 1);
            bang.Controls.Add(cum, 0, dong);
        }

        private void frmPhieuNhap_Load(object sender, EventArgs e)
        {
            if (split.Width > 850)
            {
                split.Panel1MinSize = 500;
                split.Panel2MinSize = 450;
                split.SplitterDistance = 650;
            }

            if (PhienDangNhap.IsLoggedIn && PhienDangNhap.ChucVu != "Admin")
            {
                MessageBox.Show("Chức năng chỉ dành cho quản trị viên (Admin).", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }

            try
            {
                TaiDuLieu();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể tải dữ liệu phiếu nhập", ex);
            }
        }

        private void TaiDuLieu()
        {
            fx.TaoXml("SELECT * FROM NhaCungCap", "NhaCungCap", "NhaCungCap.xml");
            fx.TaoXml("SELECT * FROM NguyenLieu", "NguyenLieu", "NguyenLieu.xml");
            fx.TaoXml("SELECT * FROM PhieuNhap", "PhieuNhap", "PhieuNhap.xml");
            fx.TaoXml("SELECT * FROM ChiTietPhieuNhap", "ChiTietPhieuNhap", "ChiTietPhieuNhap.xml");
            nhaCungCap = fx.HienThi("NhaCungCap.xml");
            nguyenLieu = fx.HienThi("NguyenLieu.xml");
            cboNhaCungCap.DataSource = TaoBangNhaCungCapHienThi();
            cboNhaCungCap.DisplayMember = "TenHienThi";
            cboNhaCungCap.ValueMember = "MaNCC";
            cboNguyenLieu.DataSource = TaoBangNguyenLieuHienThi();
            cboNguyenLieu.DisplayMember = "TenHienThi";
            cboNguyenLieu.ValueMember = "MaNL";
            TaoChiTietNhap();
            TaoMaPhieuMoi();
            TaiLichSuPhieuNhap();
        }

        private DataTable TaoBangNhaCungCapHienThi()
        {
            var bang = nhaCungCap.Copy();
            bang.Columns.Add("TenHienThi", typeof(string));
            foreach (DataRow dong in bang.Rows)
                dong["TenHienThi"] = dong["MaNCC"] + " - " + dong["TenNCC"];
            return bang;
        }

        private DataTable TaoBangNguyenLieuHienThi()
        {
            var bang = nguyenLieu.Copy();
            bang.Columns.Add("TenHienThi", typeof(string));
            foreach (DataRow dong in bang.Rows)
                dong["TenHienThi"] = dong["MaNL"] + " - " + dong["TenNL"] + " (" + dong["DVT"] + ")";
            return bang;
        }

        private void TaoChiTietNhap()
        {
            chiTietNhap = new DataTable();
            chiTietNhap.Columns.Add("MaNL", typeof(string));
            chiTietNhap.Columns.Add("TenNL", typeof(string));
            chiTietNhap.Columns.Add("DVT", typeof(string));
            chiTietNhap.Columns.Add("SoLuong", typeof(decimal));
            chiTietNhap.Columns.Add("DonGia", typeof(decimal));
            chiTietNhap.Columns.Add("ThanhTien", typeof(decimal));
            HienThiChiTietNhap();
        }

        private void TaoMaPhieuMoi()
        {
            txtMaPN.Text = fx.TaoMa("PN", "PhieuNhap.xml", "MaPN");
            dtpNgayNhap.Value = DateTime.Now;
            cboNhaCungCap.SelectedIndex = -1;
            cboNguyenLieu.SelectedIndex = -1;
            txtSoLuong.Clear();
            txtDonGia.Clear();
        }

        private void HienThiChiTietNhap()
        {
            dgvChiTietNhap.DataSource = chiTietNhap;
            GiaoDien.LamDepBang(dgvChiTietNhap, new Dictionary<string, string>
            {
                { "MaNL", "Mã NL" }, { "TenNL", "Tên nguyên liệu" },
                { "DVT", "Đơn vị tính" }, { "SoLuong", "Số lượng" },
                { "DonGia", "Đơn giá" }, { "ThanhTien", "Thành tiền" }
            });
            if (dgvChiTietNhap.Columns.Contains("SoLuong"))
                dgvChiTietNhap.Columns["SoLuong"].DefaultCellStyle.Format = "#,##0.##";
            GiaoDien.DinhDangTien(dgvChiTietNhap, "DonGia", "ThanhTien");
            CapNhatTongTien();
        }

        private void CapNhatTongTien()
        {
            decimal tong = 0m;
            if (chiTietNhap != null)
                foreach (DataRow dong in chiTietNhap.Rows) tong += Convert.ToDecimal(dong["ThanhTien"]);
            lblTongTien.Text = "Tổng tiền nhập: " + tong.ToString("#,##0", new CultureInfo("vi-VN")) + " đ";
        }

        private void btnThemDong_Click(object sender, EventArgs e)
        {
            if (cboNguyenLieu.SelectedValue == null)
            {
                BaoLoi("Vui lòng chọn nguyên liệu.");
                return;
            }
            decimal soLuong;
            decimal donGia;
            if (!TryLaySo(txtSoLuong.Text, out soLuong) || soLuong <= 0m)
            {
                BaoLoi("Số lượng nhập phải là số lớn hơn 0.");
                txtSoLuong.Focus();
                return;
            }
            if (!TryLaySo(txtDonGia.Text, out donGia) || donGia < 0m)
            {
                BaoLoi("Đơn giá nhập phải là số lớn hơn hoặc bằng 0.");
                txtDonGia.Focus();
                return;
            }

            string maNL = cboNguyenLieu.SelectedValue.ToString();
            foreach (DataRow dong in chiTietNhap.Rows)
            {
                if (Convert.ToString(dong["MaNL"]) == maNL)
                {
                    BaoLoi("Nguyên liệu này đã có trong phiếu. Vui lòng xóa dòng cũ rồi thêm lại.");
                    return;
                }
            }

            DataRow nguyenLieuChon = TimNguyenLieu(maNL);
            if (nguyenLieuChon == null)
            {
                BaoLoi("Không tìm thấy nguyên liệu đã chọn.");
                return;
            }
            DataRow dongMoi = chiTietNhap.NewRow();
            dongMoi["MaNL"] = maNL;
            dongMoi["TenNL"] = nguyenLieuChon["TenNL"];
            dongMoi["DVT"] = nguyenLieuChon["DVT"];
            dongMoi["SoLuong"] = decimal.Round(soLuong, 2);
            dongMoi["DonGia"] = decimal.Round(donGia, 0);
            dongMoi["ThanhTien"] = decimal.Round(soLuong * donGia, 0);
            chiTietNhap.Rows.Add(dongMoi);
            HienThiChiTietNhap();
        }

        private void btnXoaDong_Click(object sender, EventArgs e)
        {
            if (dgvChiTietNhap.CurrentRow == null || dgvChiTietNhap.CurrentRow.Index < 0)
            {
                BaoLoi("Vui lòng chọn dòng nguyên liệu cần xóa.");
                return;
            }
            if (MessageBox.Show("Bạn có chắc muốn xóa dòng nguyên liệu này?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            chiTietNhap.Rows.RemoveAt(dgvChiTietNhap.CurrentRow.Index);
            HienThiChiTietNhap();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            TaoChiTietNhap();
            TaoMaPhieuMoi();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (cboNhaCungCap.SelectedValue == null)
            {
                BaoLoi("Vui lòng chọn nhà cung cấp.");
                return;
            }
            if (chiTietNhap == null || chiTietNhap.Rows.Count == 0)
            {
                BaoLoi("Phiếu nhập phải có ít nhất một dòng nguyên liệu.");
                return;
            }
            foreach (DataRow dong in chiTietNhap.Rows)
            {
                if (Convert.ToDecimal(dong["SoLuong"]) <= 0m || Convert.ToDecimal(dong["DonGia"]) < 0m)
                {
                    BaoLoi("Số lượng phải lớn hơn 0 và đơn giá không được âm.");
                    return;
                }
            }

            try
            {
                string maPN = txtMaPN.Text.Trim();
                string maNCC = cboNhaCungCap.SelectedValue.ToString();
                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            string maNV = LayMaNhanVien(conn, tran);
                            using (var cmd = new SqlCommand(
                                "INSERT INTO PhieuNhap (MaPN, MaNCC, MaNV, NgayNhap) " +
                                "VALUES (@MaPN, @MaNCC, @MaNV, @NgayNhap)", conn, tran))
                            {
                                cmd.Parameters.Add("@MaPN", SqlDbType.NVarChar, 10).Value = maPN;
                                cmd.Parameters.Add("@MaNCC", SqlDbType.NVarChar, 10).Value = maNCC;
                                cmd.Parameters.Add("@MaNV", SqlDbType.NVarChar, 10).Value = maNV;
                                cmd.Parameters.Add("@NgayNhap", SqlDbType.DateTime).Value = dtpNgayNhap.Value;
                                cmd.ExecuteNonQuery();
                            }

                            var maNguyenLieuDaNhap = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                            foreach (DataRow dong in chiTietNhap.Rows)
                            {
                                string maNL = Convert.ToString(dong["MaNL"]);
                                decimal soLuong = Convert.ToDecimal(dong["SoLuong"]);
                                decimal donGia = Convert.ToDecimal(dong["DonGia"]);
                                using (var cmd = new SqlCommand(
                                    "INSERT INTO ChiTietPhieuNhap (MaPN, MaNL, SoLuong, DonGia) " +
                                    "VALUES (@MaPN, @MaNL, @SoLuong, @DonGia)", conn, tran))
                                {
                                    cmd.Parameters.Add("@MaPN", SqlDbType.NVarChar, 10).Value = maPN;
                                    cmd.Parameters.Add("@MaNL", SqlDbType.NVarChar, 10).Value = maNL;
                                    GanDecimal(cmd, "@SoLuong", soLuong, 18, 2);
                                    GanDecimal(cmd, "@DonGia", donGia, 18, 0);
                                    cmd.ExecuteNonQuery();
                                }
                                using (var cmd = new SqlCommand(
                                    "UPDATE NguyenLieu SET TonKho = TonKho + @SoLuong WHERE MaNL = @MaNL", conn, tran))
                                {
                                    GanDecimal(cmd, "@SoLuong", soLuong, 18, 2);
                                    cmd.Parameters.Add("@MaNL", SqlDbType.NVarChar, 10).Value = maNL;
                                    cmd.ExecuteNonQuery();
                                }
                                maNguyenLieuDaNhap.Add(maNL);
                            }

                            Kho.CapNhatTrangThaiMonAn(conn, tran, maNguyenLieuDaNhap);
                            tran.Commit();
                        }
                        catch
                        {
                            try { tran.Rollback(); } catch { }
                            throw;
                        }
                    }
                }

                try
                {
                    DongBoXmlSauKhiLuu();
                }
                catch (Exception ex)
                {
                    HienLoi("Đã lưu phiếu nhưng chưa đồng bộ được XML", ex);
                }
                MessageBox.Show("Đã lưu phiếu nhập và cập nhật tồn kho.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                TaiDuLieu();
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                HienLoi("Mã phiếu nhập đã tồn tại. Vui lòng tạo lại phiếu mới", ex);
            }
            catch (Exception ex)
            {
                HienLoi("Không thể lưu phiếu nhập", ex);
            }
        }

        private string LayMaNhanVien(SqlConnection conn, SqlTransaction tran)
        {
            if (PhienDangNhap.IsLoggedIn && !string.IsNullOrWhiteSpace(PhienDangNhap.MaNV))
                return PhienDangNhap.MaNV.Trim();
            using (var cmd = new SqlCommand(
                "SELECT TOP 1 MaNV FROM NhanVien WHERE ChucVu = N'Admin' ORDER BY MaNV", conn, tran))
            {
                object ketQua = cmd.ExecuteScalar();
                if (ketQua == null || ketQua == DBNull.Value)
                    throw new InvalidOperationException("Không tìm thấy nhân viên Admin để lập phiếu nhập.");
                return Convert.ToString(ketQua);
            }
        }

        private void DongBoXmlSauKhiLuu()
        {
            fx.TaoXml("SELECT * FROM PhieuNhap", "PhieuNhap", "PhieuNhap.xml");
            fx.TaoXml("SELECT * FROM ChiTietPhieuNhap", "ChiTietPhieuNhap", "ChiTietPhieuNhap.xml");
            fx.TaoXml("SELECT * FROM NguyenLieu", "NguyenLieu", "NguyenLieu.xml");
            fx.TaoXml("SELECT * FROM MonAn", "MonAn", "MonAn.xml");
        }

        private void TaiLichSuPhieuNhap()
        {
            using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
            using (var da = new SqlDataAdapter(
                "SELECT pn.MaPN, ncc.TenNCC, nv.TenNV, pn.NgayNhap, " +
                "ISNULL(SUM(ct.SoLuong * ct.DonGia), 0) AS TongTien " +
                "FROM PhieuNhap pn INNER JOIN NhaCungCap ncc ON ncc.MaNCC = pn.MaNCC " +
                "INNER JOIN NhanVien nv ON nv.MaNV = pn.MaNV " +
                "LEFT JOIN ChiTietPhieuNhap ct ON ct.MaPN = pn.MaPN " +
                "GROUP BY pn.MaPN, ncc.TenNCC, nv.TenNV, pn.NgayNhap ORDER BY pn.NgayNhap DESC, pn.MaPN DESC", conn))
            {
                var bang = new DataTable();
                da.Fill(bang);
                dgvPhieuNhap.DataSource = bang;
                GiaoDien.LamDepBang(dgvPhieuNhap, new Dictionary<string, string>
                {
                    { "MaPN", "Mã phiếu" }, { "TenNCC", "Nhà cung cấp" },
                    { "TenNV", "Người lập" }, { "NgayNhap", "Ngày nhập" },
                    { "TongTien", "Tổng tiền" }
                });
                if (dgvPhieuNhap.Columns.Contains("NgayNhap"))
                    dgvPhieuNhap.Columns["NgayNhap"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                GiaoDien.DinhDangTien(dgvPhieuNhap, "TongTien");
            }
            TaiChiTietLichSu();
        }

        private void dgvPhieuNhap_SelectionChanged(object sender, EventArgs e)
        {
            TaiChiTietLichSu();
        }

        private void TaiChiTietLichSu()
        {
            string maPN = "";
            if (dgvPhieuNhap.CurrentRow != null && dgvPhieuNhap.CurrentRow.Cells["MaPN"].Value != null)
                maPN = Convert.ToString(dgvPhieuNhap.CurrentRow.Cells["MaPN"].Value);
            var bang = new DataTable();
            if (maPN.Length > 0)
            {
                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                using (var cmd = new SqlCommand(
                    "SELECT ct.MaNL, nl.TenNL, nl.DVT, ct.SoLuong, ct.DonGia, " +
                    "ct.SoLuong * ct.DonGia AS ThanhTien " +
                    "FROM ChiTietPhieuNhap ct INNER JOIN NguyenLieu nl ON nl.MaNL = ct.MaNL " +
                    "WHERE ct.MaPN = @MaPN ORDER BY ct.MaNL", conn))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.Add("@MaPN", SqlDbType.NVarChar, 10).Value = maPN;
                    da.Fill(bang);
                }
            }
            dgvChiTietLichSu.DataSource = bang;
            GiaoDien.LamDepBang(dgvChiTietLichSu, new Dictionary<string, string>
            {
                { "MaNL", "Mã NL" }, { "TenNL", "Tên nguyên liệu" },
                { "DVT", "Đơn vị tính" }, { "SoLuong", "Số lượng" },
                { "DonGia", "Đơn giá" }, { "ThanhTien", "Thành tiền" }
            });
            if (dgvChiTietLichSu.Columns.Contains("SoLuong"))
                dgvChiTietLichSu.Columns["SoLuong"].DefaultCellStyle.Format = "#,##0.##";
            GiaoDien.DinhDangTien(dgvChiTietLichSu, "DonGia", "ThanhTien");
        }

        private DataRow TimNguyenLieu(string maNL)
        {
            foreach (DataRow dong in nguyenLieu.Rows)
                if (Convert.ToString(dong["MaNL"]) == maNL) return dong;
            return null;
        }

        private static bool TryLaySo(string giaTri, out decimal so)
        {
            so = 0m;
            if (string.IsNullOrWhiteSpace(giaTri)) return false;
            string text = giaTri.Trim();
            if (text.Contains(",") && text.Contains(".")) text = text.Replace(".", "").Replace(',', '.');
            else if (text.Contains(",")) text = text.Replace(',', '.');
            return decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out so);
        }

        private static void GanDecimal(SqlCommand cmd, string ten, decimal giaTri, byte precision, byte scale)
        {
            var p = cmd.Parameters.Add(ten, SqlDbType.Decimal);
            p.Precision = precision;
            p.Scale = scale;
            p.Value = giaTri;
        }

        private static void BaoLoi(string noiDung)
        {
            MessageBox.Show(noiDung, "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private static void HienLoi(string noiDung, Exception ex)
        {
            MessageBox.Show(noiDung + ":\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
