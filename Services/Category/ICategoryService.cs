namespace ProductManagementSystem.Services.Category;

public interface ICategoryService
{
    Task<IEnumerable<Entities.Category>> GetAll();
    Task<IEnumerable<Entities.CategoryWithProductCount>> GetCategoriesWithProductCount();
    Task<Entities.Category> GetById(int id);
    Task<bool> Create(Entities.Category category);
    Task<bool> Update(Entities.Category category);
    Task<bool> Delete(int id);
}