using NUnit.Framework;
using Script2;

namespace TestProject;

public class Script2ExtenalFuncTest
{
    [Test]
    public void TestMaxFunc1()
    {
        var env = new Script2Environment();
        env.RegisterFunc<int, int, int>("Test1", (arg1, arg2) => arg1 + arg2);
        var r = Script2Parser.Execute("Test1(9, 81)", env);
        Assert.That(r, Is.EqualTo(90f));
    }

    [Test]
    public void TestMaxFunc2()
    {
        var env = new Script2Environment();
        env.RegisterFunc<float, float, float>("Test1", (arg1, arg2) => arg1 + arg2);
        var r = Script2Parser.Execute("Test1(9, 81)", env);
        Assert.That(r, Is.EqualTo(90f));
    }

    [Test]
    public void TestMaxFunc3()
    {
        var env = new Script2Environment();
        env.RegisterFunc<bool, bool>("Test1", (arg1) => arg1);
        var s = @"
var a = true;
if (Test1(a)) {
    5;
}
else {
    10;
}
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(5));
    }

    [Test]
    public void TestMaxFunc4()
    {
        var env = new Script2Environment();
        env.RegisterFunc<string, string>("Test1", (arg1) => arg1);
        var s = @"
var a = ""aab"";
if (Test1(a) == ""aab"") {
    5;
}
else {
    10;
}
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(5));
    }
}