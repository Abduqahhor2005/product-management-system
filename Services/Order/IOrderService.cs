namespace ProductManagementSystem.Services.Order;

public interface IOrderService
{
    Task<IEnumerable<Entities.Order>> GetAll();
    Task<IEnumerable<Entities.Order>> GetOrdersBySupplier(int supplierId, string status);
    Task<IEnumerable<Entities.Order>> GetOrdersByDate(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Entities.Order>> GetAllByPagination(int pageNumber, int pageSize);
    Task<Entities.Order> GetById(int id);
    Task<bool> Create(Entities.Order order);
    Task<bool> Update(Entities.Order order);
    Task<bool> Delete(int id);
}