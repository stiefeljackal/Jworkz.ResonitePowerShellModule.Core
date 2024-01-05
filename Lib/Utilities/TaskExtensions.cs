namespace Jworkz.ResonitePowerShellModule.Core.Utilities;

public static class TaskExtensions
{
    public static T GetAwaiterResult<T>(this Task<T> task) => task.GetAwaiter().GetResult();

    public static T GetAwaiterResult<T>(this ValueTask<T> vTask) => vTask.GetAwaiter().GetResult();
}
