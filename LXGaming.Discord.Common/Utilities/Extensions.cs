using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using LXGaming.Discord.Common.Access;
using LXGaming.Discord.Common.Listeners;
using LXGaming.Discord.Common.Scheduler;
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

    public static IServiceCollection AddDiscordClient<TService>(this IServiceCollection services, TService client)
        where TService : BaseSocketClient {
        return services
            .AddSingleton(client)
            .AddSingleton<BaseSocketClient>(provider => provider.GetRequiredService<TService>())
            .AddSingleton<BaseDiscordClient>(provider => provider.GetRequiredService<TService>())
            .AddSingleton<IDiscordClient>(provider => provider.GetRequiredService<TService>())
            .AddSingleton<IHostedService, LogListener>();
    }

    public static IServiceCollection AddDiscordService<TService>(this IServiceCollection services)
        where TService : class, IDiscordService, IHostedService {
        return services
            .AddSingleton<TService>()
            .AddSingleton<IDiscordService>(provider => provider.GetRequiredService<TService>())
            .AddSingleton<IHostedService>(provider => provider.GetRequiredService<TService>());
    }

    public static IServiceCollection AddInteractionService(this IServiceCollection services) {
        return services.AddInteractionService(new InteractionServiceConfig());
    }

    public static IServiceCollection AddInteractionService(this IServiceCollection services,
        InteractionServiceConfig config) {
        return services
            .AddSingleton(provider => {
                // Scope creation is handled by this library.
                config.AutoServiceScopes = false;
                // Async execution is handled by this library.
                config.DefaultRunMode = RunMode.Sync;
                // Exceptions are returned by the ExecuteResult and subsequently handled by this library.
                config.ThrowOnError = false;

                var client = provider.GetRequiredService<BaseSocketClient>();
                return new InteractionService(client, config);
            })
            .AddSingleton<InteractionListener>()
            .AddSingleton<IHostedService>(provider => provider.GetRequiredService<InteractionListener>());
    }

    public static IServiceCollection AddSchedulerService(this IServiceCollection services) {
        return services.AddSchedulerService<DefaultSchedulerService>();
    }

    public static IServiceCollection AddSchedulerService<TService>(this IServiceCollection services)
        where TService : class, ISchedulerService, IHostedService {
        return services
            .AddSingleton<TService>()
            .AddSingleton<ISchedulerService>(provider => provider.GetRequiredService<TService>())
            .AddSingleton<IHostedService>(provider => provider.GetRequiredService<TService>())
            .AddSingleton<SchedulerListener>()
            .AddSingleton<IHostedService>(provider => provider.GetRequiredService<SchedulerListener>());
    }
}