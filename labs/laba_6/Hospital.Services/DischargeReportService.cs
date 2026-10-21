using Hospital.Business;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Hospital.Services;

public sealed class DischargeReportService
{
    private const string Ink = "#17313A";
    private const string Muted = "#647982";
    private const string Teal = "#087F78";
    private const string Border = "#DCE6E7";

    public byte[] CreatePdf(Patient patient)
    {
        if (!patient.IsDischarged || patient.DischargeSummary is null)
            throw new ValidationException("PDF-эпикриз доступен только для выписанного пациента.");

        var summary = patient.DischargeSummary;
        var nurses = patient.Orders
            .Where(order => order.Type == OrderType.Medication)
            .Select(order => order.ResponsibleStaff)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.CurrentCultureIgnoreCase)
            .ToArray();

        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(42);
                page.DefaultTextStyle(style => style.FontFamily("Lato").FontSize(9).FontColor(Ink));
                page.Header().Column(header =>
                {
                    header.Item().Row(row =>
                    {
                        row.RelativeItem().Column(title =>
                        {
                            title.Item().Text("ВЫПИСНОЙ ЭПИКРИЗ").FontSize(19).Bold().FontColor(Ink);
                            title.Item().PaddingTop(4).Text("Стационар · электронная медицинская карта").FontSize(9).FontColor(Muted);
                        });
                        row.ConstantItem(112).AlignRight().Column(meta =>
                        {
                            meta.Item().Text("КАРТА ПАЦИЕНТА").FontSize(7).Bold().FontColor(Teal);
                            meta.Item().PaddingTop(3).Text(patient.Id.ToString("N")[..8].ToUpperInvariant()).FontSize(9).SemiBold();
                        });
                    });
                    header.Item().PaddingTop(13).LineHorizontal(1).LineColor(Border);
                });

                page.Content().PaddingTop(17).Column(content =>
                {
                    content.Spacing(16);
                    content.Item().Column(section =>
                    {
                        SectionTitle(section, "Сведения о пациенте");
                        section.Item().PaddingTop(7).Table(table =>
                        {
                            table.ColumnsDefinition(columns => { columns.RelativeColumn(); columns.RelativeColumn(); });
                            DetailCell(table, "ФИО", summary.PatientName);
                            DetailCell(table, "Дата рождения", summary.BirthDate.ToString("dd.MM.yyyy"));
                            DetailCell(table, "Телефон", string.IsNullOrWhiteSpace(patient.Phone) ? "Не указан" : patient.Phone);
                            DetailCell(table, "Отделение", summary.Department);
                            DetailCell(table, "Лечащий врач", patient.AttendingDoctor);
                            DetailCell(table, "Медсестра(ы), выполнявшие назначения", nurses.Length == 0 ? "Медикаментозных назначений нет" : string.Join(", ", nurses));
                        });
                    });

                    content.Item().Column(section =>
                    {
                        SectionTitle(section, "Период лечения");
                        section.Item().PaddingTop(7).Background("#F3F8F7").Border(1).BorderColor(Border).Padding(11).Row(row =>
                        {
                            row.RelativeItem().Column(col => { Label(col, "ПОСТУПЛЕНИЕ"); Value(col, summary.AdmittedAt.ToString("dd.MM.yyyy, HH:mm")); });
                            row.RelativeItem().Column(col => { Label(col, "ВЫПИСКА"); Value(col, summary.DischargedAt.ToString("dd.MM.yyyy, HH:mm")); });
                            row.RelativeItem().Column(col => { Label(col, "ДЛИТЕЛЬНОСТЬ"); Value(col, StayLength(summary.AdmittedAt, summary.DischargedAt)); });
                        });
                        section.Item().PaddingTop(8).Text(text =>
                        {
                            text.Span("Диагноз при поступлении: ").Bold();
                            text.Span(summary.AdmissionDiagnosis);
                        });
                    });

                    content.Item().Column(section =>
                    {
                        SectionTitle(section, $"Назначения ({patient.Orders.Count})");
                        if (patient.Orders.Count == 0)
                            section.Item().PaddingTop(7).Text("Назначения не зарегистрированы.").FontColor(Muted);
                        else
                            section.Item().PaddingTop(7).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(23);
                                    columns.RelativeColumn(2.2f);
                                    columns.RelativeColumn(2.4f);
                                    columns.RelativeColumn(1.5f);
                                    columns.RelativeColumn(1.3f);
                                });
                                table.Header(header =>
                                {
                                    HeaderCell(header, "№");
                                    HeaderCell(header, "НАЗНАЧЕНИЕ");
                                    HeaderCell(header, "ВИД И ПАРАМЕТРЫ");
                                    HeaderCell(header, "ОТВЕТСТВЕННЫЙ");
                                    HeaderCell(header, "НАЗНАЧЕНО / ВЫПОЛНЕНО");
                                });
                                var ordered = patient.Orders.OrderBy(order => order.CreatedAt).ToArray();
                                for (var index = 0; index < ordered.Length; index++)
                                {
                                    var order = ordered[index];
                                    var background = index % 2 == 0 ? "#FFFFFF" : "#F6F9F9";
                                    BodyCell(table, (index + 1).ToString(), background);
                                    BodyCell(table, order.Name, background);
                                    BodyCell(table, DescribeOrder(order), background);
                                    BodyCell(table, order.ResponsibleStaff, background);
                                    var dates = $"Назначено: {order.CreatedAt:dd.MM.yy HH:mm}\nВыполнено: {order.CompletedAt?.ToString("dd.MM.yy HH:mm") ?? "нет"}";
                                    BodyCell(table, dates, background);
                                }
                            });
                    });

                    content.Item().Column(section =>
                    {
                        SectionTitle(section, "Записи электронной медицинской карты");
                        var entries = patient.Record.OrderBy(entry => entry.OccurredAt).ToArray();
                        if (entries.Length == 0)
                            section.Item().PaddingTop(7).Text("Записей нет.").FontColor(Muted);
                        else
                            section.Item().PaddingTop(7).Column(list =>
                            {
                                list.Spacing(7);
                                foreach (var entry in entries)
                                    list.Item().Row(row =>
                                    {
                                        row.ConstantItem(88).Text(entry.OccurredAt.ToString("dd.MM.yy HH:mm")).FontSize(8).FontColor(Muted);
                                        row.RelativeItem().Text(text =>
                                        {
                                            text.Span(entry.Text);
                                            text.Span($" · {entry.Author}").FontColor(Muted);
                                        });
                                    });
                            });
                    });
                });

                page.Footer().PaddingTop(8).BorderTop(1).BorderColor(Border).Row(row =>
                {
                    row.RelativeItem().PaddingTop(7).Text("Сформировано информационной системой больницы").FontSize(7).FontColor(Muted);
                    row.ConstantItem(80).AlignRight().PaddingTop(7).Text(text =>
                    {
                        text.Span("Страница ").FontSize(7).FontColor(Muted);
                        text.CurrentPageNumber().FontSize(7).FontColor(Muted);
                        text.Span(" / ").FontSize(7).FontColor(Muted);
                        text.TotalPages().FontSize(7).FontColor(Muted);
                    });
                });
            });
        }).GeneratePdf();
    }

    private static string DescribeOrder(MedicalOrder order)
    {
        if (order.Type == OrderType.Medication)
            return order.MedicationForm == MedicationForm.Injection
                ? $"Медикаментозное, инъекция; количество: {order.Quantity}"
                : $"Медикаментозное, таблетки; {order.Dosage}; дней: {order.Days}";

        return order.Type == OrderType.Diagnostic ? "Диагностическое" : "Профилактическое";
    }

    private static string StayLength(DateTime admittedAt, DateTime dischargedAt)
    {
        var days = Math.Max(1, (int)(dischargedAt - admittedAt).TotalDays);
        return $"{days} {Pluralize(days, "день", "дня", "дней")}";
    }

    private static string Pluralize(int value, string one, string few, string many)
    {
        var lastTwo = value % 100;
        if (lastTwo is >= 11 and <= 14) return many;
        return (value % 10) switch { 1 => one, 2 or 3 or 4 => few, _ => many };
    }

    private static void SectionTitle(ColumnDescriptor section, string title) =>
        section.Item().Text(title).FontSize(11).Bold().FontColor(Teal);

    private static void Label(ColumnDescriptor column, string text) =>
        column.Item().Text(text).FontSize(7).Bold().FontColor(Muted);

    private static void Value(ColumnDescriptor column, string text) =>
        column.Item().PaddingTop(3).Text(text).FontSize(9).SemiBold();

    private static void DetailCell(TableDescriptor table, string label, string value) =>
        table.Cell().PaddingVertical(5).PaddingRight(12).Column(column =>
        {
            Label(column, label.ToUpperInvariant());
            Value(column, value);
        });

    private static void HeaderCell(TableCellDescriptor header, string text) =>
        header.Cell().Background(Teal).PaddingVertical(7).PaddingHorizontal(5)
            .Text(text).FontSize(7).Bold().FontColor(Colors.White);

    private static void BodyCell(TableDescriptor table, string text, string background) =>
        table.Cell().Background(background).BorderBottom(0.5f).BorderColor(Border)
            .PaddingVertical(6).PaddingHorizontal(5).Text(text).FontSize(7.5f);
}
