using Discord;
using LXGaming.Discord.Prompts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LXGaming.Discord.Common.Prompts;

public static class PromptExtensions {

    public static IServiceCollection AddPromptService(this IServiceCollection services, PromptServiceOptions options) {
        return services
            .AddSingleton(provider => {
                var client = provider.GetRequiredService<IDiscordClient>();
                var logger = provider.GetRequiredService<ILogger<PromptService>>();
                return new PromptService(client, logger, options);
            })
            .AddSingleton<PromptListener>()
            .AddSingleton<IHostedService>(provider => provider.GetRequiredService<PromptListener>());
    }
}