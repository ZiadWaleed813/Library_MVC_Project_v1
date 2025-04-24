public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var logDetails = $"{DateTime.Now}: {context.Request.Method} {context.Request.Path}";
        Console.WriteLine(logDetails);

        await _next(context); // Call the next middleware in the pipeline
    }
}