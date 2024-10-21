namespace ProductManagementSystem.Services.Product;

public interface IProductService
{
    Task<IEnumerable<Entities.Product>> GetAll();
    Task<IEnumerable<Entities.Product>> GetProductsByCategory(int categoryId);
    Task<IEnumerable<Entities.Product>> GetProductsByQuantity(int quantity);
    Task<Entities.ProductsWithCategoryAndSupplier> GetProductsWithCategoryAndSupplier(int id);
    Task<IEnumerable<Entities.Product>> GetProductsByOrderCount();

    Task<IEnumerable<Entities.ProductsWithCategoryAndSupplier>>
        GetProductsWithCategoryAndSupplierByPagination(int pageNumber, int pageSize);
    Task<Entities.Product> GetById(int id);
    Task<bool> Create(Entities.Product product);
    Task<bool> Update(Entities.Product product);
    Task<bool> Delete(int id);
}