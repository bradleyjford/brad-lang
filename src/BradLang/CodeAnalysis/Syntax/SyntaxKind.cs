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
        SemicolonToken,
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
        ElseKeyword,
        IfKeyword,
        LetKeyword,
        VarKeyword,
        TrueKeyword,
        FalseKeyword,

        // Statements
        BlockStatement,
        ElseClause,
        IfStatement,
        StatementExpression,
        VariableDeclarationStatement,


        // Expressions
        AssignmentExpression,
        BinaryExpression,
        LiteralExpression,
        LiteralStringExpression,
        NameExpression,
        ParenthesizedExpression,
        TernaryExpression,
        UnaryExpression,
    }
}
