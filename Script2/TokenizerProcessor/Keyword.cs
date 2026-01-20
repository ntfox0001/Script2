using System.Collections.Generic;
using System.Linq;
using Script2.Parser;
using Superpower.Model;

namespace Script2.TokenizerProcessor
{
    public class Keyword : ITokenProcessor
    {
        private readonly Dictionary<string, Script2Token> _keywords;
        private readonly HashSet<char> _firstChar;
        private readonly string _expectations;
        
        public Keyword(Dictionary<string, Script2Token> keywords, string expectations)
        {
            _keywords = keywords;
            _expectations = expectations;
            _firstChar = new HashSet<char>(_keywords.Keys.Select(o => o[0]));
        }
        
        public bool Can(char ch)
        {
            return _firstChar.Contains(ch);
        }

        public Result<Script2Token> Process(Result<char> span, out Result<TextSpan> result)
        {
            result = Strings.ContainsString(_keywords.Keys)(span.Location);
            if (result.HasValue)
            {
                var strVal = result.Location.ToStringValue();
                if (_keywords.TryGetValue(strVal, out var token))
                {
                    return Result.Value(token, result.Location, result.Remainder);
                }    
            }
            
            return Result.Empty<Script2Token>(span.Location, new[] { _expectations });
        }
    }
}