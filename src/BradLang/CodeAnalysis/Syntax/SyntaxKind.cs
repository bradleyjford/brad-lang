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
        PlusPlusToken,
        MinusToken,
        MinusMinusToken,
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
        FalseKeyword,
        ForKeyword,
        IfKeyword,
        LetKeyword,
        ReturnKeyword,
        ToKeyword,
        TrueKeyword,
        VarKeyword,
        WhileKeyword,

        // Statements
        BlockStatement,
        ElseClause,
        ForStatement,
        IfStatement,
        MethodDeclaration,
        MethodInvocation,
        ReturnStatement,
        StatementExpression,
        VariableDeclarationStatement,
        WhileStatement,

        // Expressions
        AssignmentExpression,
        BinaryExpression,
        LiteralExpression,
        LiteralStringExpression,
        NameExpression,
        ParenthesizedExpression,
        TernaryExpression,
        UnaryExpression,
        PostfixUnaryExpression,
    }
}
