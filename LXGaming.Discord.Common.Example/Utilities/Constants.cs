using Discord;
using LXGaming.Common.Utilities;

namespace LXGaming.Discord.Common.Example.Utilities;

public static class Constants {

    public static class Application {

        public const string Name = "Discord.Common";
        public const string Website = "https://github.com/LXGaming/Discord.Common";

        public static readonly string Version = AssemblyUtils.GetVersion(typeof(Constants).Assembly) ?? "Unknown";
    }

    public static class Discord {

        public static readonly Color Blurple = new(5793266U);
        public static readonly Color LegacyBlurple = new(7506394U);
    }
}