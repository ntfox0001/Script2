using NUnit.Framework;
using Script2;

namespace TestProject;

[TestFixture(true)]
[TestFixture(false)]
public class Script2CallFuncTest(bool useInterpreter)
{
    private Script2Environment _env;
    [SetUp]
    public void SetUp()
    {
        _env = new Script2Environment(useInterpreter);
    }

    [Test]
    public void TestCallFunc_WithNoArgs()
    {
        _env.RegisterFunc("TestFunc", () => 42);
        var result = Script2Parser.CallFunc(_env, "TestFunc");
        Assert.That(result, Is.EqualTo(42f));
    }

    [Test]
    public void TestCallFunc_WithSingleArg()
    {
        _env.RegisterFunc<int, int>("TestFunc", (arg) => arg * 2);
        var result = Script2Parser.CallFunc(_env, "TestFunc", 21);
        Assert.That(result, Is.EqualTo(42f));
    }

    [Test]
    public void TestCallFunc_WithMultipleArgs()
    {
        _env.RegisterFunc<int, int, int>("TestFunc", (a, b) => a + b);
        var result = Script2Parser.CallFunc(_env, "TestFunc", 10, 32);
        Assert.That(result, Is.EqualTo(42f));
    }

    [Test]
    public void TestCallFunc_WithStringArgs()
    {
        _env.RegisterFunc<string, string, string>("TestFunc", (a, b) => a + b);
        var result = Script2Parser.CallFunc(_env, "TestFunc", "Hello, ", "World!");
        Assert.That(result, Is.EqualTo("Hello, World!"));
    }

    [Test]
    public void TestCallFunc_WithBoolArg()
    {
        _env.RegisterFunc<bool, string>("TestFunc", (arg) => arg ? "true" : "false");
        var result = Script2Parser.CallFunc(_env, "TestFunc", true);
        Assert.That(result, Is.EqualTo("true"));
    }

    [Test]
    public void TestCallFunc_WithMixedArgTypes()
    {
        _env.RegisterFunc<string, int, bool>("TestFunc", (name, age) => age > 18);
        var result = Script2Parser.CallFunc(_env, "TestFunc", "John", 20);
        Assert.That(result, Is.EqualTo(true));
    }

    [Test]
    public void TestCallFunc_WithNullArg()
    {
        _env.RegisterFunc<object, string>("TestFunc", (arg) => arg == null ? "null" : "not null");
        Assert.Catch<ArgumentNullException>(() =>
        {
            var result = Script2Parser.CallFunc(_env, "TestFunc", null);
        });
    }

    [Test]
    public void TestCallFunc_AfterEnvironmentVariableSet()
    {
        _env.RegisterFunc<int, int>("TestFunc", (arg) => arg * 2);
        _env.SetVariableValue("value", 21);
        var result = Script2Parser.CallFunc(_env, "TestFunc", _env.GetVariableValue("value"));
        Assert.That(result, Is.EqualTo(42f));
    }
}