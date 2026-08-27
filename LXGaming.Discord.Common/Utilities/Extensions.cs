using LXGaming.Discord.Common.Access;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LXGaming.Discord.Common.Utilities;

public static class Extensions {

    public static IServiceCollection AddAccessService<TService>(this IServiceCollection services)
        where TService : class, IAccessService {
        return services
            .AddScoped<TService>()
            .AddScoped<IAccessService>(provider => provider.GetRequiredService<TService>());
    }

    public static IServiceCollection AddDiscordService<TService>(this IServiceCollection services)
        where TService : class, IDiscordService, IHostedService {
        return services
            .AddSingleton<TService>()
            .AddSingleton<IDiscordService>(provider => provider.GetRequiredService<TService>())
            .AddSingleton<IHostedService>(provider => provider.GetRequiredService<TService>());
    }
}