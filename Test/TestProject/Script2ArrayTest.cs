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
        Assert.That(r, Is.EqualTo(63f));
    }

    // ==================== 函数参数为数组的测试 ====================

    [Test]
    public void Test_FuncParam_ArraySum()
    {
        var s = @"
sum(arr) {
    var total = 0;
    for item in arr {
        total = total + item;
    }
    return total;
}
sum([1, 2, 3, 4, 5]);";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(15f));
    }

    [Test]
    public void Test_FuncParam_ArrayModify()
    {
        var s = @"
addOne(arr) {
    var i = 0;
    while (i < len(arr)) {
        arr[i] = arr[i] + 1;
        i = i + 1;
    }
    return arr;
}
var a = [1, 2, 3];
addOne(a);
a[0];";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(2f));
    }

    [Test]
    public void Test_FuncParam_ArrayModify_AllElements()
    {
        var s = @"
doubleAll(arr) {
    var i = 0;
    while (i < len(arr)) {
        arr[i] = arr[i] * 2;
        i = i + 1;
    }
}
var a = [1, 2, 3];
doubleAll(a);
a;";
        var r = Script2Parser.Execute(s, _env);
        var list = (List<object>)r;
        Assert.That(list, Is.EqualTo(new List<object> { 2f, 4f, 6f }));
    }

    [Test]
    public void Test_FuncParam_ArrayReturn()
    {
        var s = @"
reverse(arr) {
    var result = [];
    var i = len(arr) - 1;
    while (i >= 0) {
        push(result, arr[i]);
        i = i - 1;
    }
    return result;
}
reverse([1, 2, 3]);";
        var r = Script2Parser.Execute(s, _env);
        var list = (List<object>)r;
        Assert.That(list, Is.EqualTo(new List<object> { 3f, 2f, 1f }));
    }

    [Test]
    public void Test_FuncParam_MultipleParamsWithArray()
    {
        var s = @"
findIndex(arr, target) {
    var i = 0;
    while (i < len(arr)) {
        if (arr[i] == target) {
            return i;
        }
        i = i + 1;
    }
    return -1;
}
findIndex([10, 20, 30], 20);";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(1f));
    }

    [Test]
    public void Test_FuncParam_MultipleParamsWithArray_NotFound()
    {
        var s = @"
findIndex(arr, target) {
    var i = 0;
    while (i < len(arr)) {
        if (arr[i] == target) {
            return i;
        }
        i = i + 1;
    }
    return -1;
}
findIndex([10, 20, 30], 99);";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(-1f));
    }

    [Test]
    public void Test_FuncParam_ArrayWithBuiltIn()
    {
        var s = @"
appendAndReturn(arr, val) {
    push(arr, val);
    return len(arr);
}
var a = [1, 2];
appendAndReturn(a, 3);";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(3f));
    }

    [Test]
    public void Test_FuncParam_ArrayWithBuiltIn_VerifyModified()
    {
        var s = @"
appendAndReturn(arr, val) {
    push(arr, val);
    return len(arr);
}
var a = [1, 2];
appendAndReturn(a, 3);
a[2];";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(3f));
    }

    [Test]
    public void Test_FuncParam_NestedArrayParam()
    {
        var s = @"
flatten(matrix) {
    var result = [];
    for row in matrix {
        for item in row {
            push(result, item);
        }
    }
    return result;
}
flatten([[1, 2], [3, 4]]);";
        var r = Script2Parser.Execute(s, _env);
        var list = (List<object>)r;
        Assert.That(list, Is.EqualTo(new List<object> { 1f, 2f, 3f, 4f }));
    }

    [Test]
    public void Test_FuncParam_PassArrayVariable()
    {
        var s = @"
getFirst(arr) {
    return arr[0];
}
var myArr = [100, 200, 300];
getFirst(myArr);";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(100f));
    }

    [Test]
    public void Test_FuncParam_ArrayAsCondition()
    {
        var s = @"
isEmpty(arr) {
    return len(arr) == 0;
}
isEmpty([]);";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(true));
    }

    [Test]
    public void Test_FuncParam_ArrayAsCondition_NotEmpty()
    {
        var s = @"
isEmpty(arr) {
    return len(arr) == 0;
}
isEmpty([1]);";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(false));
    }

    [Test]
    public void Test_FuncParam_ChainArrayFunctions()
    {
        var s = @"
sum(arr) {
    var total = 0;
    for item in arr {
        total = total + item;
    }
    return total;
}
average(arr) {
    return sum(arr) / len(arr);
}
average([10, 20, 30]);";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(20f));
    }

    [Test]
    public void Test_FuncParam_ArrayWithStrings()
    {
        var s = @"
concatAll(arr) {
    var result = """";
    for item in arr {
        result = concat(result, item);
    }
    return result;
}
concatAll([""a"", ""b"", ""c""]);";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo("abc"));
    }

    [Test]
    public void Test_FuncParam_ArrayRemoveInFunction()
    {
        var s = @"
removeFirst(arr) {
    return remove(arr, 0);
}
var a = [10, 20, 30];
removeFirst(a);
a[0];";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(20f));
    }

    [Test]
    public void Test_FuncParam_EmptyArrayParam()
    {
        var s = @"
safeFirst(arr) {
    if (len(arr) == 0) {
        return -1;
    }
    return arr[0];
}
safeFirst([]);";
        var r = Script2Parser.Execute(s, _env);
        Assert.That(r, Is.EqualTo(-1f));
    }

    [Test]
    public void Test_FuncParam_ArrayInsertInFunction()
    {
        var s = @"
insertSorted(arr, val) {
    var i = 0;
    while (i < len(arr)) {
        if (arr[i] > val) {
            insert(arr, i, val);
            return arr;
        }
        i = i + 1;
    }
    push(arr, val);
    return arr;
}
insertSorted([1, 3, 5], 4);";
        var r = Script2Parser.Execute(s, _env);
        var list = (List<object>)r;
        Assert.That(list, Is.EqualTo(new List<object> { 1f, 3f, 4f, 5f }));
    }
}
