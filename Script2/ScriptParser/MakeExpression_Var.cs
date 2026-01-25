using System.Linq.Expressions;

namespace Script2.ScriptParser;

internal partial class MakeExpression
{
    public static Expression SetVariable(string varName, Expression value)
    {
        // 检查是否为 void 表达式（VoidValue.Instance）
        if (value.NodeType == ExpressionType.MemberAccess &&
            ((MemberExpression)value).Member.DeclaringType == typeof(VoidValue) &&
            ((MemberExpression)value).Member.Name == "Instance")
        {
            throw new InvalidOperationException(
                $"Cannot assign void value to variable '{varName}'. Functions that don't return a value cannot be assigned to variables.");
        }

        // 检查值类型是否为 void
        if (value.Type == typeof(void))
        {
            throw new InvalidOperationException(
                $"Cannot assign void value to variable '{varName}'. Functions that don't return a value cannot be assigned to variables.");
        }

        var setVarMethod = typeof(Script2Environment)
            .GetMethod(nameof(Script2Environment.SetVariableValue));

        Expression valueToAssign;
        // 如果值已经是 object 类型，直接使用
        if (value.Type == typeof(object))
            valueToAssign = value;
        else
        {
            try
            {
                valueToAssign = Expression.Convert(value, typeof(object));
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Cannot assign value to variable '{varName}': {ex.Message}", ex);
            }
        }

        return Expression.Call(
            Global._envParam,
            setVarMethod!,
            Expression.Constant(varName),
            valueToAssign
        );
    }

    public static Expression ReassignVariable(string varName, Expression value)
    {
        // 检查是否为 void 表达式（VoidValue.Instance）
        if (value.NodeType == ExpressionType.MemberAccess &&
            ((MemberExpression)value).Member.DeclaringType == typeof(VoidValue) &&
            ((MemberExpression)value).Member.Name == "Instance")
        {
            throw new InvalidOperationException(
                $"Cannot assign void value to variable '{varName}'. Functions that don't return a value cannot be assigned to variables.");
        }

        // 检查值类型是否为 void
        if (value.Type == typeof(void))
        {
            throw new InvalidOperationException(
                $"Cannot assign void value to variable '{varName}'. Functions that don't return a value cannot be assigned to variables.");
        }

        var setVarMethod = typeof(Script2Environment)
            .GetMethod(nameof(Script2Environment.SetVariableValue));

        Expression valueToAssign;
        // 如果值已经是 object 类型，直接使用
        if (value.Type == typeof(object))
            valueToAssign = value;
        else
        {
            try
            {
                valueToAssign = Expression.Convert(value, typeof(object));
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Cannot assign value to variable '{varName}': {ex.Message}", ex);
            }
        }

        // 构建赋值表达式：先调用 SetVariableValue，然后返回赋值的值
        var setVarCall = Expression.Call(
            Global._envParam,
            setVarMethod!,
            Expression.Constant(varName),
            valueToAssign
        );

        // 返回一个块表达式：先执行赋值，然后返回值
        return Expression.Block(
            typeof(object),
            new[] { setVarCall, valueToAssign }
        );
    }

    public static Expression GetVariable(string varName)
    {
        var getVarMethod = typeof(Script2Environment)
            .GetMethod(nameof(Script2Environment.GetVariableValue));

        return Expression.Call(
            Global._envParam,
            getVarMethod!,
            Expression.Constant(varName)
        );
    }
}