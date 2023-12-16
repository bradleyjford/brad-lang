using BradLang.CodeAnalysis.Symbols;
using BradLang.CodeAnalysis.Syntax;

namespace BradLang.CodeAnalysis.Binding;

internal sealed class BoundTernaryOperator
{
    private BoundTernaryOperator(TypeSymbol conditionType, TypeSymbol trueType, TypeSymbol falseType)
    {
        ConditionType = conditionType;
        TrueType = trueType;
        FalseType = falseType;
    }

    public SyntaxKind SyntaxKind => SyntaxKind.TernaryExpression;
    public TypeSymbol Type => TrueType;
        
    public TypeSymbol ConditionType { get; }
    public TypeSymbol TrueType { get; }
    public TypeSymbol FalseType { get; }

    public static BoundTernaryOperator Bind(SyntaxKind kind, TypeSymbol conditionType, TypeSymbol trueType, TypeSymbol falseType)
    {
        if (conditionType == TypeSymbol.Bool && trueType == falseType)
        {
            return new BoundTernaryOperator(conditionType, trueType, falseType);
        }

        return null;
    }
}
