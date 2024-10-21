using Microsoft.AspNetCore.Mvc;
using ProductManagementSystem.Entities;
using ProductManagementSystem.Services.Order;

namespace ProductManagementSystem.Controller;

[ApiController]
[Route("api/order")]
public class OrderController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetOrders(IOrderService orderService)
    {
        return Results.Ok(await orderService.GetAll());
    }
    [HttpGet("api/orders/supplierId={supplierId}&status={status}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetOrdersBySupplier(IOrderService orderService,int supplierId, string status)
    {
        return Results.Ok(await orderService.GetOrdersBySupplier(supplierId, status));
    }
    [HttpGet("api/orders/startDate={startDate}&endDate={endDate}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetOrdersByDate(IOrderService orderService,DateTime startDate,DateTime endDate)
    {
        return Results.Ok(await orderService.GetOrdersByDate(startDate, endDate));
    }
    [HttpGet("api/orders/pageNumber={pageNumber}&pageSize={pageSize}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetAllByPagination(IOrderService orderService,int pageNumber, int pageSize)
    {
        return Results.Ok(await orderService.GetAllByPagination(pageNumber, pageSize));
    }
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetOrderById(IOrderService orderService,[FromRoute] int id)
    {
        Order? order = await orderService.GetById(id);
        if(order==null) return Results.NotFound(new { message = "Order not found" });
        return Results.Ok(order);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> CreateOrder(IOrderService orderService, [FromBody] Order? order)
    {
        if(order==null) return Results.NotFound(new { message = "Order not created" });
        await orderService.Create(order);
        return Results.Ok(new { message = "Order created" });
    }
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> UpdateOrder(IOrderService orderService, [FromBody] Order? order)
    {
        if(order==null) return Results.NotFound(new { message = "Order not updated" });
        await orderService.Update(order);
        return Results.Ok(new { message = "Order updated" });
    }
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> DeleteOrder(IOrderService orderService,[FromRoute] int id)
    {
        if(id<0) return Results.NotFound(new { message = "Order not deleted" });
        await orderService.Delete(id);
        return Results.Ok(new { message = "Order deleted" });
    }
}