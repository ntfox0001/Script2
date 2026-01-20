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
            var a = Strings.Keyword(new[] { "var", "if" })(new TextSpan("var a = 1"));
            Assert.That("var", Is.EqualTo(a.Value.ToStringValue()));
        }
        [Test]
        public void Test2()
        {
            var a = Strings.Keyword(new[] { "var", "if" })(new TextSpan("var a = 1"));
            Assert.That("var", Is.EqualTo(a.Value.ToStringValue()));
        }
        [Test]
        public void Test2_1()
        {
            // 关键字都是字母
            var a = Strings.Keyword(new[] { "(", ")" })(new TextSpan("(1,14"));
            Assert.That(false, Is.EqualTo(a.HasValue));
        }
        [Test]
        public void Test3()
        {
            var a = Strings.QuotedString(new TextSpan("\"var a = 1\"aabb"));
            Assert.That("\"var a = 1\"", Is.EqualTo(a.Value.ToStringValue()));
        }
    }
}