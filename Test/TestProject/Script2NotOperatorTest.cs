using NUnit.Framework;
using Script2;

namespace TestProject;

/// <summary>
/// 测试 not 运算符
/// </summary>
[TestFixture(false)]
[TestFixture(true)]
public class Script2NotOperatorTest(bool useInterpreter)
{
    private Script2Environment _env;
    [SetUp]
    public void SetUp()
    {
        _env = new Script2Environment(useInterpreter);
    }

    /// <summary>
    /// 测试 not true
    /// </summary>
    [Test]
    public void TestNotTrue()
    {
        var r = Script2Parser.Execute("not true", _env);
        Assert.That(r, Is.EqualTo(false));
    }

    /// <summary>
    /// 测试 not false
    /// </summary>
    [Test]
    public void TestNotFalse()
    {
        var r = Script2Parser.Execute("not false", _env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试双重否定
    /// </summary>
    [Test]
    public void TestDoubleNot()
    {
        var r = Script2Parser.Execute("not not true", _env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 not 在 if 语句中
    /// </summary>
    [Test]
    public void TestNotInIf()
    {
        var s = @"
if (not false) {
    1;
} else {
    0;
}
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(1));
    }

    /// <summary>
    /// 测试 not 与比较运算符结合
    /// </summary>
    [Test]
    public void TestNotWithComparison()
    {
        var r = Script2Parser.Execute("not (5 > 10)", _env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 not 与 and 结合
    /// </summary>
    [Test]
    public void TestNotWithAnd()
    {
        var r = Script2Parser.Execute("not (true and false)", _env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 not 与 or 结合
    /// </summary>
    [Test]
    public void TestNotWithOr()
    {
        var r = Script2Parser.Execute("not (false or false)", _env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 not 与变量结合
    /// </summary>
    [Test]
    public void TestNotWithVariable()
    {
        var s = @"
var a = true;
not a;
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(false));
    }

    /// <summary>
    /// 测试 not 在函数中使用
    /// </summary>
    [Test]
    public void TestNotInFunction()
    {
        var s = @"
isFalse(b) {
    return not b;
}
isFalse(true);
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(false));
    }

    /// <summary>
    /// 测试 not 运算符优先级 - not 高于 and/or
    /// </summary>
    [Test]
    public void TestNotPrecedence()
    {
        var r = Script2Parser.Execute("not true or false", _env);
        // not true = false, then false or false = false
        Assert.That(r, Is.EqualTo(false));
    }

    /// <summary>
    /// 测试复杂逻辑表达式
    /// </summary>
    [Test]
    public void TestComplexNotExpression()
    {
        var r = Script2Parser.Execute("not (5 > 3 and 10 < 20)", _env);
        // 5 > 3 = true, 10 < 20 = true, true and true = true, not true = false
        Assert.That(r, Is.EqualTo(false));
    }

    /// <summary>
    /// 测试 not 在 while 循环条件中
    /// </summary>
    [Test]
    public void TestNotInWhile()
    {
        var s = @"
var counter = 0;
while (counter < 3) {
    counter = counter + 1;
}
not (counter == 3);
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(false));
    }
}
