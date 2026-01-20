using Superpower;
using Superpower.Model;

namespace Script2
{
    public interface ITokenProcessor
    {
        bool Can(char ch); 
        Result<Script2Token> Process(Result<char> span, out Result<TextSpan> result);
    }
    // 分词器
    public class Script2Tokenizer : Tokenizer<Script2Token>
    {
        private readonly List<ITokenProcessor> _processors = new();

        public Script2Tokenizer(params ITokenProcessor[] processors)
        {
            _processors.AddRange(processors);
        }
    
        protected override IEnumerable<Result<Script2Token>> Tokenize(TextSpan span)
        {
            var next = SkipWhiteSpace(span);
            if (!next.HasValue)
                yield break;

            do
            {
                var ch = next.Value;
                bool proccessed = false;
                foreach (var processor in _processors)
                {
                    if (processor.Can(ch))
                    {
                        var result = processor.Process(next, out var spanResult);
                        if (result.HasValue)
                        {
                            yield return result;
                            next = spanResult.Remainder.ConsumeChar();
                            proccessed = true;
                            break;
                        }
                    }
                }
                
                if (!proccessed)
                {
                    yield return Result.Empty<Script2Token>(next.Location, new[] { "number", "operator", "identifier" });
                }
                next = SkipWhiteSpace(next.Location);
            } while (next.HasValue);
            
            // do
            // {
            //     var ch = next.Value;
            //     if (char.IsDigit(ch) || ch == '.')
            //     {
            //         var integer = Numerics.Decimal(next.Location);
            //         next = integer.Remainder.ConsumeChar();
            //         yield return Result.Value(Script2Token.Number, integer.Location, integer.Remainder);
            //     }
            //     else if (_operators.TryGetValue(ch, out var charToken))
            //     {
            //         // 检查多字符运算符
            //         if (ch == '=' || ch == '>' || ch == '<')
            //         {
            //             var str = Strings.ContainsString(_multiOperatorChars)(next.Location);
            //             if (str.HasValue)
            //             {
            //                 var strVal = str.Location.ToStringValue();
            //                 if (_multiOperators.TryGetValue(strVal, out var token))
            //                 {
            //                     yield return Result.Value(token, str.Location, str.Remainder);
            //                 }
            //                 else
            //                 {
            //                     yield return Result.Empty<Script2Token>(str.Location, new[] { "expected multi-char operator" });
            //                 }
            //                 
            //                 next = str.Remainder.ConsumeChar();
            //             }
            //             else
            //             {
            //                 yield return Result.Value(charToken, next.Location, next.Remainder);
            //                 next = next.Remainder.ConsumeChar();
            //             }
            //         }
            //         else
            //         {
            //             yield return Result.Value(charToken, next.Location, next.Remainder);
            //             next = next.Remainder.ConsumeChar();
            //         }
            //     }
            //     else if (ch == '"')
            //     {
            //         var str = QuotedString.CStyle(next.Location);
            //         if (!str.HasValue)
            //         {
            //             yield return Result.Empty<Script2Token>(str.Location, new[] { "expected quote" });
            //         }
            //         
            //         yield return Result.Value(Script2Token.String, str.Location, str.Remainder);
            //         next = str.Remainder.ConsumeChar();
            //     }
            //     else if (char.IsLetter(ch) || ch == '_')
            //     {
            //         var identifier = Identifier.CStyle(next.Location);
            //         
            //         var idVal = identifier.Location.ToStringValue();
            //         if (_keywords.TryGetValue(idVal, out var token))
            //         {
            //             yield return Result.Value(token, identifier.Location, identifier.Remainder);
            //         }
            //         else
            //         {
            //             yield return Result.Value(Script2Token.Identifier, identifier.Location, identifier.Remainder);
            //         }
            //         next = identifier.Remainder.ConsumeChar();
            //     }
            //     else
            //     {
            //         yield return Result.Empty<Script2Token>(next.Location,
            //             new[] { "number", "operator", "identifier" });
            //     }
            //
            //     next = SkipWhiteSpace(next.Location);
            // } while (next.HasValue);
        }
    }
}