using System;

namespace BradLang.CodeAnalysis.Binding
{
    sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundUnaryOperator @operator, BoundExpression operand)
        {
            Operator = @operator;
            Operand = operand;
        }

        public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
        public override Type Type => Operand.Type;

        public BoundUnaryOperator Operator { get; }
        public BoundExpression Operand { get; }
    }
}
