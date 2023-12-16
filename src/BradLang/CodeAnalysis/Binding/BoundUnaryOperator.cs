using BradLang.CodeAnalysis.Symbols;
using BradLang.CodeAnalysis.Syntax;

namespace BradLang.CodeAnalysis.Binding;

sealed class BoundUnaryOperator
{
    static readonly BoundUnaryOperator[] Operators =
    {
        new BoundUnaryOperator(SyntaxKind.BangToken, BoundUnaryOperatorKind.LogicalNegation, TypeSymbol.Bool),
        new BoundUnaryOperator(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, TypeSymbol.Int),
        new BoundUnaryOperator(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, TypeSymbol.Int),
        new BoundUnaryOperator(SyntaxKind.PlusPlusToken, BoundUnaryOperatorKind.PrefixIncrement, TypeSymbol.Int),
        new BoundUnaryOperator(SyntaxKind.MinusMinusToken, BoundUnaryOperatorKind.PrefixDecrement, TypeSymbol.Int),
        new BoundUnaryOperator(SyntaxKind.TildeToken, BoundUnaryOperatorKind.OnesCompliment, TypeSymbol.Int),
    };

    BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, TypeSymbol operandType)
        : this(syntaxKind, kind, operandType, operandType)
    {
    }

    BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, TypeSymbol operandType, TypeSymbol resultType)
    {
        SyntaxKind = syntaxKind;
        Kind = kind;
        OperandType = operandType;
        Type = resultType;
    }

    public SyntaxKind SyntaxKind { get; }
    public BoundUnaryOperatorKind Kind { get; }
    public TypeSymbol OperandType { get; }
    public TypeSymbol Type { get; }

    public static BoundUnaryOperator Bind(SyntaxKind syntaxKind, TypeSymbol operandType)
    {
        foreach (var op in Operators)
        {
            if (op.SyntaxKind == syntaxKind && op.OperandType == operandType)
            {
                return op;
            }
        }

        return null;
    }
}
