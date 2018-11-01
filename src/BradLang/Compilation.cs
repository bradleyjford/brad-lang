using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using BradLang.CodeAnalysis.Binding;
using BradLang.CodeAnalysis.Syntax;

namespace BradLang
{
    public sealed class Compilation
    {
        public SyntaxTree Syntax { get; }

        public Compilation(SyntaxTree syntax)
        {
            Syntax = syntax;
        }

        public EvaluationResult Evaluate(IDictionary<VariableSymbol, object> variables)
        {
            var binder = new Binder(Syntax, variables);

            var boundExpression = binder.Bind();

            var diagnostics = Syntax.Diagnostics.Concat(binder.Diagnostics).ToImmutableArray();

            var evaluator = new Evaluator(boundExpression, variables);

            var result = evaluator.Evaluate();

            return new EvaluationResult(diagnostics, result);
        }
    }

    public sealed class EvaluationResult
    {
        public EvaluationResult(ImmutableArray<Diagnostic> diagnostics, object value)
        {
            Diagnostics = diagnostics;
            Value = value;
        }

        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public object Value { get; }
    }
}
