using System.Linq.Expressions;
using Superpower;
using Superpower.Parsers;

namespace Script2
{
    public static class Script2Parser
    {
        // 静态字段：统一的参数
        private static readonly ParameterExpression _envParam = Expression.Parameter(typeof(Script2Environment), "env");
        private static readonly Dictionary<string, Expression> _funcDecls = new();

        /// <summary>
        /// 表达式访问器，用于替换环境参数
        /// </summary>
        private class EnvParameterReplacer : ExpressionVisitor
        {
            private readonly Expression _newEnv;

            public EnvParameterReplacer(Expression newEnv)
            {
                _newEnv = newEnv;
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Expression == _envParam)
                {
                    return Expression.MakeMemberAccess(_newEnv, node.Member);
                }
                return base.VisitMember(node);
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Object == _envParam)
                {
                    var newArguments = Visit(node.Arguments);
                    return Expression.Call(_newEnv, node.Method, newArguments);
                }
                return base.VisitMethodCall(node);
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (node == _envParam)
                    return _newEnv;
                return base.VisitParameter(node);
            }
        }

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
            select MakeFuncDecl(id.ToStringValue(), paramList, body);
        
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
            .Or(FuncDecl).Try()  // 先尝试匹配函数声明
            .Or(FuncCall).Try()  // 先尝试匹配函数调用
            .Or(GetVar)    // 再匹配变量引用
            .Or(Constant);

        public static readonly TokenListParser<Script2Token, Expression> Operand = 
            (from sign in Token.EqualTo(Script2Token.Minus)
                from factor in Factor
                select (Expression)Expression.Negate(factor))
            .Or(Factor).Named("expression");

        public static readonly TokenListParser<Script2Token, Expression> Term1 = 
            Parse.Chain(Multiply.Or(Divide), Operand, MakeBinaryWithConversion);

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
            from lp in Token.EqualTo(Script2Token.LParen)
            from condition in Parse.Ref(() => Expr)
            from rp in Token.EqualTo(Script2Token.RParen)
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
                {
                    var stmt = statements[0];
                    // 如果是 void 类型，执行它并返回 null
                    if (stmt.Type == typeof(void))
                        return Expression.Lambda<Func<Script2Environment, object>>(
                            Expression.Block(
                                typeof(object),
                                stmt,
                                Expression.Constant(null, typeof(object))
                            ),
                            _envParam);
                    return Expression.Lambda<Func<Script2Environment, object>>(
                        Expression.Convert(stmt, typeof(object)),
                        _envParam);
                }

                // 如果有多个语句，创建一个块表达式
                var nonLastStatements = statements.Take(statements.Length - 1);
                var lastStatement = statements.Last();

                // 如果最后一个语句是 void 类型，执行它并返回 null
                Expression lastExpr;
                if (lastStatement.Type == typeof(void))
                    lastExpr = Expression.Constant(null, typeof(object));
                else
                    lastExpr = Expression.Convert(lastStatement, typeof(object));

                return Expression.Lambda<Func<Script2Environment, object>>(
                    Expression.Block(
                        typeof(object),
                        nonLastStatements.Concat(new[] { lastExpr }).ToArray()
                    ),
                    _envParam
                );
            });

        static Expression MakeFuncDecl(string id, string[] paramList, Expression body)
        {
            // 创建参数表达式数组，使用参数名作为参数表达式名称
            var parameters = paramList.Select(paramName =>
                Expression.Parameter(typeof(object), paramName))
                .ToArray();

            // 创建新的局部变量字典表达式来隔离函数作用域
            var newEnvMethod = typeof(Script2Environment)
                .GetMethod(nameof(Script2Environment.CloneWithVariables));

            var newEnvExpr = Expression.Call(_envParam, newEnvMethod!);

            // 声明一个局部变量来存储新的环境对象
            var newEnvVar = Expression.Variable(typeof(Script2Environment), "newEnv");

            // 为每个参数创建变量赋值
            var argAssignments = new List<Expression>();
            for (int i = 0; i < parameters.Length; i++)
            {
                var variablesExpr = Expression.Property(newEnvVar, "Variables");
                var varExpr = Expression.Property(
                    variablesExpr,
                    "Item",
                    Expression.Constant(paramList[i])
                );
                argAssignments.Add(Expression.Assign(
                    varExpr,
                    parameters[i]
                ));
            }

            // 替换 body 中的 _envParam 为 newEnvVar
            var envReplacer = new EnvParameterReplacer(newEnvVar);
            var newBody = envReplacer.Visit(body);

            // 构建函数体：赋值参数 + 原函数体
            Expression functionBody;
            if (newBody.Type != typeof(object))
            {
                functionBody = Expression.Convert(newBody, typeof(object));
            }
            else
            {
                functionBody = newBody;
            }

            // 构建完整的语句列表：赋值newEnv变量 + 参数赋值 + 函数体
            var allStatements = new List<Expression>();
            allStatements.Add(Expression.Assign(newEnvVar, newEnvExpr));
            allStatements.AddRange(argAssignments);
            allStatements.Add(functionBody);

            var combinedBody = Expression.Block(
                new[] { newEnvVar },
                allStatements.ToArray()
            );

            // 创建闭包委托
            var delegateType = Expression.GetFuncType(
                parameters.Select(p => typeof(object)).Concat(new[] { typeof(object) }).ToArray()
            );

            var lambdaExpr = Expression.Lambda(delegateType, combinedBody, parameters);

            // 调用 RegisterFunction 方法注册函数
            var registerMethod = typeof(Script2Environment)
                .GetMethod(nameof(Script2Environment.RegisterFunction));

            var lambdaObjExpr = Expression.Convert(lambdaExpr, typeof(Delegate));

            return Expression.Call(
                _envParam,
                registerMethod!,
                Expression.Constant(id),
                lambdaObjExpr
            );
        }
        
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
            // 检查左右操作数的类型
            var leftType = left.Type;
            var rightType = right.Type;
    
            // 如果都是float类型，直接比较数值
            if (leftType == typeof(float) && rightType == typeof(float))
            {
                return Expression.MakeBinary(comparisonType, left, right);
            }
    
            // 如果都是字符串类型，直接比较字符串
            if (leftType == typeof(string) && rightType == typeof(string))
            {
                return Expression.MakeBinary(comparisonType, left, right);
            }
    
            // 尝试将非float类型转换为float进行比较
            try
            {
                Expression convertedLeft = leftType == typeof(float) ? left : Expression.Convert(left, typeof(float));
                Expression convertedRight = rightType == typeof(float) ? right : Expression.Convert(right, typeof(float));
                return Expression.MakeBinary(comparisonType, convertedLeft, convertedRight);
            }
            catch
            {
                // 如果转换为float失败，再尝试转换为字符串进行比较
                var stringLeft = leftType == typeof(string) ? left : Expression.Call(left, "ToString", Type.EmptyTypes);
                var stringRight = rightType == typeof(string) ? right : Expression.Call(right, "ToString", Type.EmptyTypes);
                return Expression.MakeBinary(comparisonType, stringLeft, stringRight);
            }
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
