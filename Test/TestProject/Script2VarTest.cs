using NUnit.Framework;
using Script2;

namespace TestProject;

public class Script2VarTest
{
    [Test]
    public void TestWhileLoop1()
    {
        var env = new Script2Environment();
        var s = @"
var a = 0;

var i = Max(1, 2) 
a = a + i

return a";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(2));
    }
    
    [Test]
    public void TestWhileLoop2()
    {
        var env = new Script2Environment();
        var s = @"
var a = 0;
a = 1

return a";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(1));
    }
}