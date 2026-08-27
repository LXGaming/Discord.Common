using Discord;
using Discord.Interactions;
using LXGaming.Discord.Common.Access;
using LXGaming.Discord.Common.Interactions.Results;
using Microsoft.Extensions.DependencyInjection;

namespace LXGaming.Discord.Common.Interactions.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireApplicationOwnerAttribute : PreconditionAttribute {

    public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context,
        ICommandInfo command, IServiceProvider services) {
        IAccessService accessService;
        try {
            accessService = services.GetRequiredService<IAccessService>();
        } catch (InvalidOperationException ex) {
            return ApplicationOwnerResult.FromError(ex);
        }

        var owner = await accessService.GetApplicationOwnerAsync().ConfigureAwait(false);
        if (owner == null) {
            return ApplicationOwnerResult.FromError(
                "Unable to determine the application owner.", context.User.Id, null);
        }

        if (context.User.Id != owner.Id) {
            return ApplicationOwnerResult.FromError(
                "You must be the application owner to run this interaction.", context.User.Id, owner.Id);
        }

        return ApplicationOwnerResult.FromSuccess();
    }
}