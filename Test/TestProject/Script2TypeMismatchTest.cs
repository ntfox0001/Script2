using NUnit.Framework;
using Script2;

namespace TestProject;

/// <summary>
/// 测试类型不匹配的错误处理
/// </summary>
public class Script2TypeMismatchTest
{
    /// <summary>
    /// 测试 == 运算符 - 数字与字符串比较应该报错
    /// </summary>
    [Test]
    public void TestEqualNumberAndString()
    {
        var env = new Script2Environment();
        var s = @"
var num = 5;
var str = ""hello"";
num == str;
";
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            Script2Parser.Execute(s, env);
        });
        Assert.That(ex.Message, Does.Contain("Type mismatch"));
        Assert.That(ex.Message, Does.Contain("float"));
        Assert.That(ex.Message, Does.Contain("string"));
    }

    /// <summary>
    /// 测试 == 运算符 - 字符串与数字比较应该报错
    /// </summary>
    [Test]
    public void TestEqualStringAndNumber()
    {
        var env = new Script2Environment();
        var s = @"
var str = ""hello"";
var num = 5;
str == num;
";
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            Script2Parser.Execute(s, env);
        });
        Assert.That(ex.Message, Does.Contain("Type mismatch"));
    }

    /// <summary>
    /// 测试 == 运算符 - 数字与布尔值比较应该报错
    /// </summary>
    [Test]
    public void TestEqualNumberAndBoolean()
    {
        var env = new Script2Environment();
        var s = @"
var num = 5;
var boolVal = true;
num == boolVal;
";
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            Script2Parser.Execute(s, env);
        });
        Assert.That(ex.Message, Does.Contain("Type mismatch"));
        Assert.That(ex.Message, Does.Contain("float"));
        Assert.That(ex.Message, Does.Contain("bool"));
    }

    /// <summary>
    /// 测试 == 运算符 - 字符串与布尔值比较应该报错
    /// </summary>
    [Test]
    public void TestEqualStringAndBoolean()
    {
        var env = new Script2Environment();
        var s = @"
var str = ""hello"";
var boolVal = true;
str == boolVal;
";
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            Script2Parser.Execute(s, env);
        });
        Assert.That(ex.Message, Does.Contain("Type mismatch"));
    }

    /// <summary>
    /// 测试 == 运算符 - 布尔值与数字比较应该报错
    /// </summary>
    [Test]
    public void TestEqualBooleanAndNumber()
    {
        var env = new Script2Environment();
        var s = @"
var boolVal = false;
var num = 10;
boolVal == num;
";
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            Script2Parser.Execute(s, env);
        });
        Assert.That(ex.Message, Does.Contain("Type mismatch"));
    }

    /// <summary>
    /// 测试 != 运算符 - 数字与字符串比较应该报错
    /// </summary>
    [Test]
    public void TestNotEqualNumberAndString()
    {
        var env = new Script2Environment();
        var s = @"
var num = 5;
var str = ""hello"";
num != str;
";
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            Script2Parser.Execute(s, env);
        });
        Assert.That(ex.Message, Does.Contain("Type mismatch"));
        Assert.That(ex.Message, Does.Contain("!="));
    }

    /// <summary>
    /// 测试 != 运算符 - 字符串与布尔值比较应该报错
    /// </summary>
    [Test]
    public void TestNotEqualStringAndBoolean()
    {
        var env = new Script2Environment();
        var s = @"
var str = ""test"";
var boolVal = true;
str != boolVal;
";
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            Script2Parser.Execute(s, env);
        });
        Assert.That(ex.Message, Does.Contain("Type mismatch"));
    }

    /// <summary>
    /// 测试 != 运算符 - 数字与布尔值比较应该报错
    /// </summary>
    [Test]
    public void TestNotEqualNumberAndBoolean()
    {
        var env = new Script2Environment();
        var s = @"
var num = 5;
var boolVal = false;
num != boolVal;
";
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            Script2Parser.Execute(s, env);
        });
        Assert.That(ex.Message, Does.Contain("Type mismatch"));
    }

    /// <summary>
    /// 测试 == 运算符 - 常量数字与字符串比较应该报错
    /// </summary>
    [Test]
    public void TestEqualLiteralNumberAndString()
    {
        var env = new Script2Environment();
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            Script2Parser.Execute("5 == \"hello\"", env);
        });
        Assert.That(ex.Message, Does.Contain("Type mismatch"));
    }

    /// <summary>
    /// 测试 == 运算符 - 常量字符串与数字比较应该报错
    /// </summary>
    [Test]
    public void TestEqualLiteralStringAndNumber()
    {
        var env = new Script2Environment();
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            Script2Parser.Execute("\"hello\" == 5", env);
        });
        Assert.That(ex.Message, Does.Contain("Type mismatch"));
    }

    /// <summary>
    /// 测试 == 运算符 - 常量数字与布尔值比较应该报错
    /// </summary>
    [Test]
    public void TestEqualLiteralNumberAndBoolean()
    {
        var env = new Script2Environment();
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            Script2Parser.Execute("5 == true", env);
        });
        Assert.That(ex.Message, Does.Contain("Type mismatch"));
    }

    /// <summary>
    /// 测试 == 运算符 - 常量布尔值与数字比较应该报错
    /// </summary>
    [Test]
    public void TestEqualLiteralBooleanAndNumber()
    {
        var env = new Script2Environment();
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            Script2Parser.Execute("false == 0", env);
        });
        Assert.That(ex.Message, Does.Contain("Type mismatch"));
    }

    /// <summary>
    /// 测试 != 运算符 - 常量数字与字符串比较应该报错
    /// </summary>
    [Test]
    public void TestNotEqualLiteralNumberAndString()
    {
        var env = new Script2Environment();
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            Script2Parser.Execute("5 != \"hello\"", env);
        });
        Assert.That(ex.Message, Does.Contain("Type mismatch"));
    }

    /// <summary>
    /// 测试在 if 语句中使用类型不匹配的比较
    /// </summary>
    [Test]
    public void TestTypeMismatchInIf()
    {
        var env = new Script2Environment();
        var s = @"
var num = 5;
var str = ""hello"";
if (num == str) {
    1;
} else {
    0;
}
";
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            Script2Parser.Execute(s, env);
        });
        Assert.That(ex.Message, Does.Contain("Type mismatch"));
    }

    /// <summary>
    /// 测试在 while 循环中使用类型不匹配的比较
    /// </summary>
    [Test]
    public void TestTypeMismatchInWhile()
    {
        var env = new Script2Environment();
        var s = @"
var num = 0;
var str = ""hello"";
while (num != str) {
    num = num + 1;
}
";
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            Script2Parser.Execute(s, env);
        });
        Assert.That(ex.Message, Does.Contain("Type mismatch"));
    }

    /// <summary>
    /// 测试在函数中使用类型不匹配的比较（两个都是 object 类型，但实际参数类型不同）
    /// </summary>
    [Test]
    public void TestTypeMismatchInFunction()
    {
        var env = new Script2Environment();
        var s = @"
compare(a, b) {
    return a == b;
}
compare(5, ""hello"");
";
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            Script2Parser.Execute(s, env);
        });
        Assert.That(ex.Message, Does.Contain("Type mismatch"));
    }

    /// <summary>
    /// 测试函数参数为 object 类型时能自动转换为数字进行比较
    /// </summary>
    [Test]
    public void TestFunctionParamObjectToNumber()
    {
        var env = new Script2Environment();
        var s = @"
isEqual(n) {
    return n == 5;
}
isEqual(5);
";
        var r = Script2Parser.Execute(s, env);
        // 函数参数 n 是 object 类型，应该能自动转换为 float 与 5 比较
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试函数参数为 object 类型时能自动转换为字符串进行比较
    /// </summary>
    [Test]
    public void TestFunctionParamObjectToString()
    {
        var env = new Script2Environment();
        var s = @"
isEqual(str) {
    return str == ""hello"";
}
isEqual(""hello"");
";
        var r = Script2Parser.Execute(s, env);
        // 函数参数 str 是 object 类型，应该能自动转换为 string 与 "hello" 比较
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试函数参数为 object 类型时能自动转换为布尔值进行比较
    /// </summary>
    [Test]
    public void TestFunctionParamObjectToBoolean()
    {
        var env = new Script2Environment();
        var s = @"
isEqual(b) {
    return b == true;
}
isEqual(true);
";
        var r = Script2Parser.Execute(s, env);
        // 函数参数 b 是 object 类型，应该能自动转换为 bool 与 true 比较
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试函数参数为 object 类型时能自动转换为数字进行不等于比较
    /// </summary>
    [Test]
    public void TestFunctionParamObjectToNumberNotEqual()
    {
        var env = new Script2Environment();
        var s = @"
isNotEqual(n) {
    return n != 5;
}
isNotEqual(10);
";
        var r = Script2Parser.Execute(s, env);
        // 函数参数 n 是 object 类型，应该能自动转换为 float 与 5 比较
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 == 运算符 - 相同类型的数字比较应该成功
    /// </summary>
    [Test]
    public void TestEqualSameTypeNumbers()
    {
        var env = new Script2Environment();
        var s = @"
var a = 5;
var b = 5;
a == b;
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 == 运算符 - 相同类型的字符串比较应该成功
    /// </summary>
    [Test]
    public void TestEqualSameTypeStrings()
    {
        var env = new Script2Environment();
        var s = @"
var a = ""hello"";
var b = ""hello"";
a == b;
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 == 运算符 - 相同类型的布尔值比较应该成功
    /// </summary>
    [Test]
    public void TestEqualSameTypeBooleans()
    {
        var env = new Script2Environment();
        var s = @"
var a = true;
var b = true;
a == b;
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 != 运算符 - 相同类型的数字比较应该成功
    /// </summary>
    [Test]
    public void TestNotEqualSameTypeNumbers()
    {
        var env = new Script2Environment();
        var s = @"
var a = 5;
var b = 10;
a != b;
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 != 运算符 - 相同类型的字符串比较应该成功
    /// </summary>
    [Test]
    public void TestNotEqualSameTypeStrings()
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
    /// 测试 != 运算符 - 相同类型的布尔值比较应该成功
    /// </summary>
    [Test]
    public void TestNotEqualSameTypeBooleans()
    {
        var env = new Script2Environment();
        var s = @"
var a = true;
var b = false;
a != b;
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 > 运算符 - 不同类型可以比较（只检查 == 和 !=）
    /// </summary>
    [Test]
    public void TestGreaterDifferentTypesAllowed()
    {
        var env = new Script2Environment();
        // > 运算符不应该抛出类型不匹配错误
        var s = @"
var num = 5;
var str = ""hello"";
num > 0;
";
        var r = Script2Parser.Execute(s, env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 < 运算符 - 不同类型可以比较（只检查 == 和 !=）
    /// </summary>
    [Test]
    public void TestLessDifferentTypesAllowed()
    {
        var env = new Script2Environment();
        // < 运算符不应该抛出类型不匹配错误
        var r = Script2Parser.Execute("10 < 20", env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 >= 运算符 - 不同类型可以比较（只检查 == 和 !=）
    /// </summary>
    [Test]
    public void TestGreaterEqualDifferentTypesAllowed()
    {
        var env = new Script2Environment();
        // >= 运算符不应该抛出类型不匹配错误
        var r = Script2Parser.Execute("10 >= 10", env);
        Assert.That(r, Is.EqualTo(true));
    }

    /// <summary>
    /// 测试 <= 运算符 - 不同类型可以比较（只检查 == 和 !=）
    /// </summary>
    [Test]
    public void TestLessEqualDifferentTypesAllowed()
    {
        var env = new Script2Environment();
        // <= 运算符不应该抛出类型不匹配错误
        var r = Script2Parser.Execute("5 <= 10", env);
        Assert.That(r, Is.EqualTo(true));
    }
}
