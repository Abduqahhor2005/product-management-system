namespace ProductManagementSystem;

public class ErrorHandlingMiddleware(RequestDelegate requestDelegate)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await requestDelegate(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        Console.WriteLine($"Ошибка: {exception.Message}");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("Произошла ошибка. Пожалуйста, повторите попытку позже.");
    }
}
