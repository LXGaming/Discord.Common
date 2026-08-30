using System.Collections.Immutable;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LXGaming.Discord.Common.Scheduler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LXGaming.Discord.Common.Example.Services.Discord;

public class DiscordService(
    IConfiguration configuration,
    DiscordSocketClient discordClient,
    ILogger<DiscordService> logger,
    ISchedulerService schedulerService,
    IServiceScopeFactory serviceScopeFactory,
    InteractionService? interactionService = null) : IDiscordService, IHostedService {

    private readonly DiscordOptions _options = configuration.GetSection(DiscordOptions.Key).Get<DiscordOptions>()
                                               ?? new DiscordOptions();

    private readonly Lazy<Task<IApplication>> _lazyApplication = new(async () => {
        // ReSharper disable once ConvertToLambdaExpression
        return await discordClient.Rest.GetApplicationInfoAsync().ConfigureAwait(false);
    });

    public async Task StartAsync(CancellationToken cancellationToken) {
        if (interactionService != null) {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var services = scope.ServiceProvider;

            var assembly = typeof(DiscordService).Assembly;
            var modules = (await interactionService.AddModulesAsync(assembly, services)).ToImmutableArray();
            logger.LogInformation("Discovered {Count} module(s)", modules.Length);
        }

        discordClient.Ready += OnReadyAsync;

        await discordClient.LoginAsync(TokenType.Bot, _options.Token);
        await discordClient.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken) {
        discordClient.Ready -= OnReadyAsync;

        await discordClient.StopAsync();
        await discordClient.LogoutAsync();
    }

    public Task<IApplication> GetApplicationAsync(CancellationToken cancellationToken = default) {
        return _lazyApplication.Value;
    }

    private Task OnReadyAsync() {
        return schedulerService.ScheduleEventAsync(async _ => {
            logger.LogTrace("Ready");
            if (interactionService != null) {
                await interactionService.RegisterCommandsGloballyAsync();
            }
        });
    }
}