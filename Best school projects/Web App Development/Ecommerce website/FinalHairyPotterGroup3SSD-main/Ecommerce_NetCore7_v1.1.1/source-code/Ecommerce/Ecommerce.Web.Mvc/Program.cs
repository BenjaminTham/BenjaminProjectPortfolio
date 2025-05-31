using AspNetCoreRateLimit;

using Ecommerce.Application.Identity;
using Ecommerce.Application.Interfaces;
using Ecommerce.Application.Ioc;

using Ecommerce.Infrastructure.Sql.Ioc;
using Ecommerce.Infrastructure.Sql.Repositories;
using Ecommerce.Web.Mvc.Extension;
using Ecommerce.Application.Services;
using Microsoft.AspNetCore.HttpOverrides;

using System.Globalization;
using System.Text.Json.Serialization;
using Stripe;
using System.Security.Claims;
using Ecommerce.Domain.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ecommerce.Infrastructure.Sql.DataAccess;

var builder = WebApplication.CreateBuilder(args);
// Add logging configurations
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information); // Ensure it's set to Information or lower


// Add logging configurations
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddFile("Logs/audit.log");
builder.Logging.SetMinimumLevel(LogLevel.Information);


// Load connection string from environment variable
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__AppConnection")
                       ?? builder.Configuration.GetConnectionString("AppConnection");

// Load secret key for tokens
var tokenSecretKey = Environment.GetEnvironmentVariable("TokenSettings__SecretKey")
                     ?? builder.Configuration["TokenSettings:SecretKey"];

// Load Stripe configuration
var stripePublishKey = Environment.GetEnvironmentVariable("Stripe__PublishKey")
                       ?? builder.Configuration["Stripe:PublishKey"];
var stripeSecretKey = Environment.GetEnvironmentVariable("Stripe__SecretKey")
                      ?? builder.Configuration["Stripe:SecretKey"];
var stripeCurrency = Environment.GetEnvironmentVariable("Stripe__Currency")
                     ?? builder.Configuration["Stripe:Currency"];

// Load PayPal configuration
var paypalClientId = Environment.GetEnvironmentVariable("Paypal__ClientId")
                     ?? builder.Configuration["Paypal:ClientId"];
var paypalCurrency = Environment.GetEnvironmentVariable("Paypal__Currency")
                     ?? builder.Configuration["Paypal:Currency"];

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddMemoryCache();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureSqlServices(builder.Configuration);
builder.Services.AddScoped<IAccountService, Ecommerce.Application.Services.AccountService>();

// Load the secret key from the configuration
var secretKey = builder.Configuration.GetSection("TokenSettings:SecretKey").Value;

// Register the TokenService1 with the secret key
builder.Services.AddSingleton(new TokenService1(secretKey));


builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Paths for login, logout, and access denied actions
    options.LoginPath = "/My/Login";
    options.LogoutPath = "/Logout";
    options.AccessDeniedPath = "/AccessDenied";

    // Cookie settings
    options.Cookie.Name = "db7RzeAKUBHT7oCltNkFZLLbp51Fg9EfB7hdlhKioZ8"; // Unique name for the cookie
    options.Cookie.HttpOnly = true; // Prevents JavaScript access to the cookie
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensures the cookie is only sent over HTTPS
    options.Cookie.SameSite = SameSiteMode.Strict; // Prevents CSRF attacks
    options.ExpireTimeSpan = TimeSpan.FromMinutes(15); // Cookie expiration time
    options.SlidingExpiration = true; // Extends the session on activity
});



// Rate Limiting Configuration
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();


//builder.Services.AddControllersWithViews();

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options => { options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });

builder.Services.AddMvc();
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = false;
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});





var app = builder.Build();
app.MigrateAndSeed();

// Use custom audit logging middleware
app.UseMiddleware<AuditLoggingMiddleware>();

//DependencyInjection.ExecuteMigrate(app.Services);


//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}


app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = context =>
    {
        IHeaderDictionary headers = context.Context.Response.Headers;
        string contentType = headers["Content-Type"];
        if (contentType == "application/x-gzip")
        {
            if (context.File.Name.EndsWith("js.gz"))
            {
                contentType = "application/javascript";
            }
            else if (context.File.Name.EndsWith("css.gz"))
            {
                contentType = "text/css";
            }
            headers.Add("Content-Encoding", "gzip");
            headers["Content-Type"] = contentType;
        };

        // Cache static files for 30 days
        if (!app.Environment.IsDevelopment())
        {
            context.Context.Response.Headers.Append("Cache-Control", "public,max-age=2592000");
            context.Context.Response.Headers.Append("Expires", DateTime.UtcNow.AddDays(30).ToString("R", CultureInfo.InvariantCulture));
        }
    }
});



//app.UseWebMarkupMin();
app.UseIpRateLimiting(); // Add IP rate limiting middleware 
app.UseSession(); // Ensure this is called before UseRouting and custom middleware
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Add the custom token validation middleware
//app.UseMiddleware<TokenValidationMiddleware>();
// Use custom session timeout middleware
app.UseMiddleware<SessionTimeoutMiddleware>();



app.MapRazorPages();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.UseForwardedHeaders();


app.Run();

/*
public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next; // Delegate representing the next middleware in the pipeline
    private readonly TokenService1 _tokenService; // Service to handle token-related operations

    // Constructor to initialize the middleware with the next delegate and the token service
    public TokenValidationMiddleware(RequestDelegate next, TokenService1 tokenService)
    {
        _next = next;
        _tokenService = tokenService;
    }

    // Method to handle the incoming HTTP request
    public async Task InvokeAsync(HttpContext context)
    {
        // Check if the request contains an authentication cookie named "db7RzeAKUBHT7oCltNkFZLLbp51Fg9EfB7hdlhKioZ8"
        if (context.Request.Cookies.TryGetValue("db7RzeAKUBHT7oCltNkFZLLbp51Fg9EfB7hdlhKioZ8", out var token))
        {
                // Extract parts from the token
                var tokenParts = ExtractPartsFromToken(token);

                // Validate the token using the extracted email
                if (_tokenService.ValidateToken(token, tokenParts.email))
                {
                    // If the token is valid, create a list of claims for the user
                    var userClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, tokenParts.email), // Add the email as a claim
                        new Claim("Timestamp", tokenParts.timestamp), // Add the timestamp as a claim
                        new Claim("Nonce", tokenParts.nonce) // Add the nonce as a claim
                    };

                    // Create a claims identity with the custom scheme
                    var claimsIdentity = new ClaimsIdentity(userClaims, "CustomScheme");
                    // Create a claims principal with the claims identity
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    // Set the current user in the HTTP context to the claims principal
                    context.User = claimsPrincipal;
                }
                else
                {
                    // If the token is invalid, remove the cookie and redirect to the login page
                    context.Response.Cookies.Delete("db7RzeAKUBHT7oCltNkFZLLbp51Fg9EfB7hdlhKioZ8");
                    context.Response.Redirect("/Login");
                    return;
                }
            
        }

        // Call the next middleware in the pipeline
        await _next(context);
    }

    // Method to extract parts from the token
    private (string email, string timestamp, string nonce) ExtractPartsFromToken(string token)
    {
        // Decode the Base64 encoded token to get the encrypted data
        var encryptedData = Convert.FromBase64String(token);
        // Decrypt the encrypted data using the token service and the secret key
        var decryptedData = _tokenService.DecryptData(encryptedData, _tokenService.SecretKey);
        // Split the decrypted data to get the components (email, timestamp, nonce)
        var parts = decryptedData.Split(':');
        // Return the parts as a tuple
        return (parts[0], parts[1], parts[2]);
    }
}
*/

public class SessionTimeoutMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SessionTimeoutMiddleware> _logger;

    public SessionTimeoutMiddleware(RequestDelegate next, ILogger<SessionTimeoutMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var logger = context.RequestServices.GetService<ILogger<SessionTimeoutMiddleware>>();

        if (context.User.Identity.IsAuthenticated)
        {
            if (context.Session.TryGetValue("LastActivityTime", out byte[] value))
            {
                var lastActivity = new DateTime(BitConverter.ToInt64(value, 0));
                var timeElapsed = DateTime.UtcNow - lastActivity;
                logger?.LogInformation("Last activity was at {LastActivity}, {ElapsedSeconds} seconds ago", lastActivity, timeElapsed.TotalSeconds);

                if (timeElapsed.TotalMinutes > 15)
                {
                    logger?.LogInformation("Session expired. Redirecting to login page.");

                    if (!context.Response.HasStarted)
                    {
                        // Clear any existing session state 
                        context.Session.Clear();

                        // Redirect to the logout action in Accountcontroller
                        context.Response.Redirect("/logout"); 
                        return; // Make sure no further processing is done
                    }
                }
                else
                {
                    // Update the session time if the response has not started
                    context.Session.Set("LastActivityTime", BitConverter.GetBytes(DateTime.UtcNow.Ticks));
                }
            }
            else
            {
                logger?.LogInformation("Initializing last activity time.");
                context.Session.Set("LastActivityTime", BitConverter.GetBytes(DateTime.UtcNow.Ticks));
            }
        }

        await _next(context);
    }
}



public class TestStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<DataContext>(options =>
            options.UseInMemoryDatabase("InMemoryDb"));

        services.AddControllers();

        // Add rate limiting services
        services.AddMemoryCache();
        services.Configure<IpRateLimitOptions>(options =>
        {
            options.GeneralRules = new System.Collections.Generic.List<RateLimitRule>
            {
                // Specific rule for /checkout endpoint
                new RateLimitRule
                {
                    Endpoint = "POST:/checkout",
                    Period = "1h",
                    Limit = 50
                },
                // General rule for all endpoints
                new RateLimitRule
                {
                    Endpoint = "*",
                    Period = "1m",
                    Limit = 100
                }
            };
        });
        services.AddInMemoryRateLimiting();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseIpRateLimiting();

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}


public class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLoggingMiddleware> _logger;

    public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log the request details
        _logger.LogInformation("Handling request: {Method} {Url} {Timestamp}",
            context.Request.Method, context.Request.Path, DateTime.UtcNow);

        await _next(context);

        // Log the response details
        _logger.LogInformation("Handled request: {Method} {Url} {Timestamp} - Response: {StatusCode}",
            context.Request.Method, context.Request.Path, DateTime.UtcNow, context.Response.StatusCode);
    }
}
