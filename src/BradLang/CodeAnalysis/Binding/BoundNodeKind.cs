using System;

namespace BradLang.CodeAnalysis.Binding
{
    enum BoundNodeKind
    {
        // Statements
        BlockStatement,
        ExpressionStatement,
        IfStatement,
        ForStatement,
        VariableDeclaration,
        WhileStatement,
        
        // Expressions
        AssignmentExpression,
        BinaryExpression,
        LiteralExpression,       
        TernaryExpression,
        UnaryExpression,
        VariableExpression,
    }
}
