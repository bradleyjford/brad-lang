using System.Collections.Immutable;

namespace BradLang.CodeAnalysis.Symbols;

public sealed class FunctionSymbol : Symbol
{
    public FunctionSymbol(string name, ImmutableArray<ParameterSymbol> parameters, TypeSymbol type)
        : base(name)
    {
        Parameters = parameters;
        Type = type;
    }

    public override SymbolKind Kind => SymbolKind.FunctionSymbol;

    public ImmutableArray<ParameterSymbol> Parameters { get; }
    public TypeSymbol Type { get; }
}
