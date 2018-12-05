using System;

namespace BradLang.CodeAnalysis.Binding
{
    enum BoundNodeKind
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
        Label,
        GotoStatement,
        ConditionalGotoStatement,
    }
}
