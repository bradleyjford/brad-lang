using System;

namespace BradLang
{
    public sealed class VariableSymbol
    {
        internal VariableSymbol(string name, Type type, bool isConstant)
        {
            Name = name;
            Type = type;
            IsConstant = isConstant;
        }

        public string Name { get; }
        public Type Type { get; }
        public bool IsConstant { get; }
    }
}
