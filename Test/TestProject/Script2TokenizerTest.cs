using System.Linq;
using GoFire.Kernel.Script2;
using NUnit.Framework;
using Superpower;

namespace TestProject
{
    public class Script2TokenizerTest
    {
        private Script2Tokenizer _tokenizer;

        [SetUp]
        public void SetUp()
        {
            _tokenizer = TokenizerBuilder.Build();
        }

        [Test]
        public void TestConstNumber1()
        {
            var tokens = _tokenizer.Tokenize("9");
            var sample = new[] { "9" };
            Assert.AreEqual(sample, tokens.Select(x => x.ToStringValue()).ToArray());
        }

        [Test]
        public void TestConstString1()
        {
            var tokens = _tokenizer.Tokenize(@"""aab""");
            var sample = new[] { @"""aab""" };
            Assert.AreEqual(sample, tokens.Select(x => x.ToStringValue()).ToArray());
        }

        [Test]
        public void TestConstString2()
        {
            Assert.Catch<ParseException>(() => { _tokenizer.Tokenize(@"""aab"); });
        }

        [Test]
        public void TestFuncCall1()
        {
            var tokens = _tokenizer.Tokenize("max(9, 81)");
            var sample = new[] { "max", "(", "9", ",", "81", ")" };
            Assert.AreEqual(sample, tokens.Select(x => x.ToStringValue()).ToArray());
        }

        [Test]
        public void TestFuncCall2()
        {
            var tokens = _tokenizer.Tokenize(@"max(""aab"", 81)");
            var sample = new[] { "max", "(", @"""aab""", ",", "81", ")" };
            Assert.AreEqual(sample, tokens.Select(x => x.ToStringValue()).ToArray());
        }

        [Test]
        public void TestFuncCall3()
        {
            var tokens = _tokenizer.Tokenize("_max(9, 81, 3)");
            var sample = new[] { "_max", "(", "9", ",", "81", ",", "3", ")" };
            Assert.AreEqual(sample, tokens.Select(x => x.ToStringValue()).ToArray());
        }

        [Test]
        public void TestFuncCall4()
        {
            var tokens = _tokenizer.Tokenize("max_a(9, 81, 3)");
            var sample = new[] { "max_a", "(", "9", ",", "81", ",", "3", ")" };
            Assert.AreEqual(sample, tokens.Select(x => x.ToStringValue()).ToArray());
        }

        [Test]
        public void TestOperator1()
        {
            var tokens = _tokenizer.Tokenize("9 + 81 - 3 * 522 / 2");
            var sample = new[] { "9", "+", "81", "-", "3", "*", "522", "/", "2" };
            Assert.AreEqual(sample, tokens.Select(x => x.ToStringValue()).ToArray());
        }

        [Test]
        public void TestConstFloat1()
        {
            var tokens = _tokenizer.Tokenize("9.81");
            var sample = new[] { "9.81" };
            Assert.AreEqual(sample, tokens.Select(x => x.ToStringValue()).ToArray());
        }

        [Test]
        public void TestTokenizer1()
        {
            var tokens = _tokenizer.Tokenize("var a = 1");
            var sample = new[] { "var", "a", "=", "1" };
            foreach (var t in tokens)
            {
                Debug.Log($"{t.Span.ToStringValue()} -> {t.Kind.ToString()}");
            }
            Assert.AreEqual(sample, tokens.Select(x => x.ToStringValue()).ToArray());
        }

        [Test]
        public void TestDebugMaxVar()
        {
            var s = "5;";
            var tokens = _tokenizer.Tokenize(s);
            Debug.Log("=== Tokens' ===");
            for (int i = 0; i < tokens.Count(); i++)
            {
                var t = tokens.ElementAt(i);
                Debug.Log($"[{i}] {t.Kind}: '{t.Span.ToStringValue()}'");
            }

            try
            {
                var r = Script2Parser.Factor.Parse(tokens);
                Debug.Log($"=== Success: {r} ===");
            }
            catch (ParseException ex)
            {
                Debug.LogError($"=== Error ===");
                Debug.LogError($"Message: {ex.Message}");
                Debug.LogError($"Position: Line {ex.ErrorPosition.Line}, Column {ex.ErrorPosition.Column}");
                throw;
            }
        }
    }


}