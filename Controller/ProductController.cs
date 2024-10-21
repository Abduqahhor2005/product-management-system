using Microsoft.AspNetCore.Mvc;
using ProductManagementSystem.Entities;
using ProductManagementSystem.Services.Product;

namespace ProductManagementSystem.Controller;

[ApiController]
[Route("api/product")]
public class ProductController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetProducts(IProductService productService)
    {
        return Results.Ok(await productService.GetAll());
    }
    [HttpGet("api/products/categoryId={categoryId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetProductsByCategory(IProductService productService, int categoryId)
    {
        return Results.Ok(await productService.GetProductsByCategory(categoryId));
    }
    [HttpGet("api/products/quantity={quantity}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetProductsByQuantity(IProductService productService, int quantity)
    {
        return Results.Ok(await productService.GetProductsByQuantity(quantity));
    }
    [HttpGet("api/products/{id}/details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetProductsWithCategoryAndSupplier(IProductService productService, int id)
    {
        return Results.Ok(await productService.GetProductsWithCategoryAndSupplier(id));
    }
    [HttpGet("api/api/products/mostOrdered/minOrders=5")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetProductsByOrderCount(IProductService productService)
    {
        return Results.Ok(await productService.GetProductsByOrderCount());
    }
    [HttpGet("api/products/pageNumber={pageNumber}&pageSize={pageSize}&includeDetails=true")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetProductsWithCategoryAndSupplierByPagination
        (IProductService productService, int pageNumber, int pageSize)
    {
        return Results.Ok(await productService.GetProductsWithCategoryAndSupplierByPagination
            (pageNumber, pageSize));
    }
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetProductById(IProductService productService,[FromRoute] int id)
    {
        Product? product = await productService.GetById(id);
        if(product==null) return Results.NotFound(new { message = "Product not found" });
        return Results.Ok(product);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> CreateProduct(IProductService productService, [FromBody] Product? product)
    {
        if(product==null) return Results.NotFound(new { message = "Product not created" });
        await productService.Create(product);
        return Results.Ok(new { message = "Product created" });
    }
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> UpdateProduct(IProductService productService, [FromBody] Product? product)
    {
        if(product==null) return Results.NotFound(new { message = "Product not updated" });
        await productService.Update(product);
        return Results.Ok(new { message = "Product updated" });
    }
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> DeleteProduct(IProductService productService,[FromRoute] int id)
    {
        if(id<0) return Results.NotFound(new { message = "Product not deleted" });
        await productService.Delete(id);
        return Results.Ok(new { message = "Product deleted" });
    }
}