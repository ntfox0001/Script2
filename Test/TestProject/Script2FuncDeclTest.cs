using NUnit.Framework;
using Script2;

namespace TestProject;

[TestFixture(true)]
[TestFixture(false)]
public class Script2FuncDeclTest(bool useInterpreter)
{
    private Script2Environment _env;
    [SetUp]
    public void SetUp()
    {
        _env = new Script2Environment(useInterpreter);
    }

    /// <summary>
    /// 测试函数声明和调用
    /// </summary>
    [Test]
    public void TestFuncDeclaration()
    {
        var s = @"
    add() { return 5 }
    ";
        try
        {
            var r = Script2Parser.Execute(s, _env);
            Assert.That(r, Is.Null);
            Assert.That(_env.HasFunction("add"), Is.True, "Function 'add' should be registered");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// 测试函数调用（使用已声明的函数）
    /// </summary>
    [Test]
    public void TestFuncCall()
    {
        var s1 = @"
    add(a, b) { return a + b }
    add(3, 5)
    ";
        var r = Script2Parser.Execute(s1, _env);
        Assert.That(r, Is.EqualTo(8.0f));
    }

    /// <summary>
    /// 测试函数声明和调用 - 使用内部变量
    /// </summary>
    [Test]
    public void TestFuncDeclarationWithVars()
    {
        var s = @"
    var x = 22
    var y = 11
    add(a, b) {
        var x = a * 2
        var y = b * 3
        return x + y
    }
    add(2, 3)
    ";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(13.0f));
    }

    /// <summary>
    /// 测试函数声明和调用 - 多参数
    /// </summary>
    [Test]
    public void TestFuncDeclarationMultiArgs()
    {
        var s = @"
    sum(a, b, c) { return a + b + c }
    sum(1, 2, 3)
    ";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(6.0f));
    }

    /// <summary>
    /// 测试函数显式return语句
    /// </summary>
    [Test]
    public void TestFuncWithReturn()
    {
        var s = @"
    add(a, b) {
        return a + b
    }
    add(3, 5)
    ";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(8.0f));
    }

    /// <summary>
    /// 测试没有return语句的函数（返回void）
    /// </summary>
    [Test]
    public void TestFuncWithoutReturn()
    {
        var s = @"
    printHello() {
        5
    }
    printHello()
    ";
        var r = Script2Parser.Execute(s, _env);
        // 没有return语句，应该返回void标记值
        Assert.That(r, Is.TypeOf<VoidValue>());
    }

    /// <summary>
    /// 测试return 0（返回0，不是void）
    /// </summary>
    [Test]
    public void TestReturnZero()
    {
        var s = @"
    returnZero() {
        return 0
    }
    returnZero()
    ";
        var r = Script2Parser.Execute(s, _env);
        // 显式return 0应该成功
        Assert.That(r, Is.EqualTo(0.0f));
    }

    /// <summary>
    /// 测试return null（显式返回null，不是void）
    /// </summary>
    [Test]
    public void TestReturnNull()
    {
        var s = @"
    returnNull() {
        return null
    }
    returnNull()
    ";
        var r = Script2Parser.Execute(s, _env);
        // 显式return null应该成功
        Assert.That(r, Is.Null);
    }

    [Test]
    public void TestReadFile()
    {
        // 注册 s2 脚本中调用的外部函数
        _env.RegisterFunc<string, bool>("activePart", (name, act) =>
        {
            Console.WriteLine($"activePart({name}, {act})");
        });
        _env.RegisterFunc("active", () =>
        {
            Console.WriteLine("active()");
        });
        _env.RegisterFunc<float, string>("tick", (delay, eventName) =>
        {
            Console.WriteLine($"tick({delay}, \"{eventName}\")");
        });
        _env.RegisterFunc<float, string>("wait", (delay, eventName) =>
        {
            Console.WriteLine($"wait({delay}, \"{eventName}\")");
        });
        _env.RegisterFunc<bool>("roller", (enable) =>
        {
            Console.WriteLine($"roller({enable})");
        });

        var s = File.ReadAllText("spider_boss.s2");
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.TypeOf<VoidValue>());

        var r2 = Script2Parser.CallFunc(_env, "onStart1");
        Assert.That(r2, Is.TypeOf<VoidValue>());

        var r3 = Script2Parser.CallFunc(_env, "onStart2");
        Assert.That(r3, Is.EqualTo(2.0f));
    }

    [Test]
    public void TestCallFunc1()
    {
        Script2.Script2 s2 = new Script2.Script2(useInterpreter);

        var s = @"
    returnTwo() {
        return 2
    }

    ";
        s2.Execute(s);
        var r = s2.CallFunc("returnTwo");
        // 显式return null应该成功
        Assert.That(r, Is.EqualTo(2.0f));
    }
}