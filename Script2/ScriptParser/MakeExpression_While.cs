using System.Linq.Expressions;

namespace Script2.ScriptParser;

internal partial class MakeExpression
{
    // 实现MakeWhileStatement方法
    public static Expression MakeWhileStatement(Expression condition, Expression body)
    {
        // 将条件转换为bool类型
        var conditionAsBool = Expression.Convert(condition, typeof(bool));

        // 处理 body 的类型
        Expression bodyExpr = body;
        if (body.Type == typeof(void))
        {
            bodyExpr = Expression.Block(
                body,
                Expression.Constant(null, typeof(object))
            );
        }
        else if (body.Type != typeof(object))
        {
            bodyExpr = Expression.Convert(body, typeof(object));
        }

        // 构建while循环：while (condition) { body }
        // 使用 Expression.Loop 和 Expression.Break
        var breakLabel = Expression.Label(typeof(object), "whileBreak");
        var loop = Expression.Loop(
            Expression.IfThenElse(
                conditionAsBool,
                bodyExpr,
                Expression.Break(breakLabel, Expression.Constant(null, typeof(object)))
            ),
            breakLabel
        );

        return loop;
    }
}