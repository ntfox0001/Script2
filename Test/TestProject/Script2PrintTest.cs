using NUnit.Framework;
using Script2;

namespace TestProject;

[TestFixture(false)]
[TestFixture(true)]
public class Script2PrintTest(bool useInterpreter)
{
    private Script2Environment _env;
    [SetUp]
    public void SetUp()
    {
        _env = new Script2Environment(useInterpreter);
    }

    [Test]
    public void TestPrintFunction()
    {
        var s = @"
print(""Hello, World!"")
";
        var r = Script2Parser.Execute(s, _env);
        // print 函数返回 VoidValue.Instance
        Assert.That(r, Is.EqualTo(VoidValue.Instance));
    }

    [Test]
    public void TestPrintWithVariable()
    {
        var s = @"
var msg = ""Hello""
print(msg, "" world"")
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(VoidValue.Instance));
    }
}