using System.Diagnostics;
using Discord;
using Discord.Interactions;
using Discord.Rest;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LXGaming.Discord.Common.Listeners;

public class LogListener(
    BaseDiscordClient discordClient,
    ILogger<LogListener> logger,
    InteractionService? interactionService = null) : IHostedService {

    /// <inheritdoc />
    public virtual Task StartAsync(CancellationToken cancellationToken) {
        discordClient.Log += OnLogAsync;
        if (interactionService != null) {
            interactionService.Log += OnLogAsync;
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task StopAsync(CancellationToken cancellationToken) {
        if (interactionService != null) {
            interactionService.Log -= OnLogAsync;
        }

        discordClient.Log -= OnLogAsync;
        return Task.CompletedTask;
    }

    protected virtual Task OnLogAsync(LogMessage message) {
        var level = message.Severity switch {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Debug,
            LogSeverity.Debug => LogLevel.Trace,
            _ => LogLevel.None
        };

        Debug.Assert(level != LogLevel.None, $"{message.Severity} is not supported.");
        if (logger.IsEnabled(level)) {
            logger.Log(level, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
        }

        return Task.CompletedTask;
    }
}