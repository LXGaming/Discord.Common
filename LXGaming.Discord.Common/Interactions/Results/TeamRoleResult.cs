using Discord;
using Discord.Interactions;

namespace LXGaming.Discord.Common.Interactions.Results;

public sealed class TeamRoleResult : PreconditionResult, IExceptionResult {

    private static readonly TeamRoleResult Success = new(null, null, null, null, null);

    public Exception? Exception { get; }

    public TeamRole? ActualRole { get; }

    public TeamRole? ExpectedRole { get; }

    private TeamRoleResult(InteractionCommandError? error, string? reason, Exception? exception, TeamRole? actualRole,
        TeamRole? expectedRole) : base(error, reason) {
        Exception = exception;
        ActualRole = actualRole;
        ExpectedRole = expectedRole;
    }

    public static TeamRoleResult FromError(string reason, TeamRole? actualRole, TeamRole? expectedRole) =>
        new(InteractionCommandError.UnmetPrecondition, reason, null, actualRole, expectedRole);

    public new static TeamRoleResult FromError(Exception exception) =>
        new(InteractionCommandError.Exception, exception.Message, exception, null, null);

    public new static TeamRoleResult FromSuccess() => Success;
}