using Discord;
using Discord.Interactions;
using LXGaming.Discord.Common.Prompts;

namespace LXGaming.Discord.Common.Example.Interactions.Prompt;

[Group("prompt", "Prompt Interaction")]
public partial class PromptInteraction(IServiceProvider provider)
    : PromptInteractionBase<IInteractionContext>(provider);