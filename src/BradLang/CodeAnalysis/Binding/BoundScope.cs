using System.Collections.Immutable;
using BradLang.CodeAnalysis.Symbols;

namespace BradLang.CodeAnalysis.Binding;

sealed class BoundScope
{
    readonly Dictionary<string, VariableSymbol> _variables = new Dictionary<string, VariableSymbol>();

    public BoundScope(BoundScope parent)
    {
        Parent = parent;
    }

    public BoundScope Parent { get; }

    public bool TryDeclareVariable(VariableSymbol variable)
    {
        if (_variables.ContainsKey(variable.Name))
        {
            return false;
        }

        _variables.Add(variable.Name, variable);

        return true;
    }

    public bool TryLookupVariable(string name, out VariableSymbol variable)
    {
        if (_variables.ContainsKey(name))
        {
            variable = _variables[name];
            return true;
        }

        if (Parent != null && Parent.TryLookupVariable(name, out variable))
        {
            return true;
        }

        variable = null;
            
        return false;
    }

    public ImmutableArray<VariableSymbol> GetDeclaredVariables()
    {
        return _variables.Values.ToImmutableArray();
    }
}
