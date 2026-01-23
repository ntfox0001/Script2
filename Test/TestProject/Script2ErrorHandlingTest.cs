using NUnit.Framework;
using Script2;

namespace TestProject;

/// <summary>
/// 测试错误处理
/// </summary>
public class Script2ErrorHandlingTest
{
    /// <summary>
    /// 测试未定义的变量
    /// </summary>
    [Test]
    public void TestUndefinedVariable()
    {
        var env = new Script2Environment();
        Assert.Throws<InvalidOperationException>(() =>
        {
            Script2Parser.Execute("undefinedVar", env);
        });
    }

    /// <summary>
    /// 测试未定义的函数
    /// </summary>
    [Test]
    public void TestUndefinedFunction()
    {
        var env = new Script2Environment();
        Assert.Throws<InvalidOperationException>(() =>
        {
            Script2Parser.Execute("undefinedFunc(1, 2)", env);
        });
    }

    /// <summary>
    /// 测试函数参数数量不匹配
    /// </summary>
    [Test]
    public void TestFunctionArgCountMismatch()
    {
        var env = new Script2Environment();
        env.RegisterFunc<int, int>("TestFunc", (arg) => arg * 2);
        Assert.Throws<InvalidOperationException>(() =>
        {
            Script2Parser.Execute("TestFunc(1, 2)", env);
        });
    }

    /// <summary>
    /// 测试除以零
    /// </summary>
    [Test]
    public void TestDivisionByZero()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("1 / 0", env);
        // 除以零结果是正无穷或负无穷
        Assert.That(float.IsInfinity((float)r));
    }

    /// <summary>
    /// 测试语法错误 - 未闭合的括号
    /// </summary>
    [Test]
    public void TestUnclosedParenthesis()
    {
        var env = new Script2Environment();
        Assert.Throws<Superpower.ParseException>(() =>
        {
            Script2Parser.Execute("Max(1, 2", env);
        });
    }

    /// <summary>
    /// 测试将 void 赋值给变量
    /// </summary>
    [Test]
    public void TestAssignVoidToVariable()
    {
        var env = new Script2Environment();
        var s = @"
noReturn() {
    var x = 1;
}
var result = noReturn();
";
        Assert.Throws<InvalidOperationException>(() =>
        {
            Script2Parser.Execute(s, env);
        });
    }

    /// <summary>
    /// 测试在子环境中注册函数
    /// </summary>
    [Test]
    public void TestRegisterFuncInChildEnv()
    {
        var rootEnv = new Script2Environment();
        var childEnv = rootEnv.CreateChildEnvironment();
        Assert.Throws<InvalidOperationException>(() =>
        {
            childEnv.RegisterFunc<int, int>("TestFunc", (arg) => arg * 2);
        });
    }

    /// <summary>
    /// 测试不合法的字符串 - 未闭合引号
    /// </summary>
    [Test]
    public void TestUnclosedString()
    {
        var env = new Script2Environment();
        Assert.Throws<Superpower.ParseException>(() =>
        {
            Script2Parser.Execute(@"""unclosed", env);
        });
    }

    /// <summary>
    /// 测试无效的表达式
    /// </summary>
    [Test]
    public void TestInvalidExpression()
    {
        var env = new Script2Environment();
        Assert.Throws<Superpower.ParseException>(() =>
        {
            Script2Parser.Execute("5 + + 3", env);
        });
    }
}
