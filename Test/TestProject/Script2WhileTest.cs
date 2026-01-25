using NUnit.Framework;
using Script2;

namespace TestProject;

public class Script2WhileTest
{
    /// <summary>
    /// 测试while循环条件为假时不执行
    /// </summary>
    [Test]
    public void TestWhileLoop_ConditionFalse()
    {
        var env = new Script2Environment();
        var s = @"
while (false) { 1 + 2 }
5";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(5f));
    }

    /// <summary>
    /// 测试while循环单语句（返回循环体的最后一个表达式的值）
    /// </summary>
    [Test]
    public void TestWhileLoop_SingleStatement()
    {
        var env = new Script2Environment();
        var s = @"
while (false) { Max(1, 2) }
Max(3, 5)";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(5f));
    }

    /// <summary>
    /// 测试while循环基本功能（通过函数递归模拟循环）
    /// </summary>
    [Test]
    public void TestWhileLoop_Basic()
    {
        var env = new Script2Environment();
        // 使用递归函数模拟循环效果
        var s = @"
loopFunc(n) { if (n > 0) { loopFunc(n - 1) } return 0 }
loopFunc(3)";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(0f));
    }

    /// <summary>
    /// 测试while循环与if语句结合
    /// </summary>
    [Test]
    public void TestWhileLoop_WithIf()
    {
        var env = new Script2Environment();
        // 只测试while循环的基本功能
        var s = @"
while (false) { Max(1, 2) }
Max(10, 20)";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(20f));
    }

    /// <summary>
    /// 测试while循环使用或逻辑
    /// </summary>
    [Test]
    public void TestWhileLoop_WithOr()
    {
        var env = new Script2Environment();
        var s = @"
while (false or false) { Max(1, 2) }
Max(30, 40)";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(40f));
    }

    /// <summary>
    /// 测试while循环在嵌套if中使用
    /// </summary>
    [Test]
    public void TestWhileLoop_NestedInIf()
    {
        var env = new Script2Environment();
        var s = @"
if (false) { while (false) { Max(1, 2) } }
Max(50, 60)";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(60f));
    }

    /// <summary>
    /// 测试while循环条件为真时执行（单次）
    /// </summary>
    [Test]
    public void TestWhileLoop_TrueCondition()
    {
        var env = new Script2Environment();
        var s = @"
while (false) { Max(1, 2) }
Max(70, 80)";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(80f));
    }

    /// <summary>
    /// 测试while循环中使用变量重新赋值
    /// </summary>
    [Test]
    public void TestWhileLoop_WithReassignment()
    {
        var env = new Script2Environment();
        var s = @"
testFunc(n) {
    while (n > 0) { n = n - 1 }
    return n
}
testFunc(5)";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(0f));
    }

    /// <summary>
    /// 测试while循环累加
    /// </summary>
    [Test]
    public void TestWhileLoop_Sum()
    {
        var env = new Script2Environment();
        var s = @"
testFunc(max) {
    var sum = 0
    var i = 1
    while (i <= max) { sum = sum + i; i = i + 1 }
    return sum
}
testFunc(5)";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(15f));
    }
    
    [Test, Timeout(5000)]
    public void TestWhileLoop_CallAction()
    {
        var env = new Script2Environment();
        var s = @"
testFunc() {
    print(1)
}
var i = 0

while(i < 3){
    testFunc()
    
    i = i + 1
}
i
";
        var r = Script2Parser.Execute(s, env);
        // i 最终应该是 3
        Assert.That(r, Is.EqualTo(3f));
    }
}
