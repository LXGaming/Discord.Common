using System.Runtime.CompilerServices;
using Discord;
using LXGaming.Common.Threading.Tasks;
using LXGaming.Common.Threading.Tasks.Models;
using LXGaming.Discord.Common.Scheduler.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LXGaming.Discord.Common.Scheduler;

public class DefaultSchedulerService(
    ILogger<DefaultSchedulerService> logger,
    IServiceScopeFactory serviceScopeFactory) : ISchedulerService, IHostedService {

    private readonly CancellableTaskCollection<EventKey> _eventTasks = new();
    private readonly CancellableTaskCollection<MessageKey> _messageTasks = new();
    private bool _disposed;

    /// <inheritdoc />
    public virtual Task StartAsync(CancellationToken cancellationToken) {
        _eventTasks.Added += OnEventAddedAsync;
        _eventTasks.Removed += OnEventRemovedAsync;
        _eventTasks.UnhandledException += OnEventUnhandledExceptionAsync;

        _messageTasks.Added += OnMessageAddedAsync;
        _messageTasks.Removed += OnMessageRemovedAsync;
        _messageTasks.UnhandledException += OnMessageUnhandledExceptionAsync;

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual async Task StopAsync(CancellationToken cancellationToken) {
        try {
            await _messageTasks.RemoveAllAsync().ConfigureAwait(false);
        } catch (Exception ex) {
            if (logger.IsEnabled(LogLevel.Error)) {
                logger.LogError(ex, "Encountered an error while unscheduling all messages");
            }
        }

        try {
            await _eventTasks.RemoveAllAsync().ConfigureAwait(false);
        } catch (Exception ex) {
            if (logger.IsEnabled(LogLevel.Error)) {
                logger.LogError(ex, "Encountered an error while unscheduling all events");
            }
        }

        _messageTasks.UnhandledException -= OnMessageUnhandledExceptionAsync;
        _messageTasks.Removed -= OnMessageRemovedAsync;
        _messageTasks.Added -= OnMessageAddedAsync;

        _eventTasks.UnhandledException -= OnEventUnhandledExceptionAsync;
        _eventTasks.Removed -= OnEventRemovedAsync;
        _eventTasks.Added -= OnEventAddedAsync;
    }

    public virtual Task ScheduleEventAsync(Func<CancellableTaskContext, IServiceProvider, Task> func,
        [CallerMemberName] string? caller = null) {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return ScheduleEventAsync(async context => {
            // This replaces the InteractionService AutoServiceScopes functionality.
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            await func(context, scope.ServiceProvider).ConfigureAwait(false);
        }, caller);
    }

    public virtual Task ScheduleEventAsync(Func<CancellableTaskContext, Task> func,
        [CallerMemberName] string? caller = null) {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var key = new EventKey(Guid.NewGuid(), caller);
        return _eventTasks.AddAsync(key, func);
    }

    public virtual Task ScheduleMessageDeletionAsync(IMessage message, TimeSpan timeout) {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return ScheduleMessageDeletionAsync(message.Channel, message.Id, message.Author.Id, timeout);
    }

    protected virtual Task ScheduleMessageDeletionAsync(IMessageChannel channel, ulong messageId, ulong userId,
        TimeSpan timeout) {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var key = new MessageKey((channel as IGuildChannel)?.GuildId, channel.Id, messageId, userId, timeout);
        return _messageTasks.AddAsync(key, async context => {
            try {
                await Task.Delay(timeout, context.CancelToken).ConfigureAwait(false);
            } catch (TaskCanceledException) {
                if (!context.StopToken.IsCancellationRequested) {
                    // When the StopToken hasn't been cancelled we must cease execution,
                    // the message has either been deleted or is no longer deletable.
                    return;
                }
            }

            await channel.DeleteMessageAsync(messageId).ConfigureAwait(false);
        });
    }

    public virtual Task UnscheduleMessageDeletionsAsync(Predicate<MessageKey> match, bool stop = true) {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _messageTasks.RemoveAllAsync(match, stop);
    }

    protected virtual Task OnEventAddedAsync(object? sender, AddedEventArgs<EventKey> args) {
        if (logger.IsEnabled(LogLevel.Trace)) {
            logger.LogTrace("Scheduled event {Event}", args.Key);
        }

        return Task.CompletedTask;
    }

    protected virtual Task OnEventRemovedAsync(object? sender, RemovedEventArgs<EventKey> args) {
        if (logger.IsEnabled(LogLevel.Trace)) {
            logger.LogTrace("Unscheduled event {Event}", args.Key);
        }

        return Task.CompletedTask;
    }

    protected virtual Task OnEventUnhandledExceptionAsync(object? sender, UnhandledExceptionEventArgs<EventKey> args) {
        if (logger.IsEnabled(LogLevel.Error)) {
            logger.LogError(args.Exception, "Encountered an error while handling event {Event}", args.Key);
        }

        return Task.CompletedTask;
    }

    protected virtual Task OnMessageAddedAsync(object? sender, AddedEventArgs<MessageKey> args) {
        if (logger.IsEnabled(LogLevel.Trace)) {
            logger.LogTrace("Scheduled message {Message} with timeout {Timeout}", args.Key, args.Key.Timeout);
        }

        return Task.CompletedTask;
    }

    protected virtual Task OnMessageRemovedAsync(object? sender, RemovedEventArgs<MessageKey> args) {
        if (logger.IsEnabled(LogLevel.Trace)) {
            logger.LogTrace("Unscheduled message {Message}", args.Key);
        }

        return Task.CompletedTask;
    }

    protected virtual Task OnMessageUnhandledExceptionAsync(object? sender,
        UnhandledExceptionEventArgs<MessageKey> args) {
        if (logger.IsEnabled(LogLevel.Error)) {
            logger.LogError(args.Exception, "Encountered an error while handling message {Message}", args.Key);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync() {
        await DisposeInternalAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeInternalAsync() {
        if (_disposed) {
            return;
        }

        _disposed = true;

        await _messageTasks.DisposeAsync().ConfigureAwait(false);
        await _eventTasks.DisposeAsync().ConfigureAwait(false);
    }
}