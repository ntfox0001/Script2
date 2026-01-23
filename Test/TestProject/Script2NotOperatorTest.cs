using NUnit.Framework;
using Script2;

namespace TestProject;

/// <summary>
/// 测试 not 运算符
/// </summary>
public class Script2NotOperatorTest
{
    /// <summary>
    /// 测试 not true
    /// </summary>
    [Test]
    public void TestNotTrue()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("not true", env);
        Assert.That(r, Is.EqualTo(false));
    }

    /// <summary>
    /// 测试 not false
    /// </summary>
    [Test]
    public void TestNotFalse()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("not false", env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试双重否定
    /// </summary>
    [Test]
    public void TestDoubleNot()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("not not true", env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 not 在 if 语句中
    /// </summary>
    [Test]
    public void TestNotInIf()
    {
        var env = new Script2Environment();
        var s = @"
if (not false) {
    1;
} else {
    0;
}
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(1));
    }

    /// <summary>
    /// 测试 not 与比较运算符结合
    /// </summary>
    [Test]
    public void TestNotWithComparison()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("not (5 > 10)", env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 not 与 and 结合
    /// </summary>
    [Test]
    public void TestNotWithAnd()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("not (true and false)", env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 not 与 or 结合
    /// </summary>
    [Test]
    public void TestNotWithOr()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("not (false or false)", env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 not 与变量结合
    /// </summary>
    [Test]
    public void TestNotWithVariable()
    {
        var env = new Script2Environment();
        var s = @"
var a = true;
not a;
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(false));
    }

    /// <summary>
    /// 测试 not 在函数中使用
    /// </summary>
    [Test]
    public void TestNotInFunction()
    {
        var env = new Script2Environment();
        var s = @"
isFalse(b) {
    return not b;
}
isFalse(true);
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(false));
    }

    /// <summary>
    /// 测试 not 运算符优先级 - not 高于 and/or
    /// </summary>
    [Test]
    public void TestNotPrecedence()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("not true or false", env);
        // not true = false, then false or false = false
        Assert.That(r, Is.EqualTo(false));
    }

    /// <summary>
    /// 测试复杂逻辑表达式
    /// </summary>
    [Test]
    public void TestComplexNotExpression()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("not (5 > 3 and 10 < 20)", env);
        // 5 > 3 = true, 10 < 20 = true, true and true = true, not true = false
        Assert.That(r, Is.EqualTo(false));
    }

    /// <summary>
    /// 测试 not 在 while 循环条件中
    /// </summary>
    [Test]
    public void TestNotInWhile()
    {
        var env = new Script2Environment();
        var s = @"
var counter = 0;
while (counter < 3) {
    counter = counter + 1;
}
not (counter == 3);
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(false));
    }
}
