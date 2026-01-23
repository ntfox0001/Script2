using NUnit.Framework;
using Script2;

namespace TestProject;

/// <summary>
/// 测试 MathF 内建函数
/// </summary>
public class Script2MathFuncTest
{
    /// <summary>
    /// 测试 Abs 函数
    /// </summary>
    [Test]
    public void TestAbs()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("Abs(-5.5)", env);
        Assert.That(r, Is.EqualTo(5.5f));
    }

    /// <summary>
    /// 测试 Sqrt 函数
    /// </summary>
    [Test]
    public void TestSqrt()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("Sqrt(16)", env);
        Assert.That(r, Is.EqualTo(4.0f));
    }

    /// <summary>
    /// 测试 Pow 函数
    /// </summary>
    [Test]
    public void TestPow()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("Pow(2, 3)", env);
        Assert.That(r, Is.EqualTo(8.0f));
    }

    /// <summary>
    /// 测试 Sin 函数
    /// </summary>
    [Test]
    public void TestSin()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("Sin(0)", env);
        Assert.That(r, Is.EqualTo(0.0f));
    }

    /// <summary>
    /// 测试 Cos 函数
    /// </summary>
    [Test]
    public void TestCos()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("Cos(0)", env);
        Assert.That(r, Is.EqualTo(1.0f));
    }

    /// <summary>
    /// 测试 Min 函数
    /// </summary>
    [Test]
    public void TestMin()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("Min(3, 5)", env);
        Assert.That(r, Is.EqualTo(3.0f));
    }

    /// <summary>
    /// 测试 Max 函数
    /// </summary>
    [Test]
    public void TestMax()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("Max(3, 5)", env);
        Assert.That(r, Is.EqualTo(5.0f));
    }

    /// <summary>
    /// 测试 Round 函数
    /// </summary>
    [Test]
    public void TestRound()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("Round(3.7)", env);
        Assert.That(r, Is.EqualTo(4.0f));
    }

    /// <summary>
    /// 测试 Floor 函数
    /// </summary>
    [Test]
    public void TestFloor()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("Floor(3.9)", env);
        Assert.That(r, Is.EqualTo(3.0f));
    }

    /// <summary>
    /// 测试 Ceiling 函数
    /// </summary>
    [Test]
    public void TestCeiling()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("Ceiling(3.1)", env);
        Assert.That(r, Is.EqualTo(4.0f));
    }

    /// <summary>
    /// 测试 Log 函数
    /// </summary>
    [Test]
    public void TestLog()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("Log(2.718281828)", env);
        Assert.That(r, Is.EqualTo(1.0f).Within(0.001));
    }

    /// <summary>
    /// 测试 Log10 函数
    /// </summary>
    [Test]
    public void TestLog10()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("Log10(100)", env);
        Assert.That(r, Is.EqualTo(2.0f));
    }

    /// <summary>
    /// 测试 Exp 函数
    /// </summary>
    [Test]
    public void TestExp()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("Exp(1)", env);
        Assert.That(r, Is.EqualTo(2.718281828f).Within(0.001));
    }

    /// <summary>
    /// 测试 PI 常量
    /// </summary>
    [Test]
    public void TestPI()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("Sin(PI / 2)", env);
        Assert.That(r, Is.EqualTo(1.0f).Within(0.001));
    }
}
