using System;
using System.Collections.Generic;
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

            var diagnostics = Syntax.Diagnostics.Concat(binder.Diagnostics).ToArray();

            var evaluator = new Evaluator(boundExpression, variables);

            var result = evaluator.Evaluate();

            return new EvaluationResult(diagnostics, result);
        }
    }

    public sealed class EvaluationResult
    {
        public IReadOnlyList<Diagnostic> Diagnostics { get; }
        public object Value { get; }

        public EvaluationResult(IEnumerable<Diagnostic> diagnostics, object value)
        {
            Diagnostics = diagnostics.ToArray();
            Value = value;
        }
    }
}
