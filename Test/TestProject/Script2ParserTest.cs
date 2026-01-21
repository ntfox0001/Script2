using NUnit.Framework;
using Script2;

public class Script2ParserTest
    {
        /// <summary>
        /// 测试内建函数调用
        /// </summary>
        [Test]
        public void TestLambda1()
        {
            var env = new Script2Environment();
            var r = Script2Parser.Execute("Max(9, 81)", env);
            Assert.That(r, Is.EqualTo(81));
        }

        /// <summary>
        /// 测试变量声明，函数调用，表达式计算
        /// </summary>
        [Test]
        public void TestLambda2()
        {
            var env = new Script2Environment();
            var s = @"
var a = 9
var b = Max(3, a)
b + 2
";
            var r = Script2Parser.Execute(s, env);
            Assert.That(11, Is.EqualTo(r));
        }
        /// <summary>
        /// 测试变量声明，函数调用，表达式计算 带有分号
        /// </summary>
        [Test]
        public void TestLambda3()
        {
            var env = new Script2Environment();
            var s = @"
var a = 9;
var b = Max(3, a);
b + 2;
";
            var r = Script2Parser.Execute(s, env);
            Assert.That(11, Is.EqualTo(r));
        }
        /// <summary>
        /// 测试常量带有分号
        /// </summary>
        [Test]
        public void TestLambda4()
        {
            var env = new Script2Environment();
            var s = @"5;";
            var r = Script2Parser.Execute(s, env);
            Assert.That(5, Is.EqualTo(r));
        }
        /// <summary>
        /// 测试常量带有分号
        /// </summary>
        [Test]
        public void TestLambda5()
        {
            var env = new Script2Environment();
            var s = @"5;6";
            var r = Script2Parser.Execute(s, env);
            Assert.That(6, Is.EqualTo(r));
        }
        [Test]
        public void TestLambda7()
        {
            var env = new Script2Environment();
            var s = @"
var a = Max(3, 5)+3
";
            var r = Script2Parser.Execute(s, env);
            Assert.That(8, Is.EqualTo(r));
        }
        
        /// <summary>
        /// 测试变量声明，函数调用，表达式计算 带有分号（部分）
        /// </summary>
        [Test]
        public void TestLambda8()
        {
            var env = new Script2Environment();
            var s = @"
var a = 9
var b = Max(3, a);
b + 2
";
            var r = Script2Parser.Execute(s, env);
            Assert.That(11, Is.EqualTo(r));
        }
        
        /// <summary>
        /// 测试函数嵌套
        /// </summary>
        [Test]
        public void TestLambda9()
        {
            var env = new Script2Environment();
            var s = @"
var a = Max(3, Min(4, 1))+3
";
            var r = Script2Parser.Execute(s, env);
            Assert.That(6, Is.EqualTo(r));
        }

        /// <summary>
        /// 测试函数声明和调用
        /// </summary>
        [Test]
        public void TestFuncDeclaration()
        {
            var env = new Script2Environment();
            var s = @"
add() { 5 }
";
            try
            {
                var r = Script2Parser.Execute(s, env);
                Assert.That(r, Is.Null);
                Assert.That(env.Functions.ContainsKey("add"), Is.True, "Function 'add' should be registered");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 测试函数调用（使用已声明的函数）
        /// </summary>
        [Test]
        public void TestFuncCall()
        {
            var env = new Script2Environment();
            var s1 = @"
add(a, b) { a + b }
add(3, 5)
";
            var r = Script2Parser.Execute(s1, env);
            Assert.That(r, Is.EqualTo(8.0f));
        }

        /// <summary>
        /// 测试函数声明和调用 - 使用内部变量
        /// </summary>
        [Test]
        public void TestFuncDeclarationWithVars()
        {
            var env = new Script2Environment();
            var s = @"
add(a, b) {
    var x = a * 2
    var y = b * 3
    x + y
}
add(2, 3)
";
            var r = Script2Parser.Execute(s, env);
            Assert.That(13.0f, Is.EqualTo(r));
        }

        /// <summary>
        /// 测试函数声明和调用 - 多参数
        /// </summary>
        [Test]
        public void TestFuncDeclarationMultiArgs()
        {
            var env = new Script2Environment();
            var s = @"
sum(a, b, c) { a + b + c }
sum(1, 2, 3)
";
            var r = Script2Parser.Execute(s, env);
            Assert.That(6.0f, Is.EqualTo(r));
        }
    }