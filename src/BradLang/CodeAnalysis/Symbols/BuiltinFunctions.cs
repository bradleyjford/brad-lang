using System.Collections.Immutable;

namespace BradLang.CodeAnalysis.Symbols;

static class BuiltinFunctions
{
    public static readonly FunctionSymbol Print =
        new FunctionSymbol("print", ImmutableArray.Create(new ParameterSymbol("text", TypeSymbol.String)), TypeSymbol.Void);

    public static readonly FunctionSymbol Input =
        new FunctionSymbol("input", ImmutableArray.Create<ParameterSymbol>(), TypeSymbol.String);

    public static IEnumerable<FunctionSymbol> GetAll() => new [] { Print, Input };
}
