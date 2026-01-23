using NUnit.Framework;
using Script2;

namespace TestProject;

public class Script2StringTest
{
    [Test]
    public void TestString1()
    {
        var env = new Script2Environment();
        var s = @"
    add(a, b) {
        return a
    }
    add(""a"", 3)
    ";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo("a"));
    }
    
    [Test]
    public void TestString2()
    {
        var env = new Script2Environment();
        var s = @"
    var a= ""eee""
    ";
        var r = Script2Parser.Execute(s, env);
        Assert.That(env.GetVariableValue("a"), Is.EqualTo("eee"));
    }
    
    [Test]
    public void TestString3()
    {
        var env = new Script2Environment();
        var s = @"
    var a= ""eee""
    a = ""dd""
    ";
        var r = Script2Parser.Execute(s, env);
        Assert.That(env.GetVariableValue("a"), Is.EqualTo("dd"));
    }
}