using System.Runtime.CompilerServices;
using Discord;
using LXGaming.Common.Threading.Tasks;
using LXGaming.Discord.Common.Scheduler.Models;

namespace LXGaming.Discord.Common.Scheduler;

public interface ISchedulerService : IAsyncDisposable {

    Task ScheduleEventAsync(Func<CancellableTaskContext, IServiceProvider, Task> func,
        [CallerMemberName] string? caller = null);

    Task ScheduleEventAsync(Func<CancellableTaskContext, Task> func, [CallerMemberName] string? caller = null);

    Task ScheduleMessageDeletionAsync(IMessage message, TimeSpan timeout);

    Task UnscheduleMessageDeletionsAsync(Predicate<MessageKey> match, bool stop = true);
}