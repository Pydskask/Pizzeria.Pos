using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzeria.Pos.Wpf.ViewModels;

public class DeliveryAddressViewModel
{
    public string Symbol { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public string City { get; set; } = "";
    public string Street { get; set; } = "";
    public string HouseNumber { get; set; } = "";
    public string ApartmentNumber { get; set; } = "";
    public string PostalCode { get; set; } = "";
    public string Notes { get; set; } = "";

    public string DisplayName =>
        string.IsNullOrWhiteSpace(Symbol)
            ? CustomerName
            : $"{Symbol} - {CustomerName}";

    public string DisplayAddress =>
        $"{City}, {Street} {HouseNumber}{(string.IsNullOrWhiteSpace(ApartmentNumber) ? "" : "/" + ApartmentNumber)}";
}