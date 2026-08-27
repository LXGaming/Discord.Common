namespace LXGaming.Discord.Common.Interactions.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class DeferAttribute(bool ephemeral = false) : Attribute {

    public bool Ephemeral => ephemeral;
}