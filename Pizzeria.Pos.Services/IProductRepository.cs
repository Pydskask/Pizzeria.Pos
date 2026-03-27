using Pizzeria.Pos.Core.Models;

public interface IProductRepository
{
    List<Product> GetByCategory(string category);
    List<Product> GetAll();
    void AddProduct(Product product);  // NOWE
    void UpdateProduct(Product product); // NOWE  
    void DeleteProduct(int productId); // NOWE
}