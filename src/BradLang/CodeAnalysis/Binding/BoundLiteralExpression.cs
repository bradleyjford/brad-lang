using System;

namespace BradLang.CodeAnalysis.Binding
{
    sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(object value)
        {
            Value = value;
        }

        public object Value { get; }

        public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
        public override Type Type => Value?.GetType() ?? typeof(object);
    }
}