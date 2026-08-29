using Discord;
using Discord.WebSocket;
using LXGaming.Common.Event;
using LXGaming.Discord.Common.Scheduler;
using LXGaming.Discord.Prompts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LXGaming.Discord.Common.Prompts;

public class PromptListener(
    BaseSocketClient discordClient,
    ILogger<PromptListener> logger,
    PromptService promptService,
    ISchedulerService schedulerService) : IHostedService {

    public event AsyncEventHandler<PromptResult>? PromptExecuted;

    /// <inheritdoc />
    public virtual Task StartAsync(CancellationToken cancellationToken) {
        discordClient.ChannelDestroyed += OnChannelDestroyedAsync;
        discordClient.InteractionCreated += OnInteractionCreatedAsync;
        discordClient.LeftGuild += OnLeftGuildAsync;
        discordClient.MessageDeleted += OnMessageDeletedAsync;
        discordClient.MessagesBulkDeleted += OnMessagesBulkDeletedAsync;
        discordClient.UserLeft += OnUserLeftAsync;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual async Task StopAsync(CancellationToken cancellationToken) {
        discordClient.UserLeft -= OnUserLeftAsync;
        discordClient.MessagesBulkDeleted -= OnMessagesBulkDeletedAsync;
        discordClient.MessageDeleted -= OnMessageDeletedAsync;
        discordClient.LeftGuild -= OnLeftGuildAsync;
        discordClient.InteractionCreated -= OnInteractionCreatedAsync;
        discordClient.ChannelDestroyed -= OnChannelDestroyedAsync;

        try {
            await promptService.UnregisterAllAsync().ConfigureAwait(false);
        } catch (Exception ex) {
            logger.LogError(ex, "Encountered an error while unregistering all prompts");
        }
    }

    protected virtual Task OnChannelDestroyedAsync(SocketChannel channel) {
        return schedulerService.ScheduleEventAsync(_ => {
            return promptService.UnregisterAllAsync(key => key.ChannelId == channel.Id, false);
        });
    }

    protected virtual Task OnInteractionCreatedAsync(SocketInteraction interaction) {
        return schedulerService.ScheduleEventAsync(async _ => {
            if (interaction is IComponentInteraction componentInteraction) {
                var promptResult = await promptService.ExecuteAsync(componentInteraction).ConfigureAwait(false);
                await PromptExecuted.InvokeAsync(this, promptResult).ConfigureAwait(false);
            }
        });
    }

    protected virtual Task OnLeftGuildAsync(SocketGuild guild) {
        return schedulerService.ScheduleEventAsync(_ => {
            return promptService.UnregisterAllAsync(key => key.GuildId == guild.Id, false);
        });
    }

    protected virtual Task OnMessageDeletedAsync(Cacheable<IMessage, ulong> message,
        Cacheable<IMessageChannel, ulong> channel) {
        return schedulerService.ScheduleEventAsync(_ => {
            return promptService.UnregisterAllAsync(key => key.MessageId == message.Id, false);
        });
    }

    protected virtual Task OnMessagesBulkDeletedAsync(IReadOnlyCollection<Cacheable<IMessage, ulong>> messages,
        Cacheable<IMessageChannel, ulong> channel) {
        return schedulerService.ScheduleEventAsync(async _ => {
            foreach (var message in messages) {
                await promptService.UnregisterAllAsync(key => key.MessageId == message.Id, false).ConfigureAwait(false);
            }
        });
    }

    protected virtual Task OnUserLeftAsync(SocketGuild guild, SocketUser user) {
        return schedulerService.ScheduleEventAsync(_ => {
            return promptService.UnregisterAllAsync(key => key.GuildId == guild.Id && key.UserId == user.Id, false);
        });
    }
}