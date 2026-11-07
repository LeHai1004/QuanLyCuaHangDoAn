using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Xsl;

namespace QuanLyCuaHangDoAn
{
    /// <summary>
    /// Lop loi: SQL Server <-> XML, kiem tra XSD, xuat HTML bang XSLT.
    /// CHI HAI SUA FILE NAY. Nguoi khac can them ham thi bao Hai (xem docs/05-quy-uoc-code.md).
    /// </summary>
    public class FileXml
    {
        // Ten bang hop le. tenBang truyen vao PHAI nam trong danh sach nay (chong SQL injection).
        private static readonly HashSet<string> BangHopLe = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "DanhMuc", "MonAn", "Ban", "NhanVien", "KhachHang", "HoaDon", "ChiTietHoaDon",
            "NguyenLieu", "CongThuc", "NhaCungCap", "PhieuNhap", "ChiTietPhieuNhap"
        };

        // ---------- Duong dan va ket noi ----------

        public static string ThuMucData
        {
            get
            {
                string p = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
                Directory.CreateDirectory(p);
                return p;
            }
        }

        /// <summary>Ten file thi nam trong thu muc Data; neu truyen duong dan day du thi dung luon (dung cho Import).</summary>
        public static string DuongDan(string tenFile)
        {
            return Path.IsPathRooted(tenFile) ? tenFile : Path.Combine(ThuMucData, tenFile);
        }

        private static string DuongDanXslt(string tenXslt)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Xslt", tenXslt);
        }

        public static string ChuoiKetNoi
        {
            get
            {
                ConnectionStringSettings cs = ConfigurationManager.ConnectionStrings["QLCuaHangDoAn"];
                if (cs == null)
                    throw new InvalidOperationException("Thieu chuoi ket noi QLCuaHangDoAn trong App.config");
                return cs.ConnectionString;
            }
        }

        private static void KiemTraBang(string tenBang)
        {
            if (!BangHopLe.Contains(tenBang))
                throw new ArgumentException("Ten bang khong hop le: " + tenBang);
        }

        // ---------- Doc / tao XML ----------

        /// <summary>Doc file XML ra DataTable de gan vao DataGridView.</summary>
        public DataTable HienThi(string tenFile)
        {
            string xml = DuongDan(tenFile);
            if (!File.Exists(xml))
                throw new FileNotFoundException("Chua co file XML: " + tenFile);

            var ds = new DataSet();
            string xsd = Path.ChangeExtension(xml, ".xsd");
            if (File.Exists(xsd)) ds.ReadXmlSchema(xsd);
            ds.ReadXml(xml);
            return ds.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
        }

        /// <summary>
        /// Lay du lieu tu SQL Server ghi ra file XML (va file XSD neu chua co).
        /// sql PHAI la chuoi co dinh viet trong code, KHONG noi chuoi tu o nhap cua nguoi dung.
        /// </summary>
        public void TaoXml(string sql, string tenBang, string tenFile)
        {
            KiemTraBang(tenBang);
            using (var conn = new SqlConnection(ChuoiKetNoi))
            using (var da = new SqlDataAdapter(sql, conn))
            {
                var ds = new DataSet("DuLieu");
                da.Fill(ds, tenBang);
                LuuDataSet(ds, tenFile);
            }
        }

        internal static void LuuDataSet(DataSet ds, string tenFile)
        {
            string xml = DuongDan(tenFile);
            string xsd = Path.ChangeExtension(xml, ".xsd");
            // Khong ghi de XSD neu da co (XSD viet tay cua Duy o task T13)
            if (!File.Exists(xsd)) ds.WriteXmlSchema(xsd);
            ds.WriteXml(xml, XmlWriteMode.IgnoreSchema);
        }

        // ---------- Tien ich tren XML ----------

        /// <summary>Kiem tra gia tri da ton tai o cot chua (dung cho kiem tra trung ma).</summary>
        public bool TonTai(string tenFile, string cot, string giaTri)
        {
            string p = DuongDan(tenFile);
            if (!File.Exists(p)) return false;
            XDocument doc = XDocument.Load(p);
            return doc.Root.Elements().Any(r =>
                string.Equals((string)r.Element(cot), giaTri, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>Sinh ma moi, vi du TaoMa("MA", "MonAn.xml", "MaMon") tra ve MA009 neu max hien tai la MA008.</summary>
        public string TaoMa(string tienTo, string tenFile, string cot)
        {
            int max = 0;
            string p = DuongDan(tenFile);
            if (File.Exists(p))
            {
                XDocument doc = XDocument.Load(p);
                foreach (XElement r in doc.Root.Elements())
                {
                    string ma = (string)r.Element(cot);
                    int so;
                    if (ma != null
                        && ma.StartsWith(tienTo, StringComparison.OrdinalIgnoreCase)
                        && int.TryParse(ma.Substring(tienTo.Length), out so)
                        && so > max)
                        max = so;
                }
            }
            return tienTo + (max + 1).ToString("000");
        }

        // ---------- Them / Sua / Xoa tren file XML ----------
        // Khong dung XPath tu chuoi nguoi dung nhap (tranh XPath injection): tim dong bang cap cot-gia tri.

        private static List<XElement> TimDong(XDocument doc, IDictionary<string, string> khoa)
        {
            return doc.Root.Elements().Where(r =>
                khoa.All(k => string.Equals((string)r.Element(k.Key), k.Value, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        /// <summary>Them 1 dong. Dua cac cot vao theo DUNG THU TU cot trong bang.</summary>
        public void Them(string tenFile, string tenBang, IDictionary<string, string> giaTri)
        {
            KiemTraBang(tenBang);
            string p = DuongDan(tenFile);
            XDocument doc = XDocument.Load(p);
            var dong = new XElement(tenBang);
            foreach (var kv in giaTri) dong.Add(new XElement(kv.Key, kv.Value));
            doc.Root.Add(dong);
            doc.Save(p);
        }

        /// <summary>Sua cac dong co khoa khop. Tra ve so dong da sua.</summary>
        public int Sua(string tenFile, IDictionary<string, string> khoa, IDictionary<string, string> giaTriMoi)
        {
            string p = DuongDan(tenFile);
            XDocument doc = XDocument.Load(p);
            List<XElement> dong = TimDong(doc, khoa);
            foreach (XElement r in dong)
                foreach (var kv in giaTriMoi)
                {
                    XElement e = r.Element(kv.Key);
                    if (e == null) r.Add(new XElement(kv.Key, kv.Value));
                    else e.Value = kv.Value;
                }
            if (dong.Count > 0) doc.Save(p);
            return dong.Count;
        }

        /// <summary>Xoa cac dong co khoa khop. Tra ve so dong da xoa.</summary>
        public int Xoa(string tenFile, IDictionary<string, string> khoa)
        {
            string p = DuongDan(tenFile);
            XDocument doc = XDocument.Load(p);
            List<XElement> dong = TimDong(doc, khoa);
            foreach (XElement r in dong) r.Remove();
            if (dong.Count > 0) doc.Save(p);
            return dong.Count;
        }

        // ---------- Dong bo XML -> SQL Server ----------

        /// <summary>
        /// Cap nhat database cho khop file XML: dong co trong XML ma chua co o DB thi them, khac nhau thi sua,
        /// dong co o DB ma khong co trong XML thi xoa. Goi theo thu tu bang cha truoc, bang con sau.
        /// </summary>
        public void CapNhatDb(string tenBang, string tenFile)
        {
            KiemTraBang(tenBang);
            using (var conn = new SqlConnection(ChuoiKetNoi))
            using (var da = new SqlDataAdapter("SELECT * FROM " + tenBang, conn))
            using (var cb = new SqlCommandBuilder(da))
            {
                var dtDb = new DataTable(tenBang);
                da.FillSchema(dtDb, SchemaType.Source);
                da.Fill(dtDb);

                DataTable dtXml = dtDb.Clone();
                dtXml.ReadXml(DuongDan(tenFile));

                DongBo(dtDb, dtXml);
                da.Update(dtDb);
            }
        }

        internal static void DongBo(DataTable dtDb, DataTable dtXml)
        {
            if (dtDb.PrimaryKey.Length == 0 || dtXml.PrimaryKey.Length == 0)
                throw new InvalidOperationException("Bang " + dtDb.TableName + " chua co khoa chinh");

            // 1. Xoa: dong o DB ma khong con trong XML
            List<DataRow> canXoa = dtDb.Rows.Cast<DataRow>()
                .Where(r => dtXml.Rows.Find(LayKhoa(r, dtDb)) == null).ToList();
            foreach (DataRow r in canXoa) r.Delete();

            // 2. Them moi hoac sua
            foreach (DataRow rx in dtXml.Rows)
            {
                DataRow rd = dtDb.Rows.Find(LayKhoa(rx, dtXml));
                if (rd == null)
                {
                    dtDb.Rows.Add(rx.ItemArray);
                }
                else
                {
                    foreach (DataColumn c in dtDb.Columns)
                        if (!Equals(rd[c], rx[c.ColumnName])) rd[c] = rx[c.ColumnName];
                }
            }
        }

        private static object[] LayKhoa(DataRow r, DataTable t)
        {
            return t.PrimaryKey.Select(c => r[c.ColumnName]).ToArray();
        }

        // ---------- XSD ----------

        /// <summary>Kiem tra file XML theo file XSD. Tra ve true neu hop le; loi chua noi dung bao loi.</summary>
        public bool KiemTraXsd(string tenXml, string tenXsd, out string loi)
        {
            var sb = new StringBuilder();
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit, // chan XXE khi nhap file XML tu ben ngoai
                XmlResolver = null,
                ValidationType = ValidationType.Schema
            };
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

            var xsdSettings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null };
            using (XmlReader xs = XmlReader.Create(DuongDan(tenXsd), xsdSettings))
                settings.Schemas.Add(null, xs);

            settings.ValidationEventHandler += (s, e) =>
            {
                int dong = e.Exception != null ? e.Exception.LineNumber : 0;
                int cot = e.Exception != null ? e.Exception.LinePosition : 0;
                sb.AppendLine("Dong " + dong + ", cot " + cot + ": " + e.Message);
            };

            try
            {
                using (XmlReader r = XmlReader.Create(DuongDan(tenXml), settings))
                    while (r.Read()) { }
            }
            catch (XmlException ex)
            {
                sb.AppendLine("Loi cu phap XML: " + ex.Message);
            }

            loi = sb.ToString();
            return sb.Length == 0;
        }

        // ---------- XSLT ----------

        /// <summary>Bien file XML thanh HTML bang XSLT. File XSLT nam trong thu muc Xslt, ket qua nam trong Data.</summary>
        public void XuatHtml(string tenXml, string tenXslt, XsltArgumentList thamSo, string tenHtml)
        {
            var xslt = new XslCompiledTransform();
            xslt.Load(DuongDanXslt(tenXslt), new XsltSettings(false, false), null); // tat document() va script

            var docSettings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null };
            using (XmlReader input = XmlReader.Create(DuongDan(tenXml), docSettings))
            using (XmlWriter output = XmlWriter.Create(DuongDan(tenHtml), xslt.OutputSettings))
                xslt.Transform(input, thamSo, output);
        }

        /// <summary>Mo file (HTML) bang trinh duyet mac dinh.</summary>
        public static void MoTrinhDuyet(string tenFile)
        {
            Process.Start(new ProcessStartInfo(DuongDan(tenFile)) { UseShellExecute = true });
        }
    }
}