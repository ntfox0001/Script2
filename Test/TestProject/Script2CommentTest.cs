using NUnit.Framework;
using Script2;

namespace TestProject;

[TestFixture(false)]
[TestFixture(true)]
public class Script2CommentTest(bool useInterpreter)
{
    private Script2Environment _env;
    [SetUp]
    public void SetUp()
    {
        _env = new Script2Environment(useInterpreter);
    }

    /// <summary>
    /// 测试单行注释 - 基本功能
    /// </summary>
    [Test]
    public void TestSingleLineComment()
    {
        var s = @"
    var x = 5 // 这是注释
    var y = 10 // 这也是注释
    x + y
    ";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(15.0f));
    }

    /// <summary>
    /// 测试注释在代码行首
    /// </summary>
    [Test]
    public void TestCommentAtLineStart()
    {
        var s = @"
    // 注释行1
    // 注释行2
    var x = 10
    // 注释行3
    x
    ";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(10.0f));
    }

    /// <summary>
    /// 测试注释在表达式中间
    /// </summary>
    [Test]
    public void TestCommentInExpression()
    {
        var s = @"
    var x = 5
    var y = 10
    x + // 中间注释
    y
    ";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(15.0f));
    }

    /// <summary>
    /// 测试注释不影响函数声明
    /// </summary>
    [Test]
    public void TestCommentInFunction()
    {
        var s = @"
    add(a, b) { // 加法函数
        return a + b // 返回和
    }
    add(3, 5) // 调用函数
    ";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(8.0f));
    }

    /// <summary>
    /// 测试空注释
    /// </summary>
    [Test]
    public void TestEmptyComment()
    {
        var s = @"
    var x = 5 //
    x
    ";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(5.0f));
    }

    /// <summary>
    /// 测试注释包含特殊字符
    /// </summary>
    [Test]
    public void TestCommentWithSpecialChars()
    {
        var s = @"
    var x = 5 // 注释包含!@#$%^&*()
    var y = 10 // 注释包含<>=+-
    x + y
    ";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(15.0f));
    }

    /// <summary>
    /// 测试注释包含中文
    /// </summary>
    [Test]
    public void TestCommentWithChinese()
    {
        var s = @"
    var x = 5 // 这是一个中文注释
    var y = 10 // 计算两个数的和
    x + y
    ";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(15.0f));
    }

    /// <summary>
    /// 测试多个注释在同一行
    /// </summary>
    [Test]
    public void TestMultipleCommentsInSameLine()
    {
        var s = @"
    var x = 5 // 第一个注释
    var y = 10 // 第二个注释
    x + y // 第三个注释，但这行实际上不会执行
    ";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(15.0f));
    }

    /// <summary>
    /// 测试注释在if语句中
    /// </summary>
    [Test]
    public void TestCommentInIfStatement()
    {
        var s = @"
    var x = 10
    if (x > 5) { // 如果x大于5
        var y = x + 10 // 加上10
        y
    }
    ";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(20.0f));
    }

    /// <summary>
    /// 测试注释在while语句中
    /// </summary>
    [Test]
    public void TestCommentInWhileStatement()
    {
        var s = @"
    var x = 0
    var sum = 0
    while (x < 3) { // 循环3次
        sum = sum + x // 累加
        x = x + 1 // 递增
    }
    sum
    ";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(3.0f));
    }
}
