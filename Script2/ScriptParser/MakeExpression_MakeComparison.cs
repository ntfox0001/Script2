using System.Linq.Expressions;

namespace Script2.ScriptParser;

internal partial class MakeExpression
{
    public static Expression MakeComparison(ExpressionType comparisonType, Expression left, Expression right)
    {
        var leftType = left.Type;
        var rightType = right.Type;

        // 如果类型相同，直接比较
        if (leftType == rightType)
        {
            return MakeComparisonWithSameTypes(comparisonType, left, right);
        }

        // 对于相等和不等运算符，如果类型不同，尝试转换
        if (comparisonType == ExpressionType.Equal || comparisonType == ExpressionType.NotEqual)
        {
            // 如果一个是 object，另一个是具体类型，转换为具体类型比较
            if (leftType == typeof(object) && rightType != typeof(object))
            {
                return MakeComparisonWithSameTypes(comparisonType, Expression.Convert(left, rightType), right);
            }

            if (rightType == typeof(object) && leftType != typeof(object))
            {
                return MakeComparisonWithSameTypes(comparisonType, left, Expression.Convert(right, leftType));
            }

            // 两个都是不同具体类型，抛出错误
            throw new InvalidOperationException(
                $"Type mismatch in '{comparisonType}' comparison: cannot compare {leftType.Name} with {rightType.Name}. " +
                "Logical equality (==) and inequality (!=) operations require both operands to be of the same type.");
        }

        // 对于其他比较运算符，尝试转换为 float 比较
        return MakeComparisonAsFloat(comparisonType, left, right);
    }

    private static Expression MakeComparisonWithSameTypes(ExpressionType comparisonType, Expression left,
        Expression right)
    {
        var leftType = left.Type;

        // 对于 object 类型，需要运行时类型检查
        if (leftType == typeof(object))
        {
            // 对于相等和不等运算符，进行运行时类型检查
            if (comparisonType == ExpressionType.Equal || comparisonType == ExpressionType.NotEqual)
            {
                return MakeObjectComparisonWithRuntimeTypeCheck(comparisonType, left, right);
            }

            // 对于其他比较运算符，转换为 float 比较
            return MakeComparisonAsFloat(comparisonType, left, right);
        }

        // 直接比较
        return Expression.MakeBinary(comparisonType, left, right);
    }

    /// <summary>
    /// 对两个 object 类型的值进行比较，并在运行时检查类型是否匹配（仅用于 == 和 !=）
    /// </summary>
    private static Expression MakeObjectComparisonWithRuntimeTypeCheck(ExpressionType comparisonType, Expression left,
        Expression right)
    {
        // 获取运行时类型检查方法
        var typeCheckMethod = typeof(Script2Environment)
            .GetMethod(nameof(Script2Environment.CheckTypesForEquality));

        var typeCheckCall = Expression.Call(typeCheckMethod, left, right);

        // 如果类型检查通过，则进行字符串比较
        var stringLeft = Expression.Call(left, "ToString", Type.EmptyTypes);
        var stringRight = Expression.Call(right, "ToString", Type.EmptyTypes);
        var comparisonExpr = Expression.MakeBinary(comparisonType, stringLeft, stringRight);

        // 先进行类型检查，再进行比较
        return Expression.Block(typeCheckCall, comparisonExpr);
    }

    /// <summary>
    /// 将表达式转换为 float 进行比较（用于非 == 和 != 的比较运算符）
    /// </summary>
    private static Expression MakeComparisonAsFloat(ExpressionType comparisonType, Expression left, Expression right)
    {
        try
        {
            var convertedLeft = left.Type == typeof(float) ? left : Expression.Convert(left, typeof(float));
            var convertedRight = right.Type == typeof(float) ? right : Expression.Convert(right, typeof(float));
            return Expression.MakeBinary(comparisonType, convertedLeft, convertedRight);
        }
        catch
        {
            // 如果转换为 float 失败，尝试字符串比较
            var stringLeft = left.Type == typeof(string) ? left : Expression.Call(left, "ToString", Type.EmptyTypes);
            var stringRight = right.Type == typeof(string)
                ? right
                : Expression.Call(right, "ToString", Type.EmptyTypes);
            return Expression.MakeBinary(comparisonType, stringLeft, stringRight);
        }
    }
}