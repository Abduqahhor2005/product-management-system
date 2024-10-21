using Microsoft.AspNetCore.Mvc;
using ProductManagementSystem.Entities;
using ProductManagementSystem.Services.Supplier;

namespace ProductManagementSystem.Controller;

[ApiController]
[Route("api/supplier")]
public class SupplierController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetSuppliers(ISupplierService supplierService)
    {
        return Results.Ok(await supplierService.GetAll());
    }
    [HttpGet("api/suppliers/productQuantity={productQuantity}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetSuppliersByProductQuantity(ISupplierService supplierService,int productQuantity)
    {
        return Results.Ok(await supplierService.GetSuppliersByProductQuantity(productQuantity));
    }
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetSupplierById(ISupplierService supplierService,[FromRoute] int id)
    {
        Supplier? supplier = await supplierService.GetById(id);
        if(supplier==null) return Results.NotFound(new { message = "Supplier not found" });
        return Results.Ok(supplier);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> CreateSupplier(ISupplierService supplierService, [FromBody] Supplier? supplier)
    {
        if(supplier==null) return Results.NotFound(new { message = "Supplier not created" });
        await supplierService.Create(supplier);
        return Results.Ok(new { message = "Supplier created" });
    }
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> UpdateSupplier(ISupplierService supplierService, [FromBody] Supplier? supplier)
    {
        if(supplier==null) return Results.NotFound(new { message = "Supplier not updated" });
        await supplierService.Update(supplier);
        return Results.Ok(new { message = "Supplier updated" });
    }
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> DeleteSupplier(ISupplierService supplierService,[FromRoute] int id)
    {
        if(id<0) return Results.NotFound(new { message = "Supplier not deleted" });
        await supplierService.Delete(id);
        return Results.Ok(new { message = "Supplier deleted" });
    }
}