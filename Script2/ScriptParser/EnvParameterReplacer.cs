using System.Linq.Expressions;

namespace Script2.ScriptParser;

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
        internal class EnvParameterReplacer : ExpressionVisitor
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
                if (node.Expression == Global._envParam)
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
                if (node.Object == Global._envParam)
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
                if (node == Global._envParam)
                    return _newEnv;
                return base.VisitParameter(node);
            }
        }