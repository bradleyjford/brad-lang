using System;

namespace BradLang.CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        // Tokens
        UnknownToken,
        EndOfFileToken,
        WhiteSpaceToken,

        NumberToken,

        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        PercentToken,

        ColonToken,
        QuestionMarkToken,

        OpenParenthesisToken,
        CloseParenthesisToken,

        LessThanToken,
        LessThanEqualsToken,
        GreaterThanToken,
        GreaterThanEqualsToken,

        EqualsToken,
        EqualsEqualsToken,

        BangToken,
        BangEqualsToken,

        AmpersandToken,
        AmpersandAmpersandToken,
        PipeToken,
        PipePipeToken,

        IdentifierToken,

        StringToken,

        // Keyword
        TrueKeyword,
        FalseKeyword,

        // Expressions
        LiteralExpression,
        LiteralStringExpression,
        NameExpression,
        UnaryExpression,
        BinaryExpression,
        TernaryExpression,
        ParenthesizedExpression,
        AssignmentExpression
    }
}
