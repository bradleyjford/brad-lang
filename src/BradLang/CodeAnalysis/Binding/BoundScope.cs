using System.Collections.Generic;
using System.Collections.Immutable;

namespace BradLang.CodeAnalysis.Binding
{
    sealed class BoundScope
    {
        readonly Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>();
        readonly Dictionary<string, VariableSymbol> _variables = new Dictionary<string, VariableSymbol>();

        public BoundScope(BoundScope parent)
        {
            Parent = parent;
        }

        public BoundScope Parent { get; }

        public bool TryDelcareMethod(MethodInfo method)
        {
            if (_methods.ContainsKey(method.Name))
            {
                return false;
            }

            _methods.Add(method.Name, method);

            return true;
        }

        public bool TryLookupMethod(string name, out MethodInfo method)
        {
            if (_methods.ContainsKey(name))
            {
                method = _methods[name];
                return true;
            }

            if (Parent != null && Parent.TryLookupMethod(name, out method))
            {
                return true;
            }

            method = null;
            
            return false;
        }

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
}
