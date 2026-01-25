using System.Linq.Expressions;

namespace Script2.ScriptParser;

internal class Global
{
    // 静态字段：统一的参数
    public static readonly ParameterExpression _envParam = Expression.Parameter(typeof(Script2Environment), "env");
    // 标记当前解析的函数是否有return语句
    // 使用 ThreadLocal 是因为：1) 解析器是静态的，需要在线程间隔离状态；2) ReturnStatement 在解析时需要修改外层 FuncDecl 的状态
    public static readonly ThreadLocal<bool> _hasReturnInFunction = new(() => false);
}