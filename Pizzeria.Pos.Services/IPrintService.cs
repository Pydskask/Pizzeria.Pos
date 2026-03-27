using Pizzeria.Pos.Core.Models;

namespace Pizzeria.Pos.Services
{
    public interface IPrintService
    {
        bool PrintOrderReceipt(Order order, User? printedBy = null);
        bool PrintKitchenBon(Order order, User? printedBy = null);
        bool PrintDeliveryBon(Order order, User? printedBy = null);
        bool PrintTextDocument(string title, string content, int? orderId = null, User? printedBy = null, string documentType = "Tekst");
    }
}