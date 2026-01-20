using GoFire.Kernel.Script2;
using NUnit.Framework;

namespace script.Kernel.Script.Tests
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
            Assert.AreEqual(10, r);
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
            Assert.AreEqual(5, r);
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
            Assert.AreEqual(10, r);
        }

    }
}