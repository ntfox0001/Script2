using NUnit.Framework;
using Script2.Parser;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace TestProject
{
    public class Script2StringParserTest
    {
        [Test]
        public void Test1()
        {
            var a = Strings.ContainsString(new[] { "var", "if" })(new TextSpan("var a = 1"));
            Assert.That("var", Is.EqualTo(a.Value.ToStringValue()));
        }
        [Test]
        public void Test2()
        {
            var a = Strings.ContainsString(new[] { "var", "if" })(new TextSpan("var a = 1"));
            Assert.That("var", Is.EqualTo(a.Value.ToStringValue()));
        }
    }
}