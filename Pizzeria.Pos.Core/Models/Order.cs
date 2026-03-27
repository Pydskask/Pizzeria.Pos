// Pizzeria.Pos.Core/Models/Order.cs
using System;
using System.Collections.Generic;

namespace Pizzeria.Pos.Core.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Type { get; set; } = "M";
        public decimal Total { get; set; }
        public bool IsPaid { get; set; } = false;
        public bool IsCanceled { get; set; } = false;

        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string HouseNumber { get; set; } = string.Empty;
        public string ApartmentNumber { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        public string DeliverySymbol { get; set; } = string.Empty;
        public string DeliveryCity { get; set; } = string.Empty;
        public string DeliveryStreet { get; set; } = string.Empty;
        public string DeliveryHouseNumber { get; set; } = string.Empty;
        public string DeliveryApartmentNumber { get; set; } = string.Empty;
        public string DeliveryPostalCode { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public List<OrderItem> Items { get; set; } = new();
    }
}