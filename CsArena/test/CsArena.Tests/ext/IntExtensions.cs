using System.Runtime.CompilerServices;

namespace CsArena.Tests.ext;

public static class IntExtensions
{
    public static IEnumerator<int> GetEnumerator(this int number)
    {
        var direction = number > 0 ? number : 0;
        for (var i = number - direction; i <= direction; i++) 
            yield return i;
    }

    public static IntAwaiter GetAwaiter(this int number) => new IntAwaiter(number);

    public class IntAwaiter(int number) : INotifyCompletion
    {
        public bool IsCompleted => true;

        public void OnCompleted(Action continuation) { }

        public int GetResult() => number * number; 
    }
}