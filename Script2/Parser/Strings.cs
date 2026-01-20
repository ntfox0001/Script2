using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace Script2.Parser
{
    public static class Strings
    {
        public static TextParser<char> ContainsChar(ICollection<char> contains)
        {
            return Character.Matching(new Func<char, bool>(contains.Contains), "ContainsChar");
        }

        public static TextParser<TextSpan> Keyword(ICollection<string> contains)
        {
            return Span.MatchedBy(Character.Letter.Many())
                .Where(span => 
                    contains.Contains(span.ToString()));
        }
        
        /// <summary>
        /// 只匹配指定长度的字符串
        /// </summary>
        /// <param name="contains"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static TextParser<TextSpan> ContainsStringByLen(ICollection<string> contains, int len)
        {
            return Span.MatchedBy(Character.AnyChar.Repeat(len))
                .Where(span => 
                    contains.Contains(span.ToString()));
        }
        
        public static TextParser<char> CStringContentChar { get; } = Span.EqualTo("\\\"").Value<TextSpan, char>('"')
            .Try<char>().Or<char>(Character.ExceptIn('"', '\\', '\r', '\n'));
        
        public static TextParser<TextSpan> QuotedString { get; } = Span.MatchedBy(Character.EqualTo('"').IgnoreThen(CStringContentChar
            .Many()
            .Then(s => Character.EqualTo('"').Value(new TextSpan(new string(s))))));
    }
}