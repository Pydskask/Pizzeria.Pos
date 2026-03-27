using System;
using System.Collections.Generic;
using System.Linq;
using Pizzeria.Pos.Core.Models;
using Pizzeria.Pos.Wpf.ViewModels;

namespace Pizzeria.Pos.Wpf.Helpers
{
    public static class DeliveryOrderHelper
    {
        public const int DeliveryChargeProductId = -1;
        public const string DeliveryChargeName = "Dostawa";

        public static bool IsDeliveryCharge(string? name, int productId)
        {
            return productId == DeliveryChargeProductId
                || string.Equals(name, DeliveryChargeName, StringComparison.OrdinalIgnoreCase);
        }

        public static decimal GetDeliveryPrice(IEnumerable<OrderItem> items, decimal fallback = 0m)
        {
            return items?
                .FirstOrDefault(x => IsDeliveryCharge(x.Name, x.ProductId))
                ?.Price ?? fallback;
        }

        public static decimal GetDeliveryPrice(IEnumerable<CartItemViewModel> items, decimal fallback = 0m)
        {
            return items?
                .FirstOrDefault(x => IsDeliveryCharge(x.Name, x.ProductId))
                ?.UnitPrice ?? fallback;
        }

        public static void UpsertDeliveryCharge(ICollection<OrderItem> items, decimal deliveryPrice)
        {
            var existing = items.FirstOrDefault(x => IsDeliveryCharge(x.Name, x.ProductId));

            if (deliveryPrice <= 0m)
            {
                if (existing != null)
                    items.Remove(existing);

                return;
            }

            if (existing == null)
            {
                items.Add(new OrderItem
                {
                    ProductId = DeliveryChargeProductId,
                    Name = DeliveryChargeName,
                    Price = deliveryPrice,
                    Quantity = 1,
                    BaseProductName = DeliveryChargeName,
                    BaseProductPrice = deliveryPrice,
                    ConfigurationJson = null
                });

                return;
            }

            existing.ProductId = DeliveryChargeProductId;
            existing.Name = DeliveryChargeName;
            existing.Price = deliveryPrice;
            existing.Quantity = 1;
            existing.BaseProductName = DeliveryChargeName;
            existing.BaseProductPrice = deliveryPrice;
            existing.ConfigurationJson = null;
        }

        public static void UpsertDeliveryCharge(ICollection<CartItemViewModel> items, decimal deliveryPrice)
        {
            var existing = items.FirstOrDefault(x => IsDeliveryCharge(x.Name, x.ProductId));

            if (deliveryPrice <= 0m)
            {
                if (existing != null)
                    items.Remove(existing);

                return;
            }

            if (existing == null)
            {
                items.Add(new CartItemViewModel
                {
                    ProductId = DeliveryChargeProductId,
                    Name = DeliveryChargeName,
                    UnitPrice = deliveryPrice,
                    Quantity = 1,
                    IsPizzaConfigurable = false,
                    BaseProductName = DeliveryChargeName,
                    BaseProductPrice = deliveryPrice,
                    PizzaConfig = null
                });

                return;
            }

            existing.ProductId = DeliveryChargeProductId;
            existing.Name = DeliveryChargeName;
            existing.UnitPrice = deliveryPrice;
            existing.Quantity = 1;
            existing.IsPizzaConfigurable = false;
            existing.BaseProductName = DeliveryChargeName;
            existing.BaseProductPrice = deliveryPrice;
            existing.PizzaConfig = null;
        }

        public static void RemoveDeliveryCharge(ICollection<OrderItem> items)
        {
            var toRemove = items
                .Where(x => IsDeliveryCharge(x.Name, x.ProductId))
                .ToList();

            foreach (var item in toRemove)
                items.Remove(item);
        }

        public static void ApplyDeliveryDataToOrder(Order order, DeliveryData data)
        {
            order.CustomerPhone = data.Phone ?? string.Empty;
            order.CustomerName = data.CustomerName ?? string.Empty;
            order.Address = data.FullAddress ?? string.Empty;
            order.Notes = data.Notes ?? string.Empty;

            order.DeliverySymbol = data.Symbol ?? string.Empty;

            order.City = data.City ?? string.Empty;
            order.Street = data.Street ?? string.Empty;
            order.HouseNumber = data.HouseNumber ?? string.Empty;
            order.ApartmentNumber = data.ApartmentNumber ?? string.Empty;
            order.PostalCode = data.PostalCode ?? string.Empty;

            order.DeliveryCity = data.City ?? string.Empty;
            order.DeliveryStreet = data.Street ?? string.Empty;
            order.DeliveryHouseNumber = data.HouseNumber ?? string.Empty;
            order.DeliveryApartmentNumber = data.ApartmentNumber ?? string.Empty;
            order.DeliveryPostalCode = data.PostalCode ?? string.Empty;
        }

        public static void ClearDeliveryFields(Order order)
        {
            order.Address = string.Empty;
            order.DeliverySymbol = string.Empty;

            order.City = string.Empty;
            order.Street = string.Empty;
            order.HouseNumber = string.Empty;
            order.ApartmentNumber = string.Empty;
            order.PostalCode = string.Empty;

            order.DeliveryCity = string.Empty;
            order.DeliveryStreet = string.Empty;
            order.DeliveryHouseNumber = string.Empty;
            order.DeliveryApartmentNumber = string.Empty;
            order.DeliveryPostalCode = string.Empty;
        }
    }
}