using BradLang.CodeAnalysis.Symbols;

namespace BradLang.CodeAnalysis.Binding;

sealed class BoundUnaryExpression : BoundExpression
{
    public BoundUnaryExpression(BoundUnaryOperator @operator, BoundExpression operand)
    {
        Operator = @operator;
        Operand = operand;
    }

    public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
    public override TypeSymbol Type => Operand.Type;

    public BoundUnaryOperator Operator { get; }
    public BoundExpression Operand { get; }
}
