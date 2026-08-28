namespace LXGaming.Discord.Common.Scheduler.Models;

public sealed record EventKey(Guid Id, string? Name) {

    public override string ToString() {
        return $"{Name} ({Id})";
    }
}