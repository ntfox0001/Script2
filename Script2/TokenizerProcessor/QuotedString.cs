using System.ComponentModel;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace Script2.TokenizerProcessor
{
    public class QuotedString : ITokenProcessor
    {
        public bool Can(char ch)
        {
            return ch == '"';
        }

        private static readonly TextParser<char> CStringContentChar = Span.EqualTo("\\\"").Value<TextSpan, char>('"')
            .Try<char>().Or<char>(Character.ExceptIn('"', '\\', '\r', '\n'));

        public Result<Script2Token> Process(Result<char> span, out Result<TextSpan> result)
        {
            result = Span.MatchedBy(Character.EqualTo('"').IgnoreThen(
                CStringContentChar
                    .Many()
                    .Then(s => Character.EqualTo('"')
                        .Value(new TextSpan(new string(s))))))(span.Location);
            
            return result.HasValue ? Result.Value(Script2Token.String, result.Location, result.Remainder) :
                Result.Empty<Script2Token>(result.Remainder, new []{ "invalid string" });
        }
    }
}