using Discord.Interactions;

namespace LXGaming.Discord.Common.Utilities;

public static class InteractionUtils {

    public static IEnumerable<T> GetAttributes<T>(ModuleInfo module) where T : Attribute {
        return module.Attributes.OfType<T>();
    }

    public static T? GetAttribute<T>(ModuleInfo module) where T : Attribute {
        return GetAttributes<T>(module).FirstOrDefault();
    }

    public static IEnumerable<T> GetAttributes<T>(ICommandInfo command) where T : Attribute {
        return command.Attributes.OfType<T>();
    }

    public static T? GetAttribute<T>(ICommandInfo command) where T : Attribute {
        return GetAttributes<T>(command).FirstOrDefault() ?? GetAttribute<T>(command.Module);
    }
}