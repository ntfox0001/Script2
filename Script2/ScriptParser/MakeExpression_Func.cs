using System.Linq.Expressions;

namespace Script2.ScriptParser;

internal partial class MakeExpression
{
        public static Expression MakeFuncDecl(string id, string[] paramList, Expression body, bool hasReturn)
        {
            // 保存 hasReturn 标志，然后重置它，以便后续的函数声明可以使用
            bool functionHasReturn = hasReturn;
            Global._hasReturnInFunction.Value = false;

            // 创建参数表达式数组，使用参数名作为参数表达式名称
            var parameters = paramList.Select(paramName =>
                Expression.Parameter(typeof(object), paramName))
                .ToArray();

            // 创建子环境来隔离函数作用域
            var newEnvMethod = typeof(Script2Environment)
                .GetMethod(nameof(Script2Environment.CreateChildEnvironment));

            var newEnvExpr = Expression.Call(Global._envParam, newEnvMethod!);

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
                // 没有return语句，先执行函数体，然后返回void标记值
                var voidField = typeof(VoidValue).GetField("Instance");
                var voidValue = Expression.Field(null, voidField!);

                functionBody = Expression.Block(
                    Expression.Block(newBody),  // 先执行函数体（如 print 等）
                    voidValue                     // 然后返回 void
                );
            }

            // 构建完整的语句列表：赋值newEnv变量 + 参数赋值 + 函数体
            var allStatements = new List<Expression> { Expression.Assign(newEnvVar, newEnvExpr) };
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
                Global._envParam,
                registerMethod!,
                Expression.Constant(id),
                lambdaObjExpr
            );
        }
        
        public static Expression MakeFuncCall(string fn, Expression[] args)
        {
            var argsArrayExpr = Expression.NewArrayInit(typeof(object),
                args.Select(arg => Expression.Convert(arg, typeof(object))));

            var callMethod = typeof(Script2Environment)
                .GetMethod(nameof(Script2Environment.CallFunction));

            var callExpr = Expression.Call(
                Global._envParam,
                callMethod!,
                Expression.Constant(fn),
                argsArrayExpr
            );

            // 明确转换为 object 类型，以确保类型推断正确
            if (callExpr.Type != typeof(object))
                return Expression.Convert(callExpr, typeof(object));
            return callExpr;
        }
}