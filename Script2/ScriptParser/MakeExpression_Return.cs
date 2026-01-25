using System.Linq.Expressions;

namespace Script2.ScriptParser;

internal partial class MakeExpression
{
    // 实现MakeReturnStatement方法
    public static Expression MakeReturnStatement(Expression expr)
    {
        // 标记函数有return语句
        Global._hasReturnInFunction.Value = true;

        // 处理返回值表达式
        Expression returnValueExpr;
        if (expr == null ||
            (expr.NodeType == ExpressionType.Constant &&
             ((ConstantExpression)expr).Value == null &&
             expr.Type == typeof(object)))
        {
            returnValueExpr = Expression.Constant(null, typeof(object));
        }
        else if (expr.NodeType == ExpressionType.Call &&
                 ((MethodCallExpression)expr).Method.Name == "GetVariableValue")
        {
            var variableName = ((MethodCallExpression)expr).Arguments[0];
            if (variableName.NodeType == ExpressionType.Constant &&
                ((ConstantExpression)variableName).Value is string varName &&
                varName == "null")
            {
                returnValueExpr = Expression.Constant(null, typeof(object));
            }
            else
            {
                returnValueExpr = expr.Type == typeof(object) ? expr : Expression.Convert(expr, typeof(object));
            }
        }
        else
        {
            returnValueExpr = expr.Type == typeof(object) ? expr : Expression.Convert(expr, typeof(object));
        }

        // 抛出 ReturnValueException 实现提前返回
        var exceptionCtor = typeof(ReturnValueException).GetConstructor(new[] { typeof(object) });
        var throwExpr = Expression.Throw(
            Expression.New(exceptionCtor, returnValueExpr),
            typeof(object)
        );

        return throwExpr;
    }
}