using Discord;

namespace LXGaming.Discord.Common.Access;

public abstract class AccessServiceBase(IDiscordService discordService) : IAccessService {

    public virtual async Task<IUser?> GetApplicationOwnerAsync(CancellationToken cancellationToken = default) {
        var application = await discordService.GetApplicationAsync(cancellationToken).ConfigureAwait(false);
        if (application.Team != null) {
            return application.Team.TeamMembers
                .Where(member => member.MembershipState == MembershipState.Accepted)
                .Where(member => member.User.Id == application.Team.OwnerUserId)
                .Select(member => member.User)
                .SingleOrDefault();
        }

        return application.Owner;
    }

    public virtual async Task<TeamRole?> GetTeamRoleAsync(IUser user, CancellationToken cancellationToken = default) {
        var application = await discordService.GetApplicationAsync(cancellationToken).ConfigureAwait(false);
        return application.Team?.TeamMembers
            .Where(member => member.MembershipState == MembershipState.Accepted)
            .Where(member => member.User.Id == user.Id)
            .Select(member => member.Role)
            .SingleOrDefault();
    }
}