using NUnit.Framework;
using Script2;

namespace TestProject;

[TestFixture(false)]
[TestFixture(true)]
public class Script2ArrayTest(bool useInterpreter)
{
    private Script2Environment _env;

    [SetUp]
    public void SetUp()
    {
        _env = new Script2Environment(useInterpreter);
    }

    // ==================== 基础功能测试 ====================

    [Test]
    public void Test_ArrayLiteral_Integers()
    {
        var r = Script2Parser.Execute("[1, 2, 3]", _env);
        Assert.That(r, Is.InstanceOf<List<object>>());
        var list = (List<object>)r;
        Assert.That(list.Count, Is.EqualTo(3));
        Assert.That(list[0], Is.EqualTo(1f));
        Assert.That(list[1], Is.EqualTo(2f));
        Assert.That(list[2], Is.EqualTo(3f));
    }

    [Test]
    public void Test_ArrayLiteral_Strings()
    {
        var r = Script2Parser.Execute("[\"a\", \"b\", \"c\"]", _env);
        Assert.That(r, Is.InstanceOf<List<object>>());
        var list = (List<object>)r;
        Assert.That(list.Count, Is.EqualTo(3));
        Assert.That(list[0], Is.EqualTo("a"));
        Assert.That(list[1], Is.EqualTo("b"));
        Assert.That(list[2], Is.EqualTo("c"));
    }

    [Test]
    public void Test_ArrayLiteral_Mixed()
    {
        var r = Script2Parser.Execute("[1, \"hello\", true]", _env);
        var list = (List<object>)r;
        Assert.That(list.Count, Is.EqualTo(3));
        Assert.That(list[0], Is.EqualTo(1f));
        Assert.That(list[1], Is.EqualTo("hello"));
        Assert.That(list[2], Is.EqualTo(true));
    }

    [Test]
    public void Test_ArrayLiteral_Empty()
    {
        var r = Script2Parser.Execute("[]", _env);
        Assert.That(r, Is.InstanceOf<List<object>>());
        var list = (List<object>)r;
        Assert.That(list.Count, Is.EqualTo(0));
    }

    [Test]
    public void Test_ArrayIndexRead()
    {
        var s = "var arr = [10, 20, 30]; arr[0];";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(10f));
    }

    [Test]
    public void Test_ArrayIndexRead_Middle()
    {
        var s = "var arr = [10, 20, 30]; arr[1];";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(20f));
    }

    [Test]
    public void Test_ArrayIndexWrite()
    {
        var s = "var arr = [1, 2, 3]; arr[1] = 99; arr[1];";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(99f));
    }

    [Test]
    public void Test_ArrayIndexWrite_ThenReadAll()
    {
        var s = @"
var arr = [1, 2, 3];
arr[0] = 10;
arr[1] = 20;
arr[2] = 30;
arr[2];";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(30f));
    }

    [Test]
    public void Test_ArrayOutOfBounds_Read()
    {
        AssertEx.Throws<IndexOutOfRangeException, System.Reflection.TargetInvocationException>(() =>
        {
            Script2Parser.Execute("var arr = [1, 2, 3]; arr[5];", _env);
        });
    }

    [Test]
    public void Test_ArrayOutOfBounds_Write()
    {
        AssertEx.Throws<IndexOutOfRangeException, System.Reflection.TargetInvocationException>(() =>
        {
            Script2Parser.Execute("var arr = [1, 2, 3]; arr[5] = 99;", _env);
        });
    }

    [Test]
    public void Test_NestedArray()
    {
        var r = Script2Parser.Execute("[[1, 2], [3, 4]]", _env);
        var list = (List<object>)r;
        Assert.That(list.Count, Is.EqualTo(2));
        Assert.That(list[0], Is.InstanceOf<List<object>>());
        Assert.That(((List<object>)list[0]).Count, Is.EqualTo(2));
    }

    [Test]
    public void Test_Len()
    {
        var r = Script2Parser.Execute("len([1, 2, 3])", _env);
        Assert.That(r, Is.EqualTo(3f));
    }

    [Test]
    public void Test_Len_Empty()
    {
        var r = Script2Parser.Execute("len([])", _env);
        Assert.That(r, Is.EqualTo(0f));
    }

    [Test]
    public void Test_Push()
    {
        var s = @"
var arr = [1, 2];
push(arr, 3);
len(arr);";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(3f));
    }

    [Test]
    public void Test_Push_ReturnsNewLength()
    {
        var r = Script2Parser.Execute("push([1, 2], 3)", _env);
        Assert.That(r, Is.EqualTo(3f));
    }

    [Test]
    public void Test_Remove()
    {
        var s = @"
var arr = [10, 20, 30];
remove(arr, 1);
arr[1];";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(30f));
    }

    [Test]
    public void Test_Remove_ReturnsRemovedElement()
    {
        var r = Script2Parser.Execute("remove([10, 20, 30], 1)", _env);
        Assert.That(r, Is.EqualTo(20f));
    }

    [Test]
    public void Test_Insert()
    {
        var s = @"
var arr = [1, 3];
insert(arr, 1, 2);
arr[1];";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(2f));
    }

    [Test]
    public void Test_Insert_ReturnsNewLength()
    {
        var r = Script2Parser.Execute("insert([1, 3], 1, 2)", _env);
        Assert.That(r, Is.EqualTo(3f));
    }

    [Test]
    public void Test_ArrayInVariable()
    {
        var s = @"
var arr = [1, 2, 3];
var x = arr[0];
x;";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(1f));
    }

    [Test]
    public void Test_ArrayInFunction()
    {
        var s = @"
first(arr) {
    return arr[0];
}
first([10, 20, 30]);";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(10f));
    }

    [Test]
    public void Test_ArrayModifyInLoop()
    {
        var s = @"
var arr = [];
var i = 0;
while (i < 5) {
    push(arr, i);
    i = i + 1;
}
len(arr);";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(5f));
    }

    // ==================== 增强功能测试 ====================

    [Test]
    public void Test_Pop()
    {
        var s = @"
var arr = [1, 2, 3];
var val = pop(arr);
val;";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(3f));
    }

    [Test]
    public void Test_Pop_ArrayLength()
    {
        var s = @"
var arr = [1, 2, 3];
pop(arr);
pop(arr);
len(arr);";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(1f));
    }

    [Test]
    public void Test_Pop_EmptyArray()
    {
        var r = Script2Parser.Execute("pop([])", _env);
        Assert.That(r, Is.Null);
    }

    [Test]
    public void Test_Contains_True()
    {
        var r = Script2Parser.Execute("contains([1, 2, 3], 2)", _env);
        Assert.That(r, Is.EqualTo(true));
    }

    [Test]
    public void Test_Contains_False()
    {
        var r = Script2Parser.Execute("contains([1, 2, 3], 5)", _env);
        Assert.That(r, Is.EqualTo(false));
    }

    [Test]
    public void Test_Contains_String()
    {
        var r = Script2Parser.Execute("contains([\"a\", \"b\", \"c\"], \"b\")", _env);
        Assert.That(r, Is.EqualTo(true));
    }

    [Test]
    public void Test_IndexOf_Found()
    {
        var r = Script2Parser.Execute("indexOf([10, 20, 30], 20)", _env);
        Assert.That(r, Is.EqualTo(1f));
    }

    [Test]
    public void Test_IndexOf_NotFound()
    {
        var r = Script2Parser.Execute("indexOf([10, 20, 30], 99)", _env);
        Assert.That(r, Is.EqualTo(-1f));
    }

    [Test]
    public void Test_IndexOf_FirstOccurrence()
    {
        var r = Script2Parser.Execute("indexOf([1, 2, 3, 2], 2)", _env);
        Assert.That(r, Is.EqualTo(1f));
    }

    [Test]
    public void Test_Slice()
    {
        var r = Script2Parser.Execute("slice([1, 2, 3, 4, 5], 1, 3)", _env);
        var list = (List<object>)r;
        Assert.That(list.Count, Is.EqualTo(2));
        Assert.That(list[0], Is.EqualTo(2f));
        Assert.That(list[1], Is.EqualTo(3f));
    }

    [Test]
    public void Test_Slice_NoEnd()
    {
        var r = Script2Parser.Execute("slice([1, 2, 3, 4, 5], 2)", _env);
        var list = (List<object>)r;
        Assert.That(list.Count, Is.EqualTo(3));
        Assert.That(list[0], Is.EqualTo(3f));
        Assert.That(list[1], Is.EqualTo(4f));
        Assert.That(list[2], Is.EqualTo(5f));
    }

    [Test]
    public void Test_Slice_NegativeStart()
    {
        var r = Script2Parser.Execute("slice([1, 2, 3, 4, 5], -2)", _env);
        var list = (List<object>)r;
        Assert.That(list.Count, Is.EqualTo(2));
        Assert.That(list[0], Is.EqualTo(4f));
        Assert.That(list[1], Is.EqualTo(5f));
    }

    [Test]
    public void Test_Slice_NegativeEnd()
    {
        var r = Script2Parser.Execute("slice([1, 2, 3, 4, 5], 1, -1)", _env);
        var list = (List<object>)r;
        Assert.That(list.Count, Is.EqualTo(3));
        Assert.That(list[0], Is.EqualTo(2f));
        Assert.That(list[1], Is.EqualTo(3f));
        Assert.That(list[2], Is.EqualTo(4f));
    }

    [Test]
    public void Test_Slice_Empty()
    {
        var r = Script2Parser.Execute("slice([1, 2, 3], 1, 1)", _env);
        var list = (List<object>)r;
        Assert.That(list.Count, Is.EqualTo(0));
    }

    [Test]
    public void Test_Slice_DoesNotModifyOriginal()
    {
        var s = @"
var arr = [1, 2, 3, 4, 5];
var sub = slice(arr, 1, 3);
arr[0];";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(1f));
    }

    [Test]
    public void Test_Join()
    {
        var r = Script2Parser.Execute("join([1, 2, 3], \", \")", _env);
        Assert.That(r, Is.EqualTo("1, 2, 3"));
    }

    [Test]
    public void Test_Join_Empty()
    {
        var r = Script2Parser.Execute("join([], \", \")", _env);
        Assert.That(r, Is.EqualTo(""));
    }

    [Test]
    public void Test_Join_SingleElement()
    {
        var r = Script2Parser.Execute("join([\"hello\"], \"-\")", _env);
        Assert.That(r, Is.EqualTo("hello"));
    }

    [Test]
    public void Test_Sort_Numbers()
    {
        var r = Script2Parser.Execute("sort([3, 1, 4, 1, 5])", _env);
        var list = (List<object>)r;
        Assert.That(list, Is.EqualTo(new List<object> { 1f, 1f, 3f, 4f, 5f }));
    }

    [Test]
    public void Test_Sort_Strings()
    {
        var r = Script2Parser.Execute("sort([\"c\", \"a\", \"b\"])", _env);
        var list = (List<object>)r;
        Assert.That(list, Is.EqualTo(new List<object> { "a", "b", "c" }));
    }

    [Test]
    public void Test_Sort_DoesNotModifyOriginal()
    {
        var s = @"
var arr = [3, 1, 2];
var sorted = sort(arr);
arr[0];";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(3f));
    }

    [Test]
    public void Test_NegativeIndex_Read()
    {
        var s = "var arr = [10, 20, 30]; arr[-1];";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(30f));
    }

    [Test]
    public void Test_NegativeIndex_ReadSecondLast()
    {
        var s = "var arr = [10, 20, 30]; arr[-2];";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(20f));
    }

    [Test]
    public void Test_NegativeIndex_Write()
    {
        var s = "var arr = [10, 20, 30]; arr[-1] = 99; arr[-1];";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(99f));
    }

    [Test]
    public void Test_NegativeIndex_OutOfBounds()
    {
        AssertEx.Throws<IndexOutOfRangeException, System.Reflection.TargetInvocationException>(() =>
        {
            Script2Parser.Execute("var arr = [1, 2, 3]; arr[-4];", _env);
        });
    }

    [Test]
    public void Test_NegativeIndex_ArrayAccessExpression()
    {
        var s = @"
var arr = [10, 20, 30];
var i = -1;
arr[i];";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(30f));
    }

    // ==================== for-in 循环测试 ====================

    [Test]
    public void Test_ForIn_Basic()
    {
        var s = @"
var arr = [10, 20, 30];
var sum = 0;
for item in arr {
    sum = sum + item;
}
sum;";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(60f));
    }

    [Test]
    public void Test_ForIn_CollectItems()
    {
        var s = @"
var arr = [1, 2, 3];
var result = [];
for item in arr {
    push(result, item);
}
result;";
        var r = Script2Parser.Execute(s, _env);
        var list = (List<object>)r;
        Assert.That(list, Is.EqualTo(new List<object> { 1f, 2f, 3f }));
    }

    [Test]
    public void Test_ForIn_EmptyArray()
    {
        var s = @"
var arr = [];
var sum = 0;
for item in arr {
    sum = sum + item;
}
sum;";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(0f));
    }

    [Test]
    public void Test_ForIn_SingleElement()
    {
        var s = @"
var arr = [42];
var val = 0;
for item in arr {
    val = item;
}
val;";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(42f));
    }

    [Test]
    public void Test_ForIn_Strings()
    {
        var s = @"
var arr = [""a"", ""b"", ""c""];
var result = """";
for item in arr {
    result = concat(result, item);
}
result;";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo("abc"));
    }

    [Test]
    public void Test_ForIn_NestedLoop()
    {
        var s = @"
var matrix = [[1, 2], [3, 4]];
var sum = 0;
for row in matrix {
    for item in row {
        sum = sum + item;
    }
}
sum;";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(10f));
    }

    [Test]
    public void Test_ForIn_WithCondition()
    {
        var s = @"
var arr = [1, 2, 3, 4, 5];
var sum = 0;
for item in arr {
    if (item > 3) {
        sum = sum + item;
    }
}
sum;";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(9f));
    }

    [Test]
    public void Test_ForIn_ModifyIterationVar()
    {
        var s = @"
var arr = [10, 20, 30];
var sum = 0;
for item in arr {
    item = item + 1;
    sum = sum + item;
}
sum;";
        var r = Script2Parser.Execute(s, _env);
        // item 每次被重新赋值为数组元素，item+1 不影响下一次迭代
        // sum = (10+1) + (20+1) + (30+1) = 63
        Assert.That(r, Is.EqualTo(63f));
    }
}
