using Discord;
using LXGaming.Discord.Common.Interactions;
using LXGaming.Discord.Prompts;
using LXGaming.Discord.Prompts.Pagination;
using LXGaming.Discord.Prompts.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace LXGaming.Discord.Common.Prompts;

public abstract class PromptInteractionBase<T>(IServiceProvider provider) : InteractionBase<T>
    where T : class, IInteractionContext {

    protected PromptService PromptService { get; } = provider.GetRequiredService<PromptService>();

    protected async Task<IUserMessage> FollowupAsync(PaginationPromptBase prompt, TimeSpan? timeout = null,
        bool isTTS = false, bool ephemeral = false, RequestOptions? options = null, PollProperties? poll = null,
        MessageFlags flags = MessageFlags.None) {
        return await PromptService.FollowupAsync(Context.Interaction, prompt, timeout, isTTS, ephemeral, options, poll,
            flags).ConfigureAwait(false);
    }

    protected async Task<IUserMessage> FollowupAsync(PromptBase prompt, TimeSpan? timeout = null,
        IEnumerable<FileAttachment>? attachments = null, string? text = null, Embed[]? embeds = null,
        bool isTTS = false, bool ephemeral = false, AllowedMentions? allowedMentions = null, Embed? embed = null,
        RequestOptions? options = null, PollProperties? poll = null, MessageFlags flags = MessageFlags.None) {
        return await PromptService.FollowupAsync(Context.Interaction, prompt, timeout, attachments, text, embeds, isTTS,
            ephemeral, allowedMentions, embed, options, poll, flags).ConfigureAwait(false);
    }

    protected async Task<IUserMessage> ModifyOriginalResponseAsync(PaginationPromptBase prompt,
        TimeSpan? timeout = null, RequestOptions? options = null) {
        return await PromptService.ModifyOriginalResponseAsync(Context.Interaction, prompt, timeout, options)
            .ConfigureAwait(false);
    }

    protected async Task<IUserMessage> ModifyOriginalResponseAsync(PromptBase prompt, TimeSpan? timeout = null,
        IEnumerable<FileAttachment>? attachments = null, string? text = null, Embed[]? embeds = null,
        AllowedMentions? allowedMentions = null, Embed? embed = null, RequestOptions? options = null) {
        return await PromptService.ModifyOriginalResponseAsync(Context.Interaction, prompt, timeout, attachments, text,
            embeds, allowedMentions, embed, options).ConfigureAwait(false);
    }

    protected async Task<IUserMessage> RespondAsync(PaginationPromptBase prompt, TimeSpan? timeout = null,
        bool isTTS = false, bool ephemeral = false, RequestOptions? options = null, PollProperties? poll = null,
        MessageFlags flags = MessageFlags.None) {
        return await PromptService.RespondAsync(Context.Interaction, prompt, timeout, isTTS, ephemeral, options, poll,
            flags).ConfigureAwait(false);
    }

    protected async Task<IUserMessage> RespondAsync(PromptBase prompt, TimeSpan? timeout = null,
        IEnumerable<FileAttachment>? attachments = null, string? text = null, Embed[]? embeds = null,
        bool isTTS = false, bool ephemeral = false, AllowedMentions? allowedMentions = null, Embed? embed = null,
        RequestOptions? options = null, PollProperties? poll = null, MessageFlags flags = MessageFlags.None) {
        return await PromptService.RespondAsync(Context.Interaction, prompt, timeout, attachments, text, embeds, isTTS,
            ephemeral, allowedMentions, embed, options, poll, flags).ConfigureAwait(false);
    }
}