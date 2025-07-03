using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HospitalManagementSystem_HMS_.BL.AppRepo.Utility.IService;
using HospitalManagementSystem_HMS_.Dtos.AppDtos.PatientDtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HospitalManagementSystem_HMS_.BL.AppRepo.Utility.Implementation
{
    public class PdfService : IPdfService
    {
        public byte[] GeneratePrescriptionPdf(PrescriptionPdfDto dto)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // Header
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("🏥 Hospital Management System")
                                .FontSize(20)
                                .Bold()
                                .FontColor(Colors.Blue.Medium);

                            col.Item().Text("Prescription Report")
                                .FontSize(14)
                                .Italic()
                                .FontColor(Colors.Grey.Medium);
                        });

                        row.ConstantItem(100).AlignRight().Text($"Date:\n{dto.Date:dd MMM yyyy}")
                            .AlignRight()
                            .FontSize(10);
                    });

                    // Content
                    page.Content().PaddingVertical(20).Element(container =>
                    {
                        container.Column(col =>
                        {
                            col.Spacing(8);

                            col.Item().Text($"👨‍⚕️ Doctor: {dto.DoctorName}")
                                .FontSize(13).Bold();

                            col.Item().Text($"🧑‍🦰 Patient: {dto.PatientName}")
                                .FontSize(13).Bold();

                            col.Item().Text($"📝 Diagnosis: {dto.Diagnosis}")
                                .FontColor(Colors.Red.Darken1)
                                .FontSize(12);

                            if (!string.IsNullOrWhiteSpace(dto.Note))
                            {
                                col.Item().Text($"📌 Note: {dto.Note}")
                                    .FontSize(12)
                                    .Italic()
                                    .FontColor(Colors.Grey.Darken2);
                            }

                            // Medicine Table Header
                            col.Item().PaddingTop(20).Text("💊 Prescribed Medicines")
                                .FontSize(17).Bold();

                            // Medicines Table
                            col.Item().PaddingTop(10).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2); // Medicine Name
                                    columns.RelativeColumn();   // Dosage
                                    columns.RelativeColumn();   // Frequency
                                    columns.RelativeColumn();   // Duration
                                    columns.RelativeColumn(2); // Instructions
                                });

                                // Table Header
                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("Medicine");
                                    header.Cell().Element(CellStyle).Text("Dosage");
                                    header.Cell().Element(CellStyle).Text("Frequency");
                                    header.Cell().Element(CellStyle).Text("Duration");
                                    header.Cell().Element(CellStyle).Text("Instructions");

                                    static IContainer CellStyle(IContainer container) =>
                                        container.DefaultTextStyle(x => x.SemiBold().FontColor(Colors.Blue.Darken2))
                                                 .PaddingVertical(5)
                                                 .BorderBottom(1)
                                                 .BorderColor(Colors.Grey.Lighten2);
                                });

                                // Table Rows
                                foreach (var med in dto.Medicines)
                                {
                                    table.Cell().Element(e => e.PaddingVertical(5)).Text(med.MedicineName);
                                    table.Cell().Element(e => e.PaddingVertical(5)).Text(med.Dosage);
                                    table.Cell().Element(e => e.PaddingVertical(5)).Text(med.Frequency);
                                    table.Cell().Element(e => e.PaddingVertical(5)).Text(med.Duration);
                                    table.Cell().Element(e => e.PaddingVertical(5)).Text(med.Instructions ?? "-");
                                }

                            });
                        });
                    });

                    // Footer
                    page.Footer().AlignCenter().Text("Generated by Hospital Management System")
                        .FontSize(10).FontColor(Colors.Grey.Medium);
                });
            });

            return document.GeneratePdf();
        }
    }
}
