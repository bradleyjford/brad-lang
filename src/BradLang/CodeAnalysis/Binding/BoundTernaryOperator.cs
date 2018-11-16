using System;
using BradLang.CodeAnalysis.Syntax;

namespace BradLang.CodeAnalysis.Binding
{
    sealed class BoundTernaryOperator
    {
        BoundTernaryOperator(Type conditionType, Type trueType, Type falseType)
        {
            ConditionType = conditionType;
            TrueType = trueType;
            FalseType = falseType;
        }

        public SyntaxKind SyntaxKind => SyntaxKind.TernaryExpression;
        public Type Type => TrueType;
        
        public Type ConditionType { get; }
        public Type TrueType { get; }
        public Type FalseType { get; }

        public static BoundTernaryOperator Bind(SyntaxKind kind, Type conditionType, Type trueType, Type falseType)
        {
            if (conditionType == typeof(bool) && trueType == falseType)
            {
                return new BoundTernaryOperator(conditionType, trueType, falseType);
            }

            return null;
        }
    }
}
