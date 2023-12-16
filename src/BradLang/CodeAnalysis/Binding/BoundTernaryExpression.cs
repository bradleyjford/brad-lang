using BradLang.CodeAnalysis.Symbols;

namespace BradLang.CodeAnalysis.Binding;

internal sealed class BoundTernaryExpression : BoundExpression
{
    public BoundTernaryExpression(BoundTernaryOperator @operator, BoundExpression condition, BoundExpression trueExpression, BoundExpression falseExpression)
    {
        Operator = @operator;
        Condition = condition;
        True = trueExpression;
        False = falseExpression;
    }

    public override BoundNodeKind Kind => BoundNodeKind.TernaryExpression;
    public override TypeSymbol Type => Operator.Type;

    public BoundTernaryOperator Operator { get; }
    public BoundExpression Condition { get; }
    public BoundExpression True { get; }
    public BoundExpression False { get; }
}
