using NUnit.Framework;
using Script2.Parser;
using Superpower.Model;

namespace TestProject
{
    public class Script2StringParserTest
    {
        [Test]
        public void Test1()
        {
            var a = Strings.Keyword(new[] { "var", "if" })(new TextSpan("var a = 1"));
            Assert.That(a.Value.ToStringValue(), Is.EqualTo("var"));
        }
        [Test]
        public void Test2()
        {
            var a = Strings.Keyword(new[] { "var", "if" })(new TextSpan("var a = 1"));
            Assert.That(a.Value.ToStringValue(), Is.EqualTo("var"));
        }
        [Test]
        public void Test2_1()
        {
            // 关键字都是字母
            var a = Strings.Keyword(new[] { "(", ")" })(new TextSpan("(1,14"));
            Assert.That(a.HasValue, Is.False);
        }
        [Test]
        public void Test3()
        {
            var a = Strings.QuotedString(new TextSpan("\"var a = 1\"aabb"));
            Assert.That(a.Value.ToStringValue(), Is.EqualTo("\"var a = 1\""));
        }
    }
}