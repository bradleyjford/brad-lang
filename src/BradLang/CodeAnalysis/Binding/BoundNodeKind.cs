using System;

namespace BradLang.CodeAnalysis.Binding
{
    public enum BoundNodeKind
    {
        // Statements
        BlockStatement,
        ExpressionStatement,
        VariableDeclaration,
        
        UnaryExpression,
        LiteralExpression,
        BinaryExpression,
        TernaryExpression,
        VariableExpression,
        AssignmentExpression,
    }
}
