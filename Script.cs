using Superpower.Parsers;
using Superpower;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Superpower.Display;
using Superpower.Model;

namespace IntCalc
{
    enum ArithmeticExpressionToken
    {
        None,

        Number,

        Identifier,

        [Token(Category = "operator", Example = "+")]
        Plus,

        [Token(Category = "operator", Example = "-")]
        Minus,

        [Token(Category = "operator", Example = "*")]
        Times,

        [Token(Category = "operator", Example = "/")]
        Divide,

        [Token(Example = "(")] LParen,

        [Token(Example = ")")] RParen,

        [Token(Example = ",")] Comma
    }

    class ArithmeticExpressionParser
    {
        static TokenListParser<ArithmeticExpressionToken, ExpressionType> Operator(ArithmeticExpressionToken op,
            ExpressionType opType)
        {
            return Token.EqualTo(op).Value(opType);
        }

        static readonly TokenListParser<ArithmeticExpressionToken, ExpressionType> Add =
            Operator(ArithmeticExpressionToken.Plus, ExpressionType.AddChecked);

        static readonly TokenListParser<ArithmeticExpressionToken, ExpressionType> Subtract =
            Operator(ArithmeticExpressionToken.Minus, ExpressionType.SubtractChecked);

        static readonly TokenListParser<ArithmeticExpressionToken, ExpressionType> Multiply =
            Operator(ArithmeticExpressionToken.Times, ExpressionType.MultiplyChecked);

        static readonly TokenListParser<ArithmeticExpressionToken, ExpressionType> Divide =
            Operator(ArithmeticExpressionToken.Divide, ExpressionType.Divide);

        static readonly TokenListParser<ArithmeticExpressionToken, Expression> Constant =
            Token.EqualTo(ArithmeticExpressionToken.Number)
                .Apply(Numerics.IntegerInt32)
                .Select(n => (Expression)Expression.Constant(n));

        static readonly TokenListParser<ArithmeticExpressionToken, string> Identifier =
            Token.EqualTo(ArithmeticExpressionToken.Identifier)
                .Apply(Superpower.Parsers.Identifier.CStyle)
                .Select(s => s.ToString());

        private static readonly TokenListParser<ArithmeticExpressionToken, Expression> FunctionCall =
            (from name in Identifier
             from args in (
                 from lparen in Token.EqualTo(ArithmeticExpressionToken.LParen)
                 from exprList in Parse.Ref(() => ArgList!)
                 from rparen in Token.EqualTo(ArithmeticExpressionToken.RParen)
                 select exprList
             ).Or(Parse.Return<ArithmeticExpressionToken, Expression[]>(Array.Empty<Expression>()))
             select args.Length == 0
                 ? (Expression)Expression.Parameter(typeof(int), name)
                 : MakeFunctionCall(name, args))
            .Named("function or variable");

        private static readonly TokenListParser<ArithmeticExpressionToken, Expression[]> ArgList =
            Parse.Ref(() => Expr!)
                .ManyDelimitedBy(Token.EqualTo(ArithmeticExpressionToken.Comma));

        static Expression MakeFunctionCall(string name, Expression[] args)
        {
            var methodInfo = typeof(Math).GetMethod(name);
            if (methodInfo == null)
                throw new System.InvalidOperationException($"Function '{name}' is not supported.");

            var parameters = methodInfo.GetParameters();
            if (parameters.Length != args.Length)
                throw new System.InvalidOperationException($"Function '{name}' expects {parameters.Length} arguments, got {args.Length}.");

            return Expression.Call(methodInfo, args);
        }

        static readonly TokenListParser<ArithmeticExpressionToken, Expression> Factor =
            (from lparen in Token.EqualTo(ArithmeticExpressionToken.LParen)
                from expr in Parse.Ref(() => Expr!)
                from rparen in Token.EqualTo(ArithmeticExpressionToken.RParen)
                select expr)
            .Or(FunctionCall)
            .Or(Constant);

        static readonly TokenListParser<ArithmeticExpressionToken, Expression> Operand =
            (from sign in Token.EqualTo(ArithmeticExpressionToken.Minus)
                from factor in Factor
                select (Expression)Expression.Negate(factor))
            .Or(Factor).Named("expression");

        static readonly TokenListParser<ArithmeticExpressionToken, Expression> Term =
            Parse.Chain(Multiply.Or(Divide), Operand, Expression.MakeBinary);

        static readonly TokenListParser<ArithmeticExpressionToken, Expression> Expr =
            Parse.Chain(Add.Or(Subtract), Term, Expression.MakeBinary);

        private static readonly TokenListParser<ArithmeticExpressionToken, Expression<Func<int, int>>> Lambda =
            Expr
                .AtEnd()
                .Select(body => Expression.Lambda<Func<int, int>>(body));

        public static int Execute(string expression)
        {
            var tok = new ArithmeticExpressionTokenizer();
            var tokens = tok.Tokenize(expression);
            var expr = Lambda.Parse(tokens);
            var compiled = expr.Compile();
            var result = compiled(0);
            return result;
        }
    }

    class ArithmeticExpressionTokenizer : Tokenizer<ArithmeticExpressionToken>
    {
        readonly Dictionary<char, ArithmeticExpressionToken> _operators =
            new Dictionary<char, ArithmeticExpressionToken>
            {
                ['+'] = ArithmeticExpressionToken.Plus,
                ['-'] = ArithmeticExpressionToken.Minus,
                ['*'] = ArithmeticExpressionToken.Times,
                ['/'] = ArithmeticExpressionToken.Divide,
                ['('] = ArithmeticExpressionToken.LParen,
                [')'] = ArithmeticExpressionToken.RParen,
                [','] = ArithmeticExpressionToken.Comma,
            };

        protected override IEnumerable<Result<ArithmeticExpressionToken>> Tokenize(TextSpan span)
        {
            var next = SkipWhiteSpace(span);
            if (!next.HasValue)
                yield break;

            do
            {
                ArithmeticExpressionToken charToken;

                var ch = next.Value;
                if (ch >= '0' && ch <= '9')
                {
                    var integer = Numerics.Integer(next.Location);
                    next = integer.Remainder.ConsumeChar();
                    yield return Result.Value(ArithmeticExpressionToken.Number, integer.Location, integer.Remainder);
                }
                else if (_operators.TryGetValue(ch, out charToken))
                {
                    yield return Result.Value(charToken, next.Location, next.Remainder);
                    next = next.Remainder.ConsumeChar();
                }
                else if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || ch == '_')
                {
                    var identifier = Character.AnyChar(next.Location);
                    next = identifier.Remainder.ConsumeChar();
                    yield return Result.Value(ArithmeticExpressionToken.Identifier, identifier.Location, identifier.Remainder);
                }
                else
                {
                    yield return Result.Empty<ArithmeticExpressionToken>(next.Location, new[] { "number", "operator", "identifier" });
                }

                next = SkipWhiteSpace(next.Location);
            } while (next.HasValue);
        }
    }
}