using Superpower.Model;

namespace Script2.TokenizerProcessor
{
    public class Identifier : ITokenProcessor
    {
        public bool Can(char ch)
        {
            return char.IsLetter(ch) || ch == '_';
        }

        public Result<Script2Token> Process(Result<char> span, out Result<TextSpan> result)
        {
            result = Superpower.Parsers.Identifier.CStyle(span.Location);
            return Result.Value(Script2Token.Identifier, result.Location, result.Remainder);
        }
    }    
}
