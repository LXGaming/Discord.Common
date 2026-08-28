using System.Diagnostics;
using System.Text;
using Discord;

namespace LXGaming.Discord.Common.Utilities;

public static class EntityUtils {

    public static string? ToString<TEntity, TId>(Cacheable<TEntity, TId> cacheable)
        where TEntity : IEntity<TId>
        where TId : IEquatable<TId> {
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (cacheable.Value != null) {
            return ToString(cacheable.Value);
        }

        return cacheable.Id.ToString();
    }

    public static string ToString<T>(IEntity<T> entity, bool withId = true) where T : IEquatable<T> {
        var stringBuilder = new StringBuilder();
        switch (entity) {
            #region IEntity
            case IGuildScheduledEvent guildScheduledEvent:
                stringBuilder.Append(guildScheduledEvent.Name);
                break;
            case IInvite invite: {
                stringBuilder.Append(invite.Code);
                break;
            }
            #endregion
            #region ISnowflakeEntity
            case Emote emote:
                stringBuilder.Append(emote.Name);
                break;
            case ForumTag forumTag:
                stringBuilder.Append(forumTag.Name);
                break;
            case IApplication application:
                stringBuilder.Append(application.Name);
                break;
            case IApplicationCommand applicationCommand:
                stringBuilder.Append(applicationCommand.Name);
                break;
            case IAttachment attachment:
                stringBuilder.Append(attachment.Filename);
                break;
            case IAuditLogEntry auditLogEntry:
                stringBuilder.Append(auditLogEntry.Action);
                break;
            case IAutoModRule autoModRule:
                stringBuilder.Append(autoModRule.Name);
                break;
            case IChannel channel:
                stringBuilder.Append(channel.Name);
                break;
            case IDiscordInteraction interaction:
                stringBuilder.Append(interaction.Type);
                break;
            case IEntitlement entitlement:
                stringBuilder.Append(entitlement.Type);
                break;
            case IGuild guild:
                stringBuilder.Append(guild.Name);
                break;
            case IGuildOnboardingPrompt guildOnboardingPrompt:
                stringBuilder.Append(guildOnboardingPrompt.Title);
                break;
            case IGuildOnboardingPromptOption guildOnboardingPromptOption:
                stringBuilder.Append(guildOnboardingPromptOption.Title);
                break;
            case IMessage message:
                stringBuilder.Append(message.Type);
                break;
            case IMessageInteractionMetadata messageInteractionMetadata:
                stringBuilder.Append(messageInteractionMetadata.Type);
                break;
            case IRole role:
                stringBuilder.Append(role.Name);
                break;
            case ISubscription subscription:
                stringBuilder.Append(subscription.Status);
                break;
            case IUser user:
                stringBuilder.Append(user.Username);
                if (user.DiscriminatorValue != 0) {
                    stringBuilder.Append($"#{user.DiscriminatorValue:D4}");
                }

                break;
            case IUserGuild userGuild:
                stringBuilder.Append(userGuild.Name);
                break;
            case IWebhook webhook:
                stringBuilder.Append(webhook.Name);
                break;
            case PartialGuild partialGuild:
                stringBuilder.Append(partialGuild.Name);
                break;
            case SKU sku:
                stringBuilder.Append(sku.Name);
                break;
            #endregion
        }

        if (withId || stringBuilder.Length == 0) {
            AppendId(stringBuilder, entity);
        }

        Debug.Assert(stringBuilder.Length > 0);
        return stringBuilder.ToString();
    }

    internal static void AppendId<T>(StringBuilder stringBuilder, IEntity<T> entity) where T : IEquatable<T> {
        if (stringBuilder.Length == 0) {
            stringBuilder.Append(entity.Id);
        } else {
            stringBuilder.Append($" ({entity.Id})");
        }
    }
}