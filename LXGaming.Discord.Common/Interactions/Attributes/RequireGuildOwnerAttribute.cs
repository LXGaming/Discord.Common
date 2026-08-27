using Discord;
using Discord.Interactions;
using LXGaming.Discord.Common.Interactions.Results;

namespace LXGaming.Discord.Common.Interactions.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireGuildOwnerAttribute : PreconditionAttribute {

    public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context,
        ICommandInfo command, IServiceProvider services) {
        if (context.User is not IGuildUser guildUser) {
            return Task.FromResult<PreconditionResult>(GuildOwnerResult.FromError(
                "You must be in a guild to run this interaction.", context.User.Id, null));
        }

        if (guildUser.Id != guildUser.Guild.OwnerId) {
            return Task.FromResult<PreconditionResult>(GuildOwnerResult.FromError(
                "You must be the guild owner to run this interaction.", guildUser.Id, guildUser.Guild.OwnerId));
        }

        return Task.FromResult<PreconditionResult>(GuildOwnerResult.FromSuccess());
    }
}