using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using QuanLyCuaHangDoAn.Class;

namespace QuanLyCuaHangDoAn.GUI
{
    public class frmNhapXml : Form
    {
        private readonly FileXml fx = new FileXml();
        private readonly Button btnXuat = GiaoDien.TaoNut("Xuất thực đơn XML", GiaoDien.MauChinh);
        private readonly Button btnChon = GiaoDien.TaoNut("Chọn và kiểm tra XML", GiaoDien.MauChinh);
        private readonly Button btnNhap = GiaoDien.TaoNut("Nhập vào hệ thống", GiaoDien.MauXanh);
        private readonly Label lblTrangThai = new Label();
        private readonly Label lblTenFile = new Label();
        private readonly Label lblCheDo = new Label();
        private readonly Panel pnlThanhTrangThai = new Panel();
        private readonly TextBox txtKetQua = new TextBox();
        private readonly DataGridView dgvPreview = new DataGridView();
        private readonly ToolTip toolTip = new ToolTip();
        private string duongDanXmlDaChon = "";
        private DataTable duLieuXemTruoc;
        private bool choDongBo;
        private bool daNhapDuLieu;
        private int soMonThemChoDongBo;
        private int soMonTrungChoDongBo;

        public frmNhapXml()
        {
            KhoiTaoGiaoDien();
            Load += frmNhapXml_Load;
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Nhập / xuất thực đơn XML";
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 246, 248);
            Font = new Font("Segoe UI", 10F);
            ClientSize = new Size(1100, 700);
            MinimumSize = new Size(1116, 739);
            DoubleBuffered = true;

            btnXuat.Name = "btnXuat";
            btnChon.Name = "btnChon";
            btnNhap.Name = "btnNhap";
            lblTrangThai.Name = "lblTrangThai";
            lblTenFile.Name = "lblTenFile";
            lblCheDo.Name = "lblCheDo";
            txtKetQua.Name = "txtKetQua";
            dgvPreview.Name = "dgvPreview";
            pnlThanhTrangThai.Name = "pnlThanhTrangThai";
            CauHinhNut(btnXuat, Color.FromArgb(232, 101, 26), Color.White, Color.FromArgb(207, 85, 16));
            btnXuat.Text = "Xuất thực đơn ra file XML";
            CauHinhNut(btnChon, Color.White, Color.FromArgb(232, 101, 26), Color.FromArgb(255, 243, 234));
            btnChon.Text = "Chọn file XML...";
            CauHinhNut(btnNhap, Color.FromArgb(22, 163, 74), Color.White, Color.FromArgb(21, 128, 61));
            btnNhap.Text = "Nhập vào hệ thống";

            var pnlHeader = new Panel
            {
                Name = "pnlHeader",
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(232, 101, 26)
            };
            pnlHeader.Controls.Add(new Label
            {
                Name = "lblTieuDe",
                Text = "NHẬP / XUẤT THỰC ĐƠN XML",
                Location = new Point(30, 12),
                Size = new Size(650, 30),
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            });
            pnlHeader.Controls.Add(new Label
            {
                Name = "lblMoTa",
                Text = "Sao lưu và chuyển thực đơn giữa các máy",
                Location = new Point(32, 48),
                Size = new Size(600, 20),
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            });

            var pnlTrai = TaoThe("pnlTrai", new Point(30, 100), new Size(360, 580));
            pnlTrai.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
            TaoKhuVucChucNang(pnlTrai);

            var pnlPhai = TaoThe("pnlPhai", new Point(410, 100), new Size(660, 580));
            pnlPhai.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            TaoKhuVucXemTruoc(pnlPhai);

            Controls.Add(pnlTrai);
            Controls.Add(pnlPhai);
            Controls.Add(pnlHeader);
            pnlHeader.BringToFront();

            btnXuat.Click += btnXuat_Click;
            btnChon.Click += btnChon_Click;
            btnNhap.Click += btnNhap_Click;
        }

        private void TaoKhuVucChucNang(Panel khuVuc)
        {
            khuVuc.Controls.Add(TaoNhan("1. XUẤT THỰC ĐƠN", "lblTieuDeXuat", 11F, true, Color.FromArgb(232, 101, 26),
                new Point(20, 15), new Size(320, 22)));
            khuVuc.Controls.Add(TaoNhan("Lưu toàn bộ món ăn hiện có ra file XML để sao lưu hoặc chuyển sang máy khác.",
                "lblMoTaXuat", 9F, false, Color.FromArgb(107, 114, 128), new Point(20, 42), new Size(320, 36)));

            btnXuat.Location = new Point(20, 85);
            btnXuat.Size = new Size(320, 44);
            khuVuc.Controls.Add(btnXuat);

            khuVuc.Controls.Add(new Panel
            {
                Name = "pnlDuongKe",
                Location = new Point(20, 150),
                Size = new Size(320, 1),
                BackColor = Color.FromArgb(229, 231, 235)
            });

            khuVuc.Controls.Add(TaoNhan("2. NHẬP THỰC ĐƠN TỪ FILE", "lblTieuDeNhap", 11F, true,
                Color.FromArgb(232, 101, 26), new Point(20, 165), new Size(320, 22)));
            khuVuc.Controls.Add(TaoNhan("Chọn file XML. Hệ thống tự kiểm tra theo XSD và mã danh mục trước khi cho nhập.",
                "lblMoTaNhap", 9F, false, Color.FromArgb(107, 114, 128), new Point(20, 192), new Size(320, 36)));

            btnChon.Location = new Point(20, 235);
            btnChon.Size = new Size(320, 44);
            khuVuc.Controls.Add(btnChon);

            lblTenFile.Location = new Point(20, 288);
            lblTenFile.Size = new Size(320, 20);
            lblTenFile.Text = "Chưa chọn file";
            lblTenFile.AutoEllipsis = true;
            lblTenFile.ForeColor = Color.FromArgb(107, 114, 128);
            lblTenFile.Font = new Font("Segoe UI", 9F);
            khuVuc.Controls.Add(lblTenFile);

            var pnlKetQua = new Panel
            {
                Name = "pnlKetQua",
                Location = new Point(20, 315),
                Size = new Size(320, 150),
                BackColor = Color.FromArgb(249, 250, 251)
            };
            pnlThanhTrangThai.Location = new Point(0, 0);
            pnlThanhTrangThai.Size = new Size(4, 150);
            pnlThanhTrangThai.BackColor = Color.FromArgb(156, 163, 175);
            lblTrangThai.Location = new Point(14, 8);
            lblTrangThai.Size = new Size(294, 22);
            lblTrangThai.Text = "Chưa kiểm tra";
            lblTrangThai.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTrangThai.ForeColor = Color.FromArgb(55, 65, 81);
            txtKetQua.Location = new Point(14, 34);
            txtKetQua.Size = new Size(294, 106);
            txtKetQua.Multiline = true;
            txtKetQua.ReadOnly = true;
            txtKetQua.ScrollBars = ScrollBars.Vertical;
            txtKetQua.BorderStyle = BorderStyle.None;
            txtKetQua.BackColor = Color.FromArgb(249, 250, 251);
            txtKetQua.Font = new Font("Segoe UI", 9F);
            txtKetQua.TabStop = false;
            pnlKetQua.Controls.Add(pnlThanhTrangThai);
            pnlKetQua.Controls.Add(lblTrangThai);
            pnlKetQua.Controls.Add(txtKetQua);
            khuVuc.Controls.Add(pnlKetQua);

            btnNhap.Location = new Point(20, 480);
            btnNhap.Size = new Size(320, 44);
            btnNhap.Enabled = false;
            khuVuc.Controls.Add(btnNhap);
        }

        private void TaoKhuVucXemTruoc(Panel khuVuc)
        {
            khuVuc.Controls.Add(TaoNhan("XEM TRƯỚC DỮ LIỆU", "lblTieuDeXemTruoc", 11F, true,
                Color.FromArgb(232, 101, 26), new Point(20, 15), new Size(260, 22)));
            lblCheDo.Location = new Point(285, 15);
            lblCheDo.Size = new Size(355, 22);
            lblCheDo.Text = "Đang xem: Thực đơn hiện tại (0 món)";
            lblCheDo.TextAlign = ContentAlignment.MiddleRight;
            lblCheDo.ForeColor = Color.FromArgb(107, 114, 128);
            lblCheDo.Font = new Font("Segoe UI", 9F);
            lblCheDo.AutoEllipsis = true;
            khuVuc.Controls.Add(lblCheDo);

            CauHinhBangXemTruoc();
            dgvPreview.Location = new Point(20, 50);
            dgvPreview.Size = new Size(620, 510);
            dgvPreview.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            khuVuc.Controls.Add(dgvPreview);
        }

        private static Panel TaoThe(string ten, Point viTri, Size kichThuoc)
        {
            var khuVuc = new Panel
            {
                Name = ten,
                Location = viTri,
                Size = kichThuoc,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            khuVuc.Paint += (sender, e) =>
            {
                using (var but = new Pen(Color.FromArgb(229, 231, 235)))
                    e.Graphics.DrawRectangle(but, 0, 0, khuVuc.ClientSize.Width - 1, khuVuc.ClientSize.Height - 1);
            };
            return khuVuc;
        }

        private static Label TaoNhan(string noiDung, string ten, float coChu, bool dam, Color mau,
            Point viTri, Size kichThuoc)
        {
            return new Label
            {
                Name = ten,
                Text = noiDung,
                Location = viTri,
                Size = kichThuoc,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", coChu, dam ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = mau,
                AutoEllipsis = true
            };
        }

        private static void CauHinhNut(Button nut, Color nen, Color chu, Color hover)
        {
            nut.FlatStyle = FlatStyle.Flat;
            nut.FlatAppearance.BorderSize = nen == Color.White ? 1 : 0;
            nut.FlatAppearance.BorderColor = Color.FromArgb(232, 101, 26);
            nut.FlatAppearance.MouseOverBackColor = hover;
            nut.FlatAppearance.MouseDownBackColor = hover;
            nut.BackColor = nen;
            nut.ForeColor = chu;
            nut.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            nut.Cursor = Cursors.Hand;
            nut.TextAlign = ContentAlignment.MiddleCenter;
        }

        private void CauHinhBangXemTruoc()
        {
            dgvPreview.Dock = DockStyle.None;
            dgvPreview.ReadOnly = true;
            dgvPreview.AllowUserToAddRows = false;
            dgvPreview.AllowUserToDeleteRows = false;
            dgvPreview.AllowUserToResizeRows = false;
            dgvPreview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPreview.MultiSelect = false;
            dgvPreview.RowHeadersVisible = false;
            dgvPreview.AutoGenerateColumns = false;
            dgvPreview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPreview.BorderStyle = BorderStyle.None;
            dgvPreview.BackgroundColor = Color.White;
            dgvPreview.GridColor = Color.FromArgb(229, 231, 235);
            dgvPreview.EnableHeadersVisualStyles = false;
            dgvPreview.ColumnHeadersHeight = 36;
            dgvPreview.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvPreview.RowTemplate.Height = 32;
            dgvPreview.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 243, 234);
            dgvPreview.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(154, 52, 18);
            dgvPreview.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvPreview.DefaultCellStyle.BackColor = Color.White;
            dgvPreview.DefaultCellStyle.ForeColor = Color.FromArgb(31, 41, 55);
            dgvPreview.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 243, 234);
            dgvPreview.DefaultCellStyle.SelectionForeColor = Color.FromArgb(31, 41, 55);
            dgvPreview.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            dgvPreview.Columns.Add(TaoCot("MaMon", "Mã món", 14F));
            dgvPreview.Columns.Add(TaoCot("TenMon", "Tên món", 29F));
            dgvPreview.Columns.Add(TaoCot("MaDM", "Danh mục", 16F));
            var cotDonGia = TaoCot("DonGia", "Đơn giá", 17F);
            cotDonGia.DefaultCellStyle.Format = "N0";
            cotDonGia.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPreview.Columns.Add(cotDonGia);
            dgvPreview.Columns.Add(TaoCot("DVT", "ĐVT", 10F));
            dgvPreview.Columns.Add(TaoCot("TrangThai", "Trạng thái", 14F));
        }

        private static DataGridViewTextBoxColumn TaoCot(string ten, string tieuDe, float tiLe)
        {
            return new DataGridViewTextBoxColumn
            {
                Name = ten,
                DataPropertyName = ten,
                HeaderText = tieuDe,
                FillWeight = tiLe,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
        }

        private void frmNhapXml_Load(object sender, EventArgs e)
        {
            if (!PhienDangNhap.IsLoggedIn || PhienDangNhap.ChucVu != "Admin")
            {
                MessageBox.Show("Chức năng chỉ dành cho quản trị viên (Admin).", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }

            NapThucDonHienTai();
        }

        private void btnXuat_Click(object sender, EventArgs e)
        {
            if (choDongBo)
                return;
            using (var dialog = new SaveFileDialog())
            {
                dialog.Title = "Lưu bản sao thực đơn XML";
                dialog.Filter = "Tệp XML|*.xml";
                dialog.FileName = "ThucDon_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xml";
                dialog.AddExtension = true;
                dialog.DefaultExt = "xml";
                dialog.OverwritePrompt = true;
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    fx.TaoXml("SELECT MaMon, TenMon, MaDM, DonGia, DVT, TrangThai FROM MonAn", "MonAn", "MonAn.xml");
                    string nguon = FileXml.DuongDan("MonAn.xml");
                    if (!string.Equals(Path.GetFullPath(nguon), Path.GetFullPath(dialog.FileName),
                        StringComparison.OrdinalIgnoreCase))
                        File.Copy(nguon, dialog.FileName, true);
                    int soMon = fx.HienThi("MonAn.xml").Rows.Count;
                    MessageBox.Show("Đã xuất " + soMon + " món ra: " + dialog.FileName,
                        "Xuất thực đơn thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (MessageBox.Show("Bạn có muốn mở thư mục chứa file không?", "Mở thư mục",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        try
                        {
                            Process.Start(new ProcessStartInfo("explorer.exe",
                                "/select,\"" + dialog.FileName + "\"") { UseShellExecute = true });
                        }
                        catch (Exception ex)
                        {
                            HienLoi("Không thể mở thư mục chứa file", ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    HienLoi("Không thể xuất thực đơn XML", ex);
                }
            }
        }

        private void btnChon_Click(object sender, EventArgs e)
        {
            if (choDongBo)
                return;
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Chọn file thực đơn XML";
                dialog.Filter = "Tệp XML|*.xml";
                dialog.CheckFileExists = true;
                dialog.Multiselect = false;
                if (dialog.ShowDialog(this) != DialogResult.OK) return;

                duongDanXmlDaChon = dialog.FileName;
                duLieuXemTruoc = null;
                daNhapDuLieu = false;
                lblTenFile.Text = Path.GetFileName(duongDanXmlDaChon);
                toolTip.SetToolTip(lblTenFile, duongDanXmlDaChon);
                CapNhatTrangThaiNut();

                if (!string.Equals(Path.GetExtension(duongDanXmlDaChon), ".xml",
                    StringComparison.OrdinalIgnoreCase))
                {
                    HienThiKetQua(false, "Tệp đã chọn không có phần mở rộng .xml.", 1);
                    return;
                }

                try
                {
                    string loi;
                    if (!fx.KiemTraXsd(duongDanXmlDaChon, "MonAn.xsd", out loi))
                    {
                        HienThiKetQua(false, loi, DemSoDong(loi));
                        return;
                    }

                    DataTable bangNhap = fx.HienThi(duongDanXmlDaChon);
                    duLieuXemTruoc = bangNhap;
                    if (!KiemTraDanhMuc(out string loiDanhMuc))
                    {
                        duLieuXemTruoc = null;
                        HienThiKetQua(false, loiDanhMuc, DemSoDongDanhMuc(loiDanhMuc));
                        return;
                    }

                    HienThiXemTruoc(bangNhap);
                    lblCheDo.Text = "Đang xem: file " + Path.GetFileName(duongDanXmlDaChon) +
                        " (" + bangNhap.Rows.Count + " món)";
                    toolTip.SetToolTip(lblCheDo, duongDanXmlDaChon);
                    HienThiKetQua(true, "", 0);
                }
                catch (Exception ex)
                {
                    duLieuXemTruoc = null;
                    CapNhatTrangThaiNut();
                    HienThiKetQua(false, "Không thể kiểm tra hoặc đọc file:\r\n" + ex.Message, 1);
                }
            }
        }

        private void HienThiXemTruoc(DataTable bang)
        {
            dgvPreview.DataSource = bang;
        }

        private void btnNhap_Click(object sender, EventArgs e)
        {
            if (choDongBo)
            {
                ThuDongBo();
                return;
            }
            if (!PhienDangNhap.IsLoggedIn || PhienDangNhap.ChucVu != "Admin")
            {
                MessageBox.Show("Bạn không có quyền nhập thực đơn XML.", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!btnNhap.Enabled || duLieuXemTruoc == null || string.IsNullOrWhiteSpace(duongDanXmlDaChon))
            {
                BaoLoi("Vui lòng chọn và kiểm tra một file XML hợp lệ trước.");
                return;
            }
            if (MessageBox.Show("Bạn có chắc muốn nhập " + duLieuXemTruoc.Rows.Count +
                " món từ file đã chọn không?", "Xác nhận nhập thực đơn",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                if (!KiemTraDanhMuc(out string loiDanhMuc))
                {
                    duLieuXemTruoc = null;
                    HienThiKetQua(false, loiDanhMuc, DemSoDongDanhMuc(loiDanhMuc));
                    return;
                }

                fx.TaoXml("SELECT * FROM MonAn", "MonAn", "MonAn.xml");
                var maMonBoQua = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                int soMonThem = 0;
                int soMonTrung = 0;
                foreach (DataRow dong in duLieuXemTruoc.Rows)
                {
                    string maMon = Convert.ToString(dong["MaMon"]);
                    if (fx.TonTai("MonAn.xml", "MaMon", maMon))
                    {
                        maMonBoQua.Add(maMon);
                        soMonTrung++;
                        continue;
                    }

                    fx.Them("MonAn.xml", "MonAn", TaoGiaTriMonAn(dong));
                    soMonThem++;
                }

                daNhapDuLieu = true;
                soMonThemChoDongBo = soMonThem;
                soMonTrungChoDongBo = soMonTrung;
                if (soMonThem == 0)
                {
                    MessageBox.Show("Thêm mới 0, trùng mã " + soMonTrungChoDongBo +
                        " (đã bỏ qua theo luật hiện có).", "Đã hoàn tất nhập thực đơn",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    HoanTatNhap();
                    return;
                }

                choDongBo = true;
                CapNhatTrangThaiNut();
                ThuDongBo();
            }
            catch (Exception ex)
            {
                HienLoi("Không thể nhập thực đơn XML vào hệ thống", ex);
            }
        }

        private static IDictionary<string, string> TaoGiaTriMonAn(DataRow dong)
        {
            string[] thuTuCot = { "MaMon", "TenMon", "MaDM", "DonGia", "DVT", "TrangThai" };
            var giaTri = new SortedDictionary<string, string>(Comparer<string>.Create((a, b) =>
                Array.IndexOf(thuTuCot, a).CompareTo(Array.IndexOf(thuTuCot, b))));
            giaTri.Add("MaMon", Convert.ToString(dong["MaMon"]));
            giaTri.Add("TenMon", Convert.ToString(dong["TenMon"]));
            giaTri.Add("MaDM", Convert.ToString(dong["MaDM"]));
            giaTri.Add("DonGia", Convert.ToDecimal(dong["DonGia"]).ToString("0", CultureInfo.InvariantCulture));
            giaTri.Add("DVT", Convert.ToString(dong["DVT"]));
            giaTri.Add("TrangThai", Convert.ToString(dong["TrangThai"]));
            return giaTri;
        }

        private void ThuDongBo()
        {
            try
            {
                fx.CapNhatDb("MonAn", "MonAn.xml");
                fx.TaoXml("SELECT * FROM MonAn", "MonAn", "MonAn.xml");
            }
            catch (Exception ex)
            {
                CapNhatTrangThaiNut();
                lblTrangThai.Text = "Đồng bộ chưa hoàn tất; nhấn nút Đồng bộ lại để thử.";
                HienLoi("XML đang chờ đồng bộ vào SQL Server. Nhấn Đồng bộ lại để thử tiếp", ex);
                return;
            }

            choDongBo = false;
            MessageBox.Show("Thêm mới " + soMonThemChoDongBo + ", trùng mã " +
                soMonTrungChoDongBo + " (đã bỏ qua theo luật hiện có).",
                "Nhập thực đơn thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            HoanTatNhap();
        }

        private void CapNhatTrangThaiNut()
        {
            btnXuat.Enabled = !choDongBo;
            btnChon.Enabled = !choDongBo;
            btnNhap.Enabled = choDongBo
                || (!daNhapDuLieu && duLieuXemTruoc != null);
            btnNhap.Text = choDongBo ? "Đồng bộ lại" : "Nhập vào hệ thống";
            if (choDongBo)
            {
                lblTrangThai.Text = "Đang chờ đồng bộ SQL Server";
                txtKetQua.Text = "XML đã được chuẩn bị. Nếu đồng bộ lỗi, nhấn “Đồng bộ lại”.";
                pnlThanhTrangThai.BackColor = Color.FromArgb(245, 158, 11);
            }
            btnNhap.BackColor = btnNhap.Enabled ? Color.FromArgb(22, 163, 74) : Color.FromArgb(209, 213, 219);
            btnNhap.ForeColor = btnNhap.Enabled ? Color.White : Color.FromArgb(156, 163, 175);
        }

        private void NapThucDonHienTai()
        {
            try
            {
                var bang = new DataTable();
                using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
                using (var adapter = new SqlDataAdapter(
                    "SELECT MaMon, TenMon, MaDM, DonGia, DVT, TrangThai FROM MonAn", conn))
                    adapter.Fill(bang);
                HienThiXemTruoc(bang);
                lblCheDo.Text = "Đang xem: Thực đơn hiện tại (" + bang.Rows.Count + " món)";
                toolTip.SetToolTip(lblCheDo, "");
                DatKetQuaChuaKiemTra();
            }
            catch (Exception ex)
            {
                HienThiKetQua(false, "Không thể tải thực đơn hiện tại:\r\n" + ex.Message, 1);
            }
        }

        private void HoanTatNhap()
        {
            choDongBo = false;
            daNhapDuLieu = false;
            duLieuXemTruoc = null;
            duongDanXmlDaChon = "";
            lblTenFile.Text = "Chưa chọn file";
            toolTip.SetToolTip(lblTenFile, "");
            lblCheDo.Text = "Đang xem: Thực đơn hiện tại";
            toolTip.SetToolTip(lblCheDo, "");
            DatKetQuaChuaKiemTra();
            CapNhatTrangThaiNut();
            NapThucDonHienTai();
        }

        private void DatKetQuaChuaKiemTra()
        {
            lblTrangThai.Text = "Chưa kiểm tra";
            lblTrangThai.ForeColor = Color.FromArgb(55, 65, 81);
            txtKetQua.Text = "";
            pnlThanhTrangThai.BackColor = Color.FromArgb(156, 163, 175);
            CapNhatTrangThaiNut();
        }

        private void HienThiKetQua(bool hopLe, string loi, int soLoi)
        {
            if (hopLe)
            {
                int soMon = duLieuXemTruoc == null ? 0 : duLieuXemTruoc.Rows.Count;
                lblTrangThai.Text = "Hợp lệ - " + soMon + " món";
                lblTrangThai.ForeColor = Color.FromArgb(22, 101, 52);
                txtKetQua.Text = "Đã kiểm tra XSD và mã danh mục.";
                pnlThanhTrangThai.BackColor = Color.FromArgb(22, 163, 74);
            }
            else
            {
                lblTrangThai.Text = "Không hợp lệ - " + soLoi + " lỗi";
                lblTrangThai.ForeColor = Color.FromArgb(153, 27, 27);
                txtKetQua.Text = loi;
                pnlThanhTrangThai.BackColor = Color.FromArgb(220, 38, 38);
            }
            CapNhatTrangThaiNut();
        }

        private static int DemSoDong(string noiDung)
        {
            if (string.IsNullOrWhiteSpace(noiDung))
                return 1;
            return noiDung.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        private static int DemSoDongDanhMuc(string noiDung)
        {
            int soLoi = 0;
            foreach (string dong in noiDung.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (dong.StartsWith("Dòng món ", StringComparison.Ordinal))
                    soLoi++;
            }
            return soLoi == 0 ? 1 : soLoi;
        }

        private bool KiemTraDanhMuc(out string loi)
        {
            var maDanhMuc = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow dong in duLieuXemTruoc.Rows)
                maDanhMuc.Add(Convert.ToString(dong["MaDM"]));

            var maDanhMucTonTai = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var conn = new SqlConnection(FileXml.ChuoiKetNoi))
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM DanhMuc WHERE MaDM = @MaDM", conn))
            {
                cmd.Parameters.Add("@MaDM", SqlDbType.NVarChar, 10);
                conn.Open();
                foreach (string maDM in maDanhMuc)
                {
                    cmd.Parameters["@MaDM"].Value = maDM;
                    if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                        continue;
                    maDanhMucTonTai.Add(maDM);
                }
            }

            var dongSai = new List<string>();
            for (int i = 0; i < duLieuXemTruoc.Rows.Count; i++)
            {
                DataRow dong = duLieuXemTruoc.Rows[i];
                string maDM = Convert.ToString(dong["MaDM"]);
                if (!maDanhMucTonTai.Contains(maDM))
                {
                    dongSai.Add("Dòng món " + (i + 1) + " (" + Convert.ToString(dong["MaMon"]) +
                        " - " + Convert.ToString(dong["TenMon"]) + "): mã danh mục " + maDM);
                }
            }

            loi = dongSai.Count == 0
                ? ""
                : "Các dòng sau tham chiếu mã danh mục chưa tồn tại trong SQL Server:\n" +
                    string.Join("\n", dongSai) + "\nKhông thể nhập thực đơn.";
            return dongSai.Count == 0;
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
