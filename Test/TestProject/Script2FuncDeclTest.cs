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
    add() { return 5 }
    ";
                try
                {
                    var r = Script2Parser.Execute(s, env);
                    Assert.That(r, Is.Null);
                    Assert.That(env.HasFunction("add"), Is.True, "Function 'add' should be registered");
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
    add(a, b) { return a + b }
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
    var x = 22
    var y = 11
    add(a, b) {
        var x = a * 2
        var y = b * 3
        return x + y
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
    sum(a, b, c) { return a + b + c }
    sum(1, 2, 3)
    ";
                var r = Script2Parser.Execute(s, env);
                Assert.That(r, Is.EqualTo(6.0f));
            }

            /// <summary>
            /// 测试函数显式return语句
            /// </summary>
            [Test]
            public void TestFuncWithReturn()
            {
                var env = new Script2Environment();
                var s = @"
    add(a, b) {
        return a + b
    }
    add(3, 5)
    ";
                var r = Script2Parser.Execute(s, env);
                Assert.That(r, Is.EqualTo(8.0f));
            }

            /// <summary>
            /// 测试没有return语句的函数（返回void）
            /// </summary>
            [Test]
            public void TestFuncWithoutReturn()
            {
                var env = new Script2Environment();
                var s = @"
    printHello() {
        5
    }
    printHello()
    ";
                var r = Script2Parser.Execute(s, env);
                // 没有return语句，应该返回void标记值
                Assert.That(r, Is.TypeOf<VoidValue>());
            }

            /// <summary>
            /// 测试return 0（返回0，不是void）
            /// </summary>
            [Test]
            public void TestReturnZero()
            {
                var env = new Script2Environment();
                var s = @"
    returnZero() {
        return 0
    }
    returnZero()
    ";
                var r = Script2Parser.Execute(s, env);
                // 显式return 0应该成功
                Assert.That(r, Is.EqualTo(0.0f));
            }

            /// <summary>
            /// 测试return null（显式返回null，不是void）
            /// </summary>
            [Test]
            public void TestReturnNull()
            {
                var env = new Script2Environment();
                var s = @"
    returnNull() {
        return null
    }
    returnNull()
    ";
                var r = Script2Parser.Execute(s, env);
                // 显式return null应该成功
                Assert.That(r, Is.Null);
            }
    }