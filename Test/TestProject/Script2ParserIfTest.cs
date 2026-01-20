using Script2;
using NUnit.Framework;

namespace TestProject
{
    public class Script2ParserIfTest
    {
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
            Assert.That(10, Is.EqualTo(r));
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
            Assert.That(5, Is.EqualTo(r));
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
            Assert.That(10, Is.EqualTo(r));
        }

    }
}