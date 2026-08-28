using Discord.Interactions;
using Discord.WebSocket;
using LXGaming.Common.Event;
using LXGaming.Discord.Common.Scheduler;
using LXGaming.Discord.Common.Utilities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LXGaming.Discord.Common.Listeners;

public class InteractionListener(
    BaseSocketClient discordClient,
    InteractionService interactionService,
    ILogger<InteractionListener> logger,
    ISchedulerService schedulerService) : IHostedService {

    public event AsyncEventHandler<IResult>? InteractionExecuted;

    /// <inheritdoc />
    public virtual Task StartAsync(CancellationToken cancellationToken) {
        discordClient.InteractionCreated += OnInteractionCreatedAsync;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task StopAsync(CancellationToken cancellationToken) {
        discordClient.InteractionCreated -= OnInteractionCreatedAsync;
        return Task.CompletedTask;
    }

    protected virtual Task OnInteractionCreatedAsync(SocketInteraction interaction) {
        return schedulerService.ScheduleEventAsync(async (_, provider) => {
            if (logger.IsEnabled(LogLevel.Trace)) {
                logger.LogTrace("Interaction {Interaction} Created", EntityUtils.ToString(interaction));
            }

            if (logger.IsEnabled(LogLevel.Debug)) {
                logger.LogDebug("Processing Interaction {Interaction} for User {User}",
                    InteractionUtils.ToString(interaction), EntityUtils.ToString(interaction.User));
            }

            var interactionContext = InteractionUtils.CreateInteractionContext(discordClient, interaction);

            IResult result;
            try {
                result = await interactionService.ExecuteCommandAsync(interactionContext, provider)
                    .ConfigureAwait(false);
            } catch (Exception ex) {
                result = ExecuteResult.FromError(ex);
            }

            await InteractionExecuted.InvokeAsync(this, result).ConfigureAwait(false);
        });
    }
}