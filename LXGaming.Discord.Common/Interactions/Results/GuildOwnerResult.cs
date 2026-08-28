using Discord.Interactions;

namespace LXGaming.Discord.Common.Interactions.Results;

public sealed class GuildOwnerResult : PreconditionResult, IExceptionResult {

    private static readonly GuildOwnerResult Success = new(null, null, null, null, null);

    public Exception? Exception { get; }

    public ulong? ActualUserId { get; }

    public ulong? ExpectedUserId { get; }

    private GuildOwnerResult(InteractionCommandError? error, string? reason, Exception? exception, ulong? actualUserId,
        ulong? expectedUserId) : base(error, reason) {
        Exception = exception;
        ActualUserId = actualUserId;
        ExpectedUserId = expectedUserId;
    }

    public static GuildOwnerResult FromError(string reason, ulong? actualUserId, ulong? expectedUserId) =>
        new(InteractionCommandError.UnmetPrecondition, reason, null, actualUserId, expectedUserId);

    public new static GuildOwnerResult FromError(Exception exception) =>
        new(InteractionCommandError.Exception, exception.Message, exception, null, null);

    public new static GuildOwnerResult FromSuccess() => Success;
}