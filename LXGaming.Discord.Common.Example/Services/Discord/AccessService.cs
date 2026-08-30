using LXGaming.Discord.Common.Access;

namespace LXGaming.Discord.Common.Example.Services.Discord;

public class AccessService(IDiscordService discordService) : AccessServiceBase(discordService);