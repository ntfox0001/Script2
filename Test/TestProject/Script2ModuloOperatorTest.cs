using NUnit.Framework;
using Script2;

namespace TestProject;

/// <summary>
/// 测试 % 取模运算符
/// </summary>
public class Script2ModuloOperatorTest
{
    /// <summary>
    /// 测试 % - 基本运算 10 % 3
    /// </summary>
    [Test]
    public void TestModuloBasic()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("10 % 3", env);
        Assert.That(r, Is.EqualTo(1.0f));
    }

    /// <summary>
    /// 测试 % - 整除情况 10 % 2 = 0
    /// </summary>
    [Test]
    public void TestModuloDivisible()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("10 % 2", env);
        Assert.That(r, Is.EqualTo(0.0f));
    }

    /// <summary>
    /// 测试 % - 15 % 4 = 3
    /// </summary>
    [Test]
    public void TestModulo15By4()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("15 % 4", env);
        Assert.That(r, Is.EqualTo(3.0f));
    }

    /// <summary>
    /// 测试 % - 浮点数运算（取模使用整数运算）
    /// </summary>
    [Test]
    public void TestModuloFloats()
    {
        var env = new Script2Environment();
        var r1 = Script2Parser.Execute("10.5 % 3", env);
        Assert.That(r1, Is.EqualTo(1.0f).Within(0.001));

        var r2 = Script2Parser.Execute("7.8 % 2.5", env);
        Assert.That(r2, Is.EqualTo(1.0f).Within(0.001));
    }

    /// <summary>
    /// 测试 % - 判断奇数
    /// </summary>
    [Test]
    public void TestModulusOddNumber()
    {
        var env = new Script2Environment();
        var s = @"
var num = 7;
num % 2;
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(1.0f));
    }

    /// <summary>
    /// 测试 % - 判断偶数
    /// </summary>
    [Test]
    public void TestModulusEvenNumber()
    {
        var env = new Script2Environment();
        var s = @"
var num = 8;
num % 2;
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(0.0f));
    }

    /// <summary>
    /// 测试 % - 运算符优先级（% 高于 +）
    /// </summary>
    [Test]
    public void TestModulusPrecedencePlus()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("10 + 5 % 3", env);
        // 5 % 3 = 2, 10 + 2 = 12
        Assert.That(r, Is.EqualTo(12.0f));
    }

    /// <summary>
    /// 测试 % - 运算符优先级（% 高于 -）
    /// </summary>
    [Test]
    public void TestModulusPrecedenceMinus()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("10 - 5 % 3", env);
        // 5 % 3 = 2, 10 - 2 = 8
        Assert.That(r, Is.EqualTo(8.0f));
    }

    /// <summary>
    /// 测试 % - 与乘除同级，从左到右
    /// </summary>
    [Test]
    public void TestModulusWithMultiplyDivide()
    {
        var env = new Script2Environment();
        var r1 = Script2Parser.Execute("20 % 6 * 2", env);
        // 20 % 6 = 2, 2 * 2 = 4
        Assert.That(r1, Is.EqualTo(4.0f));

        var r2 = Script2Parser.Execute("20 * 2 % 6", env);
        // 20 * 2 = 40, 40 % 6 = 4
        Assert.That(r2, Is.EqualTo(4.0f));
    }

    /// <summary>
    /// 测试 % - 在 if 语句中判断奇偶
    /// </summary>
    [Test]
    public void TestModulusInIf()
    {
        var env = new Script2Environment();
        var s = @"
var num = 7;
if (num % 2 == 1) {
    ""odd"";
} else {
    ""even"";
}
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo("odd"));
    }

    /// <summary>
    /// 测试 % - 在 if 语句中判断偶数
    /// </summary>
    [Test]
    public void TestModulusInIfEven()
    {
        var env = new Script2Environment();
        var s = @"
var num = 8;
if (num % 2 == 0) {
    ""even"";
} else {
    ""odd"";
}
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo("even"));
    }

    /// <summary>
    /// 测试 % - 在 while 循环中使用
    /// </summary>
    [Test]
    public void TestModulusInWhile()
    {
        var env = new Script2Environment();
        var s = @"
var counter = 0;
while (counter % 10 != 0 or counter == 0) {
    counter = counter + 1;
    if (counter > 20) {
        break;
    }
}
counter;
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(10.0f));
    }

    /// <summary>
    /// 测试 % - 在函数中使用
    /// </summary>
    [Test]
    public void TestModulusInFunction()
    {
        var env = new Script2Environment();
        var s = @"
remainder(a, b) {
    return a % b;
}
remainder(17, 5);
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(2.0f));
    }

    /// <summary>
    /// 测试 % - 判断是否能被整除
    /// </summary>
    [Test]
    public void TestModulusDivisibility()
    {
        var env = new Script2Environment();
        var s = @"
isDivisible(n, divisor) {
    return n % divisor == 0;
}
isDivisible(15, 5);
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 % - 负数取模
    /// </summary>
    [Test]
    public void TestModulusNegative()
    {
        var env = new Script2Environment();
        var r1 = Script2Parser.Execute("-10 % 3", env);
        Assert.That(r1, Is.EqualTo(-1.0f));

        var r2 = Script2Parser.Execute("10 % -3", env);
        Assert.That(r2, Is.EqualTo(1.0f));
    }

    /// <summary>
    /// 测试 % - 链式运算
    /// </summary>
    [Test]
    public void TestModulusChain()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("100 % 30 % 7", env);
        // 100 % 30 = 10, 10 % 7 = 3
        Assert.That(r, Is.EqualTo(3.0f));
    }

    /// <summary>
    /// 测试 % - 与括号结合改变优先级
    /// </summary>
    [Test]
    public void TestModulusWithParentheses()
    {
        var env = new Script2Environment();
        var r1 = Script2Parser.Execute("(10 + 5) % 4", env);
        // (10 + 5) = 15, 15 % 4 = 3
        Assert.That(r1, Is.EqualTo(3.0f));

        var r2 = Script2Parser.Execute("10 + (5 % 4)", env);
        // 5 % 4 = 1, 10 + 1 = 11
        Assert.That(r2, Is.EqualTo(11.0f));
    }

    /// <summary>
    /// 测试 % - 0 作为除数
    /// </summary>
    [Test]
    public void TestModulusByZero()
    {
        var env = new Script2Environment();
        var ex = Assert.Throws<System.DivideByZeroException>(() =>
        {
            Script2Parser.Execute("10 % 0", env);
        });
    }

    /// <summary>
    /// 测试 % - 复杂表达式
    /// </summary>
    [Test]
    public void TestModulusComplexExpression()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("(100 + 50) % 7 * 2 - 5", env);
        // (100 + 50) = 150, 150 % 7 = 3, 3 * 2 = 6, 6 - 5 = 1
        Assert.That(r, Is.EqualTo(1.0f));
    }

    /// <summary>
    /// 测试 % - 与变量结合
    /// </summary>
    [Test]
    public void TestModulusWithVariables()
    {
        var env = new Script2Environment();
        var s = @"
var a = 100;
var b = 7;
var c = a % b;
c;
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(2.0f));
    }

    /// <summary>
    /// 测试 % - 求最大公约数（使用欧几里得算法）
    /// </summary>
    [Test]
    public void TestModulusGCD()
    {
        var env = new Script2Environment();
        var s = @"
gcd(a, b) {
    while (b != 0) {
        var temp = b;
        var remainder = a % temp;
        b = remainder;
        a = temp;
    }
    return a;
}
gcd(48, 18);
";
        var r = Script2Parser.Execute(s, env);
        // gcd(48, 18) = 6
        Assert.That(r, Is.EqualTo(6.0f));
    }

    /// <summary>
    /// 测试 % - 获取数字的各位数
    /// </summary>
    [Test]
    public void TestModulusGetDigits()
    {
        var env = new Script2Environment();
        var s = @"
var num = 1234;
var ones = num % 10;
var hundreds = (num / 100) % 10;
ones;
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(4.0f));
        Assert.That(env.GetVariableValue("hundreds"), Is.EqualTo(2.0f));
    }

    /// <summary>
    /// 测试 % - 循环计数
    /// </summary>
    [Test]
    public void TestModulusLoopCounter()
    {
        var env = new Script2Environment();
        var s = @"
var i = 0;
var result = 0;
while (i < 10) {
    if (i % 2 == 0) {
        result = result + 1;
    } else {
        result = result - 1;
    }
    i = i + 1;
}
result;
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(0));
    }
}
