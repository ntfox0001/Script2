using NUnit.Framework;
using Script2;

namespace TestProject;

public class Script2CallFuncTest
{
    [Test]
    public void TestCallFunc_WithNoArgs()
    {
        var env = new Script2Environment();
        env.RegisterFunc("TestFunc", () => 42);
        var result = Script2Parser.CallFunc(env, "TestFunc");
        Assert.That(result, Is.EqualTo(42f));
    }

    [Test]
    public void TestCallFunc_WithSingleArg()
    {
        var env = new Script2Environment();
        env.RegisterFunc<int, int>("TestFunc", (arg) => arg * 2);
        var result = Script2Parser.CallFunc(env, "TestFunc", 21);
        Assert.That(result, Is.EqualTo(42f));
    }

    [Test]
    public void TestCallFunc_WithMultipleArgs()
    {
        var env = new Script2Environment();
        env.RegisterFunc<int, int, int>("TestFunc", (a, b) => a + b);
        var result = Script2Parser.CallFunc(env, "TestFunc", 10, 32);
        Assert.That(result, Is.EqualTo(42f));
    }

    [Test]
    public void TestCallFunc_WithStringArgs()
    {
        var env = new Script2Environment();
        env.RegisterFunc<string, string, string>("TestFunc", (a, b) => a + b);
        var result = Script2Parser.CallFunc(env, "TestFunc", "Hello, ", "World!");
        Assert.That(result, Is.EqualTo("Hello, World!"));
    }

    [Test]
    public void TestCallFunc_WithBoolArg()
    {
        var env = new Script2Environment();
        env.RegisterFunc<bool, string>("TestFunc", (arg) => arg ? "true" : "false");
        var result = Script2Parser.CallFunc(env, "TestFunc", true);
        Assert.That(result, Is.EqualTo("true"));
    }

    [Test]
    public void TestCallFunc_WithMixedArgTypes()
    {
        var env = new Script2Environment();
        env.RegisterFunc<string, int, bool>("TestFunc", (name, age) => age > 18);
        var result = Script2Parser.CallFunc(env, "TestFunc", "John", 20);
        Assert.That(result, Is.EqualTo(true));
    }

    [Test]
    public void TestCallFunc_WithNullArg()
    {
        var env = new Script2Environment();
        env.RegisterFunc<object, string>("TestFunc", (arg) => arg == null ? "null" : "not null");
        Assert.Catch<ArgumentNullException>(() =>
        {
            var result = Script2Parser.CallFunc(env, "TestFunc", null);
        });
    }

    [Test]
    public void TestCallFunc_AfterEnvironmentVariableSet()
    {
        var env = new Script2Environment();
        env.RegisterFunc<int, int>("TestFunc", (arg) => arg * 2);
        env.SetVariableValue("value", 21);
        var result = Script2Parser.CallFunc(env, "TestFunc", env.GetVariableValue("value"));
        Assert.That(result, Is.EqualTo(42f));
    }
}