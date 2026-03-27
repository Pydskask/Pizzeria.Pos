using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pizzeria.Pos.Core.Models;
using Pizzeria.Pos.Data;


namespace Pizzeria.Pos.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly PosDataContext context;

        public ProductRepository(PosDataContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<Product> GetAll()
        {
            return context.Products.ToList();
        }

        public List<Product> GetByCategory(string category)
        {
            return context.Products
                .Where(p => p.Category == category)
                .OrderBy(p => p.Name)
                .ToList();
        }

        public void AddProduct(Product product)
        {
            context.Products.Add(product);
            context.SaveChanges();
        }

        public void UpdateProduct(Product product)
        {
            var existingProduct = context.Products.FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct == null)
                return;

            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Category = product.Category;

            context.SaveChanges();
        }

        public void DeleteProduct(int productId)
        {
            var product = context.Products.Find(productId);
            if (product == null)
                return;

            var isUsedInOrders = context.OrderItems.Any(x => x.ProductId == productId);
            if (isUsedInOrders)
                throw new InvalidOperationException("Nie można usunąć produktu, który został już użyty w zamówieniach.");

            context.Products.Remove(product);
            context.SaveChanges();
        }


    }
}
