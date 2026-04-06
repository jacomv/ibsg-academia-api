using Academia.Application.Common.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Academia.Infrastructure.Certificates;

public class QuestPdfCertificateGenerator : ICertificateGenerator
{
    static QuestPdfCertificateGenerator()
    {
        // QuestPDF community license — free for open-source
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] Generate(CertificateData data)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(40);
                page.Background(Colors.White);

                page.Content().Column(col =>
                {
                    // Border decoration
                    col.Item().BorderBottom(3).BorderColor("#1e40af").PaddingBottom(12).Row(row =>
                    {
                        row.RelativeItem().Text("✝ IBSG Academia")
                            .FontSize(28).Bold().FontColor("#1e40af");
                        row.ConstantItem(200).AlignRight().Text("Certificado de Completado")
                            .FontSize(12).FontColor("#6b7280");
                    });

                    col.Item().PaddingTop(40).PaddingBottom(20).AlignCenter()
                        .Text("CERTIFICA QUE")
                        .FontSize(14).FontColor("#6b7280").LetterSpacing(3);

                    col.Item().AlignCenter()
                        .Text(data.StudentFullName)
                        .FontSize(36).Bold().FontColor("#111827");

                    col.Item().PaddingTop(20).PaddingBottom(20).AlignCenter()
                        .Text("ha completado exitosamente el curso")
                        .FontSize(14).FontColor("#6b7280");

                    col.Item().AlignCenter()
                        .Text(data.CourseTitle)
                        .FontSize(24).Bold().FontColor("#1e40af");

                    if (!string.IsNullOrWhiteSpace(data.CourseDescription))
                    {
                        col.Item().PaddingTop(12).AlignCenter()
                            .Text(data.CourseDescription)
                            .FontSize(11).FontColor("#9ca3af").Italic();
                    }

                    col.Item().PaddingTop(50).Row(row =>
                    {
                        row.RelativeItem().Column(inner =>
                        {
                            inner.Item().BorderTop(1).BorderColor("#d1d5db").PaddingTop(8)
                                .Text(data.IssuedAt.ToString("dd 'de' MMMM 'de' yyyy",
                                    new System.Globalization.CultureInfo("es-MX")))
                                .FontSize(11).FontColor("#6b7280");
                            inner.Item().Text("Fecha de emisión").FontSize(9).FontColor("#9ca3af");
                        });

                        row.ConstantItem(200).AlignRight().Column(inner =>
                        {
                            inner.Item().BorderTop(1).BorderColor("#d1d5db").PaddingTop(8)
                                .AlignRight()
                                .Text($"#{data.CertificateNumber}")
                                .FontSize(11).FontColor("#6b7280");
                            inner.Item().AlignRight()
                                .Text("Número de certificado").FontSize(9).FontColor("#9ca3af");
                        });
                    });
                });
            });
        }).GeneratePdf();
    }
}
