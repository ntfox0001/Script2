using NUnit.Framework;
using Script2;

namespace TestProject;

public class Script2ReturnTest
{
    /// <summary>
    /// 测试 return 立即退出函数，不执行后续语句
    /// </summary>
    [Test]
    public void TestReturnEarlyExit()
    {
        var env = new Script2Environment();
        var s = @"
            test() {
                return 1;
                var x = 999;
                return x;
            }
            test();
        ";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(1.0f));
    }

    /// <summary>
    /// 测试没有 return 语句的函数返回 void
    /// </summary>
    [Test]
    public void TestNoReturnNoValue()
    {
        var env = new Script2Environment();
        var s = @"
            test() {
                var x = 1;
            }
            test();
        ";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.TypeOf<VoidValue>());
    }

    /// <summary>
    /// 测试条件 return - 大数情况
    /// </summary>
    [Test]
    public void TestConditionalReturnLarge()
    {
        var env = new Script2Environment();
        var s = @"
            test(n) {
                if (n > 5) {
                    return 1;
                }
                return 0;
            }
            test(10);
        ";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(1.0f));
    }

    /// <summary>
    /// 测试条件 return - 小数情况
    /// </summary>
    [Test]
    public void TestConditionalReturnSmall()
    {
        var env = new Script2Environment();
        var s = @"
            test(n) {
                if (n > 5) {
                    return 1;
                }
                return 0;
            }
            test(3);
        ";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(0.0f));
    }

    /// <summary>
    /// 测试脚本中return - return 后的代码不执行
    /// </summary>
    [Test]
    public void TestReturn1()
    {
        var env = new Script2Environment();
        var s = @"
            return 1
            2
        ";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(1.0f));
    }

    /// <summary>
    /// 测试脚本中 return 在 if 语句块内
    /// </summary>
    [Test]
    public void TestReturnInIfInScript()
    {
        var env = new Script2Environment();
        var s = @"
            if (3 > 2) {
                return 1
            }
            2
        ";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(1.0f));
    }

    /// <summary>
    /// 测试脚本中 return 在 else 分支内
    /// </summary>
    [Test]
    public void TestReturnInElseInScript()
    {
        var env = new Script2Environment();
        var s = @"
            if (1 > 2) {
                3
            } else {
                return 1
            }
            2
        ";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(1.0f));
    }

    /// <summary>
    /// 测试脚本中 return 在嵌套 if 内
    /// </summary>
    [Test]
    public void TestReturnInNestedIfInScript()
    {
        var env = new Script2Environment();
        var s = @"
            if (1 > 0) {
                if (2 > 1) {
                    return 100
                }
            }
            2
        ";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(100.0f));
    }

    /// <summary>
    /// 测试脚本中 return null
    /// </summary>
    [Test]
    public void TestReturnNullInScript()
    {
        var env = new Script2Environment();
        var s = @"
            return null
            2
        ";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.Null);
    }

    /// <summary>
    /// 测试脚本中没有 return 语句
    /// </summary>
    [Test]
    public void TestNoReturnInScript()
    {
        var env = new Script2Environment();
        var s = @"
            var x = 1
            var y = 2
            x + y
        ";
        var r = Script2Parser.Execute(s, env);
        // 没有return，返回最后一个表达式的值
        Assert.That(r, Is.EqualTo(3.0f));
    }

    /// <summary>
    /// 测试脚本中 return 表达式结果
    /// </summary>
    [Test]
    public void TestReturnExpressionInScript()
    {
        var env = new Script2Environment();
        var s = @"
            return 5 * 10 + 3
            999
        ";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(53.0f));
    }

    /// <summary>
    /// 测试 return 后面的代码不会执行
    /// </summary>
    [Test]
    public void TestReturnSkipsSubsequentCode()
    {
        var env = new Script2Environment();
        var s = @"
            test() {
                var x = 1;
                if (x == 1) {
                    return 100;
                }
                var y = 999;
                return y;
            }
            test();
        ";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(100.0f));
    }

    /// <summary>
    /// 测试嵌套 if 中的 return
    /// </summary>
    [Test]
    public void TestNestedIfReturn()
    {
        var env = new Script2Environment();
        var s = @"
            test(a, b, c) {
                if (a > 0) {
                    if (b > 0) {
                        return a + b + c;
                    }
                    return a;
                }
                return c;
            }
            test(1, 2, 3);
        ";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(6.0f));
    }

    /// <summary>
    /// 测试函数末尾的 return（正常返回）
    /// </summary>
    [Test]
    public void TestReturnAtEndOfFunction()
    {
        var env = new Script2Environment();
        var s = @"
            test(a, b) {
                var sum = a + b;
                return sum;
            }
            test(3, 7);
        ";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(10.0f));
    }

    /// <summary>
    /// 测试多个 return 语句在同一个函数中
    /// </summary>
    [Test]
    public void TestMultipleReturns()
    {
        var env = new Script2Environment();
        var s = @"
            test(x) {
                if (x == 1) {
                    return 10;
                }
                if (x == 2) {
                    return 20;
                }
                if (x == 3) {
                    return 30;
                }
                return 0;
            }
            test(2);
        ";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(20.0f));
    }

    /// <summary>
    /// 测试 return 表达式的结果
    /// </summary>
    [Test]
    public void TestReturnExpression()
    {
        var env = new Script2Environment();
        var s = @"
            test(a, b) {
                return a * 2 + b * 3;
            }
            test(5, 10);
        ";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(40.0f));
    }

    /// <summary>
    /// 测试 else 分支中的 return
    /// </summary>
    [Test]
    public void TestReturnInElse()
    {
        var env = new Script2Environment();
        var s = @"
            test(x) {
                if (x > 10) {
                    return 100;
                } else {
                    return 200;
                }
            }
            test(5);
        ";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(200.0f));
    }

    /// <summary>
    /// 测试没有 return 语句但函数体不为空
    /// </summary>
    [Test]
    public void TestFunctionWithoutReturnHasStatements()
    {
        var env = new Script2Environment();
        var s = @"
            test() {
                var a = 1;
                var b = 2;
                a + b;
            }
            test();
        ";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.TypeOf<VoidValue>());
    }
}
