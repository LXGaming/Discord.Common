namespace LXGaming.Discord.Common.Scheduler.Models;

public sealed record MessageKey(ulong? GuildId, ulong ChannelId, ulong MessageId, ulong UserId, TimeSpan? Timeout) {

    public override string ToString() {
        return MessageId.ToString();
    }
}