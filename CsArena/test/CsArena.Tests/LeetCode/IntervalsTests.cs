namespace CsArena.Tests.LeetCode;

using static Intervals;

public class IntervalsTests
{
    [Fact]
    public void SummaryRangesTest()
    {
        Assert.Equal(["0->2", "4->5", "7"], SummaryRanges([0, 1, 2, 4, 5, 7]));
        Assert.Equal(["0", "2->4", "6", "8->9"], SummaryRanges([0, 2, 3, 4, 6, 8, 9]));
    }
}