using Discord.Interactions;

namespace LXGaming.Discord.Common.Interactions.Results;

public sealed class InteractionResult : RuntimeResult {

    private static readonly InteractionResult Success = new(null, null, null);

    public Exception? Exception { get; }

    private InteractionResult(InteractionCommandError? error, string? reason, Exception? exception)
        : base(error, reason) {
        Exception = exception;
    }

    public static InteractionResult FromError(string reason) => new(InteractionCommandError.Unsuccessful, reason, null);

    public static InteractionResult FromError(Exception exception) =>
        new(InteractionCommandError.Exception, exception.Message, exception);

    public static InteractionResult FromSuccess() => Success;
}