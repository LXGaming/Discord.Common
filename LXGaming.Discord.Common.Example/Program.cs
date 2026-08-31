using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LXGaming.Common.Serilog;
using LXGaming.Discord.Common.Example.Services.Discord;
using LXGaming.Discord.Common.Prompts;
using LXGaming.Discord.Common.Utilities;
using LXGaming.Discord.Prompts;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.ControlledBy(new EnvironmentLoggingLevelSwitch(LogEventLevel.Verbose, LogEventLevel.Debug))
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Initialising...");

try {
    var builder = Host.CreateApplicationBuilder(args);
    builder.Services.AddSerilog();

    builder.Services.AddAccessService<AccessService>();
    builder.Services.AddDiscordClient(new DiscordSocketClient(new DiscordSocketConfig {
        GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.DirectMessages,
        // Requires a stable system clock, easier to just disable it.
        UseInteractionSnowflakeDate = false
    }));
    builder.Services.AddDiscordService<DiscordService>();
    builder.Services.AddInteractionService(new InteractionServiceConfig {
        InteractionCustomIdDelimiters = [' '],
        UseCompiledLambda = true
    });
    builder.Services.AddPromptService(new PromptServiceOptions {
        DefaultTimeout = TimeSpan.FromMinutes(15),
    });
    builder.Services.AddSchedulerService();

    var host = builder.Build();

    await host.RunAsync();
    return 0;
} catch (Exception ex) {
    Log.Fatal(ex, "Application failed to initialise");
    return 1;
} finally {
    await Log.CloseAndFlushAsync();
}