using NUnit.Framework;
using Script2;

namespace TestProject;

[TestFixture(false)]
[TestFixture(true)]
public class Script2PrintTest(bool useInterpreter)
{
    [SetUp]
    public void SetUp()
    {
        Script2Parser.UseInterpreterMode = useInterpreter;
    }

    [Test]
    public void TestPrintFunction()
    {
        var env = new Script2Environment();
        var s = @"
print(""Hello, World!"")
";
        var r = Script2Parser.Execute(s, env);
        // print 函数返回 VoidValue.Instance
        Assert.That(r, Is.EqualTo(VoidValue.Instance));
    }

    [Test]
    public void TestPrintWithVariable()
    {
        var env = new Script2Environment();
        var s = @"
var msg = ""Hello""
print(msg, "" world"")
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(VoidValue.Instance));
    }
}