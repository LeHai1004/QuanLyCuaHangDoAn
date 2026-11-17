<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" encoding="utf-8" indent="yes" />
  <xsl:decimal-format name="vi" decimal-separator="," grouping-separator="." />
  <xsl:param name="maHD" />

  <xsl:template match="/">
    <html lang="vi">
      <head>
        <meta charset="utf-8" />
        <title>Hóa đơn <xsl:value-of select="$maHD" /></title>
        <style type="text/css">
          body {
            margin: 0;
            padding: 18px;
            background: #f2f2f2;
            color: #242424;
            font-family: "Segoe UI", Arial, sans-serif;
          }
          .hoa-don {
            box-sizing: border-box;
            width: 80mm;
            margin: 0 auto;
            padding: 7mm 6mm;
            background: #fff;
            box-shadow: 0 2px 12px rgba(0, 0, 0, .14);
          }
          h1 {
            margin: 0 0 3mm;
            color: #e66414;
            font-size: 19px;
            text-align: center;
            text-transform: uppercase;
          }
          .phu-de, .cam-on {
            text-align: center;
          }
          .phu-de {
            margin: 0 0 5mm;
            color: #555;
            font-size: 11px;
          }
          .thong-tin {
            margin-bottom: 4mm;
            font-size: 12px;
            line-height: 1.55;
          }
          .dong-thong-tin {
            display: flex;
            justify-content: space-between;
            gap: 5px;
          }
          .nhan {
            color: #666;
          }
          .ma-hoa-don {
            font-weight: bold;
          }
          table {
            width: 100%;
            border-collapse: collapse;
            font-size: 11px;
          }
          thead {
            border-top: 1px dashed #888;
            border-bottom: 1px dashed #888;
          }
          th {
            padding: 2mm 0;
            text-align: left;
          }
          th.so-luong, td.so-luong {
            width: 10mm;
            text-align: center;
          }
          th.tien, td.tien {
            text-align: right;
            white-space: nowrap;
          }
          td {
            padding: 2mm 0;
            vertical-align: top;
          }
          .ten-mon {
            padding-right: 2mm;
          }
          .tong-ket {
            margin-top: 2mm;
            padding-top: 2mm;
            border-top: 1px dashed #888;
            font-size: 12px;
          }
          .tong-ket .dong-thong-tin {
            padding: 1mm 0;
          }
          .tong-cong {
            margin-top: 1mm;
            padding-top: 2mm;
            border-top: 1px solid #444;
            font-size: 15px;
            font-weight: bold;
          }
          .cam-on {
            margin: 6mm 0 0;
            font-size: 12px;
            font-weight: bold;
          }
          .loi-khong-tim-thay {
            text-align: center;
          }
          @media print {
            @page {
              size: 80mm auto;
              margin: 4mm;
            }
            body {
              padding: 0;
              background: #fff;
            }
            .hoa-don {
              width: auto;
              margin: 0;
              padding: 0;
              box-shadow: none;
            }
          }
        </style>
      </head>
      <body>
        <xsl:choose>
          <xsl:when test="/DuLieu/HoaDon[MaHD = $maHD]">
            <xsl:for-each select="/DuLieu/HoaDon[MaHD = $maHD]">
              <main class="hoa-don">
                <h1>Cửa Hàng Đồ Ăn</h1>
                <p class="phu-de">HÓA ĐƠN THANH TOÁN</p>
                <section class="thong-tin">
                  <div class="dong-thong-tin ma-hoa-don">
                    <span class="nhan">Mã hóa đơn</span>
                    <span><xsl:value-of select="MaHD" /></span>
                  </div>
                  <div class="dong-thong-tin">
                    <span class="nhan">Ngày lập</span>
                    <span>
                      <xsl:value-of select="substring(NgayLap, 9, 2)" />/<xsl:value-of select="substring(NgayLap, 6, 2)" />/<xsl:value-of select="substring(NgayLap, 1, 4)" />
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="substring(NgayLap, 12, 5)" />
                    </span>
                  </div>
                  <div class="dong-thong-tin">
                    <span class="nhan">Bàn</span>
                    <span><xsl:value-of select="TenBan" /></span>
                  </div>
                  <div class="dong-thong-tin">
                    <span class="nhan">Nhân viên</span>
                    <span><xsl:value-of select="TenNhanVien" /></span>
                  </div>
                  <xsl:if test="string-length(normalize-space(TenKhachHang)) &gt; 0">
                    <div class="dong-thong-tin">
                      <span class="nhan">Khách hàng</span>
                      <span><xsl:value-of select="TenKhachHang" /></span>
                    </div>
                  </xsl:if>
                </section>
                <table>
                  <thead>
                    <tr>
                      <th class="ten-mon">Món</th>
                      <th class="so-luong">SL</th>
                      <th class="tien">Đơn giá</th>
                      <th class="tien">Thành tiền</th>
                    </tr>
                  </thead>
                  <tbody>
                    <xsl:for-each select="ChiTietHoaDon/ChiTiet">
                      <tr>
                        <td class="ten-mon"><xsl:value-of select="TenMon" /></td>
                        <td class="so-luong"><xsl:value-of select="SoLuong" /></td>
                        <td class="tien">
                          <xsl:value-of select="format-number(number(DonGia), '#.##0', 'vi')" /> đ
                        </td>
                        <td class="tien">
                          <xsl:value-of select="format-number(number(ThanhTien), '#.##0', 'vi')" /> đ
                        </td>
                      </tr>
                    </xsl:for-each>
                  </tbody>
                </table>
                <section class="tong-ket">
                  <div class="dong-thong-tin">
                    <span>Tạm tính</span>
                    <span><xsl:value-of select="format-number(number(TamTinh), '#.##0', 'vi')" /> đ</span>
                  </div>
                  <div class="dong-thong-tin">
                    <span>Giảm giá</span>
                    <span><xsl:value-of select="format-number(number(PhanTramGiam), '0,##', 'vi')" />%</span>
                  </div>
                  <div class="dong-thong-tin tong-cong">
                    <span>Tổng tiền</span>
                    <span><xsl:value-of select="format-number(number(TongTien), '#.##0', 'vi')" /> đ</span>
                  </div>
                </section>
                <p class="cam-on">Cảm ơn quý khách và hẹn gặp lại!</p>
              </main>
            </xsl:for-each>
          </xsl:when>
          <xsl:otherwise>
            <main class="hoa-don loi-khong-tim-thay">
              <p>Không tìm thấy hóa đơn <xsl:value-of select="$maHD" />.</p>
            </main>
          </xsl:otherwise>
        </xsl:choose>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
