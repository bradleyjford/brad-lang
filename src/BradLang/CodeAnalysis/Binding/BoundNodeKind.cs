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
        LiteralExpression,
        TernaryExpression,
        UnaryExpression,
        VariableExpression,
        LabelStatement,
        GotoStatement,
        ConditionalGotoStatement,
    }
}
