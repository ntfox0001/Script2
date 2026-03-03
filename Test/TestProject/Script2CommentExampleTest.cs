using NUnit.Framework;
using Script2;

namespace TestProject;

[TestFixture(true)]
[TestFixture(false)]
public class Script2CommentExampleTest(bool useInterpreter)
{
    [SetUp]
    public void SetUp()
    {
        Script2Parser.UseInterpreterMode = useInterpreter;
    }

    [Test]
    public void TestCommentExample()
    {
        var env = new Script2Environment();
        var content = @"
// 这是一个示例脚本，展示双斜杠注释功能

// 变量声明示例
var x = 10  // 声明变量x并赋值为10
var y = 20  // 声明变量y并赋值为20

// 函数定义示例
add(a, b) {
    // 这是一个加法函数
    // 返回两个参数的和
    return a + b
}

// 函数调用示例
var result = add(x, y)  // 调用add函数计算x和y的和

// 条件语句示例
if (result > 25) {
    // 如果结果大于25，打印信息
    var message = ""结果大于25""  // 定义消息变量
    message
}

// 循环语句示例
var sum = 0
var i = 0
while (i < 5) {  // 循环5次
    sum = sum + i  // 累加i的值
    i = i + 1      // i递增
}

// 最终返回sum的值
sum

// 这行也是注释，不会被解释器处理
// 下面这行代码不会执行，因为它在注释中
// var ignored = 999
";
        var result = Script2Parser.Execute(content, env);
        Assert.That(result, Is.EqualTo(10.0f)); // 0+1+2+3+4 = 10
        Console.WriteLine($"Result: {result}");
    }
}
