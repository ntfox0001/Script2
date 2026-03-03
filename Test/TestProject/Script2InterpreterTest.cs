using NUnit.Framework;
using Script2;

namespace TestProject
{
    [TestFixture]
    public class Script2InterpreterTest
    {
        private Script2Environment _env;

        [SetUp]
        public void Setup()
        {
            _env = new Script2Environment();
        }

        [TearDown]
        public void TearDown()
        {
            // 确保恢复默认模式
            Script2Parser.UseInterpreterMode = false;
        }

        [Test]
        public void TestInterpreter_SimpleArithmetic()
        {
            // 使用解释器模式
            Script2Parser.UseInterpreterMode = true;

            var result = Script2Parser.Execute("3 + 5 * 2", _env);
            Assert.That(result, Is.EqualTo(13.0f));
        }

        [Test]
        public void TestInterpreter_Variables()
        {
            Script2Parser.UseInterpreterMode = true;

            Script2Parser.Execute("var x = 10", _env);
            Script2Parser.Execute("var y = 20", _env);
            var result = Script2Parser.Execute("x + y", _env);

            Assert.That(result, Is.EqualTo(30.0f));
        }

        [Test]
        public void TestInterpreter_FunctionDeclaration()
        {
            Script2Parser.UseInterpreterMode = true;

            Script2Parser.Execute(@"
add(a, b) {
    return a + b;
}
", _env);

            var result = Script2Parser.Execute("add(3, 5)", _env);
            Assert.That(result, Is.EqualTo(8.0f));
        }

        [Test]
        public void TestInterpreter_Comparison()
        {
            Script2Parser.UseInterpreterMode = true;

            var result1 = Script2Parser.Execute("5 > 3", _env);
            Assert.That(result1, Is.EqualTo(true));

            var result2 = Script2Parser.Execute("5 == 5", _env);
            Assert.That(result2, Is.EqualTo(true));

            var result3 = Script2Parser.Execute("5 != 3", _env);
            Assert.That(result3, Is.EqualTo(true));
        }

        [Test]
        public void TestInterpreter_Logical()
        {
            Script2Parser.UseInterpreterMode = true;

            var result1 = Script2Parser.Execute("true and false", _env);
            Assert.That(result1, Is.EqualTo(false));

            var result2 = Script2Parser.Execute("true or false", _env);
            Assert.That(result2, Is.EqualTo(true));

            var result3 = Script2Parser.Execute("not true", _env);
            Assert.That(result3, Is.EqualTo(false));
        }

        [Test]
        public void TestInterpreter_IfElse()
        {
            Script2Parser.UseInterpreterMode = true;

            Script2Parser.Execute(@"
test(x) {
    if (x > 5) {
        return 10;
    } else {
        return 20;
    }
}
", _env);

            var result1 = Script2Parser.Execute("test(3)", _env);
            Assert.That(result1, Is.EqualTo(20.0f));

            var result2 = Script2Parser.Execute("test(10)", _env);
            Assert.That(result2, Is.EqualTo(10.0f));
        }

        [Test]
        public void TestInterpreter_WhileLoop()
        {
            Script2Parser.UseInterpreterMode = true;

            Script2Parser.Execute(@"
sum(n) {
    var total = 0;
    var i = 1;
    while (i <= n) {
        total = total + i;
        i = i + 1;
    }
    return total;
}
", _env);

            var result = Script2Parser.Execute("sum(5)", _env);
            Assert.That(result, Is.EqualTo(15.0f));
        }

        [Test]
        public void TestInterpreter_Closure()
        {
            Script2Parser.UseInterpreterMode = true;

            Script2Parser.Execute("var x = 10", _env);
            Script2Parser.Execute(@"
addX(a) {
    return a + x;
}
", _env);

            var result = Script2Parser.Execute("addX(5)", _env);
            Assert.That(result, Is.EqualTo(15.0f));
        }

        [Test]
        public void TestInterpreter_Recursion()
        {
            Script2Parser.UseInterpreterMode = true;

            Script2Parser.Execute(@"
fib(n) {
    if (n <= 1) {
        return n;
    }
    return fib(n - 1) + fib(n - 2);
}
", _env);

            var result = Script2Parser.Execute("fib(6)", _env);
            Assert.That(result, Is.EqualTo(8.0f));
        }

        [Test]
        public void TestInterpreter_MathFunctions()
        {
            Script2Parser.UseInterpreterMode = true;

            var result1 = Script2Parser.Execute("Abs(-5)", _env);
            Assert.That(result1, Is.EqualTo(5.0f));

            var result2 = Script2Parser.Execute("Sqrt(16)", _env);
            Assert.That(result2, Is.EqualTo(4.0f));

            var result3 = Script2Parser.Execute("Pow(2, 3)", _env);
            Assert.That(result3, Is.EqualTo(8.0f));
        }

        [Test]
        public void TestInterpreter_BuiltInConstants()
        {
            Script2Parser.UseInterpreterMode = true;

            var result1 = Script2Parser.Execute("PI", _env);
            Assert.That(result1, Is.EqualTo(3.1415926f));

            var result2 = Script2Parser.Execute("E", _env);
            Assert.That(result2, Is.EqualTo(2.71828f));
        }

        [Test]
        public void TestInterpreter_CompiledModeEquivalence()
        {
            // 测试编译模式和解释器模式的结果是否一致

            // 编译模式
            Script2Parser.UseInterpreterMode = false;
            var compiledSum = 0f;
            Script2Parser.Execute(@"
sum(n) {
    var total = 0;
    var i = 1;
    while (i <= n) {
        total = total + i;
        i = i + 1;
    }
    return total;
}
", _env);
            compiledSum = (float)Script2Parser.Execute("sum(10)", _env);

            // 解释器模式
            _env = new Script2Environment();
            Script2Parser.UseInterpreterMode = true;
            Script2Parser.Execute(@"
sum(n) {
    var total = 0;
    var i = 1;
    while (i <= n) {
        total = total + i;
        i = i + 1;
    }
    return total;
}
", _env);
            var interpretedSum = (float)Script2Parser.Execute("sum(10)", _env);

            Assert.That(compiledSum, Is.EqualTo(interpretedSum));
        }

        [Test]
        public void TestInterpreter_StringConcat()
        {
            Script2Parser.UseInterpreterMode = true;

            var result = Script2Parser.Execute("concat(\"Hello\", \" \", \"World\")", _env);
            Assert.That(result, Is.EqualTo("Hello World"));
        }

        [Test]
        public void TestInterpreter_StringFormat()
        {
            Script2Parser.UseInterpreterMode = true;

            var result1 = Script2Parser.Execute("format(\"Score: {0}\", 95)", _env);
            Assert.That(result1, Is.EqualTo("Score: 95"));

            var result2 = Script2Parser.Execute("format(\"{0} + {1} = {2}\", 2, 3, 5)", _env);
            Assert.That(result2, Is.EqualTo("2 + 3 = 5"));
        }

        [Test]
        public void TestInterpreter_Modulo()
        {
            Script2Parser.UseInterpreterMode = true;

            var result = Script2Parser.Execute("10 % 3", _env);
            Assert.That(result, Is.EqualTo(1.0f));
        }
    }
}
