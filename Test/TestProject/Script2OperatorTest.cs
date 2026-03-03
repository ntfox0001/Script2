using NUnit.Framework;
using Script2;

namespace TestProject;

/// <summary>
/// 测试运算符
/// </summary>
[TestFixture(true)]
[TestFixture(false)]
public class Script2OperatorTest(bool useInterpreter)
{
    private Script2Environment _env;
    [SetUp]
    public void SetUp()
    {
        _env = new Script2Environment { UseInterpreterMode = useInterpreter };
    }

    /// <summary>
    /// 测试加法
    /// </summary>
    [Test]
    public void TestAddition()
    {
        
        var r = Script2Parser.Execute("5 + 3", _env);
        Assert.That(r, Is.EqualTo(8.0f));
    }

    /// <summary>
    /// 测试减法
    /// </summary>
    [Test]
    public void TestSubtraction()
    {
        
        var r = Script2Parser.Execute("10 - 4", _env);
        Assert.That(r, Is.EqualTo(6.0f));
    }

    /// <summary>
    /// 测试乘法
    /// </summary>
    [Test]
    public void TestMultiplication()
    {
        
        var r = Script2Parser.Execute("6 * 7", _env);
        Assert.That(r, Is.EqualTo(42.0f));
    }

    /// <summary>
    /// 测试除法
    /// </summary>
    [Test]
    public void TestDivision()
    {
        
        var r = Script2Parser.Execute("15 / 3", _env);
        Assert.That(r, Is.EqualTo(5.0f));
    }

    /// <summary>
    /// 测试取模 - 基本运算
    /// </summary>
    [Test]
    public void TestModulusBasic()
    {
        
        var r = Script2Parser.Execute("10 % 3", _env);
        Assert.That(r, Is.EqualTo(1.0f));
    }

    /// <summary>
    /// 测试取模 - 整除情况
    /// </summary>
    [Test]
    public void TestModulusDivisible()
    {
        
        var r = Script2Parser.Execute("10 % 2", _env);
        Assert.That(r, Is.EqualTo(0.0f));
    }

    /// <summary>
    /// 测试取模 - 浮点数（取模使用整数运算）
    /// </summary>
    [Test]
    public void TestModulusFloats()
    {
        
        var r = Script2Parser.Execute("10.5 % 3", _env);
        // 10.5 转为 int 是 10, 10 % 3 = 1
        Assert.That(r, Is.EqualTo(1.0f).Within(0.001));
    }

    /// <summary>
    /// 测试取模 - 判断奇偶
    /// </summary>
    [Test]
    public void TestModulusEvenOdd()
    {
        
        var s = @"
var num = 7;
num % 2;
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(1.0f));
    }

    /// <summary>
    /// 测试取模 - 运算符优先级
    /// </summary>
    [Test]
    public void TestModulusPrecedence()
    {
        
        var r = Script2Parser.Execute("10 + 5 % 3", _env);
        // 先计算 5 % 3 = 2，然后 10 + 2 = 12
        Assert.That(r, Is.EqualTo(12.0f));
    }

    /// <summary>
    /// 测试取模 - 与乘除混合
    /// </summary>
    [Test]
    public void TestModulusWithMultiplyDivide()
    {
        
        var r = Script2Parser.Execute("20 % 6 * 2", _env);
        // 20 % 6 = 2, 2 * 2 = 4
        Assert.That(r, Is.EqualTo(4.0f));
    }

    /// <summary>
    /// 测试取模 - 在 if 语句中使用
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
    /// 测试取模 - 在函数中使用
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
    /// 测试取模 - 负数
    /// </summary>
    [Test]
    public void TestModulusNegative()
    {
        
        var r = Script2Parser.Execute("-10 % 3", _env);
        Assert.That(r, Is.EqualTo(-1.0f));
    }

    /// <summary>
    /// 测试取模 - 链式运算
    /// </summary>
    [Test]
    public void TestModulusChain()
    {
        
        var r = Script2Parser.Execute("100 % 30 % 7", _env);
        // 100 % 30 = 10, 10 % 7 = 3
        Assert.That(r, Is.EqualTo(3.0f));
    }

    /// <summary>
    /// 测试等于
    /// </summary>
    [Test]
    public void TestEquals()
    {
        
        var r = Script2Parser.Execute("5 == 5", _env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试不等于 - 数字
    /// </summary>
    [Test]
    public void TestNotEqualsNumbers()
    {
        
        var r = Script2Parser.Execute("5 != 3", _env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试不等于 - 相等数字返回 false
    /// </summary>
    [Test]
    public void TestNotEqualsEqualNumbers()
    {
        
        var r = Script2Parser.Execute("5 != 5", _env);
        Assert.That(r, Is.EqualTo(false));
    }

    /// <summary>
    /// 测试不等于 - 字符串
    /// </summary>
    [Test]
    public void TestNotEqualsStrings()
    {
        
        var s = @"
var a = ""hello"";
var b = ""world"";
a != b;
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试不等于 - 相等字符串返回 false
    /// </summary>
    [Test]
    public void TestNotEqualsEqualStrings()
    {
        
        var s = @"
var a = ""hello"";
var b = ""hello"";
a != b;
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(false));
    }

    /// <summary>
    /// 测试不等于 - 布尔值
    /// </summary>
    [Test]
    public void TestNotEqualsBooleans()
    {
        
        var r = Script2Parser.Execute("true != false", _env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试不等于 - 在 if 语句中使用
    /// </summary>
    [Test]
    public void TestNotEqualsInIf()
    {
        
        var s = @"
var a = 5;
if (a != 10) {
    1;
} else {
    0;
}
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(1));
    }

    /// <summary>
    /// 测试不等于 - 在 while 循环条件中使用
    /// </summary>
    [Test]
    public void TestNotEqualsInWhile()
    {
        
        var s = @"
var counter = 0;
while (counter != 3) {
    counter = counter + 1;
}
counter;
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(3.0f));
    }

    /// <summary>
    /// 测试不等于 - 与逻辑运算符结合
    /// </summary>
    [Test]
    public void TestNotEqualsWithLogicalOps()
    {
        
        var r = Script2Parser.Execute("5 != 3 and 10 != 20", _env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试不等于 - 与 not 运算符结合
    /// </summary>
    [Test]
    public void TestNotEqualsWithNot()
    {
        
        var r = Script2Parser.Execute("not (5 != 5)", _env);
        // 5 != 5 返回 false，not false 返回 true
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试大于
    /// </summary>
    [Test]
    public void TestGreaterThan()
    {
        
        var r = Script2Parser.Execute("10 > 5", _env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试小于
    /// </summary>
    [Test]
    public void TestLessThan()
    {
        
        var r = Script2Parser.Execute("3 < 7", _env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试大于等于
    /// </summary>
    [Test]
    public void TestGreaterThanOrEqual()
    {
        
        var r = Script2Parser.Execute("5 >= 5", _env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试小于等于
    /// </summary>
    [Test]
    public void TestLessThanOrEqual()
    {
        
        var r = Script2Parser.Execute("3 <= 5", _env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试逻辑非 - not true
    /// </summary>
    [Test]
    public void TestLogicalNotTrue()
    {
        
        var r = Script2Parser.Execute("not true", _env);
        Assert.That(r, Is.EqualTo(false));
    }

    /// <summary>
    /// 测试逻辑非 - not false
    /// </summary>
    [Test]
    public void TestLogicalNotFalse()
    {
        
        var r = Script2Parser.Execute("not false", _env);
        Assert.That(r, Is.EqualTo(true));
    }
    
    /// <summary>
    /// 测试逻辑非 - 与比较表达式结合
    /// </summary>
    [Test]
    public void TestLogicalNotWithComparison()
    {
        
        var r = Script2Parser.Execute("not (5 > 10)", _env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试逻辑非 - 与逻辑运算符结合
    /// </summary>
    [Test]
    public void TestLogicalNotWithLogicalOps()
    {
        
        var r = Script2Parser.Execute("not (true and false)", _env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试逻辑与
    /// </summary>
    [Test]
    public void TestLogicalAnd()
    {
        
        var r = Script2Parser.Execute("true and false", _env);
        Assert.That(r, Is.EqualTo(false));
    }

    /// <summary>
    /// 测试逻辑或
    /// </summary>
    [Test]
    public void TestLogicalOr()
    {
        
        var r = Script2Parser.Execute("true or false", _env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试运算符优先级 - 乘除优先于加减
    /// </summary>
    [Test]
    public void TestPrecedence1()
    {
        
        var r = Script2Parser.Execute("2 + 3 * 4", _env);
        Assert.That(r, Is.EqualTo(14.0f));
    }

    /// <summary>
    /// 测试运算符优先级 - 括号改变优先级
    /// </summary>
    [Test]
    public void TestPrecedence2()
    {
        
        var r = Script2Parser.Execute("(2 + 3) * 4", _env);
        Assert.That(r, Is.EqualTo(20.0f));
    }

    /// <summary>
    /// 测试运算符优先级 - 比较运算符
    /// </summary>
    [Test]
    public void TestPrecedence3()
    {
        
        var r = Script2Parser.Execute("5 + 3 > 7", _env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试复杂表达式
    /// </summary>
    [Test]
    public void TestComplexExpression()
    {
        
        var r = Script2Parser.Execute("(5 + 3) * 2 - 4 / 2", _env);
        Assert.That(r, Is.EqualTo(14.0f));
    }

    /// <summary>
    /// 测试字符串连接
    /// </summary>
    [Test]
    public void TestStringConcatenation()
    {
        
        var s = @"
var a = ""Hello"";
var b = ""World"";
a + "" "" + b;
";
        Assert.Catch<InvalidOperationException>(() =>
        {
            var r = Script2Parser.Execute(s, _env);
        });
    }
}
