using System.Linq.Expressions;

namespace Script2.ScriptParser;

internal partial class MakeExpression
{
    // 实现MakeStatementBlock方法
    public static Expression MakeStatementBlock(Expression[] statements)
    {
        if (statements.Length == 0)
            return Expression.Constant(null, typeof(object));

        if (statements.Length == 1)
            return statements[0];

        // 检查最后一个语句是否是 void 表达式（表示函数结束）
        // 在语句块中，return 语句会直接返回值，而不是作为语句块的返回值
        var nonLastStatements = statements.Take(statements.Length - 1);
        var lastStatement = statements.Last();

        // 如果最后一个语句是 void 类型（Block without return），则直接执行所有语句并返回 null
        if (lastStatement.NodeType == ExpressionType.MemberAccess &&
            ((MemberExpression)lastStatement).Member.DeclaringType == typeof(VoidValue) &&
            ((MemberExpression)lastStatement).Member.Name == "Instance")
        {
            // 这是一个 void 函数，执行所有语句并返回 void
            return Expression.Block(
                statements.Concat(new[] { lastStatement }).ToArray()
            );
        }

        return Expression.Block(
            typeof(object),
            nonLastStatements.Concat(new[] { Expression.Convert(lastStatement, typeof(object)) }).ToArray()
        );
    }
}