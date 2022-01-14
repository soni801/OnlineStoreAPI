using OnlineStoreAPI.Models;

namespace OnlineStoreAPI.Interfaces;

public interface IProductService
{
    public Product GetProduct(int id);
    public IEnumerable<Product> GetAllProducts();
}