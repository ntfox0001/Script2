using Script2.TokenizerProcessor;

namespace Script2
{
    public static class TokenizerBuilder
    {
        private static readonly Dictionary<string, Script2Token> Operators =
            new()
            {
                ["+"] = Script2Token.Plus,
                ["-"] = Script2Token.Minus,
                ["*"] = Script2Token.Times,
                ["/"] = Script2Token.Divide,
                ["="] = Script2Token.Equals,
                ["=="] = Script2Token.EqualEqual,
                [">"] = Script2Token.Greater,
                ["<"] = Script2Token.Less,
                [">="] = Script2Token.GreaterEqual,
                ["<="] = Script2Token.LessEqual,
                ["var"] = Script2Token.Var,
                ["if"] = Script2Token.If,
                ["else"] = Script2Token.Else,
                ["wait"] = Script2Token.Wait,
                ["true"] = Script2Token.True,
                ["false"] = Script2Token.False,
                ["and"] = Script2Token.And,
                ["or"] = Script2Token.Or,
                
            };
        public static Script2Tokenizer Build()
        {
            var tokenizer = new Script2Tokenizer(
                new Float(),
                new Keyword(Operators, "operator")
                );
            return tokenizer;
        }
    }
}