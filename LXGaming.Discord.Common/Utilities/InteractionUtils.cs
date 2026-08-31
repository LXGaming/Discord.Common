using System.Collections;
using System.Diagnostics;
using System.Text;
using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using LXGaming.Discord.Common.Interactions.Results;

namespace LXGaming.Discord.Common.Utilities;

public static class InteractionUtils {

    public static IInteractionContext CreateInteractionContext(IDiscordClient client, RestInteraction interaction) {
        return client switch {
            DiscordRestClient restClient => new RestInteractionContext(restClient, interaction),
            _ => new InteractionContext(client, interaction, interaction.Channel)
        };
    }

    public static IInteractionContext CreateInteractionContext(IDiscordClient client, SocketInteraction interaction) {
        return client switch {
            DiscordShardedClient shardedClient => new ShardedInteractionContext(shardedClient, interaction),
            DiscordSocketClient socketClient => new SocketInteractionContext(socketClient, interaction),
            _ => new InteractionContext(client, interaction, interaction.Channel)
        };
    }

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

    public static bool IsSubCommand(ApplicationCommandOptionType type) {
        return type is ApplicationCommandOptionType.SubCommand or ApplicationCommandOptionType.SubCommandGroup;
    }

    public static string ToString(IDiscordInteraction interaction) {
        var stringBuilder = new StringBuilder();
        switch (interaction) {
            case IApplicationCommandInteraction applicationCommand:
                stringBuilder.Append(applicationCommand.Data.Name);
                Append(stringBuilder, applicationCommand.Data.Options);
                break;
            case IAutocompleteInteraction autocomplete:
                stringBuilder.Append(autocomplete.Data.CommandName);
                Append(stringBuilder, autocomplete.Data.Options);
                break;
            case IComponentInteraction component: {
                if (component.Message.InteractionMetadata != null) {
                    var metadata = ToString(component.Message.InteractionMetadata);
                    if (!string.IsNullOrEmpty(metadata)) {
                        stringBuilder.Append(metadata).Append(' ');
                    }
                }

                Append(stringBuilder, component.Data);
                break;
            }
            case IModalInteraction modal:
                stringBuilder.Append(modal.Data.CustomId);
                Append(stringBuilder, modal.Data.Components);
                break;
        }

        EntityUtils.AppendId(stringBuilder, interaction);
        Debug.Assert(stringBuilder.Length > 0);
        return stringBuilder.ToString();
    }

    public static string ToString(IMessageInteractionMetadata metadata) {
        var stringBuilder = new StringBuilder();
        if (metadata is ModalSubmitInteractionMetadata modalSubmit) {
            metadata = modalSubmit.TriggeringInteractionMetadata;
        }

        switch (metadata) {
            case ApplicationCommandInteractionMetadata applicationCommand:
                stringBuilder.Append(applicationCommand.Name);
                break;
            case MessageComponentInteractionMetadata messageComponent:
                stringBuilder.Append(messageComponent.InteractedMessageId);
                break;
        }

        Debug.Assert(stringBuilder.Length > 0);
        return stringBuilder.ToString();
    }

    public static string ToString(IResult result, bool withSensitive = false) {
        return result switch {
            _ when result.IsSuccess => "Success",
            IExceptionResult { Exception: not null } exceptionResult when withSensitive =>
                $"{exceptionResult.Exception.GetType().FullName}: {exceptionResult.Exception.Message}",
            ExecuteResult { Exception: not null } executeResult when withSensitive =>
                $"{executeResult.Exception.GetType().FullName}: {executeResult.Exception.Message}",
            _ when withSensitive => $"{result.Error}: {result.ErrorReason}",
            _ => "Error"
        };
    }

    #region IApplicationCommandInteractionDataOption
    private static void Append(StringBuilder stringBuilder,
        IReadOnlyCollection<IApplicationCommandInteractionDataOption> options) {
        foreach (var option in options) {
            Append(stringBuilder, option);
        }
    }

    private static void Append(StringBuilder stringBuilder, IApplicationCommandInteractionDataOption option) {
        if (stringBuilder.Length != 0) {
            stringBuilder.Append(' ');
        }

        stringBuilder.Append(option.Name);
        if (IsSubCommand(option.Type)) {
            Append(stringBuilder, option.Options);
            return;
        }

        AppendValue(stringBuilder, option.Value);
    }
    #endregion

    #region AutocompleteOption
    private static void Append(StringBuilder stringBuilder, IReadOnlyCollection<AutocompleteOption> options) {
        foreach (var option in options) {
            Append(stringBuilder, option);
        }
    }

    private static void Append(StringBuilder stringBuilder, AutocompleteOption option) {
        if (stringBuilder.Length != 0) {
            stringBuilder.Append(' ');
        }

        if (option.Focused) {
            stringBuilder.Append($"[{option.Name}]");
        } else {
            stringBuilder.Append(option.Name);
        }

        if (IsSubCommand(option.Type)) {
            return;
        }

        AppendValue(stringBuilder, option.Value);
    }
    #endregion

    #region IComponentInteractionData
    private static void Append(StringBuilder stringBuilder, IReadOnlyCollection<IComponentInteractionData> components) {
        foreach (var component in components) {
            Append(stringBuilder, component);
        }
    }

    private static void Append(StringBuilder stringBuilder, IComponentInteractionData component) {
        if (stringBuilder.Length != 0) {
            stringBuilder.Append(' ');
        }

        stringBuilder.Append(component.CustomId);
        if (component.Values != null && component.Values.Count != 0) {
            AppendValue(stringBuilder, component.Values);
        } else {
            AppendValue(stringBuilder, component.Value);
        }
    }
    #endregion

    private static void AppendValue(StringBuilder stringBuilder, object? value) {
        var valueString = value switch {
            IEntity<ulong> entity => EntityUtils.ToString(entity),
            IEntity<string> entity => EntityUtils.ToString(entity),
            IEnumerable enumerable => string.Join(", ", enumerable),
            _ => value?.ToString()
        };
        if (!string.IsNullOrEmpty(valueString)) {
            stringBuilder.Append(": ").Append(valueString);
        }
    }
}