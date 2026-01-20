using GoFire.Kernel.Script2;
using NUnit.Framework;
using Superpower;

namespace TestProject
{
    public class Script2ParserStepTest
    {
        private Script2Tokenizer _tokenizer;

        [SetUp]
        public void SetUp()
        {
            _tokenizer = TokenizerBuilder.Build();
        }
        [Test]
        public void TestConstant1()
        {
            var tokens = _tokenizer.Tokenize("9.81");
            var r = Script2Parser.Constant.Parse(tokens);
            Assert.AreEqual(r.ToString(), "9.81");
        }

        [Test]
        public void TestParserFuncCall1()
        {
            var tokens = _tokenizer.Tokenize("max(9, 81)");
            var r = Script2Parser.FuncCall.Parse(tokens);
            Debug.Log(r.ToString());
        }

        [Test]
        public void TestParserFactor()
        {
            var tokens = _tokenizer.Tokenize("max(9, 81)");
            var r = Script2Parser.Factor.Parse(tokens);
            Debug.Log(r.ToString());
        }

        [Test]
        public void TestParserSemicolon()
        {
            var tokens = _tokenizer.Tokenize("5;4");
            var r = Script2Parser.Program.Parse(tokens);
            Debug.Log(r.ToString());
        }

        [Test]
        public void TestParserSerVar()
        {
            var tokens = _tokenizer.Tokenize("var a = 1");
            var r = Script2Parser.SetVar.Parse(tokens);
            //foreach (var e in r)
            {
                Debug.Log(r.ToString());
            }
        }
    }
}