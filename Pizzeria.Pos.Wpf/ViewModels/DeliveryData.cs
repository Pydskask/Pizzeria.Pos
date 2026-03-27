using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzeria.Pos.Wpf.ViewModels;

public class DeliveryData
{
    public string Phone { get; set; } = "";
    public string Symbol { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public string City { get; set; } = "";
    public string Street { get; set; } = "";
    public string HouseNumber { get; set; } = "";
    public string ApartmentNumber { get; set; } = "";
    public string PostalCode { get; set; } = "";
    public string Notes { get; set; } = "";
    public decimal DeliveryPrice { get; set; }
    public DateTime DeliveryTime { get; set; } = DateTime.Now.AddMinutes(45);
    public string SelectedPaymentMethod { get; set; } = "Gotówka";

    public string FullAddress =>
        $"{City}, {Street} {HouseNumber}{(string.IsNullOrWhiteSpace(ApartmentNumber) ? "" : "/" + ApartmentNumber)}";

    public string ShortSummary =>
        $"{Phone} | {FullAddress} | {SelectedPaymentMethod}";
}
