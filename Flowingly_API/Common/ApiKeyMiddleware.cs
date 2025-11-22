namespace Flowingly.API.Common
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;

    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private const string ApiKeyHeaderName = "ApplicationKey"; // Or your desired header name

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("API Key not provided.");
                return;
            }

            var configuredApiKey = _configuration.GetValue<string>("ApiKey"); // Get from appsettings.json

            if (string.IsNullOrEmpty(configuredApiKey) || extractedApiKey != configuredApiKey)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid API Key.");
                return;
            }

            await _next(context);
        }
    }
}
