using Superpower.Model;
using Superpower.Parsers;

namespace Script2.TokenizerProcessor
{
    public class Float : ITokenProcessor
    {
        public bool Can(char ch)
        {
            return ch == '.' || char.IsDigit(ch);
        }

        public Result<Script2Token> Process(Result<char> span, out Result<TextSpan> result)
        {
            result = Numerics.Decimal(span.Location);
            return Result.Value(Script2Token.Number, result.Location, result.Remainder);
        }
    }    
}
