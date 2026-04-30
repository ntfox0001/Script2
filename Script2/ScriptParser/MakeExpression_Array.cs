using System.Linq.Expressions;

namespace Script2.ScriptParser;

internal partial class MakeExpression
{
    /// <summary>
    /// 创建数组字面量表达式：[expr, expr, ...]
    /// </summary>
    public static Expression MakeArrayLiteral(Expression[] elements)
    {
        // 使用 List<object> 来存储数组元素，便于后续修改
        var addMethod = typeof(List<object>).GetMethod(nameof(List<object>.Add), new[] { typeof(object) });

        // 创建 List<object> 并添加元素
        var listVar = Expression.Variable(typeof(List<object>), "list");

        var addExpressions = new List<Expression>();
        addExpressions.Add(Expression.Assign(listVar, Expression.New(typeof(List<object>))));

        foreach (var elem in elements)
        {
            Expression elemToAdd;
            if (elem.Type == typeof(object))
                elemToAdd = elem;
            else
                elemToAdd = Expression.Convert(elem, typeof(object));

            addExpressions.Add(Expression.Call(listVar, addMethod!, elemToAdd));
        }

        // 返回 list 作为 object
        addExpressions.Add(Expression.Convert(listVar, typeof(object)));

        return Expression.Block(typeof(object), new[] { listVar }, addExpressions);
    }

    /// <summary>
    /// 创建数组索引访问表达式：arr[index]
    /// </summary>
    public static Expression MakeArrayIndex(Expression arrayExpr, Expression indexExpr)
    {
        var getItemMethod = typeof(Script2Environment).GetMethod(nameof(Script2Environment.GetArrayItem));

        Expression arrayObj = arrayExpr.Type == typeof(object) ? arrayExpr : Expression.Convert(arrayExpr, typeof(object));
        Expression indexObj = indexExpr.Type == typeof(object) ? indexExpr : Expression.Convert(indexExpr, typeof(object));

        return Expression.Call(
            Global._envParam,
            getItemMethod!,
            arrayObj,
            indexObj
        );
    }

    /// <summary>
    /// 创建数组索引赋值表达式：arr[index] = value
    /// </summary>
    public static Expression MakeArrayIndexAssign(Expression arrayExpr, Expression indexExpr, Expression valueExpr)
    {
        var setItemMethod = typeof(Script2Environment).GetMethod(nameof(Script2Environment.SetArrayItem));

        Expression arrayObj = arrayExpr.Type == typeof(object) ? arrayExpr : Expression.Convert(arrayExpr, typeof(object));
        Expression indexObj = indexExpr.Type == typeof(object) ? indexExpr : Expression.Convert(indexExpr, typeof(object));
        Expression valueObj = valueExpr.Type == typeof(object) ? valueExpr : Expression.Convert(valueExpr, typeof(object));

        var setCall = Expression.Call(
            Global._envParam,
            setItemMethod!,
            arrayObj,
            indexObj,
            valueObj
        );

        // 返回赋值的值
        return Expression.Block(
            typeof(object),
            setCall,
            valueObj
        );
    }
}
