using Core.CrossCuttingConcerns.Serilog.Logger;
using Core.DataAccess.Security.JWT;
using Core.Security.JWT;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Core.CrossCuttingConcerns.Serilog;


namespace Core;

public static class CoreServiceRegistration
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddMemoryCache();

        services.AddScoped<ITokenHelper, JwtHelper>();

        services.AddScoped<Stopwatch>();

        services.AddTransient<LoggerServiceBase, MsSqlLogger>();
        services.AddTransient<LoggerServiceBase, FileLogger>();
        return services;
    }
}

