using Script2.Parser;
using Superpower.Model;

namespace Script2.TokenizerProcessor
{
    public class Operator : ITokenProcessor
    {
        private readonly Dictionary<string, Script2Token> _keywords;
        private readonly HashSet<char> _firstChar;
        private readonly string _expectations;
        private readonly int _len;
        
        public Operator(Dictionary<string, Script2Token> keywords, int len, string expectations)
        {
            _keywords = keywords;
            _len = len;
            _expectations = expectations;
            _firstChar = new HashSet<char>(_keywords.Keys.Select(o => o[0]));
        }
        
        public bool Can(char ch)
        {
            return _firstChar.Contains(ch);
        }

        public Result<Script2Token> Process(Result<char> span, out Result<TextSpan> result)
        {
            result = Strings.ContainsStringByLen(_keywords.Keys, _len)(span.Location);
            if (result.HasValue)
            {
                var strVal = result.Value.ToStringValue();
                if (_keywords.TryGetValue(strVal, out var token))
                {
                    return Result.Value(token, result.Location, result.Remainder);
                }    
            }
            
            return Result.Empty<Script2Token>(span.Location, new[] { _expectations });
        }
    }    
}
