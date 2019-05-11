using BradLang.CodeAnalysis.Symbols;

namespace BradLang.CodeAnalysis.Binding
{
    internal class BoundErrorExpression : BoundExpression
    {
        public override TypeSymbol Type => TypeSymbol.Error;

        public override BoundNodeKind Kind => BoundNodeKind.ErrorExpression;
    }
}
