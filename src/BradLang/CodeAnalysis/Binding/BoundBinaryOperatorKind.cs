using System;

namespace BradLang.CodeAnalysis.Binding
{
    enum BoundBinaryOperatorKind
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,
        
        LessThan,
        LessThanEquals,
        GreaterThan,
        GreaterThanEquals,
        
        Equals,
        NotEquals,
        LogicalAnd,
        LogicalOr,
        Modulus,
        BitwiseOr,
        BitwiseAnd,
        BitwiseXor
    }
}
