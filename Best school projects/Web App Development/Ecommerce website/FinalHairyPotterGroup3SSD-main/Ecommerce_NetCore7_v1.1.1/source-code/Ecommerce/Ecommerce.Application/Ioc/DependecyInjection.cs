using Ecommerce.Application.Identity;
using Ecommerce.Application.Interfaces;
using Ecommerce.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Ecommerce.Application.Ioc;
public static class DependecyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });


        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IRoleService, RoleService>();
        services.AddTransient<IProfileService, ProfileService>();
        services.AddTransient<IEmailService, EmailService>();

        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddTransient<IMemoryCacheManager, MemoryCacheManager>();
        services.AddTransient<ICookieService, CookieManager>();
        services.AddTransient<IKeyAccessor, KeyAccessor>();
        services.AddTransient<IMediaService, MediaService>();

        return services;
    }
}