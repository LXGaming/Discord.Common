using Discord;

namespace LXGaming.Discord.Common;

public interface IDiscordService {

    Task<IApplication> GetApplicationAsync(CancellationToken cancellationToken = default);
}