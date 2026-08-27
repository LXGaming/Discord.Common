using Discord;

namespace LXGaming.Discord.Common.Access;

public interface IAccessService {

    Task<IUser?> GetApplicationOwnerAsync(CancellationToken cancellationToken = default);

    Task<TeamRole?> GetTeamRoleAsync(IUser user, CancellationToken cancellationToken = default);
}