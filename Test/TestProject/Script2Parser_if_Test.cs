using Script2;
using NUnit.Framework;

namespace TestProject;

[TestFixture(false)]
[TestFixture(true)]
public class Script2ParserIfTest(bool useInterpreter)
{
    [SetUp]
    public void SetUp()
    {
        Script2Parser.UseInterpreterMode = useInterpreter;
    }

    [Test]
    public void TestIfStatement1()
    {
        var env = new Script2Environment();
        var s = @"
var a = 10;
var b = 5;
if (a > b) {
    a;
} else {
    b;
}
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(10));
    }

    [Test]
    public void TestIfStatement2()
    {
        var env = new Script2Environment();
        var s = @"
var a = 10;
var b = 5;
if (a < b) {
    a;
} else {
    b;
}
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(5));
    }

    [Test]
    public void TestIfWithoutElse()
    {
        var env = new Script2Environment();
        var s = @"
var a = 10;
var b = 5;
if (a > b) {
    a;
}
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(10));
    }

}