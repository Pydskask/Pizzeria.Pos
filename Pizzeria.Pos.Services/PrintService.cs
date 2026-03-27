using Pizzeria.Pos.Core.Models;
using Pizzeria.Pos.Data;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Pizzeria.Pos.Services;

public class PrintService : IPrintService
{
    private readonly PosDataContext context;

    public PrintService(PosDataContext context)
    {
        this.context = context;
    }

    public bool PrintOrderReceipt(Order order, User? printedBy = null)
    {
        var content = BuildReceiptContent(order);
        return PrintTextDocument(
            title: $"Paragon_{order.Id}",
            content: content,
            orderId: order.Id,
            printedBy: printedBy,
            documentType: "Paragon");
    }

    public bool PrintKitchenBon(Order order, User? printedBy = null)
    {
        var sb = new StringBuilder();
        sb.AppendLine("BON KUCHENNY");
        sb.AppendLine($"Zamówienie #{order.Id}   {TypeLabel(order.Type)}");
        sb.AppendLine($"Czas: {DateTime.Now:HH:mm:ss}");
        sb.AppendLine("--------------------------");

        foreach (var item in order.Items)
            sb.AppendLine($"{item.Quantity} x {item.Name}");

        if (!string.IsNullOrWhiteSpace(order.Notes))
        {
            sb.AppendLine();
            sb.AppendLine("UWAGI:");
            sb.AppendLine(order.Notes);
        }

        return PrintTextDocument(
            title: $"BON_KUCHENNY_{order.Id}",
            content: sb.ToString(),
            orderId: order.Id,
            printedBy: printedBy,
            documentType: "BON kuchenny");
    }

    public bool PrintDeliveryBon(Order order, User? printedBy = null)
    {
        var sb = new StringBuilder();
        sb.AppendLine("BON DOSTAWCY");
        sb.AppendLine($"Zamówienie #{order.Id}");
        sb.AppendLine($"Czas: {DateTime.Now:HH:mm:ss}");
        sb.AppendLine("--------------------------");

        foreach (var item in order.Items)
            sb.AppendLine($"{item.Quantity} x {item.Name}");

        sb.AppendLine("--------------------------");
        sb.AppendLine($"Telefon: {order.CustomerPhone}");
        sb.AppendLine($"Adres: {order.Address}");

        if (!string.IsNullOrWhiteSpace(order.Notes))
            sb.AppendLine($"Uwagi: {order.Notes}");

        sb.AppendLine($"Kwota: {order.Total:F2} zł");

        return PrintTextDocument(
            title: $"BON_DOSTAWY_{order.Id}",
            content: sb.ToString(),
            orderId: order.Id,
            printedBy: printedBy,
            documentType: "BON dostawcy");
    }

    public bool PrintTextDocument(
        string title,
        string content,
        int? orderId = null,
        User? printedBy = null,
        string documentType = "Tekst")
    {
        string outputInfo = string.Empty;
        string errorMessage = string.Empty;
        var success = false;

        try
        {
            var baseDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "PizzeriaPOS",
                "Prints",
                DateTime.Now.ToString("yyyy-MM-dd"));

            Directory.CreateDirectory(baseDir);

            var safeType = SanitizeFileName(documentType);
            var safeTitle = SanitizeFileName(title);
            var orderPart = orderId.HasValue ? $"ORD_{orderId.Value}_" : string.Empty;
            var fileName = $"{DateTime.Now:HHmmss}_{orderPart}{safeType}_{safeTitle}.txt";
            var fullPath = Path.Combine(baseDir, fileName);

            File.WriteAllText(fullPath, content, Encoding.UTF8);

            outputInfo = fullPath;
            success = true;

            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = fullPath,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
            catch
            {
            }
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            MessageBox.Show(
                $"Błąd generowania dokumentu: {ex.Message}",
                "Błąd",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        SaveLog(new PrintedDocument
        {
            OrderId = orderId,
            DocumentType = documentType,
            PrinterName = success ? outputInfo : "TXT_ERROR",
            JobName = title,
            ContentPreview = SafePreview(content),
            CreatedAt = DateTime.Now,
            Success = success,
            ErrorMessage = errorMessage,
            UserId = printedBy?.Id
        });

        return success;
    }

    private static string BuildReceiptContent(Order order)
    {
        var sb = new StringBuilder();
        sb.AppendLine(" ╔══════════════════════════════════════╗");
        sb.AppendLine(" ║            PIZZERIA POS              ║");
        sb.AppendLine(" ╠══════════════════════════════════════╣");
        sb.AppendLine($"║ Zamówienie #{order.Id}               ║");
        sb.AppendLine($"Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"Typ: {TypeLabel(order.Type)}");
        sb.AppendLine($"Płatność: {order.PaymentMethod}");
        sb.AppendLine("--------------------------------");

        foreach (var item in order.Items)
        {
            sb.AppendLine($"{item.Quantity} x {item.Name}");
            sb.AppendLine($"{item.Price:F2} x {item.Quantity} = {(item.Price * item.Quantity):F2} zł");
        }

        sb.AppendLine("--------------------------------");
        sb.AppendLine($"SUMA: {order.Total:F2} zł");

        if (!string.IsNullOrWhiteSpace(order.CustomerName))
            sb.AppendLine($"Klient: {order.CustomerName}");

        if (!string.IsNullOrWhiteSpace(order.CustomerPhone))
            sb.AppendLine($"Telefon: {order.CustomerPhone}");

        if (!string.IsNullOrWhiteSpace(order.Address))
            sb.AppendLine($"Adres: {order.Address}");

        if (!string.IsNullOrWhiteSpace(order.Notes))
            sb.AppendLine($"Uwagi: {order.Notes}");

        sb.AppendLine();
        sb.AppendLine("Dziękujemy!");
        sb.AppendLine();

        return sb.ToString();
    }

    private static string TypeLabel(string type)
    {
        return type switch
        {
            "M" => "Na miejscu",
            "W" => "Wynos",
            "D" => "Dostawa",
            _ => type
        };
    }

    private static string SafePreview(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return string.Empty;

        return content.Length <= 500
            ? content
            : content[..500];
    }

    private static string SanitizeFileName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "Dokument";

        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(value
            .Select(c => invalidChars.Contains(c) ? '_' : c)
            .ToArray());

        while (sanitized.Contains("__"))
            sanitized = sanitized.Replace("__", "_");

        return sanitized.Trim('_', ' ');
    }

    private void SaveLog(PrintedDocument log)
    {
        try
        {
            context.PrintedDocuments.Add(log);
            context.SaveChanges();
        }
        catch
        {
            context.ChangeTracker.Clear();
        }
    }
}