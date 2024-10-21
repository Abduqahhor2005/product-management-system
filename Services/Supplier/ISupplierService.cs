namespace ProductManagementSystem.Services.Supplier;

public interface ISupplierService
{
    Task<IEnumerable<Entities.Supplier>> GetAll();
    Task<IEnumerable<Entities.Supplier>> GetSuppliersByProductQuantity(int productQuantity);
    Task<Entities.Supplier> GetById(int id);
    Task<bool> Create(Entities.Supplier supplier);
    Task<bool> Update(Entities.Supplier supplier);
    Task<bool> Delete(int id);
}