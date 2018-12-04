using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using BradLang.CodeAnalysis.Binding;
using BradLang.CodeAnalysis.Syntax;

namespace BradLang.CodeAnalysis
{
    public sealed class Compilation
    {
        BoundGlobalScope _globalScope;

        public Compilation(SyntaxTree syntaxTree)
            : this(syntaxTree, null)
        {
        }

        Compilation(SyntaxTree syntaxTree, Compilation previous)
        {
            SyntaxTree = syntaxTree;
            Previous = previous;
        }

        public SyntaxTree SyntaxTree { get; }
        public Compilation Previous { get; }

        public Compilation ContinueWith(SyntaxTree syntaxTree)
        {
            return new Compilation(syntaxTree, this);
        }

        BoundGlobalScope GlobalScope 
        {
            get
            {
                if (_globalScope == null)
                {
                    var globalScope = Binder.BindGlobalScope(Previous?.GlobalScope, SyntaxTree);

                    Interlocked.CompareExchange(ref _globalScope, globalScope, null);
                }

                return _globalScope;
            }
        }

        public EvaluationResult Evaluate(IDictionary<VariableSymbol, object> variables)
        {
            var globalScope = GlobalScope;

            var diagnostics = SyntaxTree.Diagnostics.Concat(globalScope.Diagnostics).ToImmutableArray();

            if (diagnostics.Any())
            {
                return new EvaluationResult(diagnostics, null);
            }

            var evaluator = new Evaluator(globalScope.Statement, variables);

            var result = evaluator.Evaluate();

            return new EvaluationResult(ImmutableArray<Diagnostic>.Empty, result);
        }

        public void EmitTree(TextWriter writer)
        {
            var isConsole = writer == Console.Out;

            BoundTreeDiagnosticWriter.Write(writer, GlobalScope.Statement);
        }
    }
}
