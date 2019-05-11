using System;

namespace BradLang.CodeAnalysis.Binding
{
    internal enum BoundNodeKind
    {
        // Statements
        BlockStatement,
        ExpressionStatement,
        ForStatement,
        IfStatement,
        VariableDeclaration,
        WhileStatement,

        // Expressions
        AssignmentExpression,
        BinaryExpression,
        CallExpression,
        ConditionalGotoStatement,
        ErrorExpression,
        GotoStatement,
        LabelStatement,
        LiteralExpression,
        TernaryExpression,
        UnaryExpression,
        VariableExpression,
    }
}
