using System.Diagnostics;
using System.Runtime.InteropServices;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Humanizer;
using LXGaming.Common.Utilities;
using LXGaming.Discord.Common.Access;
using LXGaming.Discord.Common.Example.Utilities;
using LXGaming.Discord.Common.Interactions;
using LXGaming.Discord.Common.Interactions.Attributes;
using Microsoft.Extensions.Hosting;

namespace LXGaming.Discord.Common.Example.Interactions;

public class InformationInteraction(
    IAccessService accessService,
    BaseSocketClient discordClient,
    IHostEnvironment hostEnvironment) : InteractionBase<IInteractionContext> {

    [Defer]
    [SlashCommand("information", "Displays bot information")]
    public async Task<RuntimeResult> ExecuteAsync() {
        using var process = Process.GetCurrentProcess();
        var uptime = DateTime.UtcNow - process.StartTime.ToUniversalTime();
        var memory = process.WorkingSet64;

        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithColor(Constants.Discord.Blurple);
        embedBuilder.WithTitle($"{Constants.Application.Name} v{Constants.Application.Version}");
        embedBuilder.WithUrl(Constants.Application.Website);
        embedBuilder.AddField("Uptime", uptime.Humanize(3, maxUnit: TimeUnit.Day, minUnit: TimeUnit.Second));
        embedBuilder.AddField("Environment", hostEnvironment.EnvironmentName, true);
        embedBuilder.AddField("Memory", memory.Bytes(), true);
        embedBuilder.AddField("Latency", $"{discordClient.Latency}ms", true);
        embedBuilder.AddField(
            "Runtime",
            ""
            + (EnvironmentUtils.IsRunningInContainer() ? "\n- Docker Container" : "")
            + $"\n- {RuntimeInformation.FrameworkDescription}"
            + $"\n- {RuntimeInformation.OSDescription}");
        embedBuilder.AddField(
            "Packages",
            ""
            + $"\n- {AssemblyUtils.CreateDescription("Discord.Net.Core", "Discord.Net")}"
            + $"\n- {AssemblyUtils.CreateDescription("Humanizer", "Humanizer.Core")}"
            + $"\n- {AssemblyUtils.CreateDescription("LXGaming.Common")}"
            + $"\n- {AssemblyUtils.CreateDescription("Serilog")}");

        var owner = await accessService.GetApplicationOwnerAsync();
        if (owner != null) {
            embedBuilder.WithFooter($"Operated by {owner.GlobalName ?? owner.Username}", owner.GetDisplayAvatarUrl());
        }

        await FollowupAsync(embed: embedBuilder.Build());
        return Success();
    }
}