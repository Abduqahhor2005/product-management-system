using Microsoft.AspNetCore.Mvc;
using ProductManagementSystem.Entities;
using ProductManagementSystem.Services.Category;

namespace ProductManagementSystem.Controller;

[ApiController]
[Route("api/category")]
public class CategoryController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetCategories(ICategoryService categoryService)
    {
        return Results.Ok(await categoryService.GetAll());
    }
    [HttpGet("api/categories/withProductCount")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetCategoriesWithProductCount(ICategoryService categoryService)
    {
        return Results.Ok(await categoryService.GetCategoriesWithProductCount());
    }
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetCategoryById(ICategoryService categoryService,[FromRoute] int id)
    {
        Category? category = await categoryService.GetById(id);
        if(category==null) return Results.NotFound(new { message = "Category not found" });
        return Results.Ok(category);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> CreateCategory(ICategoryService categoryService, [FromBody] Category? category)
    {
        if(category==null) return Results.NotFound(new { message = "Category not created" });
        await categoryService.Create(category);
        return Results.Ok(new { message = "Category created" });
    }
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> UpdateCategory(ICategoryService categoryService, [FromBody] Category? category)
    {
        if(category==null) return Results.NotFound(new { message = "Category not updated" });
        await categoryService.Update(category);
        return Results.Ok(new { message = "Category updated" });
    }
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> DeleteCategory(ICategoryService categoryService,[FromRoute] int id)
    {
        if(id<0) return Results.NotFound(new { message = "Category not deleted" });
        await categoryService.Delete(id);
        return Results.Ok(new { message = "Category deleted" });
    }
}