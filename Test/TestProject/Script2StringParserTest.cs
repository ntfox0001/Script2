using NUnit.Framework;
using GoFire.Kernel.Script2.Parser;
using Superpower.Model;

namespace TestProject
{
    public class Script2StringParserTest
    {
        [Test]
        public void Test1()
        {
            var a = Strings.ContainsString(new[] { "var", "if" })(new TextSpan("var a = 1"));
            Assert.AreEqual("var", a.Value.ToStringValue());
        }
    }
}