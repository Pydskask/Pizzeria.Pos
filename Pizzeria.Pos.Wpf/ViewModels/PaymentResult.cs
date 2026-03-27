using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzeria.Pos.Wpf.ViewModels;

public class PaymentResult
{
    public bool IsPaid { get; set; }
    public string PaymentMethod { get; set; } = "";
    public decimal FinalTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal AmountPaid { get; set; }
}
