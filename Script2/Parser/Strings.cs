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

        public static TextParser<TextSpan> ContainsString(ICollection<string> contains)
        {
            return Span.WithoutAny(char.IsWhiteSpace).Where(span => contains.Contains(span.ToString()));
        }
    }
}