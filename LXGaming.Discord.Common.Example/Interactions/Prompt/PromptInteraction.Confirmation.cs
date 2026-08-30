using Discord;
using Discord.Interactions;
using LXGaming.Discord.Prompts.Confirmation;

namespace LXGaming.Discord.Common.Example.Interactions.Prompt;

public partial class PromptInteraction {

    [Group(null, null)]
    public class Confirmation(IServiceProvider provider) : PromptInteraction(provider) {

        [SlashCommand("confirmation", "The time has come...")]
        public async Task<RuntimeResult> ExecuteAsync() {
            var promptBuilder = new ConfirmationPromptBuilder()
                .WithUser(Context.User)
                .WithAction(async (interaction, value) => {
                    await interaction.UpdateAsync(properties => {
                        properties.Content = value ? "Yes my lord" : "Well this is awkward";
                        properties.Components = MessageComponent.Empty;
                    });
                    return true;
                });

            await RespondAsync(promptBuilder.Build(), text: "Execute Order 66?");
            return Success();
        }
    }
}