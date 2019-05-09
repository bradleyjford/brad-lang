using System;

namespace BradLang.CodeAnalysis.Symbols
{
    public sealed class VariableSymbol : Symbol
    {
        internal VariableSymbol(string name, TypeSymbol type, bool isReadOnly)
            : base(name)
        {
            Type = type;
            IsReadOnly = isReadOnly;
        }

        public override SymbolKind Kind => SymbolKind.VariableSymbol;

        public TypeSymbol Type { get; }
        public bool IsReadOnly { get; }
    }
}
