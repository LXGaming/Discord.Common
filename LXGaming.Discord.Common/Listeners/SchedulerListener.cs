using Discord;
using Discord.WebSocket;
using LXGaming.Discord.Common.Scheduler;
using LXGaming.Discord.Common.Utilities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LXGaming.Discord.Common.Listeners;

public class SchedulerListener(
    BaseSocketClient discordClient,
    ILogger<SchedulerListener> logger,
    ISchedulerService schedulerService) : IHostedService {

    /// <inheritdoc />
    public virtual Task StartAsync(CancellationToken cancellationToken) {
        discordClient.ChannelDestroyed += OnChannelDestroyedAsync;
        discordClient.LeftGuild += OnLeftGuildAsync;
        discordClient.MessageDeleted += OnMessageDeletedAsync;
        discordClient.MessagesBulkDeleted += OnMessagesBulkDeletedAsync;
        discordClient.UserLeft += OnUserLeftAsync;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task StopAsync(CancellationToken cancellationToken) {
        discordClient.UserLeft -= OnUserLeftAsync;
        discordClient.MessagesBulkDeleted -= OnMessagesBulkDeletedAsync;
        discordClient.MessageDeleted -= OnMessageDeletedAsync;
        discordClient.LeftGuild -= OnLeftGuildAsync;
        discordClient.ChannelDestroyed -= OnChannelDestroyedAsync;
        return Task.CompletedTask;
    }

    protected virtual Task OnChannelDestroyedAsync(SocketChannel channel) {
        return schedulerService.ScheduleEventAsync(_ => {
            if (logger.IsEnabled(LogLevel.Trace)) {
                logger.LogTrace("Channel {Channel} Destroyed", EntityUtils.ToString(channel));
            }

            return schedulerService.UnscheduleMessageDeletionsAsync(key => key.ChannelId == channel.Id, false);
        });
    }

    protected virtual Task OnLeftGuildAsync(SocketGuild guild) {
        return schedulerService.ScheduleEventAsync(_ => {
            if (logger.IsEnabled(LogLevel.Trace)) {
                logger.LogTrace("Left Guild {Guild}", EntityUtils.ToString(guild));
            }

            return schedulerService.UnscheduleMessageDeletionsAsync(key => key.GuildId == guild.Id, false);
        });
    }

    protected virtual Task OnMessageDeletedAsync(Cacheable<IMessage, ulong> message,
        Cacheable<IMessageChannel, ulong> channel) {
        return schedulerService.ScheduleEventAsync(_ => {
            if (logger.IsEnabled(LogLevel.Trace)) {
                logger.LogTrace("Message {Message} Deleted", EntityUtils.ToString(message));
            }

            return schedulerService.UnscheduleMessageDeletionsAsync(key => key.MessageId == message.Id, false);
        });
    }

    protected virtual Task OnMessagesBulkDeletedAsync(IReadOnlyCollection<Cacheable<IMessage, ulong>> messages,
        Cacheable<IMessageChannel, ulong> channel) {
        return schedulerService.ScheduleEventAsync(async _ => {
            foreach (var message in messages) {
                if (logger.IsEnabled(LogLevel.Trace)) {
                    logger.LogTrace("Message {Message} Bulk Deleted", EntityUtils.ToString(message));
                }

                await schedulerService.UnscheduleMessageDeletionsAsync(key => key.MessageId == message.Id, false)
                    .ConfigureAwait(false);
            }
        });
    }

    protected virtual Task OnUserLeftAsync(SocketGuild guild, SocketUser user) {
        return schedulerService.ScheduleEventAsync(_ => {
            if (logger.IsEnabled(LogLevel.Trace)) {
                logger.LogTrace("User {User} Left Guild {Guild}", EntityUtils.ToString(user),
                    EntityUtils.ToString(guild));
            }

            return schedulerService.UnscheduleMessageDeletionsAsync(
                key => key.GuildId == guild.Id && key.UserId == user.Id, discordClient.CurrentUser.Id != user.Id);
        });
    }
}