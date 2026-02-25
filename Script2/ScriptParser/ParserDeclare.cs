using System.Linq.Expressions;
using Superpower;
using Superpower.Parsers;

namespace Script2.ScriptParser;

internal static class ParserDeclare
{
    static TokenListParser<Script2Token, ExpressionType> Operator(Script2Token op,
        ExpressionType opType)
    {
        return Token.EqualTo(op).Value(opType);
    }

    static readonly TokenListParser<Script2Token, ExpressionType> Add =
        Operator(Script2Token.Plus, ExpressionType.AddChecked);

    static readonly TokenListParser<Script2Token, ExpressionType> Subtract =
        Operator(Script2Token.Minus, ExpressionType.SubtractChecked);

    static readonly TokenListParser<Script2Token, ExpressionType> Multiply =
        Operator(Script2Token.Times, ExpressionType.MultiplyChecked);

    static readonly TokenListParser<Script2Token, ExpressionType> Divide =
        Operator(Script2Token.Divide, ExpressionType.Divide);

    static readonly TokenListParser<Script2Token, ExpressionType> Modulo =
        Operator(Script2Token.Modulo, ExpressionType.Modulo);

    static readonly TokenListParser<Script2Token, ExpressionType> Greater =
        Operator(Script2Token.Greater, ExpressionType.GreaterThan);

    static readonly TokenListParser<Script2Token, ExpressionType> Less =
        Operator(Script2Token.Less, ExpressionType.LessThan);

    static readonly TokenListParser<Script2Token, ExpressionType> GreaterEqual =
        Operator(Script2Token.GreaterEqual, ExpressionType.GreaterThanOrEqual);

    static readonly TokenListParser<Script2Token, ExpressionType> LessEqual =
        Operator(Script2Token.LessEqual, ExpressionType.LessThanOrEqual);

    static readonly TokenListParser<Script2Token, ExpressionType> EqualEqual =
        Operator(Script2Token.EqualEqual, ExpressionType.Equal);

    static readonly TokenListParser<Script2Token, ExpressionType> NotEqual =
        Operator(Script2Token.NotEqual, ExpressionType.NotEqual);

    static readonly TokenListParser<Script2Token, ExpressionType> And =
        Operator(Script2Token.And, ExpressionType.AndAlso);

    static readonly TokenListParser<Script2Token, ExpressionType> Or =
        Operator(Script2Token.Or, ExpressionType.OrElse);

    static readonly TokenListParser<Script2Token, ExpressionType> Not =
        Operator(Script2Token.Not, ExpressionType.Not);

    public static readonly TokenListParser<Script2Token, Expression> Constant =
        Token.EqualTo(Script2Token.Number)
            .Apply(Numerics.Decimal)
            .Select(n => (Expression)Expression.Constant(float.Parse(n.ToStringValue())))
            .Or(
                Token.EqualTo(Script2Token.String)
                    .Select(s =>
                    {
                        // 去掉字符串两端的引号
                        var strValue = s.ToStringValue();
                        if (strValue.Length >= 2 && strValue.StartsWith('"') && strValue.EndsWith('"'))
                        {
                            strValue = strValue.Substring(1, strValue.Length - 2);
                        }

                        return (Expression)Expression.Constant(strValue);
                    })
            )
            .Or(
                Token.EqualTo(Script2Token.True)
                    .Select(t => (Expression)Expression.Constant(true))
            )
            .Or(
                Token.EqualTo(Script2Token.False)
                    .Select(f => (Expression)Expression.Constant(false))
            );

    // 参数列表解析器：只接受标识符，使用 ManyDelimitedBy，支持空列表
    public static readonly TokenListParser<Script2Token, string[]> ParamList =
        Token.EqualTo(Script2Token.Identifier)
            .Select(t => t.ToStringValue())
            .ManyDelimitedBy(Token.EqualTo(Script2Token.Comma));

    public static readonly TokenListParser<Script2Token, Expression[]> ArgList =
        Parse.Ref(() => Expr).ManyDelimitedBy(Token.EqualTo(Script2Token.Comma));

    public static readonly TokenListParser<Script2Token, Expression> FuncDecl =
        from id in Token.EqualTo(Script2Token.Identifier)
        from lp in Token.EqualTo(Script2Token.LParen)
        from paramList in ParamList.Try()
        from rp in Token.EqualTo(Script2Token.RParen)
        from body in Parse.Ref(() => StatementBlock)
        select MakeExpression.MakeFuncDecl(id.ToStringValue(), paramList, body, Global._hasReturnInFunction.Value);

    public static readonly TokenListParser<Script2Token, Expression> FuncCall =
        from fn in Token.EqualTo(Script2Token.Identifier)
        from lp in Token.EqualTo(Script2Token.LParen)
        from expList in ArgList
        from rp in Token.EqualTo(Script2Token.RParen)
        select MakeExpression.MakeFuncCall(fn.ToStringValue(), expList);

    public static readonly TokenListParser<Script2Token, Expression> SetVar =
        from id in Token.EqualTo(Script2Token.Var)
        from varName in Token.EqualTo(Script2Token.Identifier)
        from eq in Token.EqualTo(Script2Token.Equals)
        from value in Parse.Ref(() => Expr)
        select MakeExpression.SetVariable(varName.ToStringValue(), value);

    public static readonly TokenListParser<Script2Token, Expression> ReassignVar =
        from varName in Token.EqualTo(Script2Token.Identifier)
        from eq in Token.EqualTo(Script2Token.Equals)
        from value in Parse.Ref(() => Expr)
        select MakeExpression.ReassignVariable(varName.ToStringValue(), value);

    public static readonly TokenListParser<Script2Token, Expression> GetVar =
        from varName in Token.EqualTo(Script2Token.Identifier)
        select MakeExpression.GetVariable(varName.ToStringValue());

    public static readonly TokenListParser<Script2Token, Expression> Factor =
        (from lparen in Token.EqualTo(Script2Token.LParen)
            from expr in Parse.Ref(() => Expr)
            from rparen in Token.EqualTo(Script2Token.RParen)
            select expr)
        .Or(FuncCall).Try() // 先尝试匹配函数调用
        .Or(GetVar) // 再匹配变量引用
        .Or(Constant);

    public static readonly TokenListParser<Script2Token, Expression> NotFactor =
        from notOp in Token.EqualTo(Script2Token.Not)
        from factor in Factor.Or(Parse.Ref(() => NotFactor))
        select (Expression)Expression.Not(Expression.Convert(factor, typeof(bool)));

    public static readonly TokenListParser<Script2Token, Expression> Operand =
        (from sign in Token.EqualTo(Script2Token.Minus)
            from factor in Factor
            select (Expression)Expression.Negate(factor))
        .Or(NotFactor)
        .Or(Factor).Named("expression");

    public static readonly TokenListParser<Script2Token, Expression> Term1 =
        Parse.Chain(Multiply.Or(Divide).Or(Modulo), Operand, MakeExpression.MakeBinaryWithConversion);

    public static readonly TokenListParser<Script2Token, Expression> Term2 =
        Parse.Chain(Add.Or(Subtract), Term1, MakeExpression.MakeBinaryWithConversion);

    // 添加比较表达式
    public static readonly TokenListParser<Script2Token, Expression> ComparisonExpr =
        Parse.Chain(
            Greater.Or(Less).Or(GreaterEqual).Or(LessEqual).Or(EqualEqual).Or(NotEqual),
            Term2,
            MakeExpression.MakeComparison
        );

    // 添加逻辑与表达式
    public static readonly TokenListParser<Script2Token, Expression> LogicalAndExpr =
        Parse.Chain(And, ComparisonExpr, MakeExpression.MakeLogical);

    // 重命名表达式为逻辑或表达式
    public static readonly TokenListParser<Script2Token, Expression> Expr =
        Parse.Chain(Or, LogicalAndExpr, MakeExpression.MakeLogical);

    public static readonly TokenListParser<Script2Token, Expression> IfStatement =
        Parse.Ref(() => IfStatementImpl);

    public static readonly TokenListParser<Script2Token, Expression> WhileStatement =
        Parse.Ref(() => WhileStatementImpl);

    public static readonly TokenListParser<Script2Token, Expression> ReturnStatement =
        from returnKw in Token.EqualTo(Script2Token.Return)
        from expr in Parse.Ref(() => Expr).OptionalOrDefault(Expression.Constant(null, typeof(object)))
        select MakeExpression.MakeReturnStatement(expr);

    public static readonly TokenListParser<Script2Token, Expression> Statement =
        SetVar.Or(ReassignVar.Try()).Or(IfStatement).Or(WhileStatement).Or(ReturnStatement).Or(Expr);

    public static readonly TokenListParser<Script2Token, Expression> GlobalStatement =
        FuncDecl.Try().Or(Statement);

    public static readonly TokenListParser<Script2Token, Expression> StatementBlock =
        (from lbrace in Token.EqualTo(Script2Token.LBrace)
            from statements in Parse.Ref(() => BlockStatements)
            from rbrace in Token.EqualTo(Script2Token.RBrace)
            select MakeExpression.MakeStatementBlock(statements));

    public static readonly TokenListParser<Script2Token, Expression[]> BlockStatements =
        (from stmt in Statement
            from semicolon in Token.EqualTo(Script2Token.Semicolon).Optional()
            select stmt
        ).Many();

    public static readonly TokenListParser<Script2Token, Expression> IfStatementImpl =
        from ifKeyword in Token.EqualTo(Script2Token.If)
        from lp in Token.EqualTo(Script2Token.LParen)
        from condition in Parse.Ref(() => Expr)
        from rp in Token.EqualTo(Script2Token.RParen)
        from thenBranch in Parse.Ref(() => StatementBlock)
        from elseBranch in (
            from elseKw in Token.EqualTo(Script2Token.Else)
            from elseBlock in Parse.Ref(() => StatementBlock)
            select elseBlock
        ).Or(Parse.Return<Script2Token, Expression>(null))
        select MakeExpression.MakeIfStatement(condition, thenBranch, elseBranch);

    public static readonly TokenListParser<Script2Token, Expression> WhileStatementImpl =
        from whileKeyword in Token.EqualTo(Script2Token.While)
        from lp in Token.EqualTo(Script2Token.LParen)
        from condition in Parse.Ref(() => Expr)
        from rp in Token.EqualTo(Script2Token.RParen)
        from body in Parse.Ref(() => StatementBlock)
        select MakeExpression.MakeWhileStatement(condition, body);

    public static readonly TokenListParser<Script2Token, Expression[]> Program =
        (from stmt in GlobalStatement
            from semicolon in Token.EqualTo(Script2Token.Semicolon).Optional()
            select stmt
        ).Many();

    public static readonly TokenListParser<Script2Token, Expression<Func<Script2Environment, object>>> Lambda =
        Program.AtEnd().Select(statements =>
        {
            // 如果没有语句，返回默认值
            if (statements.Length == 0)
                return Expression.Lambda<Func<Script2Environment, object>>(
                    Expression.Constant(null), Global._envParam);

            // 构建执行语句块
            Expression body;
            if (statements.Length == 1)
            {
                body = statements[0];
            }
            else
            {
                // 如果有多个语句，创建一个块表达式
                var nonLastStatements = statements.Take(statements.Length - 1);
                var lastStatement = statements.Last();
                body = Expression.Block(
                    nonLastStatements.Concat(new[] { lastStatement }).ToArray()
                );
            }

            // 包装在 try-catch 中以捕获 ReturnValueException（脚本级别的 return）
            var returnValueVar = Expression.Variable(typeof(object), "returnValue");
            var exceptionVar = Expression.Variable(typeof(ReturnValueException), "ex");

            // 构建赋值表达式，处理 void 类型
            Expression assignExpr;
            if (body.Type == typeof(void))
            {
                assignExpr = Expression.Block(
                    body,
                    Expression.Assign(returnValueVar, Expression.Constant(null, typeof(object)))
                );
            }
            else
            {
                assignExpr = Expression.Assign(returnValueVar, Expression.Convert(body, typeof(object)));
            }

            var tryCatch = Expression.TryCatch(
                Expression.Block(
                    assignExpr,
                    returnValueVar
                ),
                Expression.Catch(
                    exceptionVar,
                    Expression.Block(
                        Expression.Assign(returnValueVar, Expression.Property(exceptionVar, "ReturnValue")),
                        returnValueVar
                    )
                )
            );

            return Expression.Lambda<Func<Script2Environment, object>>(
                Expression.Block(typeof(object), new[] { returnValueVar }, tryCatch),
                Global._envParam
            );
        });
}