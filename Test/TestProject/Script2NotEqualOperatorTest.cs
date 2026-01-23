using NUnit.Framework;
using Script2;

namespace TestProject;

/// <summary>
/// 测试 != 运算符
/// </summary>
public class Script2NotEqualOperatorTest
{
    /// <summary>
    /// 测试 != - 基本数字比较
    /// </summary>
    [Test]
    public void TestNotEqualBasicNumbers()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("5 != 3", env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 != - 相等数字
    /// </summary>
    [Test]
    public void TestNotEqualEqualNumbers()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("10 != 10", env);
        Assert.That(r, Is.EqualTo(false));
    }

    /// <summary>
    /// 测试 != - 浮点数比较
    /// </summary>
    [Test]
    public void TestNotEqualFloats()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("3.14 != 2.71", env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 != - 字符串比较
    /// </summary>
    [Test]
    public void TestNotEqualStrings()
    {
        var env = new Script2Environment();
        var s = @"
var a = ""hello"";
var b = ""world"";
a != b;
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 != - 相等字符串
    /// </summary>
    [Test]
    public void TestNotEqualEqualStrings()
    {
        var env = new Script2Environment();
        var s = @"
var a = ""test"";
var b = ""test"";
a != b;
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(false));
    }

    /// <summary>
    /// 测试 != - 布尔值比较
    /// </summary>
    [Test]
    public void TestNotEqualBooleans()
    {
        var env = new Script2Environment();
        var r1 = Script2Parser.Execute("true != false", env);
        Assert.That(r1, Is.EqualTo(true));

        var r2 = Script2Parser.Execute("true != true", env);
        Assert.That(r2, Is.EqualTo(false));
    }

    /// <summary>
    /// 测试 != - 在 if 语句中
    /// </summary>
    [Test]
    public void TestNotEqualInIf()
    {
        var env = new Script2Environment();
        var s = @"
var value = 42;
if (value != 100) {
    ""not equal"";
} else {
    ""equal"";
}
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo("not equal"));
    }

    /// <summary>
    /// 测试 != - 在 if else 中
    /// </summary>
    [Test]
    public void TestNotEqualInIfElse()
    {
        var env = new Script2Environment();
        var s = @"
var a = 5;
if (a != 5) {
    1;
} else {
    2;
}
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(2));
    }

    /// <summary>
    /// 测试 != - 在 while 循环条件中
    /// </summary>
    [Test]
    public void TestNotEqualInWhile()
    {
        var env = new Script2Environment();
        var s = @"
var counter = 0;
var target = 5;
while (counter != target) {
    counter = counter + 1;
}
counter;
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(5.0f));
    }

    /// <summary>
    /// 测试 != - 在函数中使用（函数参数是 object 类型）
    /// </summary>
    [Test]
    public void TestNotEqualInFunction()
    {
        var env = new Script2Environment();
        var s = @"
isNotZero(n) {
    return n != 0;
}
isNotZero(5);
";
        var r = Script2Parser.Execute(s, env);
        // 函数参数 n 是 object 类型，应该能自动转换为 float 与 0 比较
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 != - 函数参数为 object 类型与字符串比较
    /// </summary>
    [Test]
    public void TestNotEqualFunctionParamWithObjectToString()
    {
        var env = new Script2Environment();
        var s = @"
isNotHello(str) {
    return str != ""hello"";
}
isNotHello(""world"");
";
        var r = Script2Parser.Execute(s, env);
        // 函数参数 str 是 object 类型，应该能自动转换为 string 与 "hello" 比较
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 != - 函数参数为 object 类型与布尔值比较
    /// </summary>
    [Test]
    public void TestNotEqualFunctionParamWithObjectToBoolean()
    {
        var env = new Script2Environment();
        var s = @"
isNotFalse(b) {
    return b != false;
}
isNotFalse(true);
";
        var r = Script2Parser.Execute(s, env);
        // 函数参数 b 是 object 类型，应该能自动转换为 bool 与 false 比较
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 == - 函数参数为 object 类型与数字比较
    /// </summary>
    [Test]
    public void TestEqualFunctionParamWithObjectToNumber()
    {
        var env = new Script2Environment();
        var s = @"
isFive(n) {
    return n == 5;
}
isFive(5);
";
        var r = Script2Parser.Execute(s, env);
        // 函数参数 n 是 object 类型，应该能自动转换为 float 与 5 比较
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 == - 函数参数为 object 类型与字符串比较
    /// </summary>
    [Test]
    public void TestEqualFunctionParamWithObjectToString()
    {
        var env = new Script2Environment();
        var s = @"
isHello(str) {
    return str == ""hello"";
}
isHello(""hello"");
";
        var r = Script2Parser.Execute(s, env);
        // 函数参数 str 是 object 类型，应该能自动转换为 string 与 "hello" 比较
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 != - 与 and 结合
    /// </summary>
    [Test]
    public void TestNotEqualWithAnd()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("5 != 3 and 10 != 20", env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 != - 与 or 结合
    /// </summary>
    [Test]
    public void TestNotEqualWithOr()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("5 != 5 or 10 != 20", env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 != - 与 not 结合
    /// </summary>
    [Test]
    public void TestNotEqualWithNot()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("not (5 != 5)", env);
        // 5 != 5 返回 false，not false 返回 true
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 != - 与比较运算符混合使用
    /// </summary>
    [Test]
    public void TestNotEqualWithComparisonOperators()
    {
        var env = new Script2Environment();
        var r = Script2Parser.Execute("5 != 3 and 10 > 5", env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 != - 与 == 对比
    /// </summary>
    [Test]
    public void TestNotEqualVersusEqual()
    {
        var env = new Script2Environment();
        var s = @"
var a = 5;
var b = 10;
var notEqualResult = a != b;
var equalResult = a == b;
notEqualResult;
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(true));
        Assert.That(env.GetVariableValue("equalResult"), Is.EqualTo(false));
    }

    /// <summary>
    /// 测试 != - 与变量重新赋值
    /// </summary>
    [Test]
    public void TestNotEqualWithReassignment()
    {
        var env = new Script2Environment();
        var s = @"
var x = 0;
while (x != 5) {
    x = x + 1;
}
x;
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(5.0f));
    }

    /// <summary>
    /// 测试 != - 字符串和数字比较应该报错
    /// </summary>
    [Test]
    public void TestNotEqualStringVsNumber()
    {
        var env = new Script2Environment();
        var s = @"
var str = ""5"";
var num = 5;
str != num;
";
        // 字符串 "5" 和数字 5 类型不同，应该抛出类型不匹配错误
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            Script2Parser.Execute(s, env);
        });
        Assert.That(ex.Message, Does.Contain("Type mismatch"));
    }

    /// <summary>
    /// 测试 != - 复杂逻辑表达式
    /// </summary>
    [Test]
    public void TestNotEqualComplexExpression()
    {
        var env = new Script2Environment();
        var s = @"
var a = 10;
var b = 20;
var c = 30;
if (a != b and b != c and a != c) {
    ""all different"";
} else {
    ""some equal"";
}
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo("all different"));
    }
}
