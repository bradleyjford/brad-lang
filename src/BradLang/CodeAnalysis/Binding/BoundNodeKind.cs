using System;

namespace BradLang.CodeAnalysis.Binding
{
    enum BoundNodeKind
    {
        // Statements
        BlockStatement,
        ExpressionStatement,
        IfStatement,
        VariableDeclaration,
        
        // Expressions
        AssignmentExpression,
        BinaryExpression,
        LiteralExpression,       
        TernaryExpression,
        UnaryExpression,
        VariableExpression,
    }
}
