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
        // 标记当前解析的函数是否有return语句
        // 使用 ThreadLocal 是因为：1) 解析器是静态的，需要在线程间隔离状态；2) ReturnStatement 在解析时需要修改外层 FuncDecl 的状态
        private static readonly ThreadLocal<bool> _hasReturnInFunction = new(() => false);

        /// <summary>
        /// 表达式访问器，用于替换环境参数
        /// 将全局的 _envParam 替换为函数的局部环境变量 newEnvVar
        ///
        /// 为什么 _envParam 有时是 node、有时是 object、有时是 expression？
        /// - _envParam 本身永远是一个 ParameterExpression（env 参数）
        /// - 但它出现在表达式树的不同位置：
        ///   1. VisitMember: node.Expression == _envParam，表示成员访问的宿主对象是 env（如 env.SetVariableValue）
        ///   2. VisitMethodCall: node.Object == _envParam，表示方法调用的对象是 env（如 env.CallFunction）
        ///   3. VisitParameter: node == _envParam，表示参数引用本身就是 env（如直接传递 env 参数）
        /// </summary>
        private class EnvParameterReplacer : ExpressionVisitor
        {
            private readonly Expression _newEnv;

            public EnvParameterReplacer(Expression newEnv)
            {
                _newEnv = newEnv;
            }

            /// <summary>
            /// 处理成员访问表达式（如 env.SomeProperty）
            /// 示例：
            ///   原始表达式: env.Variables
            ///   替换后:    newEnvVar.Variables
            /// </summary>
            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Expression == _envParam)
                {
                    return Expression.MakeMemberAccess(_newEnv, node.Member);
                }
                return base.VisitMember(node);
            }

            /// <summary>
            /// 处理方法调用表达式（如 env.Method(args)）
            /// 示例：
            ///   原始表达式: env.SetVariableValue("x", 123)
            ///   替换后:    newEnvVar.SetVariableValue("x", 123)
            /// </summary>
            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Object == _envParam)
                {
                    var newArguments = Visit(node.Arguments);
                    return Expression.Call(_newEnv, node.Method, newArguments);
                }
                return base.VisitMethodCall(node);
            }

            /// <summary>
            /// 处理参数表达式（直接引用参数本身）
            /// 示例：
            ///   原始表达式: env（作为参数传递给其他函数）
            ///   替换后:    newEnvVar
            /// </summary>
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
                        .Select(s => {
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
            select MakeFuncDecl(id.ToStringValue(), paramList, body, _hasReturnInFunction.Value);
        
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
        
        public static readonly TokenListParser<Script2Token, Expression> ReturnStatement =
            from returnKw in Token.EqualTo(Script2Token.Return)
            from expr in Parse.Ref(() => Expr).OptionalOrDefault(Expression.Constant(null, typeof(object)))
            select MakeReturnStatement(expr);

        public static readonly TokenListParser<Script2Token, Expression> Statement =
            SetVar.Or(IfStatement).Or(ReturnStatement).Or(Expr);

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
                    _envParam
                );
            });

        static Expression MakeFuncDecl(string id, string[] paramList, Expression body, bool hasReturn)
        {
            // 保存 hasReturn 标志，然后重置它，以便后续的函数声明可以使用
            bool functionHasReturn = hasReturn;
            _hasReturnInFunction.Value = false;

            // 创建参数表达式数组，使用参数名作为参数表达式名称
            var parameters = paramList.Select(paramName =>
                Expression.Parameter(typeof(object), paramName))
                .ToArray();

            // 创建子环境来隔离函数作用域
            var newEnvMethod = typeof(Script2Environment)
                .GetMethod(nameof(Script2Environment.CreateChildEnvironment));

            var newEnvExpr = Expression.Call(_envParam, newEnvMethod!);

            // 声明一个局部变量来存储新的环境对象
            var newEnvVar = Expression.Variable(typeof(Script2Environment), "newEnv");

            // 为每个参数创建变量赋值
            var argAssignments = new List<Expression>();
            for (int i = 0; i < parameters.Length; i++)
            {
                var setVarMethod = typeof(Script2Environment)
                    .GetMethod(nameof(Script2Environment.SetVariableValue));

                argAssignments.Add(Expression.Call(
                    newEnvVar,
                    setVarMethod!,
                    Expression.Constant(paramList[i]),
                    parameters[i]
                ));
            }

            // 替换 body 中的 _envParam 为 newEnvVar
            var envReplacer = new EnvParameterReplacer(newEnvVar);
            var newBody = envReplacer.Visit(body);

            // 构建函数体：赋值参数 + 原函数体
            Expression functionBody;

            if (functionHasReturn)
            {
                // 有return语句，使用 try-catch 捕获 ReturnValueException
                var returnValueVar = Expression.Variable(typeof(object), "returnValue");
                var exceptionVar = Expression.Variable(typeof(ReturnValueException), "ex");

                var tryCatch = Expression.TryCatch(
                    Expression.Block(
                        Expression.Assign(returnValueVar, Expression.Convert(newBody, typeof(object))),
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

                functionBody = Expression.Block(typeof(object), new[] { returnValueVar }, tryCatch);
            }
            else
            {
                // 没有return语句，返回void标记值
                var voidField = typeof(VoidValue).GetField("Instance");
                var voidValue = Expression.Field(null, voidField!);
                functionBody = voidValue;
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

            var callExpr = Expression.Call(
                _envParam,
                callMethod!,
                Expression.Constant(fn),
                argsArrayExpr
            );

            // 明确转换为 object 类型，以确保类型推断正确
            if (callExpr.Type != typeof(object))
                return Expression.Convert(callExpr, typeof(object));
            return callExpr;
        }
        
        private static Expression SetVariable(string varName, Expression value)
        {
            // 检查是否为 void 表达式（VoidValue.Instance）
            if (value.NodeType == ExpressionType.MemberAccess &&
                ((MemberExpression)value).Member.DeclaringType == typeof(VoidValue) &&
                ((MemberExpression)value).Member.Name == "Instance")
            {
                throw new InvalidOperationException($"Cannot assign void value to variable '{varName}'. Functions that don't return a value cannot be assigned to variables.");
            }

            // 检查值类型是否为 void
            if (value.Type == typeof(void))
            {
                throw new InvalidOperationException($"Cannot assign void value to variable '{varName}'. Functions that don't return a value cannot be assigned to variables.");
            }

            var setVarMethod = typeof(Script2Environment)
                .GetMethod(nameof(Script2Environment.SetVariableValue));

            Expression valueToAssign;
            // 如果值已经是 object 类型，直接使用
            if (value.Type == typeof(object))
                valueToAssign = value;
            else
            {
                try
                {
                    valueToAssign = Expression.Convert(value, typeof(object));
                }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException($"Cannot assign value to variable '{varName}': {ex.Message}", ex);
                }
            }

            return Expression.Call(
                _envParam,
                setVarMethod!,
                Expression.Constant(varName),
                valueToAssign
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

        // 实现MakeReturnStatement方法
        static Expression MakeReturnStatement(Expression expr)
        {
            // 标记函数有return语句
            _hasReturnInFunction.Value = true;

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

        // 实现MakeIfStatement方法
        static Expression MakeIfStatement(Expression condition, Expression thenBranch, Expression elseBranch)
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

        // 实现MakeStatementBlock方法
        private static Expression MakeStatementBlock(Expression[] statements)
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

        public static object CallFunc(Script2Environment env, string fn, params object[] args)
        {
            var argsStr = string.Join(',', args.Select(arg => arg.ToLowercaseString()));
            return Execute($"{fn}({argsStr})", env);
        }
    }
}
