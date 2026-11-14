using System.Text;
using FullStack.Application.Common.Interfaces;
using FullStack.Application.Interfaces;
using FullStack.Domain.Entities;
using FullStack.Infrastructure.Auth;
using FullStack.Infrastructure.Data;
using FullStack.Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace FullStack.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.RegisterDb(config);
        services.RegisterIdentity();
        services.RegisterJwtAuthentication(config);
        services.RegisterInfrastructureServices();
        return services;
    }

    private static void RegisterDb(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("DefaultConnection")
                 ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is missing.");

        services.AddDbContext<AppDbContext>(o => o.UseNpgsql(cs));
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
    }

    private static void RegisterIdentity(this IServiceCollection services)
    {
        services.AddIdentityCore<ApplicationUser>(o =>
            {
                o.Password.RequiredLength = 8;
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = true;
                o.Password.RequireUppercase = true;
                o.Password.RequireNonAlphanumeric = true;
                o.User.RequireUniqueEmail = false;
                o.Lockout.MaxFailedAccessAttempts = 5;
                o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();
    }

    private static void RegisterJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<JwtOptions>()
            .Bind(config.GetSection(JwtOptions.SectionName))
            .Validate(o => o.AccessSecret.Length >= 32 && o.RefreshSecret.Length >= 32,
                "Jwt:AccessSecret and Jwt:RefreshSecret must be at least 32 characters.")
            .ValidateOnStart();

        var jwt = config.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.MapInboundClaims = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.AccessSecret)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };

                o.Events = new JwtBearerEvents
                {
                    // Cookie first; if absent, Token stays null and the default Authorization header handling applies.
                    OnMessageReceived = ctx =>
                    {
                        if (ctx.Request.Cookies.TryGetValue(jwt.AccessCookieName, out var token)
                            && !string.IsNullOrWhiteSpace(token))
                        {
                            ctx.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();
        services.AddHttpContextAccessor();
    }

    private static void RegisterInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IJwtCookieService, JwtCookieService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IFileStorageService, FileStorageService>();
    }
}
