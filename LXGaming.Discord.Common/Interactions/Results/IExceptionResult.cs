using Discord.Interactions;

namespace LXGaming.Discord.Common.Interactions.Results;

public interface IExceptionResult : IResult {

    public Exception? Exception { get; }
}