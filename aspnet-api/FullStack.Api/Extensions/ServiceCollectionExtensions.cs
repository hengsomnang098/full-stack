using FullStack.Application;
using FullStack.Infrastructure;
using Microsoft.OpenApi;
using Serilog;

namespace FullStack.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((ctx, services, cfg) => cfg
            .ReadFrom.Configuration(ctx.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());
        return builder;
    }

    public static IServiceCollection RegisterCors(this IServiceCollection services, IConfiguration config)
    {
        var origins = config.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

        services.AddCors(o => o.AddPolicy("Default", p =>
        {
            if (origins.Length > 0)
                p.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
        }));
        return services;
    }

    public static IServiceCollection RegisterLayers(this IServiceCollection services, IConfiguration config)
    {
        services.AddApplication();
        services.AddInfrastructure(config);
        return services;
    }

    public static IServiceCollection RegisterSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "FullStack API",
                Version = "v1",
                Description = "Authentication uses HttpOnly cookies (access_token / refresh_token) set by " +
                              "POST /api/employee/login. Browsers send them automatically; in Swagger UI call the " +
                              "login endpoint first and the cookies will be reused for subsequent requests. " +
                              "An Authorization: Bearer header is also accepted as a fallback."
            });
        });
        return services;
    }
}
