using System.Linq.Expressions;
using Script2.ScriptParser;
using Superpower;
using Superpower.Parsers;

namespace Script2
{
    public static class Script2Parser
    {
        public static object Execute(string expression, Script2Environment env)
        {
            var tok = TokenizerBuilder.Build();
            var tokens = tok.Tokenize(expression);
            var lambdaExpr = ParserDeclare.Lambda.Parse(tokens);

            if (env.UseInterpreterMode)
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
