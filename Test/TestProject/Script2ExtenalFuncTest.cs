using NUnit.Framework;
using Script2;

namespace TestProject;

[TestFixture(true)]
[TestFixture(false)]
public class Script2ExtenalFuncTest(bool useInterpreter)
{
    private Script2Environment _env;
    [SetUp]
    public void SetUp()
    {
        _env = new Script2Environment
        {
            UseInterpreterMode = useInterpreter
        };
    }

    [Test]
    public void TestMaxFunc1()
    {
        _env.RegisterFunc<int, int, int>("Test1", (arg1, arg2) => arg1 + arg2);
        var r = Script2Parser.Execute("Test1(9, 81)", _env);
        Assert.That(r, Is.EqualTo(90f));
    }

    [Test]
    public void TestMaxFunc2()
    {
        _env.RegisterFunc<float, float, float>("Test1", (arg1, arg2) => arg1 + arg2);
        var r = Script2Parser.Execute("Test1(9, 81)", _env);
        Assert.That(r, Is.EqualTo(90f));
    }

    [Test]
    public void TestMaxFunc3()
    {
        _env.RegisterFunc<bool, bool>("Test1", (arg1) => arg1);
        var s = @"
var a = true;
if (Test1(a)) {
    5;
}
else {
    10;
}
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(5));
    }

    [Test]
    public void TestMaxFunc4()
    {
        _env.RegisterFunc<string, string>("Test1", (arg1) => arg1);
        var s = @"
var a = ""aab"";
if (Test1(a) == ""aab"") {
    5;
}
else {
    10;
}
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(5));
    }
}