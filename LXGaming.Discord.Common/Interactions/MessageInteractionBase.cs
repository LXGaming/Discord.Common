using Discord;
using LXGaming.Discord.Common.Scheduler;
using Microsoft.Extensions.DependencyInjection;

namespace LXGaming.Discord.Common.Interactions;

public abstract class MessageInteractionBase<T>(IServiceProvider provider) : InteractionBase<T>
    where T : class, IInteractionContext {

    protected ISchedulerService SchedulerService { get; } = provider.GetRequiredService<ISchedulerService>();

    protected async Task<IUserMessage> FollowupTemporaryAsync(TimeSpan timeout, string? text = null,
        Embed[]? embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions? allowedMentions = null,
        RequestOptions? options = null, MessageComponent? components = null, Embed? embed = null,
        PollProperties? poll = null, MessageFlags flags = MessageFlags.None) {
        var message = await base.FollowupAsync(text, embeds, isTTS, ephemeral, allowedMentions, options, components,
            embed, poll, flags).ConfigureAwait(false);
        await SchedulerService.ScheduleMessageDeletionAsync(message, timeout).ConfigureAwait(false);
        return message;
    }

    protected async Task<IUserMessage> FollowupWithFileTemporaryAsync(TimeSpan timeout, Stream fileStream,
        string fileName, string? text = null, Embed[]? embeds = null, bool isTTS = false, bool ephemeral = false,
        AllowedMentions? allowedMentions = null, MessageComponent? components = null, Embed? embed = null,
        RequestOptions? options = null, PollProperties? poll = null, MessageFlags flags = MessageFlags.None) {
        var message = await base.FollowupWithFileAsync(fileStream, fileName, text, embeds, isTTS, ephemeral,
            allowedMentions, components, embed, options, poll, flags).ConfigureAwait(false);
        await SchedulerService.ScheduleMessageDeletionAsync(message, timeout).ConfigureAwait(false);
        return message;
    }

    protected async Task<IUserMessage> FollowupWithFileTemporaryAsync(TimeSpan timeout, string filePath,
        string? fileName = null, string? text = null, Embed[]? embeds = null, bool isTTS = false,
        bool ephemeral = false, AllowedMentions? allowedMentions = null, MessageComponent? components = null,
        Embed? embed = null, RequestOptions? options = null, PollProperties? poll = null,
        MessageFlags flags = MessageFlags.None) {
        var message = await base.FollowupWithFileAsync(filePath, fileName, text, embeds, isTTS, ephemeral,
            allowedMentions, components, embed, options, poll, flags).ConfigureAwait(false);
        await SchedulerService.ScheduleMessageDeletionAsync(message, timeout).ConfigureAwait(false);
        return message;
    }

    protected async Task<IUserMessage> FollowupWithFileTemporaryAsync(TimeSpan timeout, FileAttachment attachment,
        string? text = null, Embed[]? embeds = null, bool isTTS = false, bool ephemeral = false,
        AllowedMentions? allowedMentions = null, MessageComponent? components = null, Embed? embed = null,
        RequestOptions? options = null, PollProperties? poll = null, MessageFlags flags = MessageFlags.None) {
        var message = await base.FollowupWithFileAsync(attachment, text, embeds, isTTS, ephemeral, allowedMentions,
            components, embed, options, poll, flags).ConfigureAwait(false);
        await SchedulerService.ScheduleMessageDeletionAsync(message, timeout).ConfigureAwait(false);
        return message;
    }

    protected async Task<IUserMessage> FollowupWithFilesTemporaryAsync(TimeSpan timeout,
        IEnumerable<FileAttachment> attachments, string? text = null, Embed[]? embeds = null, bool isTTS = false,
        bool ephemeral = false, AllowedMentions? allowedMentions = null, MessageComponent? components = null,
        Embed? embed = null, RequestOptions? options = null, PollProperties? poll = null,
        MessageFlags flags = MessageFlags.None) {
        var message = await base.FollowupWithFilesAsync(attachments, text, embeds, isTTS, ephemeral, allowedMentions,
            components, embed, options, poll, flags).ConfigureAwait(false);
        await SchedulerService.ScheduleMessageDeletionAsync(message, timeout).ConfigureAwait(false);
        return message;
    }

    protected async Task<IUserMessage> ReplyTemporaryAsync(TimeSpan timeout, string? text = null, bool isTTS = false,
        Embed? embed = null, RequestOptions? options = null, AllowedMentions? allowedMentions = null,
        MessageReference? messageReference = null, MessageComponent? components = null, ISticker[]? stickers = null,
        Embed[]? embeds = null, MessageFlags flags = MessageFlags.None, PollProperties? poll = null) {
        var message = await base.ReplyAsync(text, isTTS, embed, options, allowedMentions, messageReference, components,
            stickers, embeds, flags, poll).ConfigureAwait(false);
        await SchedulerService.ScheduleMessageDeletionAsync(message, timeout).ConfigureAwait(false);
        return message;
    }

    protected async Task<IUserMessage> ModifyOriginalResponseTemporaryAsync(Action<MessageProperties> func,
        TimeSpan timeout, RequestOptions? options = null) {
        var message = await ModifyOriginalResponseAsync(func, options).ConfigureAwait(false);
        await SchedulerService.ScheduleMessageDeletionAsync(message, timeout).ConfigureAwait(false);
        return message;
    }
}