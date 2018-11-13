using System.Collections.Immutable;
using BradLang.CodeAnalysis.Syntax;

namespace BradLang.CodeAnalysis.Binding
{
    class BoundGlobalScope
    {
        public BoundGlobalScope(
            BoundGlobalScope previous,
            ImmutableArray<VariableSymbol> variables, 
            ImmutableArray<Diagnostic> diagnostics, 
            BoundExpression expression)
        {
            Previous = previous;
            Variables = variables;
            Diagnostics = diagnostics;
            Expression = expression;
        }

        public BoundGlobalScope Previous { get; }
        public ImmutableArray<VariableSymbol> Variables { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public BoundExpression Expression { get; }
    }
}
