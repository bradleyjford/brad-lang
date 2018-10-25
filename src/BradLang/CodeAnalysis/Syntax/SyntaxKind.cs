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

        OpenParenthesisToken,
        CloseParenthesisToken,

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
        BinaryExpression,
        ParenthesizedExpression,
        UnaryExpression,

        AssignmentExpression
    }
}