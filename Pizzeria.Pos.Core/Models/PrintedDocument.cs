using System;

namespace Pizzeria.Pos.Core.Models
{
    public class PrintedDocument
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string PrinterName { get; set; } = string.Empty;
        public string JobName { get; set; } = string.Empty;
        public string ContentPreview { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public int? UserId { get; set; }
    }
}