using NUnit.Framework;
using Script2;

namespace TestProject;

[TestFixture(false)]
[TestFixture(true)]
public class Script2ParserTest(bool useInterpreter)
{
    private Script2Environment _env;

    [SetUp]
    public void SetUp()
    {
        _env = new Script2Environment(useInterpreter);
    }

    /// <summary>
    /// 测试内建函数调用
    /// </summary>
    [Test]
    public void TestLambda1()
    {
        var r = Script2Parser.Execute("Max(9, 81)", _env);
        Assert.That(r, Is.EqualTo(81));
    }

    /// <summary>
    /// 测试变量声明，函数调用，表达式计算
    /// </summary>
    [Test]
    public void TestLambda2()
    {
        var s = @"
var a = 9
var b = Max(3, a)
b + 2
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(11));
    }

    /// <summary>
    /// 测试变量声明，函数调用，表达式计算 带有分号
    /// </summary>
    [Test]
    public void TestLambda3()
    {
        var s = @"
var a = 9;
var b = Max(3, a);
b + 2;
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(11));
    }

    /// <summary>
    /// 测试常量带有分号
    /// </summary>
    [Test]
    public void TestLambda4()
    {
        var s = @"5;";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(5));
    }

    /// <summary>
    /// 测试常量带有分号
    /// </summary>
    [Test]
    public void TestLambda5()
    {
        var s = @"5;6";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(6));
    }

    [Test]
    public void TestLambda7()
    {
        var s = @"
var a = Max(3, 5)+3
a;
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(8));
    }

    /// <summary>
    /// 测试变量声明，函数调用，表达式计算 带有分号（部分）
    /// </summary>
    [Test]
    public void TestLambda8()
    {
        var s = @"
var a = 9
var b = Max(3, a);
b + 2
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(11));
    }

    /// <summary>
    /// 测试函数嵌套
    /// </summary>
    [Test]
    public void TestLambda9()
    {
        var s = @"
var a = Max(3, Min(4, 1))+3;
a
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(6));
    }

    /// <summary>
    /// 测试逻辑运算符 - and
    /// </summary>
    [Test]
    public void TestLogicalAnd()
    {
        var s = @"
if (true and true) {
    1;
} else {
    0;
}
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(1));
    }

    /// <summary>
    /// 测试逻辑运算符 - or
    /// </summary>
    [Test]
    public void TestLogicalOr()
    {
        var s = @"
if (true or false) {
    1;
} else {
    0;
}
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(1));
    }

    /// <summary>
    /// 测试比较运算符
    /// </summary>
    [Test]
    public void TestComparisonOperators()
    {
        var s = @"
var a = 5;
var b = 10;
a < b;
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试算术运算符优先级
    /// </summary>
    [Test]
    public void TestOperatorPrecedence()
    {
        var s = @"
2 + 3 * 4;
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(14));
    }

    /// <summary>
    /// 测试括号改变优先级
    /// </summary>
    [Test]
    public void TestParentheses()
    {
        var s = @"
(2 + 3) * 4;
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(20));
    }
}