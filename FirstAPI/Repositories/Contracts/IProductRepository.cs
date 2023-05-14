using FirstAPI.Models;
using FirstAPI.ViewModels;

namespace FirstAPI.Repositories.Contracts;

public interface IProductRepository
{
    Task<Product> AddProductAsync(ProductViewModel product, int id);

    Task<int> GetCountProductsByUserId(int id);

    Task<List<Product>> GetAllProductsByUserId(int skip, int take, int id);

    Task<Product> GetProductById(int id);

    Task<Product> UpdateProductByIdAsync(ProductViewModel product, int id);

    Task<Product> DeleteProductByIdAsync(int id);
}

