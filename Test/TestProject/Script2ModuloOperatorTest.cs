using NUnit.Framework;
using Script2;

namespace TestProject;

/// <summary>
/// 测试 % 取模运算符
/// </summary>
[TestFixture(true)]
[TestFixture(false)]
public class Script2ModuloOperatorTest(bool useInterpreter)
{
    private Script2Environment _env;

    [SetUp]
    public void SetUp()
    {
        _env = new Script2Environment { UseInterpreterMode = useInterpreter };
    }

    /// <summary>
    /// 测试 % - 基本运算 10 % 3
    /// </summary>
    [Test]
    public void TestModuloBasic()
    {
        var r = Script2Parser.Execute("10 % 3", _env);
        Assert.That(r, Is.EqualTo(1.0f));
    }

    /// <summary>
    /// 测试 % - 整除情况 10 % 2 = 0
    /// </summary>
    [Test]
    public void TestModuloDivisible()
    {
        var r = Script2Parser.Execute("10 % 2", _env);
        Assert.That(r, Is.EqualTo(0.0f));
    }

    /// <summary>
    /// 测试 % - 15 % 4 = 3
    /// </summary>
    [Test]
    public void TestModulo15By4()
    {
        var r = Script2Parser.Execute("15 % 4", _env);
        Assert.That(r, Is.EqualTo(3.0f));
    }

    /// <summary>
    /// 测试 % - 浮点数运算（取模使用整数运算）
    /// </summary>
    [Test]
    public void TestModuloFloats()
    {
        var r1 = Script2Parser.Execute("10.5 % 3", _env);
        Assert.That(r1, Is.EqualTo(1.0f).Within(0.001));

        var r2 = Script2Parser.Execute("7.8 % 2.5", _env);
        Assert.That(r2, Is.EqualTo(1.0f).Within(0.001));
    }

    /// <summary>
    /// 测试 % - 判断奇数
    /// </summary>
    [Test]
    public void TestModulusOddNumber()
    {
        var s = @"
var num = 7;
num % 2;
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(1.0f));
    }

    /// <summary>
    /// 测试 % - 判断偶数
    /// </summary>
    [Test]
    public void TestModulusEvenNumber()
    {
        var s = @"
var num = 8;
num % 2;
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(0.0f));
    }

    /// <summary>
    /// 测试 % - 运算符优先级（% 高于 +）
    /// </summary>
    [Test]
    public void TestModulusPrecedencePlus()
    {
        var r = Script2Parser.Execute("10 + 5 % 3", _env);
        // 5 % 3 = 2, 10 + 2 = 12
        Assert.That(r, Is.EqualTo(12.0f));
    }

    /// <summary>
    /// 测试 % - 运算符优先级（% 高于 -）
    /// </summary>
    [Test]
    public void TestModulusPrecedenceMinus()
    {
        var r = Script2Parser.Execute("10 - 5 % 3", _env);
        // 5 % 3 = 2, 10 - 2 = 8
        Assert.That(r, Is.EqualTo(8.0f));
    }

    /// <summary>
    /// 测试 % - 与乘除同级，从左到右
    /// </summary>
    [Test]
    public void TestModulusWithMultiplyDivide()
    {
        var r1 = Script2Parser.Execute("20 % 6 * 2", _env);
        // 20 % 6 = 2, 2 * 2 = 4
        Assert.That(r1, Is.EqualTo(4.0f));

        var r2 = Script2Parser.Execute("20 * 2 % 6", _env);
        // 20 * 2 = 40, 40 % 6 = 4
        Assert.That(r2, Is.EqualTo(4.0f));
    }

    /// <summary>
    /// 测试 % - 在 if 语句中判断奇偶
    /// </summary>
    [Test]
    public void TestModulusInIf()
    {
        var s = @"
var num = 7;
if (num % 2 == 1) {
    ""odd"";
} else {
    ""even"";
}
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo("odd"));
    }

    /// <summary>
    /// 测试 % - 在 if 语句中判断偶数
    /// </summary>
    [Test]
    public void TestModulusInIfEven()
    {
        var s = @"
var num = 8;
if (num % 2 == 0) {
    ""even"";
} else {
    ""odd"";
}
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo("even"));
    }

    /// <summary>
    /// 测试 % - 在 while 循环中使用
    /// </summary>
    [Test]
    public void TestModulusInWhile()
    {
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
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(10.0f));
    }

    /// <summary>
    /// 测试 % - 在函数中使用
    /// </summary>
    [Test]
    public void TestModulusInFunction()
    {
        var s = @"
remainder(a, b) {
    return a % b;
}
remainder(17, 5);
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(2.0f));
    }

    /// <summary>
    /// 测试 % - 判断是否能被整除
    /// </summary>
    [Test]
    public void TestModulusDivisibility()
    {
        var s = @"
isDivisible(n, divisor) {
    return n % divisor == 0;
}
isDivisible(15, 5);
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 % - 负数取模
    /// </summary>
    [Test]
    public void TestModulusNegative()
    {
        var r1 = Script2Parser.Execute("-10 % 3", _env);
        Assert.That(r1, Is.EqualTo(-1.0f));

        var r2 = Script2Parser.Execute("10 % -3", _env);
        Assert.That(r2, Is.EqualTo(1.0f));
    }

    /// <summary>
    /// 测试 % - 链式运算
    /// </summary>
    [Test]
    public void TestModulusChain()
    {
        var r = Script2Parser.Execute("100 % 30 % 7", _env);
        // 100 % 30 = 10, 10 % 7 = 3
        Assert.That(r, Is.EqualTo(3.0f));
    }

    /// <summary>
    /// 测试 % - 与括号结合改变优先级
    /// </summary>
    [Test]
    public void TestModulusWithParentheses()
    {
        var r1 = Script2Parser.Execute("(10 + 5) % 4", _env);
        // (10 + 5) = 15, 15 % 4 = 3
        Assert.That(r1, Is.EqualTo(3.0f));

        var r2 = Script2Parser.Execute("10 + (5 % 4)", _env);
        // 5 % 4 = 1, 10 + 1 = 11
        Assert.That(r2, Is.EqualTo(11.0f));
    }

    /// <summary>
    /// 测试 % - 0 作为除数
    /// </summary>
    [Test]
    public void TestModulusByZero()
    {
        var ex = Assert.Throws<System.DivideByZeroException>(() => { Script2Parser.Execute("10 % 0", _env); });
    }

    /// <summary>
    /// 测试 % - 复杂表达式
    /// </summary>
    [Test]
    public void TestModulusComplexExpression()
    {
        var r = Script2Parser.Execute("(100 + 50) % 7 * 2 - 5", _env);
        // (100 + 50) = 150, 150 % 7 = 3, 3 * 2 = 6, 6 - 5 = 1
        Assert.That(r, Is.EqualTo(1.0f));
    }

    /// <summary>
    /// 测试 % - 与变量结合
    /// </summary>
    [Test]
    public void TestModulusWithVariables()
    {
        var s = @"
var a = 100;
var b = 7;
var c = a % b;
c;
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(2.0f));
    }

    /// <summary>
    /// 测试 % - 求最大公约数（使用欧几里得算法）
    /// </summary>
    [Test]
    public void TestModulusGCD()
    {
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
        var r = Script2Parser.Execute(s, _env);
        // gcd(48, 18) = 6
        Assert.That(r, Is.EqualTo(6.0f));
    }

    /// <summary>
    /// 测试 % - 获取数字的各位数
    /// </summary>
    [Test]
    public void TestModulusGetDigits()
    {
        var s = @"
var num = 1234;
var ones = num % 10;
var hundreds = (num / 100) % 10;
ones;
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(4.0f));
        Assert.That(_env.GetVariableValue("hundreds"), Is.EqualTo(2.0f));
    }

    /// <summary>
    /// 测试 % - 循环计数
    /// </summary>
    [Test]
    public void TestModulusLoopCounter()
    {
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
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(0));
    }

    /// <summary>
    /// 测试 % - 字符串变量 % 数字变量应该报错
    /// </summary>
    [Test]
    public void TestModulusStringVarAndNumberVar()
    {
        var s = @"
var str = ""hello"";
var num = 3;
str % num;
";
        var ex = Assert.Throws<InvalidCastException>(() => { Script2Parser.Execute(s, _env); });
        Assert.That(ex.Message, Does.Contain("String"));
    }

    /// <summary>
    /// 测试 % - 数字变量 % 字符串变量应该报错
    /// </summary>
    [Test]
    public void TestModulusNumberVarAndStringVar()
    {
        var s = @"
var num = 10;
var str = ""world"";
num % str;
";
        var ex = Assert.Throws<InvalidCastException>(() => { Script2Parser.Execute(s, _env); });
        Assert.That(ex.Message, Does.Contain("String"));
    }

    /// <summary>
    /// 测试 % - 字符串常量 % 数字应该报错
    /// </summary>
    [Test]
    public void TestModulusStringLiteralAndNumber()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => { Script2Parser.Execute("\"hello\" % 3", _env); });
        Assert.That(ex.Message, Does.Contain("No coercion operator"));
    }

    /// <summary>
    /// 测试 % - 数字 % 字符串常量应该报错
    /// </summary>
    [Test]
    public void TestModulusNumberAndStringLiteral()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => { Script2Parser.Execute("10 % \"hello\"", _env); });
        Assert.That(ex.Message, Does.Contain("No coercion operator"));
    }

    /// <summary>
    /// 测试 % - 字符串 % 字符串应该报错
    /// </summary>
    [Test]
    public void TestModulusStringAndString()
    {
        var s = @"
var str1 = ""hello"";
var str2 = ""world"";
str1 % str2;
";
        var ex = Assert.Throws<InvalidCastException>(() => { Script2Parser.Execute(s, _env); });
        Assert.That(ex.Message, Does.Contain("String"));
    }
}