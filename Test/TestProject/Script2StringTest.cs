using NUnit.Framework;
using Script2;

namespace TestProject;

[TestFixture(false)]
[TestFixture(true)]
public class Script2StringTest(bool useInterpreter)
{
    private Script2Environment _env;

    [SetUp]
    public void SetUp()
    {
        _env = new Script2Environment(useInterpreter);
    }

    [Test]
    public void TestString1()
    {
        var s = @"
    add(a, b) {
        return a
    }
    add(""a"", 3)
    ";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo("a"));
    }

    [Test]
    public void TestString2()
    {
        var s = @"
    var a= ""eee""
    ";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(_env.GetVariableValue("a"), Is.EqualTo("eee"));
    }

    [Test]
    public void TestString3()
    {
        var s = @"
    var a= ""eee""
    a = ""dd""
    ";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(_env.GetVariableValue("a"), Is.EqualTo("dd"));
    }

    [Test]
    public void TestConcatTwoStrings()
    {
        var r = Script2Parser.Execute("concat(\"Hello\", \" \", \"World\")", _env);
        Assert.That(r, Is.EqualTo("Hello World"));
    }

    [Test]
    public void TestConcatStringAndNumber()
    {
        var r = Script2Parser.Execute("concat(\"The value is: \", 42)", _env);
        Assert.That(r, Is.EqualTo("The value is: 42"));
    }

    [Test]
    public void TestConcatStringAndBoolean()
    {
        var r = Script2Parser.Execute("concat(\"Result: \", true)", _env);
        Assert.That(r, Is.EqualTo("Result: True"));
    }

    [Test]
    public void TestConcatMultipleNumbers()
    {
        var r = Script2Parser.Execute("concat(1, 2, 3)", _env);
        Assert.That(r, Is.EqualTo("123"));
    }

    [Test]
    public void TestConcatWithVariables()
    {
        var s = @"
var name = ""Alice"";
var age = 25;
var result = concat(""Name: "", name, "", Age: "", age);
result;
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo("Name: Alice, Age: 25"));
    }

    [Test]
    public void TestConcatInFunction()
    {
        var s = @"
greeting(name) {
    return concat(""Hello, "", name, ""!"");
}
greeting(""Bob"");
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo("Hello, Bob!"));
    }

    [Test]
    public void TestConcatEmpty()
    {
        var r = Script2Parser.Execute("concat()", _env);
        Assert.That(r, Is.EqualTo(""));
    }

    [Test]
    public void TestConcatSingleArgument()
    {
        var r = Script2Parser.Execute("concat(\"Only one\")", _env);
        Assert.That(r, Is.EqualTo("Only one"));
    }

    [Test]
    public void TestConcatWithFloat()
    {
        var r = Script2Parser.Execute("concat(\"PI = \", 3.14159)", _env);
        Assert.That(r, Is.EqualTo("PI = 3.14159"));
    }

    [Test]
    public void TestConcatNested()
    {
        var s = @"
part1 = concat(""A"", ""B"");
part2 = concat(""C"", ""D"");
full = concat(part1, part2);
full;
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo("ABCD"));
    }

    [Test]
    public void TestConcatInConditional()
    {
        var s = @"
var score = 85;
var result = """"; if (score >= 90) {
    result = concat(""Grade: A"", "" Score: "", score);
} else {
    result = concat(""Grade: B"", "" Score: "", score);
}
result;
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo("Grade: B Score: 85"));
    }

    [Test]
    public void TestConcatInLoop()
    {
        var s = @"
var i = 0;
var result = """";
while (i < 3) {
    result = concat(result, i);
    i = i + 1;
}
result;
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo("012"));
    }

    [Test]
    public void TestFormatSimple()
    {
        var r = Script2Parser.Execute("format(\"Hello {0}\", \"World\")", _env);
        Assert.That(r, Is.EqualTo("Hello World"));
    }

    [Test]
    public void TestFormatMultiplePlaceholders()
    {
        var r = Script2Parser.Execute("format(\"{0} + {1} = {2}\", 2, 3, 5)", _env);
        Assert.That(r, Is.EqualTo("2 + 3 = 5"));
    }

    [Test]
    public void TestFormatWithNumbers()
    {
        var r = Script2Parser.Execute("format(\"Score: {0}\", 95.5)", _env);
        Assert.That(r, Is.EqualTo("Score: 95.5"));
    }

    [Test]
    public void TestFormatWithVariables()
    {
        var s = @"
var name = ""Alice"";
var age = 25;
var result = format(""Name: {0}, Age: {1}"", name, age);
result;
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo("Name: Alice, Age: 25"));
    }

    [Test]
    public void TestFormatInFunction()
    {
        var s = @"
greet(name) {
    return format(""Hello, {0}!"", name);
}
greet(""Bob"");
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo("Hello, Bob!"));
    }

    [Test]
    public void TestFormatRepeatedPlaceholders()
    {
        var r = Script2Parser.Execute("format(\"{0} {1} {0}\", \"Hello\", \"World\")", _env);
        Assert.That(r, Is.EqualTo("Hello World Hello"));
    }

    [Test]
    public void TestFormatNoPlaceholders()
    {
        var r = Script2Parser.Execute("format(\"Just a string\")", _env);
        Assert.That(r, Is.EqualTo("Just a string"));
    }

    [Test]
    public void TestFormatWithBoolean()
    {
        var r = Script2Parser.Execute("format(\"Result: {0}\", true)", _env);
        Assert.That(r, Is.EqualTo("Result: True"));
    }

    [Test]
    public void TestFormatMultipleCalls()
    {
        var s = @"
var a = 10;
var b = 20;
var result1 = format(""A = {0}"", a);
var result2 = format(""B = {0}"", b);
result1;
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo("A = 10"));
    }

    [Test]
    public void TestFormatInConditional()
    {
        var s = @"
var score = 88;
var result = """"; if (score >= 90) {
    result = format(""Grade: A, Score: {0}"", score);
} else {
    result = format(""Grade: B, Score: {0}"", score);
}
result;
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo("Grade: B, Score: 88"));
    }

    [Test]
    public void TestFormatInLoop()
    {
        var s = @"
var i = 0;
var result = """"; while (i < 3) {
    result = format(""{0}{1}"", result, i);
    i = i + 1;
}
result;
";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo("012"));
    }

    [Test]
    public void TestFormatComplexExpression()
    {
        var r = Script2Parser.Execute("format(\"{0} * {1} = {2}\", 5, 4, 20)", _env);
        Assert.That(r, Is.EqualTo("5 * 4 = 20"));
    }
}
