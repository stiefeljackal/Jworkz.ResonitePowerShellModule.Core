using System.Runtime.CompilerServices;

namespace Jworkz.ResonitePowerShellModule.Core.Utilities;

public static class TaskExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetAwaiterResult(this Task task) => task.GetAwaiter().GetResult();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetAwaiterResult<T>(this Task<T> task) => task.GetAwaiter().GetResult();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetAwaiterResult<T>(this ValueTask<T> vTask) => vTask.GetAwaiter().GetResult();
}
