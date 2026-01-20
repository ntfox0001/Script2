using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace Script2
{
    using Expression = System.Linq.Expressions.Expression;

    public static class Script2Parser
    {
        // 静态字段：统一的参数
        private static readonly ParameterExpression _envParam = Expression.Parameter(typeof(Script2Environment), "env");
        
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

        static readonly TokenListParser<Script2Token, ExpressionType> And =
            Operator(Script2Token.And, ExpressionType.AndAlso);

        static readonly TokenListParser<Script2Token, ExpressionType> Or =
            Operator(Script2Token.Or, ExpressionType.OrElse);

        public static readonly TokenListParser<Script2Token, Expression> Constant =
            Token.EqualTo(Script2Token.Number)
                .Apply(Numerics.Decimal)
                .Select(n => (Expression)Expression.Constant(float.Parse(n.ToStringValue())))
                .Or(
                    Token.EqualTo(Script2Token.String)
                        .Select(s => (Expression)Expression.Constant(s.ToStringValue()))
                )
                .Or(
                    Token.EqualTo(Script2Token.True)
                        .Select(t => (Expression)Expression.Constant(true))
                )
                .Or(
                    Token.EqualTo(Script2Token.False)
                        .Select(f => (Expression)Expression.Constant(false))
                );
        
        public static readonly TokenListParser<Script2Token, Expression[]> ArgList = 
            Parse.Ref(() => Expr).ManyDelimitedBy(Token.EqualTo(Script2Token.Comma));

        public static readonly TokenListParser<Script2Token, Expression> FuncCall = 
            from fn in Token.EqualTo(Script2Token.Identifier)
            from lp in Token.EqualTo(Script2Token.LParen)
            from expList in ArgList
            from rp in Token.EqualTo(Script2Token.RParen)
            select MakeFuncCall(fn.ToStringValue(), expList);

        public static readonly TokenListParser<Script2Token, Expression> SetVar = 
            from id in Token.EqualTo(Script2Token.Var)
            from varName in Token.EqualTo(Script2Token.Identifier)
            from eq in Token.EqualTo(Script2Token.Equals)
            from value in Parse.Ref(() => Expr)
            select SetVariable(varName.ToStringValue(), value);

        public static readonly TokenListParser<Script2Token, Expression> GetVar = 
            from varName in Token.EqualTo(Script2Token.Identifier)
            select GetVariable(varName.ToStringValue());

        public static readonly TokenListParser<Script2Token, Expression> Factor = 
            (from lparen in Token.EqualTo(Script2Token.LParen)
                from expr in Parse.Ref(() => Expr)
                from rparen in Token.EqualTo(Script2Token.RParen)
                select expr)
            .Or(FuncCall).Try()  // 先尝试匹配函数调用
            .Or(GetVar)    // 再匹配变量引用
            .Or(Constant);

        public static readonly TokenListParser<Script2Token, Expression> Operand = 
            (from sign in Token.EqualTo(Script2Token.Minus)
                from factor in Factor
                select (Expression)Expression.Negate(factor))
            .Or(Factor).Named("expression");

        public static readonly TokenListParser<Script2Token, Expression> Term1 = 
            Parse.Chain(Multiply.Or(Divide), Operand, Expression.MakeBinary);

        public static readonly TokenListParser<Script2Token, Expression> Term2 = 
            Parse.Chain(Add.Or(Subtract), Term1, MakeBinaryWithConversion);
        
        // 添加比较表达式
        public static readonly TokenListParser<Script2Token, Expression> ComparisonExpr = 
            Parse.Chain(
                Greater.Or(Less).Or(GreaterEqual).Or(LessEqual).Or(EqualEqual), 
                Term2, 
                MakeComparison
            );

        // 添加逻辑与表达式
        public static readonly TokenListParser<Script2Token, Expression> LogicalAndExpr = 
            Parse.Chain(And, ComparisonExpr, MakeLogical);

        // 重命名表达式为逻辑或表达式
        public static readonly TokenListParser<Script2Token, Expression> Expr = 
            Parse.Chain(Or, LogicalAndExpr, MakeLogical);
        
        public static readonly TokenListParser<Script2Token, Expression> IfStatement = 
            Parse.Ref(() => IfStatementImpl);
        
        public static readonly TokenListParser<Script2Token, Expression> Statement = 
            SetVar.Or(IfStatement).Or(Expr);

        public static readonly TokenListParser<Script2Token, Expression> StatementBlock = 
            (from lbrace in Token.EqualTo(Script2Token.LBrace)
                from statements in Parse.Ref(() => Program)
                from rbrace in Token.EqualTo(Script2Token.RBrace)
                select MakeStatementBlock(statements))
            .Or(Statement);
        
        public static readonly TokenListParser<Script2Token, Expression> IfStatementImpl = 
            from ifKeyword in Token.EqualTo(Script2Token.If)
            from condition in Parse.Ref(() => Expr)
            from thenBranch in Parse.Ref(() => StatementBlock)
            from elseBranch in (
                from elseKw in Token.EqualTo(Script2Token.Else)
                from elseBlock in Parse.Ref(() => StatementBlock)
                select elseBlock
            ).Or(Parse.Return<Script2Token, Expression>(null)) 
            select MakeIfStatement(condition, thenBranch, elseBranch);
        
        public static readonly TokenListParser<Script2Token, Expression[]> Program = 
            (from stmt in Statement
                from semicolon in Token.EqualTo(Script2Token.Semicolon).Optional()
                select stmt
            ).Many();
        
        public static readonly TokenListParser<Script2Token, Expression<Func<Script2Environment, object>>> Lambda = 
            Program.AtEnd().Select(statements => 
            {
                // 如果没有语句，返回默认值
                if (statements.Length == 0)
                    return Expression.Lambda<Func<Script2Environment, object>>(
                        Expression.Constant(null), _envParam);
        
                // 如果只有一个语句，直接返回（转换为 object 类型）
                if (statements.Length == 1)
                    return Expression.Lambda<Func<Script2Environment, object>>(
                        Expression.Convert(statements[0], typeof(object)), 
                        _envParam);
        
                // 如果有多个语句，创建一个块表达式
                var nonLastStatements = statements.Take(statements.Length - 1);
                var lastStatement = statements.Last();
                return Expression.Lambda<Func<Script2Environment, object>>(
                    Expression.Block(
                        typeof(object), 
                        nonLastStatements.Concat(new[] { Expression.Convert(lastStatement, typeof(object)) }).ToArray()
                    ), 
                    _envParam
                );
            });

        
        static Expression MakeFuncCall(string fn, Expression[] args)
        {
            var argsArrayExpr = Expression.NewArrayInit(typeof(object),
                args.Select(arg => Expression.Convert(arg, typeof(object))));
            
            var callMethod = typeof(Script2Environment)
                .GetMethod(nameof(Script2Environment.CallFunction));
            
            return Expression.Call(
                _envParam,
                callMethod!,
                Expression.Constant(fn),
                argsArrayExpr
            );
        }
        
        private static Expression SetVariable(string varName, Expression value)
        {
            var variablesExpr = Expression.Property(_envParam, "Variables");
            var varExpr = Expression.Property(
                variablesExpr,
                "Item",
                Expression.Constant(varName)
            );
            return Expression.Assign(
                varExpr,
                Expression.Convert(value, typeof(object))
            );
        }

        private static Expression GetVariable(string varName)
        {
            var getVarMethod = typeof(Script2Environment)
                .GetMethod(nameof(Script2Environment.GetVariableValue));
    
            return Expression.Call(
                _envParam,
                getVarMethod!,
                Expression.Constant(varName)
            );
        }

        // 实现MakeIfStatement方法
        static Expression MakeIfStatement(Expression condition, Expression thenBranch, Expression elseBranch)
        {
            // 将条件转换为bool类型
            var conditionAsBool = Expression.Convert(condition, typeof(bool));
    
            // 如果有else分支
            if (elseBranch != null)
                return Expression.Condition(conditionAsBool, thenBranch, elseBranch);
            else
                // 没有else分支，返回默认值null
                return Expression.Condition(conditionAsBool, thenBranch, Expression.Constant(null, typeof(object)));
        }

        // 实现MakeStatementBlock方法
        private static Expression MakeStatementBlock(Expression[] statements)
        {
            if (statements.Length == 0)
                return Expression.Constant(null, typeof(object));
    
            if (statements.Length == 1)
                return statements[0];
    
            // 返回语句块，最后一个语句作为返回值
            var nonLastStatements = statements.Take(statements.Length - 1);
            var lastStatement = statements.Last();
    
            return Expression.Block(
                typeof(object),
                nonLastStatements.Concat(new[] { Expression.Convert(lastStatement, typeof(object)) }).ToArray()
            );
        }
        
        private static Expression MakeBinaryWithConversion(ExpressionType binaryType, Expression left, Expression right)
        {
            // 统一转换为 decimal 进行运算
            Expression convertedLeft = Expression.Convert(left, typeof(float));
            Expression convertedRight = Expression.Convert(right, typeof(float));
            return Expression.MakeBinary(binaryType, convertedLeft, convertedRight);
        }
        
        private static Expression MakeComparison(ExpressionType comparisonType, Expression left, Expression right)
        {
            // 将左右操作数转换为可比较的类型
            Expression convertedLeft = Expression.Convert(left, typeof(float));
            Expression convertedRight = Expression.Convert(right, typeof(float));
            return Expression.MakeBinary(comparisonType, convertedLeft, convertedRight);
        }
        
        private static Expression MakeLogical(ExpressionType logicalType, Expression left, Expression right)
        {
            // 将左右操作数转换为bool类型
            Expression convertedLeft = Expression.Convert(left, typeof(bool));
            Expression convertedRight = Expression.Convert(right, typeof(bool));
            return Expression.MakeBinary(logicalType, convertedLeft, convertedRight);
        }
        
        public static object Execute(string expression, Script2Environment env)
        {
            var tok = TokenizerBuilder.Build();
            var tokens = tok.Tokenize(expression);
            var lambdaExpr = Lambda.Parse(tokens);
            var compiled = lambdaExpr.Compile();
            return compiled(env);  // ← 传入 env
        }
    }
}
