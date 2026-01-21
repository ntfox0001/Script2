using NUnit.Framework;
using Script2;

namespace TestProject;

public class Script2FuncDeclTest
{
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
                Assert.That(r, Is.EqualTo(13.0f));
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
                Assert.That(r, Is.EqualTo(6.0f));
            }
}