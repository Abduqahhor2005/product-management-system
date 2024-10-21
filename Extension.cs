using ProductManagementSystem.Services.Category;
using ProductManagementSystem.Services.Order;
using ProductManagementSystem.Services.Product;
using ProductManagementSystem.Services.Supplier;

namespace ProductManagementSystem;

public static class Extension
{
    public static void AddService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ICategoryService,CategoryService>();
        serviceCollection.AddScoped<IOrderService,OrderService>();
        serviceCollection.AddScoped<IProductService,ProductService>();
        serviceCollection.AddScoped<ISupplierService,SupplierService>();
    }
}