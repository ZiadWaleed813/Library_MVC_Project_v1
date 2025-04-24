public class UserLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public UserLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userInfo = $"Timestamp: {DateTime.Now}\n" +
                           $"User: {context.User.Identity.Name}\n" +
                           $"Request Path: {context.Request.Path}\n" +
                           $"------------------------------------\n";

            // Write the user info to a .txt file
            var logFilePath = "Logs/UserInfoLog.txt";
            Directory.CreateDirectory("Logs"); // Ensure directory exists
            await File.AppendAllTextAsync(logFilePath, userInfo);
        }

        await _next(context); // Pass control to the next middleware
    }
}