using System.Linq.Expressions;

namespace Script2.ScriptParser;

internal partial class MakeExpression
{
    public static Expression MakeLogical(ExpressionType logicalType, Expression left, Expression right)
    {
        // 将左右操作数转换为bool类型
        Expression convertedLeft = Expression.Convert(left, typeof(bool));
        Expression convertedRight = Expression.Convert(right, typeof(bool));
        return Expression.MakeBinary(logicalType, convertedLeft, convertedRight);
    }
}