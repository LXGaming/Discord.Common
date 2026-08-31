using Discord;
using Discord.Interactions;
using LXGaming.Discord.Common.Interactions.Attributes;
using LXGaming.Discord.Common.Interactions.Results;
using LXGaming.Discord.Common.Utilities;

namespace LXGaming.Discord.Common.Interactions;

/// <inheritdoc />
public abstract class InteractionBase<T> : InteractionModuleBase<T> where T : class, IInteractionContext {

    protected bool HasResponded => DeferTask != null || Context.Interaction.HasResponded;

    protected Task? DeferTask { get; private set; }

    /// <inheritdoc />
    public override Task BeforeExecuteAsync(ICommandInfo command) {
        var defer = InteractionUtils.GetAttribute<DeferAttribute>(command);
        if (defer != null) {
            return DeferAsync(defer.Ephemeral);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override async Task AfterExecuteAsync(ICommandInfo command) {
        if (DeferTask == null) {
            return;
        }

        try {
            // Ensure that the DeferTask has been completed.
            await DeferTask.ConfigureAwait(false);
        } catch (Exception) {
            // no-op
        }
    }

    /// <inheritdoc />
    protected override Task DeferAsync(bool ephemeral = false, RequestOptions? options = null) {
        if (DeferTask != null) {
            throw new InvalidOperationException("Cannot defer the same interaction twice.");
        }

        EnsureResponse(false);
        // Defer awaiting defer until a later operation or after execute.
        DeferTask = base.DeferAsync(ephemeral, options);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task RespondAsync(string? text = null, Embed[]? embeds = null, bool isTTS = false,
        bool ephemeral = false, AllowedMentions? allowedMentions = null, RequestOptions? options = null,
        MessageComponent? components = null, Embed? embed = null, PollProperties? poll = null,
        MessageFlags flags = MessageFlags.None) {
        EnsureResponse(false);
        return base.RespondAsync(text, embeds, isTTS, ephemeral, allowedMentions, options, components, embed, poll,
            flags);
    }

    /// <inheritdoc />
    protected override Task RespondWithFileAsync(Stream fileStream, string fileName, string? text = null,
        Embed[]? embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions? allowedMentions = null,
        MessageComponent? components = null, Embed? embed = null, RequestOptions? options = null,
        PollProperties? poll = null, MessageFlags flags = MessageFlags.None) {
        EnsureResponse(false);
        return base.RespondWithFileAsync(fileStream, fileName, text, embeds, isTTS, ephemeral, allowedMentions,
            components, embed, options, poll, flags);
    }

    /// <inheritdoc />
    protected override Task RespondWithFileAsync(string filePath, string? fileName = null, string? text = null,
        Embed[]? embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions? allowedMentions = null,
        MessageComponent? components = null, Embed? embed = null, RequestOptions? options = null,
        PollProperties? poll = null, MessageFlags flags = MessageFlags.None) {
        EnsureResponse(false);
        return base.RespondWithFileAsync(filePath, fileName, text, embeds, isTTS, ephemeral, allowedMentions,
            components, embed, options, poll, flags);
    }

    /// <inheritdoc />
    protected override Task RespondWithFileAsync(FileAttachment attachment, string? text = null, Embed[]? embeds = null,
        bool isTTS = false, bool ephemeral = false, AllowedMentions? allowedMentions = null,
        MessageComponent? components = null, Embed? embed = null, RequestOptions? options = null,
        PollProperties? poll = null, MessageFlags flags = MessageFlags.None) {
        EnsureResponse(false);
        return base.RespondWithFileAsync(attachment, text, embeds, isTTS, ephemeral, allowedMentions, components, embed,
            options, poll, flags);
    }

    /// <inheritdoc />
    protected override Task RespondWithFilesAsync(IEnumerable<FileAttachment> attachments, string? text = null,
        Embed[]? embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions? allowedMentions = null,
        MessageComponent? components = null, Embed? embed = null, RequestOptions? options = null,
        PollProperties? poll = null, MessageFlags flags = MessageFlags.None) {
        EnsureResponse(false);
        return base.RespondWithFilesAsync(attachments, text, embeds, isTTS, ephemeral, allowedMentions, components,
            embed, options, poll, flags);
    }

    /// <inheritdoc />
    protected override async Task<IUserMessage> FollowupAsync(string? text = null, Embed[]? embeds = null,
        bool isTTS = false, bool ephemeral = false, AllowedMentions? allowedMentions = null,
        RequestOptions? options = null, MessageComponent? components = null, Embed? embed = null,
        PollProperties? poll = null, MessageFlags flags = MessageFlags.None) {
        EnsureResponse(true);
        if (DeferTask != null) {
            await DeferTask.ConfigureAwait(false);
        }

        return await base.FollowupAsync(text, embeds, isTTS, ephemeral, allowedMentions, options, components, embed,
            poll, flags).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task<IUserMessage> FollowupWithFileAsync(Stream fileStream, string fileName,
        string? text = null, Embed[]? embeds = null, bool isTTS = false, bool ephemeral = false,
        AllowedMentions? allowedMentions = null, MessageComponent? components = null, Embed? embed = null,
        RequestOptions? options = null, PollProperties? poll = null, MessageFlags flags = MessageFlags.None) {
        EnsureResponse(true);
        if (DeferTask != null) {
            await DeferTask.ConfigureAwait(false);
        }

        return await base.FollowupWithFileAsync(fileStream, fileName, text, embeds, isTTS, ephemeral, allowedMentions,
            components, embed, options, poll, flags).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task<IUserMessage> FollowupWithFileAsync(string filePath, string? fileName = null,
        string? text = null, Embed[]? embeds = null, bool isTTS = false, bool ephemeral = false,
        AllowedMentions? allowedMentions = null, MessageComponent? components = null, Embed? embed = null,
        RequestOptions? options = null, PollProperties? poll = null, MessageFlags flags = MessageFlags.None) {
        EnsureResponse(true);
        if (DeferTask != null) {
            await DeferTask.ConfigureAwait(false);
        }

        return await base.FollowupWithFileAsync(filePath, fileName, text, embeds, isTTS, ephemeral, allowedMentions,
            components, embed, options, poll, flags).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task<IUserMessage> FollowupWithFileAsync(FileAttachment attachment, string? text = null,
        Embed[]? embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions? allowedMentions = null,
        MessageComponent? components = null, Embed? embed = null, RequestOptions? options = null,
        PollProperties? poll = null, MessageFlags flags = MessageFlags.None) {
        EnsureResponse(true);
        if (DeferTask != null) {
            await DeferTask.ConfigureAwait(false);
        }

        return await base.FollowupWithFileAsync(attachment, text, embeds, isTTS, ephemeral, allowedMentions, components,
            embed, options, poll, flags).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task<IUserMessage> FollowupWithFilesAsync(IEnumerable<FileAttachment> attachments,
        string? text = null, Embed[]? embeds = null, bool isTTS = false, bool ephemeral = false,
        AllowedMentions? allowedMentions = null, MessageComponent? components = null, Embed? embed = null,
        RequestOptions? options = null, PollProperties? poll = null, MessageFlags flags = MessageFlags.None) {
        EnsureResponse(true);
        if (DeferTask != null) {
            await DeferTask.ConfigureAwait(false);
        }

        return await base.FollowupWithFilesAsync(attachments, text, embeds, isTTS, ephemeral, allowedMentions,
            components, embed, options, poll, flags).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task<IUserMessage> GetOriginalResponseAsync(RequestOptions? options = null) {
        EnsureResponse(true);
        if (DeferTask != null) {
            await DeferTask.ConfigureAwait(false);
        }

        return await base.GetOriginalResponseAsync(options).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task<IUserMessage> ModifyOriginalResponseAsync(Action<MessageProperties> func,
        RequestOptions? options = null) {
        EnsureResponse(true);
        if (DeferTask != null) {
            await DeferTask.ConfigureAwait(false);
        }

        return await base.ModifyOriginalResponseAsync(func, options).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task DeleteOriginalResponseAsync() {
        EnsureResponse(true);
        if (DeferTask != null) {
            await DeferTask.ConfigureAwait(false);
        }

        await base.DeleteOriginalResponseAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override Task RespondWithModalAsync(Modal modal, RequestOptions? options = null) {
        EnsureResponse(false);
        return base.RespondWithModalAsync(modal, options);
    }

    /// <inheritdoc />
    protected override Task RespondWithModalAsync<TModal>(string customId, TModal modal, RequestOptions? options = null,
        Action<ModalBuilder>? modifyModal = null) {
        EnsureResponse(false);
        return base.RespondWithModalAsync(customId, modal, options, modifyModal);
    }

    /// <inheritdoc />
    protected override Task RespondWithModalAsync<TModal>(string customId, RequestOptions? options = null,
        Action<ModalBuilder>? modifyModal = null) {
        EnsureResponse(false);
        return base.RespondWithModalAsync<TModal>(customId, options, modifyModal);
    }

    /// <inheritdoc />
    protected override Task RespondWithPremiumRequiredAsync(RequestOptions? options = null) {
        EnsureResponse(false);
        return base.RespondWithPremiumRequiredAsync(options);
    }

    protected InteractionResult Error(string reason) {
        return InteractionResult.FromError(reason);
    }

    protected InteractionResult Error(Exception ex) {
        return InteractionResult.FromError(ex);
    }

    protected InteractionResult Success() {
        return InteractionResult.FromSuccess();
    }

    private void EnsureResponse(bool value) {
        if (value != HasResponded) {
            throw new InvalidOperationException(value
                ? "Cannot follow up the interaction."
                : "Cannot respond to the interaction.");
        }
    }
}