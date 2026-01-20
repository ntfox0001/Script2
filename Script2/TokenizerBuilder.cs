using Script2.TokenizerProcessor;

namespace Script2
{
    public static class TokenizerBuilder
    {
        private static readonly Dictionary<string, Script2Token> SingleOperators =
            new()
            {
                ["+"] = Script2Token.Plus,
                ["-"] = Script2Token.Minus,
                ["*"] = Script2Token.Times,
                ["/"] = Script2Token.Divide,
                
                ["="] = Script2Token.Equals,
                
                [">"] = Script2Token.Greater,
                ["<"] = Script2Token.Less,
                
                [","] = Script2Token.Comma,
                [";"] = Script2Token.Semicolon,
                
                ["{"] = Script2Token.LBrace,
                ["}"] = Script2Token.RBrace,
                
                ["("] = Script2Token.LParen,
                [")"] = Script2Token.RParen,
            };
        
        private static readonly Dictionary<string, Script2Token> TwoCharOperators =
            new()
            {
                ["=="] = Script2Token.EqualEqual,
                [">="] = Script2Token.GreaterEqual,
                ["<="] = Script2Token.LessEqual,
            };
        
        private static readonly Dictionary<string, Script2Token> Keywords =
            new()
            {
                ["var"] = Script2Token.Var,
                
                ["or"] = Script2Token.Or,
                ["and"] = Script2Token.And,
                
                ["if"] = Script2Token.If,
                ["else"] = Script2Token.Else,
                
                ["wait"] = Script2Token.Wait,
                
                ["true"] = Script2Token.True,
                ["false"] = Script2Token.False,
            };
        public static Script2Tokenizer Build()
        {
            var tokenizer = new Script2Tokenizer(
                new Float(),
                new QuotedString(),
                new Operator(TwoCharOperators, 2, "operator"),
                new Operator(SingleOperators, 1, "operator"),
                new Keyword(Keywords, "keyword"),
                new Identifier()
            );
            return tokenizer;
        }
    }
}