using System.Linq.Expressions;

namespace Script2.ScriptParser;

internal partial class MakeExpression
{
    public static Expression MakeBinaryWithConversion(ExpressionType binaryType, Expression left, Expression right)
    {
        // 对于取模运算，使用整数运算
        if (binaryType == ExpressionType.Modulo)
        {
            // 先转换为 float（因为值可能是 object 类型），再转换为 int
            Expression leftAsFloat = left.Type == typeof(float) ? left : Expression.Convert(left, typeof(float));
            Expression rightAsFloat = right.Type == typeof(float) ? right : Expression.Convert(right, typeof(float));
            Expression convertedLeft = Expression.Convert(leftAsFloat, typeof(int));
            Expression convertedRight = Expression.Convert(rightAsFloat, typeof(int));
            var intResult = Expression.MakeBinary(binaryType, convertedLeft, convertedRight);
            // 将整数结果转回 float
            return Expression.Convert(intResult, typeof(float));
        }

        // 统一转换为 float 进行其他运算
        Expression convertedLeftFloat = Expression.Convert(left, typeof(float));
        Expression convertedRightFloat = Expression.Convert(right, typeof(float));
        return Expression.MakeBinary(binaryType, convertedLeftFloat, convertedRightFloat);
    }
}