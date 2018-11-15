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
        OpenBraceToken,
        CloseBraceToken,
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

        // Nodes
        CompilationUnit,

        // Keyword
        LetKeyword,
        VarKeyword,
        TrueKeyword,
        FalseKeyword,

        // Statements
        StatementExpression,
        BlockStatement,
        VariableDeclarationStatement,

        // Expressions
        LiteralExpression,
        LiteralStringExpression,
        NameExpression,
        UnaryExpression,
        BinaryExpression,
        TernaryExpression,
        ParenthesizedExpression,
        AssignmentExpression,
    }
}
