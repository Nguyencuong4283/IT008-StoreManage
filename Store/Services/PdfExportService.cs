using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Store.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Store.Services
{
    public static class PdfExportService
    {
        static PdfExportService()
        {
            // Cấu hình license (Community - miễn phí cho mục đích phi thương mại)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public static void XuatHoaDonPdf(
            string maHD,
            int soHD,
            DateTime ngayLap,
            string tenKhachHang,
            string sdtKhachHang,
            string tenNhanVien,
            List<ChiTiet_HoaDon> chiTietHoaDons,
            decimal tongTriGia,
            decimal tongGiamGia,
            decimal tongThanhTien,
            string outputPath)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header()
                        .Height(100)
                        .Background(Colors.Blue.Lighten3)
                        .Padding(20)
                        .Column(column =>
                        {
                            column.Item().AlignCenter().Text("HÓA ĐƠN BÁN HÀNG")
                                .FontSize(24).Bold().FontColor(Colors.Blue.Darken2);
                            column.Item().AlignCenter().Text("STOREQuality")
                                .FontSize(12).Italic().FontColor(Colors.Grey.Darken1);
                        });

                    page.Content()
                        .PaddingVertical(20)
                        .Column(column =>
                        {
                            // Thông tin hóa đơn
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text($"Mã hóa đơn: {maHD}").Bold();
                                    col.Item().Text($"Số hóa đơn: {soHD}");
                                    col.Item().Text($"Ngày lập: {ngayLap:dd/MM/yyyy HH:mm:ss}");
                                });
                            });

                            column.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                            // Thông tin khách hàng và nhân viên
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("THÔNG TIN KHÁCH HÀNG").Bold().FontSize(12);
                                    col.Item().PaddingTop(5).Text($"Họ tên: {tenKhachHang}");
                                    col.Item().Text($"Số điện thoại: {sdtKhachHang}");
                                });

                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("NHÂN VIÊN BÁN HÀNG").Bold().FontSize(12);
                                    col.Item().PaddingTop(5).Text($"Họ tên: {tenNhanVien}");
                                });
                            });

                            column.Item().PaddingVertical(15).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                            // Bảng chi tiết sản phẩm
                            column.Item().Text("CHI TIẾT SẢN PHẨM").Bold().FontSize(14);
                            
                            column.Item().PaddingTop(10).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(40);  // STT
                                    columns.ConstantColumn(80);  // Mã SP
                                    columns.RelativeColumn(3);   // Tên SP
                                    columns.ConstantColumn(60);  // Size
                                    columns.ConstantColumn(80);  // Đơn giá
                                    columns.ConstantColumn(40);  // SL
                                    columns.ConstantColumn(50);  // KM%
                                    columns.ConstantColumn(90);  // Thành tiền
                                });

                                // Header
                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("STT").Bold();
                                    header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("Mã SP").Bold();
                                    header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("Tên sản phẩm").Bold();
                                    header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("Size").Bold();
                                    header.Cell().Background(Colors.Blue.Lighten3).Padding(5).AlignRight().Text("Đơn giá").Bold();
                                    header.Cell().Background(Colors.Blue.Lighten3).Padding(5).AlignCenter().Text("SL").Bold();
                                    header.Cell().Background(Colors.Blue.Lighten3).Padding(5).AlignCenter().Text("KM%").Bold();
                                    header.Cell().Background(Colors.Blue.Lighten3).Padding(5).AlignRight().Text("Thành tiền").Bold();
                                });

                                // Rows
                                int stt = 1;
                                foreach (var item in chiTietHoaDons)
                                {
                                    var bgColor = stt % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;
                                    
                                    table.Cell().Background(bgColor).Padding(5).Text(stt.ToString());
                                    table.Cell().Background(bgColor).Padding(5).Text(item.MaSP);
                                    table.Cell().Background(bgColor).Padding(5).Text(item.TenSP);
                                    table.Cell().Background(bgColor).Padding(5).Text(item.KichThuocSP);
                                    table.Cell().Background(bgColor).Padding(5).AlignRight().Text($"{item.DonGia:N0}");
                                    table.Cell().Background(bgColor).Padding(5).AlignCenter().Text(item.SoLuong.ToString());
                                    table.Cell().Background(bgColor).Padding(5).AlignCenter().Text($"{item.KhuyenMai}%");
                                    table.Cell().Background(bgColor).Padding(5).AlignRight().Text($"{item.ThanhTien:N0}");
                                    
                                    stt++;
                                }
                            });

                            column.Item().PaddingTop(20);

                            // Tổng tiền
                            column.Item().AlignRight().Column(col =>
                            {
                                col.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5)
                                    .Row(row =>
                                    {
                                        row.RelativeItem().Text("Tổng trị giá:");
                                        row.ConstantItem(120).AlignRight().Text($"{tongTriGia:N0} ₫");
                                    });

                                col.Item().PaddingTop(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5)
                                    .Row(row =>
                                    {
                                        row.RelativeItem().Text("Giảm giá:");
                                        row.ConstantItem(120).AlignRight().Text($"{tongGiamGia:N0} ₫").FontColor(Colors.Red.Medium);
                                    });

                                col.Item().PaddingTop(10).Background(Colors.Blue.Lighten4).Padding(10)
                                    .Row(row =>
                                    {
                                        row.RelativeItem().Text("THÀNH TIỀN:").Bold().FontSize(14);
                                        row.ConstantItem(120).AlignRight().Text($"{tongThanhTien:N0} ₫").Bold().FontSize(14).FontColor(Colors.Blue.Darken2);
                                    });
                            });

                            // Lời cảm ơn
                            column.Item().PaddingTop(30).AlignCenter().Text("Cảm ơn quý khách! Hẹn gặp lại!")
                                .Italic().FontSize(12).FontColor(Colors.Grey.Darken1);
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Trang ");
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });
                });
            })
            .GeneratePdf(outputPath);
        }
    }
}
