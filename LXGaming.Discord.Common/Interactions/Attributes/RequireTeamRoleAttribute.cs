using Discord;
using Discord.Interactions;
using LXGaming.Discord.Common.Access;
using LXGaming.Discord.Common.Interactions.Results;
using Microsoft.Extensions.DependencyInjection;

namespace LXGaming.Discord.Common.Interactions.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireTeamRoleAttribute(TeamRole role) : PreconditionAttribute {

    public TeamRole Role => role;

    public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context,
        ICommandInfo command, IServiceProvider services) {
        IAccessService accessService;
        try {
            accessService = services.GetRequiredService<IAccessService>();
        } catch (InvalidOperationException ex) {
            return TeamRoleResult.FromError(ex);
        }

        var userRole = await accessService.GetTeamRoleAsync(context.User).ConfigureAwait(false);
        if (userRole == null) {
            return TeamRoleResult.FromError("You must be a team member to run this interaction.", null, Role);
        }

        if (userRole > Role) {
            return TeamRoleResult.FromError(
                $"You must be a team member with the '{Role}' role to run this interaction.", userRole, Role);
        }

        return TeamRoleResult.FromSuccess();
    }
}