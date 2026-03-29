using Pizzeria.Pos.Core.Models;

namespace Pizzeria.Pos.Services
{
    public interface IOrderRepository
    {
        List<Order> GetActiveOrders();
        List<Order> GetAll();
        List<Order> GetCanceledOrders();
        List<Order> GetPaidOrders();
        Order? GetById(int id);
        Order AddOrder(Order order);
        void UpdateOrder(Order order);
        void CancelOrder(int orderId, string? note = null);

        List<DeliveryAddressLookupResult> GetDeliveryAddressesByPhone(string phone);
    }
}

