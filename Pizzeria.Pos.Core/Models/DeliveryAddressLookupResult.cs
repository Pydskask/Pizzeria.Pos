using System;

namespace Pizzeria.Pos.Core.Models
{
    public class DeliveryAddressLookupResult
    {
        public string Symbol { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string HouseNumber { get; set; } = string.Empty;
        public string ApartmentNumber { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime LastOrderAt { get; set; }
    }
}