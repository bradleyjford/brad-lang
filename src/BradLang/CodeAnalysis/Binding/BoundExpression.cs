using BradLang.CodeAnalysis.Symbols;

namespace BradLang.CodeAnalysis.Binding;

abstract class BoundExpression : BoundNode
{
    public abstract TypeSymbol Type { get; }
}
