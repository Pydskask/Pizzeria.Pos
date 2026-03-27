namespace Pizzeria.Pos.Core.Models;

public class OrderItem
{
    public int Id { get; set; }

    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public int ProductId { get; set; }
    public string BaseProductName { get; set; } = "";
    public decimal BaseProductPrice { get; set; }
    public string? ConfigurationJson { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
}