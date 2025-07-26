using System.Runtime.InteropServices;

namespace CsArena.Tests;

public class MemoryTests
{
    // Span<T> is value type.
    // Span represents a contiguous region of arbitrary memory.
    // Unlike arrays, it can point to either managed or native memory or to memory allocated on the stack.
    [Fact]
    public void Span()
    {
        int[] arr = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

        Span<int> span = arr;
        Assert.Equal(10, span.Length);
        span[0] = 111;
        span[^1] = 999;

        Assert.True(typeof(Span<int>).IsValueType);
        Assert.Equal(111, arr[0]);
        Assert.Equal(999, arr[^1]);

        var span2 = span[^5..];
        Assert.Equal([5, 6, 7, 8, 999], span2.ToArray());
    }

    [Fact]
    public void SpanOfNativeMemory()
    {
        nint native = Marshal.AllocHGlobal(10);

        Span<byte> span;
        unsafe
        {
            span = new Span<byte>(native.ToPointer(), 10);
        }

        span.Clear();
        span[0] = 1;
        span[^1] = 10;

        Assert.Equal(10, span.Length);
        Assert.Equal([1, 0, 0, 0, 0, 0, 0, 0, 0, 10], span.ToArray());
        Marshal.FreeHGlobal(native);
    }

    [Fact]
    public void SpanOfStack()
    {
        Span<byte> span = stackalloc byte[10];

        span[0] = 1;
        span[^1] = 10;

        Assert.Equal(10, span.Length);
        Assert.Equal([1, 0, 0, 0, 0, 0, 0, 0, 0, 10], span.ToArray());
    }

    [Fact]
    public void ReadonlySpan()
    {
        var str = "Victor Novik";
        ReadOnlySpan<char> span = str.AsSpan(0, 6);
        Assert.Equal('V', span[0]);
        Assert.Equal('r', span[^1]);

        // Cannot mutate through the span
        // roSpan[0] = 'a';
    }

    // Memory<T> can be stored in heap and can be used as a field and in await and yield
    [Fact]
    public void Memory()
    {
        int[] arr = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

        Memory<int> mem = arr;
        mem.Slice(0, 1).Span[0] = 111;
        mem.Slice(9, 1).Span[0] = 999;
        
        Assert.Equal(10, mem.Length);
        Assert.True(typeof(Memory<int>).IsValueType);
        Assert.Equal(111, arr[0]);
        Assert.Equal(999, arr[^1]);
    }

    [Fact]
    public void StackAlloc()
    {
        Span<int> numbers = stackalloc[] { 1, 2, 3, 4, 5, 6 };
        var index = numbers.IndexOfAny(stackalloc[] { 5, 6 });

        Assert.Equal(4, index);
    }

    [Fact]
    public void SliceSpan()
    {
        byte[] msg = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
        Process(msg);

        Assert.True(msg.All(el => el == 0));
        void Process(Span<byte> payload)
        {
            Span<byte> header = payload.Slice(0, 5);
            Span<byte> data = payload.Slice(5);
            Span<byte> signature = payload.Slice(payload.Length - 1);

            Assert.Equal([0, 1, 2, 3, 4], header.ToArray());
            Assert.Equal([5, 6, 7, 8, 9], data.ToArray());
            Assert.Equal([9], signature.ToArray());

            payload.Fill(0);
        }
    }

    [Fact]
    public void SliceArray()
    {
        int[] arr = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

        var slice1 = arr[..2];
        Assert.Equal([0, 1], slice1);

        var slice2 = arr[..];
        Assert.Equal(arr, slice2);

        Index middle = arr.Length / 2;
        var slice3 = arr[..middle];
        Assert.Equal([0, 1, 2, 3, 4], slice3);

        var fifthFromEnd = new Index(5, fromEnd: true);
        var lastFive1 = arr[fifthFromEnd..];
        Assert.Equal([5, 6, 7, 8, 9], lastFive1);

        // The same as above with concise hat operator
        var lastFive2 = arr[^5..];
        Assert.Equal(lastFive1, lastFive2);
    }

    [Fact]
    public void SliceString()
    {
        const string s = "abcdefghijklmno";
        Assert.Equal("abcd", s[..4]);
        Assert.Equal("abcdefghijk", s[..^4]);
        Assert.Equal("mno", s[^3..]);
        // Trim both ends
        Assert.Equal("bcdefghijklmn", s[1..^1]);
    }

    [Fact]
    public void Range()
    {
        int[] arr = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
        Range rng = 1..4;

        Assert.Equal([1, 2, 3], arr[rng]);
        Assert.Equal([0, 1, 2], arr[..3]);
    }

    [Fact]
    public void HatOperator()
    {
        int[] arr = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

        var lastElement = arr[^1];
        Assert.Equal(9, lastElement);
        Assert.Equal(8, arr[^2]);

        var allButLast = arr[..^1];
        Assert.Equal([0, 1, 2, 3, 4, 5, 6, 7, 8], allButLast);

        var twoLast = arr[^2..];
        Assert.Equal([8, 9], twoLast);
    }
}