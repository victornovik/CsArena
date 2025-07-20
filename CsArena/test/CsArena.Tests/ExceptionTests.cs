using System.Text;

namespace CsArena.Tests;

public class ExceptionTests
{
    [Fact]
    public void WhenTest()
    {
        var sb = new StringBuilder();
        Bottom();

        // 1-st pass: All exception filters are called.
        // 2-nd pass: All finally blocks are executed
        Assert.Equal("-Middle filter-Bottom filter-TopFinally-MiddleFinally-BottomCatch-BottomFinally", sb.ToString());
        return;

        bool AppendAndReturn(string message, bool result)
        {
            sb.Append("-").Append(message);
            return result;
        }

        void Top()
        {
            try
            {
                throw new Exception();
            }
            finally
            {
                sb.Append("-TopFinally");
            }
        }

        void Middle()
        {
            try
            {
                Top();
            }
            catch (Exception) when (AppendAndReturn("Middle filter", false))
            {
                // Never catches as exception filter returns false
            }
            finally
            {
                sb.Append("-MiddleFinally");
            }
        }

        void Bottom()
        {
            try
            {
                Middle();
            }
            catch (IOException) when (AppendAndReturn("Never called", true))
            {
                // Exception filter is not called as IOException is a wrong exception type
            }
            catch (Exception) when (AppendAndReturn("Bottom filter", true))
            {
                // Will be catched as exception filter returns true
                sb.Append("-BottomCatch");
            }
            finally
            {
                sb.Append("-BottomFinally");
            }
        }
    }
}