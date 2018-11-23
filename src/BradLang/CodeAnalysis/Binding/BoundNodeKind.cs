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
        MethodDeclaration,
        ReturnStatement,
        VariableDeclaration,
        WhileStatement,
        
        // Expressions
        AssignmentExpression,
        BinaryExpression,
        LiteralExpression,       
        MethodInvocationExpression,
        TernaryExpression,
        UnaryExpression,
        VariableExpression,
    }
}
