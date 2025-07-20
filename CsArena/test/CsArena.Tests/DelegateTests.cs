using System.Linq.Expressions;

namespace CsArena.Tests;

public delegate string Process(string message);

public class DelegateTests
{
    delegate int Printer();

    [Fact]
    public void CaptureVairablePuzzler()
    {
        var printers = new List<Printer>();
        
        for (var i = 0; i < 10; i++)
        {
            printers.Add(() => i);
        }

        foreach (var printer in printers)
        {
            Assert.Equal(10, printer());
        }
    }
    
    [Fact]
    public void MulticastDelegate()
    {
        Process? log = ReturnMsg;
        log += ReturnMsg;
        log -= ReturnMsg;
        log += ReturnMsg;
        log += ReturnLowerMsg;

        var res = log("Hello");
        Assert.Equal("hello", res);
        Assert.Equal(3, _count);
    }
    
    [Fact]
    public void RemoveNonExistingDelegate()
    {
        Process? log = ReturnMsg;
        log -= ReturnLowerMsg;
        log -= ReturnLowerMsg;

        var res = log?.Invoke("Hello");
        Assert.Equal("Hello", res);
        Assert.Equal(1, _count);
    }


    [Fact]
    public void MulticastDelegateOneLineInitialization()
    {
        var log = (Process)ReturnMsg + ReturnLowerMsg + (s => s.ToUpper());

        var res = log("Hello");
        Assert.Equal("HELLO", res);
        Assert.Equal(2, _count);
    }

    [Fact]
     public void DiscardStatement()
    {
        Process log = _ => "Hi";

        var res = log.Invoke("Hello");
        Assert.Equal("Hi", res);
    }

    [Fact]
    public void FuncAndAction()
    {
        void Process(Func<string, bool>? onInit, string initParam, Action? callback)
        {
            if (onInit?.Invoke(initParam) == true)
                callback?.Invoke();
        }

        Process(s => s.Length == 5, "Hello", () => _count++);
        Assert.Equal(1, _count);
    }

    [Fact]
    public void Lambdas()
    {
        Func<int, int> square = x => x * x;
        Func<int, int, int> add = (x, y) => x + y;
        Assert.Equal(9, square(3));
        Assert.Equal(9, add(3, 6));

        int res = 0;
        Action<int> composite = x =>
        {
            var i = square(x);
            res = x + i;
        };
        composite(2);
        Assert.Equal(6, res);
    }

    [Fact]
    public void AddRemoveLambda()
    {
        var res = string.Empty;

        var lambda1 = () => res += "aaa";
        var lambda2 = () => res += "bbb";
        lambda1 += lambda2;
        lambda1 += lambda1;
        lambda1 += () => res += "ccc";
        lambda1();

        Assert.Equal("aaabbbaaabbbccc", res);
    }

    [Fact]
    public void ExpressionTree()
    {
        //Func<int, int, int> add = (x, y) => x + y;
        //Console.WriteLine(add);

        Expression<Func<int, int, int>> addExpr = (x, y) => x + y;
        Assert.Equal("(x, y) => (x + y)", addExpr.ToString());
        //Console.WriteLine(addExpr);

        var invokable = addExpr.Compile();
        var res = invokable(3, 4);
        Assert.Equal(7, res);
    }

    [Fact]
    public void Events()
    {
        Event += (_, arg) => _count += arg;
        Event += (_, arg) => _count *= arg;
        Event(this, 10);

        Assert.Equal(100 , _count);
    }

    string ReturnMsg(string message)
    {
        _count++;
        return message;
    }

    string ReturnLowerMsg(string message)
    {
        _count++;
        return message.ToLower();
    }

    private int _count;
    private event EventHandler<int>? Event;
}