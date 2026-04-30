using System.Linq.Expressions;

namespace Script2.ScriptParser;

internal partial class MakeExpression
{
    /// <summary>
    /// 实现 for-in 循环: for item in array { body }
    /// 编译为等价的 while 循环，使用隐藏索引变量遍历数组
    /// </summary>
    public static Expression MakeForInStatement(string itemName, Expression iterableExpr, Expression body)
    {
        // 创建隐藏的索引变量和数组长度变量
        var indexVar = Expression.Variable(typeof(int), "__forInIndex");
        var countVar = Expression.Variable(typeof(int), "__forInCount");
        var arrayVar = Expression.Variable(typeof(List<object>), "__forInArray");

        // 获取 List<object>.Count 属性
        var countProperty = typeof(List<object>).GetProperty(nameof(List<object>.Count))!;

        // 获取 Script2Environment.SetVariableValue 方法
        var setVarMethod = typeof(Script2Environment).GetMethod(nameof(Script2Environment.SetVariableValue))!;

        // 获取 Script2Environment.GetArrayItem 方法
        var getArrayItemMethod = typeof(Script2Environment).GetMethod(nameof(Script2Environment.GetArrayItem))!;

        // 初始化: arrayVar = iterableExpr, countVar = arrayVar.Count, indexVar = 0
        Expression iterableAsList = iterableExpr.Type == typeof(List<object>)
            ? iterableExpr
            : Expression.Convert(iterableExpr, typeof(List<object>));

        var initAssign = Expression.Block(
            Expression.Assign(arrayVar, iterableAsList),
            Expression.Assign(countVar, Expression.Property(arrayVar, countProperty)),
            Expression.Assign(indexVar, Expression.Constant(0))
        );

        // 循环条件: indexVar < countVar
        var condition = Expression.LessThan(indexVar, countVar);

        // 循环体: 设置 item 变量, 执行用户代码, indexVar++
        Expression setItemExpr = Expression.Call(
            Global._envParam,
            setVarMethod,
            Expression.Constant(itemName),
            Expression.Call(
                Global._envParam,
                getArrayItemMethod,
                Expression.Convert(arrayVar, typeof(object)),
                Expression.Convert(indexVar, typeof(object))
            )
        );

        // 处理用户循环体的类型
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

        // 组合: 设置 item + 执行用户代码 + indexVar++
        var loopBody = Expression.Block(
            setItemExpr,
            bodyExpr,
            Expression.PostIncrementAssign(indexVar)
        );

        // 构建循环: while (indexVar < countVar) { ... }
        var breakLabel = Expression.Label(typeof(object), "forInBreak");
        var loop = Expression.Loop(
            Expression.IfThenElse(
                condition,
                loopBody,
                Expression.Break(breakLabel, Expression.Constant(null, typeof(object)))
            ),
            breakLabel
        );

        // 最终: 所有变量在同一个 Block 作用域中
        var allVariables = new[] { arrayVar, countVar, indexVar };
        var result = Expression.Block(
            typeof(object),
            allVariables,
            initAssign,
            loop
        );

        return result;
    }
}
