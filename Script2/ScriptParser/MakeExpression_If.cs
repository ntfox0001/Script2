using System.Linq.Expressions;

namespace Script2.ScriptParser;

internal partial class MakeExpression
{
    // 实现MakeIfStatement方法
    public static Expression MakeIfStatement(Expression condition, Expression thenBranch, Expression elseBranch)
    {
        // 将条件转换为bool类型
        var conditionAsBool = Expression.Convert(condition, typeof(bool));

        // 处理 thenBranch 的类型
        Expression thenExpr = thenBranch;
        if (thenBranch.Type == typeof(void))
        {
            thenExpr = Expression.Block(
                thenBranch,
                Expression.Constant(null, typeof(object))
            );
        }
        else if (thenBranch.Type != typeof(object))
        {
            thenExpr = Expression.Convert(thenBranch, typeof(object));
        }

        // 如果有else分支
        if (elseBranch != null)
        {
            // 处理 elseBranch 的类型
            Expression elseExpr = elseBranch;
            if (elseBranch.Type == typeof(void))
            {
                elseExpr = Expression.Block(
                    elseBranch,
                    Expression.Constant(null, typeof(object))
                );
            }
            else if (elseBranch.Type != typeof(object))
            {
                elseExpr = Expression.Convert(elseBranch, typeof(object));
            }
            return Expression.Condition(conditionAsBool, thenExpr, elseExpr);
        }
        else
            // 没有else分支，返回默认值null
            return Expression.Condition(conditionAsBool, thenExpr, Expression.Constant(null, typeof(object)));
    }

}