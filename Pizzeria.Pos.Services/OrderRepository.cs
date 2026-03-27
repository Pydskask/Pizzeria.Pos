using Pizzeria.Pos.Core.Models;
using Pizzeria.Pos.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Pizzeria.Pos.Services
{
    public class OrderRepository : IOrderRepository
    {
        private readonly PosDataContext context;

        public OrderRepository(PosDataContext context)
        {
            this.context = context;
        }

        public List<Order> GetActiveOrders()
        {
            return QueryWithDetails()
                .Where(o => !o.IsPaid && !o.IsCanceled)
                .OrderByDescending(o => o.CreatedAt)
                .ThenByDescending(o => o.Id)
                .ToList();
        }

        public List<Order> GetAll()
        {
            return QueryWithDetails()
                .OrderByDescending(o => o.CreatedAt)
                .ThenByDescending(o => o.Id)
                .ToList();
        }

        public List<Order> GetCanceledOrders()
        {
            return QueryWithDetails()
                .Where(o => o.IsCanceled)
                .OrderByDescending(o => o.CreatedAt)
                .ThenByDescending(o => o.Id)
                .ToList();
        }

        public List<Order> GetPaidOrders()
        {
            return QueryWithDetails()
                .Where(o => o.IsPaid && !o.IsCanceled)
                .OrderByDescending(o => o.CreatedAt)
                .ThenByDescending(o => o.Id)
                .ToList();
        }

        public Order? GetById(int id)
        {
            return QueryWithDetails()
                .FirstOrDefault(o => o.Id == id);
        }

        public Order AddOrder(Order order)
        {
            var dbOrder = new Order
            {
                Type = order.Type,
                Total = order.Total,
                IsPaid = order.IsPaid,
                IsCanceled = order.IsCanceled,
                CustomerPhone = order.CustomerPhone,
                CustomerName = order.CustomerName,
                Address = order.Address,
                Notes = order.Notes,
                PaymentMethod = order.PaymentMethod,
                City = order.City,
                Street = order.Street,
                HouseNumber = order.HouseNumber,
                ApartmentNumber = order.ApartmentNumber,
                PostalCode = order.PostalCode,
                DeliverySymbol = order.DeliverySymbol,
                DeliveryCity = order.DeliveryCity,
                DeliveryStreet = order.DeliveryStreet,
                DeliveryHouseNumber = order.DeliveryHouseNumber,
                DeliveryApartmentNumber = order.DeliveryApartmentNumber,
                DeliveryPostalCode = order.DeliveryPostalCode,
                CreatedAt = order.CreatedAt,
                UserId = order.UserId,
                Items = order.Items.Select(x => new OrderItem
                {
                    Name = x.Name,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    ProductId = x.ProductId,
                    BaseProductName = x.BaseProductName,
                    BaseProductPrice = x.BaseProductPrice,
                    ConfigurationJson = x.ConfigurationJson
                }).ToList()
            };

            context.Orders.Add(dbOrder);
            context.SaveChanges();

            return GetById(dbOrder.Id)!;
        }

        public void UpdateOrder(Order order)
        {
            var existingOrder = context.Orders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.Id == order.Id);

            if (existingOrder == null)
                return;

            var sourceItems = (order.Items ?? new List<OrderItem>())
                .Select(item => new OrderItem
                {
                    Name = item.Name,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    ProductId = item.ProductId,
                    BaseProductName = item.BaseProductName,
                    BaseProductPrice = item.BaseProductPrice,
                    ConfigurationJson = item.ConfigurationJson
                })
                .ToList();

            existingOrder.Type = order.Type;
            existingOrder.Total = order.Total;
            existingOrder.IsPaid = order.IsPaid;
            existingOrder.IsCanceled = order.IsCanceled;
            existingOrder.PaymentMethod = order.PaymentMethod;
            existingOrder.CustomerPhone = order.CustomerPhone;
            existingOrder.CustomerName = order.CustomerName;
            existingOrder.Address = order.Address;
            existingOrder.Notes = order.Notes;
            existingOrder.City = order.City;
            existingOrder.Street = order.Street;
            existingOrder.HouseNumber = order.HouseNumber;
            existingOrder.ApartmentNumber = order.ApartmentNumber;
            existingOrder.PostalCode = order.PostalCode;
            existingOrder.DeliverySymbol = order.DeliverySymbol;
            existingOrder.DeliveryCity = order.DeliveryCity;
            existingOrder.DeliveryStreet = order.DeliveryStreet;
            existingOrder.DeliveryHouseNumber = order.DeliveryHouseNumber;
            existingOrder.DeliveryApartmentNumber = order.DeliveryApartmentNumber;
            existingOrder.DeliveryPostalCode = order.DeliveryPostalCode;
            existingOrder.UserId = order.UserId;

            if (existingOrder.Items.Any())
                context.OrderItems.RemoveRange(existingOrder.Items.ToList());

            existingOrder.Items.Clear();

            foreach (var item in sourceItems)
                existingOrder.Items.Add(item);

            context.SaveChanges();
        }

        public void CancelOrder(int orderId, string? note = null)
        {
            var order = context.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
                return;

            order.IsCanceled = true;

            if (!string.IsNullOrWhiteSpace(note))
                order.Notes = string.IsNullOrWhiteSpace(order.Notes)
                    ? note
                    : order.Notes + Environment.NewLine + note;

            context.SaveChanges();
        }

        private IQueryable<Order> QueryWithDetails()
        {
            return context.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .Include(o => o.User);
        }

        public List<DeliveryAddressLookupResult> GetDeliveryAddressesByPhone(string phone)
        {
            var normalizedPhone = NormalizePhone(phone);

            if (string.IsNullOrWhiteSpace(normalizedPhone))
                return new List<DeliveryAddressLookupResult>();

            var results = context.Orders
                .AsNoTracking()
                .Where(o => o.Type == "D" && !o.IsCanceled)
                .OrderByDescending(o => o.CreatedAt)
                .ToList()
                .Where(o => NormalizePhone(o.CustomerPhone) == normalizedPhone)
                .Select(o => new DeliveryAddressLookupResult
                {
                    Symbol = o.DeliverySymbol?.Trim() ?? string.Empty,
                    CustomerName = o.CustomerName?.Trim() ?? string.Empty,
                    City = FirstNonEmpty(o.DeliveryCity, o.City),
                    Street = FirstNonEmpty(o.DeliveryStreet, o.Street),
                    HouseNumber = FirstNonEmpty(o.DeliveryHouseNumber, o.HouseNumber),
                    ApartmentNumber = FirstNonEmpty(o.DeliveryApartmentNumber, o.ApartmentNumber),
                    PostalCode = FirstNonEmpty(o.DeliveryPostalCode, o.PostalCode),
                    Notes = o.Notes?.Trim() ?? string.Empty,
                    LastOrderAt = o.CreatedAt
                })
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.City) &&
                    !string.IsNullOrWhiteSpace(x.Street) &&
                    !string.IsNullOrWhiteSpace(x.HouseNumber))
                .GroupBy(x => new
                {
                    City = NormalizeText(x.City),
                    Street = NormalizeText(x.Street),
                    HouseNumber = NormalizeText(x.HouseNumber),
                    ApartmentNumber = NormalizeText(x.ApartmentNumber),
                    PostalCode = NormalizeText(x.PostalCode)
                })
                .Select(g => g.OrderByDescending(x => x.LastOrderAt).First())
                .OrderByDescending(x => x.LastOrderAt)
                .ToList();

            return results;
        }

        private static string NormalizePhone(string? phone)
        {
            return new string((phone ?? string.Empty).Where(char.IsDigit).ToArray());
        }

        private static string FirstNonEmpty(params string?[] values)
        {
            return values.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x))?.Trim() ?? string.Empty;
        }

        private static string NormalizeText(string? value)
        {
            return (value ?? string.Empty).Trim().ToUpperInvariant();
        }
    }
}