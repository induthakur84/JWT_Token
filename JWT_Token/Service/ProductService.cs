using JWT_Token.Model;
using JWT_Token.Service.IService;
using Microsoft.EntityFrameworkCore;

namespace JWT_Token.Service
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Product> CreateProduct(Product product)
        {
          _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;

        }

        public async Task<bool> DeleteProduct(int id)
        {
           var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public  async Task<IEnumerable<Product>> GetAllProducts()
        {
            var products = await _context.Products.ToListAsync();
            return products;        }

        public async Task<Product> GetProductById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return product;
        }

        public async Task<Product> UpdateProduct(Product product)
        {
            var existingProduct = await _context.Products.FindAsync(product.Id);
            if (existingProduct == null)
                return null;
            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
                
            await _context.SaveChangesAsync();
            return product;
        }
    }
}
