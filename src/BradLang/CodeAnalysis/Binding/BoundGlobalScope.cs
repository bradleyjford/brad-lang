using System.Collections.Immutable;
using BradLang.CodeAnalysis.Symbols;

namespace BradLang.CodeAnalysis.Binding;

internal sealed class BoundGlobalScope
{
    public BoundGlobalScope(
        BoundGlobalScope previous,
        ImmutableArray<VariableSymbol> variables, 
        ImmutableArray<Diagnostic> diagnostics, 
        BoundStatement statement)
    {
        Previous = previous;
        Variables = variables;
        Diagnostics = diagnostics;
        Statement = statement;
    }

    public BoundGlobalScope Previous { get; }
    public ImmutableArray<VariableSymbol> Variables { get; }
    public ImmutableArray<Diagnostic> Diagnostics { get; }
    public BoundStatement Statement { get; }
}
