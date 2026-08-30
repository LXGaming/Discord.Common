using Discord.Interactions;
using LXGaming.Discord.Prompts.Pagination.Eager;

namespace LXGaming.Discord.Common.Example.Interactions.Prompt;

public partial class PromptInteraction {

    [Group(null, null)]
    public class Pagination(IServiceProvider provider) : PromptInteraction(provider) {

        [SlashCommand("pagination", "It's a real page-turner")]
        public async Task<RuntimeResult> ExecuteAsync(
            [Summary("count", "Count")]
            int count) {
            if (count is <= 0 or > 10) {
                return Error("Count is outside of the allowed range (1 ~ 10)");
            }

            var promptBuilder = new EagerPaginationPromptBuilder()
                .WithUser(Context.User);
            for (var index = 0; index < count; index++) {
                promptBuilder.WithPage(content: $"Page #{index + 1} Ah ah ah!");
            }

            await RespondAsync(promptBuilder.Build());
            return Success();
        }
    }
}