namespace BradLang.CodeAnalysis.Symbols
{
    public sealed class ParameterSymbol : VariableSymbol
    {
        public ParameterSymbol(string name, TypeSymbol type)
            : base(name, type, true)
        {

        }

        public override SymbolKind Kind => SymbolKind.ParameterSymbol;
    }
}
