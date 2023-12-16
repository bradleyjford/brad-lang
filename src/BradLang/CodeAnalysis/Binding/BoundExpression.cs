using BradLang.CodeAnalysis.Symbols;

namespace BradLang.CodeAnalysis.Binding;

internal abstract class BoundExpression : BoundNode
{
    public abstract TypeSymbol Type { get; }
}
