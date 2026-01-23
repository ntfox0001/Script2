using NUnit.Framework;
using Script2;

namespace TestProject;

/// <summary>
/// 测试复杂场景和综合用例
/// </summary>
public class Script2ComplexScenarioTest
{
    /// <summary>
    /// 测试斐波那契数列
    /// </summary>
    [Test]
    public void TestFibonacci()
    {
        var env = new Script2Environment();
        var s = @"
fib(n) {
    if (n <= 1) {
        return n;
    }
    return fib(n - 1) + fib(n - 2);
}
fib(6);
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(8.0f));
    }

    /// <summary>
    /// 测试阶乘
    /// </summary>
    [Test]
    public void TestFactorial()
    {
        var env = new Script2Environment();
        var s = @"
fact(n) {
    if (n <= 1) {
        return 1;
    }
    return n * fact(n - 1);
}
fact(5);
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(120.0f));
    }

    /// <summary>
    /// 测试累加器
    /// </summary>
    [Test]
    public void TestAccumulator()
    {
        var env = new Script2Environment();
        var s = @"
accumulator(n) {
    var sum = 0;
    var i = 1;
    while (i <= n) {
        sum = sum + i;
        i = i + 1;
    }
    return sum;
}
accumulator(10);
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(55.0f));
    }

    /// <summary>
    /// 测试嵌套函数调用
    /// </summary>
    [Test]
    public void TestNestedFunctionCalls()
    {
        var env = new Script2Environment();
        var s = @"
add(a, b) { return a + b }
mult(a, b) { return a * b }
result(a, b) {
    return add(a, b);
}
result(5, 6);
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(11.0f));
    }

    /// <summary>
    /// 测试闭包 - 函数捕获外部变量
    /// </summary>
    [Test]
    public void TestClosure()
    {
        var env = new Script2Environment();
        var s = @"
var x = 10;
addX(a) {
    return a + x;
}
addX(5);
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(15.0f));
    }

    /// <summary>
    /// 测试条件嵌套
    /// </summary>
    [Test]
    public void TestNestedConditions()
    {
        var env = new Script2Environment();
        var s = @"
grade(score) {
    if (score >= 90) {
        return ""A"";
    } else {
        if (score >= 80) {
            return ""B"";
        } else {
            if (score >= 70) {
                return ""C"";
            } else {
                return ""D"";
            }
        }
    }
}
grade(85);
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo("B"));
    }

    /// <summary>
    /// 测试函数递归深度
    /// </summary>
    [Test]
    public void TestRecursionDepth()
    {
        var env = new Script2Environment();
        var s = @"
countDown(n) {
    if (n <= 0) {
        return 0;
    }
    return 1 + countDown(n - 1);
}
countDown(5);
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(5.0f));
    }

    /// <summary>
    /// 测试多变量交换
    /// </summary>
    [Test]
    public void TestVariableSwap()
    {
        var env = new Script2Environment();
        var s = @"
var a = 5;
var b = 10;
var temp = a;
a = b;
b = temp;
a;
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(10.0f));
        Assert.That(env.GetVariableValue("b"), Is.EqualTo(5.0f));
    }

    /// <summary>
    /// 测试字符串处理
    /// </summary>
    [Test]
    public void TestStringProcessing()
    {
        var env = new Script2Environment();
        var s = @"
greet(name) {
    var prefix = ""Hello, "";
    var suffix = ""!"";
    return prefix + name + suffix;
}
greet(""World"");
";
        Assert.Catch<InvalidCastException>(() =>
        {
            var r = Script2Parser.Execute(s, env);
        });
    }

    /// <summary>
    /// 测试数学表达式计算器
    /// </summary>
    [Test]
    public void TestCalculator()
    {
        var env = new Script2Environment();
        var s = @"
calculate(a, b, op) {
    if (op == ""+"") {
        return a + b;
    }
    if (op == ""-"") {
        return a - b;
    }
    if (op == ""*"") {
        return a * b;
    }
    if (op == ""/"") {
        return a / b;
    }
    return 0;
}
calculate(10, 5, ""*"");
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(50.0f));
    }

    /// <summary>
    /// 测试查找最大值
    /// </summary>
    [Test]
    public void TestFindMax()
    {
        var env = new Script2Environment();
        var s = @"
findMax(a, b, c) {
    var max = a;
    if (b > max) {
        max = b;
    }
    if (c > max) {
        max = c;
    }
    return max;
}
findMax(3, 7, 5);
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(7.0f));
    }

    /// <summary>
    /// 测试是否为偶数
    /// </summary>
    [Test]
    public void TestIsEven()
    {
        var env = new Script2Environment();
        var s = @"
isEven(n) {
    if (n % 2 == 0) {
        return true;
    }
    return false;
}
isEven(4);
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(true));
    }
}
