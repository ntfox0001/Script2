using System.Linq.Expressions;
using Script2.ScriptParser;
using Superpower;
using Superpower.Parsers;

namespace Script2
{
    public static class Script2Parser
    {
        /// <summary>
        /// 是否使用解释器模式
        /// true: 使用解释器模式（兼容 IL2CPP，但速度较慢）
        /// false: 使用编译模式（快速，但需要 .NET 运行时支持 Expression.Compile）
        /// 默认为 false，在 Unity IL2CPP 环境中请手动设置为 true
        /// </summary>
        public static bool UseInterpreterMode { get; set; } = false;

        public static object Execute(string expression, Script2Environment env)
        {
            var tok = TokenizerBuilder.Build();
            var tokens = tok.Tokenize(expression);
            var lambdaExpr = ParserDeclare.Lambda.Parse(tokens);

            if (UseInterpreterMode)
            {
                // 解释器模式（兼容 IL2CPP）
                return ExpressionInterpreter.Interpret(lambdaExpr, env);
            }
            else
            {
                // 编译模式（快速，需要 .NET 运行时支持）
                var compiled = lambdaExpr.Compile();
                return compiled(env);
            }
        }

        public static object CallFunc(Script2Environment env, string fn, params object[] args)
        {
            var argsStr = string.Join(',', args.Select(arg => arg.ToLowercaseString()));
            return Execute($"{fn}({argsStr})", env);
        }
    }
}
