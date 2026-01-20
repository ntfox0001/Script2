using Superpower.Display;

namespace Script2
{
    public enum Script2Token
    {
        None,

        Number,
        String,
        Identifier,

        [Token(Category = "build-in func", Example = "wait")]
        Wait,
        
        [Token(Category = "keyword", Example = "var")]
        Var,
        [Token(Category = "keyword", Example = "if")]
        If,
        [Token(Category = "keyword", Example = "else")]
        Else,
        [Token(Category = "keyword", Example = "true")]
        True,
        [Token(Category = "keyword", Example = "false")]
        False,
        [Token(Category = "keyword", Example = "and")]
        And,
        [Token(Category = "keyword", Example = "or")]
        Or,
        
        [Token(Category = "operator", Example = "+")]
        Plus,
        [Token(Category = "operator", Example = "-")]
        Minus,
        [Token(Category = "operator", Example = "*")]
        Times,
        [Token(Category = "operator", Example = "/")]
        Divide,
        [Token(Category = "operator", Example = "=")]
        Equals,
        
        [Token(Category = "operator", Example = ">")]
        Greater,
        [Token(Category = "operator", Example = "<")]
        Less,
        [Token(Category = "operator", Example = ">=")]
        GreaterEqual,
        [Token(Category = "operator", Example = "<=")]
        LessEqual,
        [Token(Category = "operator", Example = "==")]
        EqualEqual,
        
        [Token(Category = "sign", Example = "(")]
        LParen,
        [Token(Category = "sign", Example = ")")]
        RParen,
        [Token(Category = "sign", Example = ",")]
        Comma,
        [Token(Category = "sign", Example = ";")]
        Semicolon,
        [Token(Category = "sign", Example = "{")]
        LBrace,
        [Token(Category = "sign", Example = "}")]
        RBrace,
    }
}