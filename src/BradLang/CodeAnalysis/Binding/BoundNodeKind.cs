using System;

namespace BradLang.CodeAnalysis.Binding
{
    public enum BoundNodeKind
    {
        UnaryExpression,
        LiteralExpression,
        BinaryExpression,
        TernaryExpression,
        VariableExpression,
        AssignmentExpression
    }
}
