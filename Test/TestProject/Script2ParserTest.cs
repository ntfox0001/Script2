using NUnit.Framework;
using Script2;

namespace TestProject;

public class Script2ParserTest
{
    /// <summary>
    /// 测试内建函数调用
    /// </summary>
    [Test]
    public void TestLambda1()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("Max(9, 81)", env);
        Assert.That(r, Is.EqualTo(81));
    }

    /// <summary>
    /// 测试变量声明，函数调用，表达式计算
    /// </summary>
    [Test]
    public void TestLambda2()
    {
        var env = new Script2Environment();
        var s = @"
var a = 9
var b = Max(3, a)
b + 2
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(11));
    }

    /// <summary>
    /// 测试变量声明，函数调用，表达式计算 带有分号
    /// </summary>
    [Test]
    public void TestLambda3()
    {
        var env = new Script2Environment();
        var s = @"
var a = 9;
var b = Max(3, a);
b + 2;
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(11));
    }

    /// <summary>
    /// 测试常量带有分号
    /// </summary>
    [Test]
    public void TestLambda4()
    {
        var env = new Script2Environment();
        var s = @"5;";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(5));
    }

    /// <summary>
    /// 测试常量带有分号
    /// </summary>
    [Test]
    public void TestLambda5()
    {
        var env = new Script2Environment();
        var s = @"5;6";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(6));
    }

    [Test]
    public void TestLambda7()
    {
        var env = new Script2Environment();
        var s = @"
var a = Max(3, 5)+3
a;
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(8));
    }

    /// <summary>
    /// 测试变量声明，函数调用，表达式计算 带有分号（部分）
    /// </summary>
    [Test]
    public void TestLambda8()
    {
        var env = new Script2Environment();
        var s = @"
var a = 9
var b = Max(3, a);
b + 2
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(11));
    }

    /// <summary>
    /// 测试函数嵌套
    /// </summary>
    [Test]
    public void TestLambda9()
    {
        var env = new Script2Environment();
        var s = @"
var a = Max(3, Min(4, 1))+3;
a
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(6));
    }
}