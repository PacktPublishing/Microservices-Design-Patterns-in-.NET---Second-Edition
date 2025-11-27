using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://auth.healthmanagement.com";
        options.Audience = "api-gateway";
        options.RequireHttpsMetadata = true;
    });
builder.Services.AddAuthorization();

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
var app = builder.Build();

// Custom middleware to validate API key
app.Use(async (context, next) =>
{
    const string apiKeyHeader = "X-Api-Key";

    var requestPath = context.Request.Path.Value ?? string.Empty;

    // Define a mapping section like: ApiKeys:appointments, ApiKeys:patients, etc.
    var routePrefix = requestPath.Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

    if (string.IsNullOrWhiteSpace(routePrefix))
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync("Bad request: missing route prefix.");
        return;
    }

    var configKey = $"ApiKeys:{routePrefix}";
    var expectedApiKey = builder.Configuration[configKey];

    if (string.IsNullOrWhiteSpace(expectedApiKey))
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsync($"Forbidden: No API key configured for route '{routePrefix}'.");
        return;
    }

    if (!context.Request.Headers.TryGetValue(apiKeyHeader, out var providedKey) ||
        providedKey != expectedApiKey)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Unauthorized: API key is missing or invalid.");
        return;
    }

    await next();
});
app.UseIpRateLimiting();
app.MapReverseProxy();
app.Run();
