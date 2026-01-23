using NUnit.Framework;
using Script2;

namespace TestProject;

/// <summary>
/// 测试运算符
/// </summary>
public class Script2OperatorTest
{
    /// <summary>
    /// 测试加法
    /// </summary>
    [Test]
    public void TestAddition()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("5 + 3", env);
        Assert.That(r, Is.EqualTo(8.0f));
    }

    /// <summary>
    /// 测试减法
    /// </summary>
    [Test]
    public void TestSubtraction()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("10 - 4", env);
        Assert.That(r, Is.EqualTo(6.0f));
    }

    /// <summary>
    /// 测试乘法
    /// </summary>
    [Test]
    public void TestMultiplication()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("6 * 7", env);
        Assert.That(r, Is.EqualTo(42.0f));
    }

    /// <summary>
    /// 测试除法
    /// </summary>
    [Test]
    public void TestDivision()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("15 / 3", env);
        Assert.That(r, Is.EqualTo(5.0f));
    }

    /// <summary>
    /// 测试取模 - 基本运算
    /// </summary>
    [Test]
    public void TestModulusBasic()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("10 % 3", env);
        Assert.That(r, Is.EqualTo(1.0f));
    }

    /// <summary>
    /// 测试取模 - 整除情况
    /// </summary>
    [Test]
    public void TestModulusDivisible()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("10 % 2", env);
        Assert.That(r, Is.EqualTo(0.0f));
    }

    /// <summary>
    /// 测试取模 - 浮点数（取模使用整数运算）
    /// </summary>
    [Test]
    public void TestModulusFloats()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("10.5 % 3", env);
        // 10.5 转为 int 是 10, 10 % 3 = 1
        Assert.That(r, Is.EqualTo(1.0f).Within(0.001));
    }

    /// <summary>
    /// 测试取模 - 判断奇偶
    /// </summary>
    [Test]
    public void TestModulusEvenOdd()
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
    /// 测试取模 - 运算符优先级
    /// </summary>
    [Test]
    public void TestModulusPrecedence()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("10 + 5 % 3", env);
        // 先计算 5 % 3 = 2，然后 10 + 2 = 12
        Assert.That(r, Is.EqualTo(12.0f));
    }

    /// <summary>
    /// 测试取模 - 与乘除混合
    /// </summary>
    [Test]
    public void TestModulusWithMultiplyDivide()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("20 % 6 * 2", env);
        // 20 % 6 = 2, 2 * 2 = 4
        Assert.That(r, Is.EqualTo(4.0f));
    }

    /// <summary>
    /// 测试取模 - 在 if 语句中使用
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
    /// 测试取模 - 在函数中使用
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
    /// 测试取模 - 负数
    /// </summary>
    [Test]
    public void TestModulusNegative()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("-10 % 3", env);
        Assert.That(r, Is.EqualTo(-1.0f));
    }

    /// <summary>
    /// 测试取模 - 链式运算
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
    /// 测试等于
    /// </summary>
    [Test]
    public void TestEquals()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("5 == 5", env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试不等于 - 数字
    /// </summary>
    [Test]
    public void TestNotEqualsNumbers()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("5 != 3", env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试不等于 - 相等数字返回 false
    /// </summary>
    [Test]
    public void TestNotEqualsEqualNumbers()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("5 != 5", env);
        Assert.That(r, Is.EqualTo(false));
    }

    /// <summary>
    /// 测试不等于 - 字符串
    /// </summary>
    [Test]
    public void TestNotEqualsStrings()
    {
        var env = new Script2Environment();
        var s = @"
var a = ""hello"";
var b = ""world"";
a != b;
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试不等于 - 相等字符串返回 false
    /// </summary>
    [Test]
    public void TestNotEqualsEqualStrings()
    {
        var env = new Script2Environment();
        var s = @"
var a = ""hello"";
var b = ""hello"";
a != b;
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(false));
    }

    /// <summary>
    /// 测试不等于 - 布尔值
    /// </summary>
    [Test]
    public void TestNotEqualsBooleans()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("true != false", env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试不等于 - 在 if 语句中使用
    /// </summary>
    [Test]
    public void TestNotEqualsInIf()
    {
        var env = new Script2Environment();
        var s = @"
var a = 5;
if (a != 10) {
    1;
} else {
    0;
}
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(1));
    }

    /// <summary>
    /// 测试不等于 - 在 while 循环条件中使用
    /// </summary>
    [Test]
    public void TestNotEqualsInWhile()
    {
        var env = new Script2Environment();
        var s = @"
var counter = 0;
while (counter != 3) {
    counter = counter + 1;
}
counter;
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(3.0f));
    }

    /// <summary>
    /// 测试不等于 - 与逻辑运算符结合
    /// </summary>
    [Test]
    public void TestNotEqualsWithLogicalOps()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("5 != 3 and 10 != 20", env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试不等于 - 与 not 运算符结合
    /// </summary>
    [Test]
    public void TestNotEqualsWithNot()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("not (5 != 5)", env);
        // 5 != 5 返回 false，not false 返回 true
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试大于
    /// </summary>
    [Test]
    public void TestGreaterThan()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("10 > 5", env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试小于
    /// </summary>
    [Test]
    public void TestLessThan()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("3 < 7", env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试大于等于
    /// </summary>
    [Test]
    public void TestGreaterThanOrEqual()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("5 >= 5", env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试小于等于
    /// </summary>
    [Test]
    public void TestLessThanOrEqual()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("3 <= 5", env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试逻辑非 - not true
    /// </summary>
    [Test]
    public void TestLogicalNotTrue()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("not true", env);
        Assert.That(r, Is.EqualTo(false));
    }

    /// <summary>
    /// 测试逻辑非 - not false
    /// </summary>
    [Test]
    public void TestLogicalNotFalse()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("not false", env);
        Assert.That(r, Is.EqualTo(true));
    }
    
    /// <summary>
    /// 测试逻辑非 - 与比较表达式结合
    /// </summary>
    [Test]
    public void TestLogicalNotWithComparison()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("not (5 > 10)", env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试逻辑非 - 与逻辑运算符结合
    /// </summary>
    [Test]
    public void TestLogicalNotWithLogicalOps()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("not (true and false)", env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试逻辑与
    /// </summary>
    [Test]
    public void TestLogicalAnd()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("true and false", env);
        Assert.That(r, Is.EqualTo(false));
    }

    /// <summary>
    /// 测试逻辑或
    /// </summary>
    [Test]
    public void TestLogicalOr()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("true or false", env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试运算符优先级 - 乘除优先于加减
    /// </summary>
    [Test]
    public void TestPrecedence1()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("2 + 3 * 4", env);
        Assert.That(r, Is.EqualTo(14.0f));
    }

    /// <summary>
    /// 测试运算符优先级 - 括号改变优先级
    /// </summary>
    [Test]
    public void TestPrecedence2()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("(2 + 3) * 4", env);
        Assert.That(r, Is.EqualTo(20.0f));
    }

    /// <summary>
    /// 测试运算符优先级 - 比较运算符
    /// </summary>
    [Test]
    public void TestPrecedence3()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("5 + 3 > 7", env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试复杂表达式
    /// </summary>
    [Test]
    public void TestComplexExpression()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("(5 + 3) * 2 - 4 / 2", env);
        Assert.That(r, Is.EqualTo(14.0f));
    }

    /// <summary>
    /// 测试字符串连接
    /// </summary>
    [Test]
    public void TestStringConcatenation()
    {
        var env = new Script2Environment();
        var s = @"
var a = ""Hello"";
var b = ""World"";
a + "" "" + b;
";
        Assert.Catch<InvalidOperationException>(() =>
        {
            var r = Script2Parser.Execute(s, env);
        });
    }
}
