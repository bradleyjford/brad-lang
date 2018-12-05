using System;

namespace BradLang.CodeAnalysis
{
    public sealed class VariableSymbol
    {
        internal VariableSymbol(string name, Type type, bool isReadOnly)
        {
            Name = name;
            Type = type;
            IsReadOnly = isReadOnly;
        }

        public string Name { get; }
        public Type Type { get; }
        public bool IsReadOnly { get; }

        public override string ToString() => Name;
    }
}
