using System;
using BradLang.CodeAnalysis.Binding;

namespace BradLang.CodeAnalysis
{
    public sealed class MethodInfo
    {
        internal MethodInfo(string name, Type returnType, VariableSymbol parameter, BoundStatement body)
        {
            Name = name;
            ReturnType = returnType;
            Parameter = parameter;
            Body = body;
        }

        public string Name { get; }
        public Type ReturnType { get; }
        public VariableSymbol Parameter { get; }
        internal BoundStatement Body { get; }
    }
}
