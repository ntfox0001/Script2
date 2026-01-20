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

        public static TextParser<TextSpan> CStyle { get; } = Span.MatchedBy<char[]>(Character.Letter
            .Or<char>(Character.EqualTo('_'))
            .IgnoreThen<char, char[]>(Character.LetterOrDigit.Or<char>(Character.EqualTo('_')).Many<char>()));

        public static TextParser<string> CStyleTS { get; } = Character.EqualTo('"')
            .IgnoreThen<char, char[]>(CStringContentChar.Many<char>())
            .Then<char[], string>(
                (Func<char[], TextParser<string>>)(s => Character.EqualTo('"').Value<char, string>(new string(s))));

        private TextParser<TextSpan> _quotedStr = Span.MatchedBy(Character.EqualTo('"').IgnoreThen(CStringContentChar
            .Many()
            .Then(s => Character.EqualTo('"').Value(new TextSpan(new string(s))))));
        public Result<Script2Token> Process(Result<char> span, out Result<TextSpan> result)
        {
            result = _quotedStr(span.Location);
            return Result.Value(Script2Token.String, result.Location, result.Remainder);
        }
    }
}