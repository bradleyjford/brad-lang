using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BradLang.CodeAnalysis;
using BradLang.CodeAnalysis.Syntax;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CommandLine
{
    sealed class BradLangRepl : Repl
    {
        private Compilation _compilation;
        private Dictionary<VariableSymbol, object> variables = new Dictionary<VariableSymbol, object>();
        private bool _showTree;
        private bool _showProgram;

        protected override void EvaluateMetaCommand(string command)
        {
            if (command == "#cls")
            {
                Console.Clear();
            }
            else if (command == "#variables")
            {
                WriteVariables(variables);
            }
            else if (command == "#showTree")
            {
                _showTree = !_showTree;
            }
            else if (command == "#showProgram")
            {
                _showProgram = !_showProgram;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Unknown command {command}.");
                Console.ResetColor();
            }
        }

        static void WriteVariables(Dictionary<VariableSymbol, object> variables)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine();

            foreach (var variable in variables)
            {
                Console.WriteLine($"    {variable.Key.Name} = {variable.Value}");
            }

            Console.WriteLine();
            Console.ResetColor();
        }

        protected override void EvaluateCommand(string command)
        {
            var sourceText = SourceText.From(command);
            var syntaxTree = SyntaxTree.Parse(sourceText);

            if (_showTree)
            {
                SyntaxNodeDiagnosticWriter.Write(Console.Out, syntaxTree.Root);
            }

            var compilation = _compilation == null
                ? new Compilation(syntaxTree)
                : _compilation.ContinueWith(syntaxTree);

            try
            {
                var result = compilation.Evaluate(variables);

                if (!result.Diagnostics.Any())
                {
                    if (_showProgram)
                    {
                        compilation.EmitTree(Console.Out);
                    }

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"= {result.Value}");
                    Console.ResetColor();

                    _compilation = compilation;
                }
                else
                {
                    WriteDiagnostics(sourceText, result.Diagnostics);
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(e);
                Console.ResetColor();
            }
        }

        protected override bool IsCompleteDocument(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return true;
            }

            var lastTwoLinesAreBlank = text.Split(Environment.NewLine)
                .Reverse()
                .TakeWhile(s => string.IsNullOrEmpty(s))
                .Take(2)
                .Count() == 2;

            if (lastTwoLinesAreBlank)
            {
                return true;
            }

            var syntaxTree = SyntaxTree.Parse(text);

            // Use Statement because we need to exclude the EndOfFileToken.
            //if (syntaxTree.Root.Statement.GetLastToken().IsMissing)
            //    return false;

            return true;
        }

        protected override void RenderLine(string line)
        {
            var tokens = SyntaxTree.ParseTokens(line);

            foreach (var token in tokens)
            {
                if (token.Kind.IsKeyword())
                    Console.ForegroundColor = ConsoleColor.Blue;
                else if (token.Kind.IsIdentifier())
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                else if (token.Kind.IsNumber())
                    Console.ForegroundColor = ConsoleColor.Cyan;
                else if (token.Kind.IsString())
                    Console.ForegroundColor = ConsoleColor.Magenta;
                else
                    Console.ForegroundColor = ConsoleColor.DarkGray;

                Console.Write(token.Text);

                Console.ResetColor();
            }
        }

        static void WriteDiagnostics(SourceText sourceText, ImmutableArray<Diagnostic> diagnostics)
        {
            Console.WriteLine();

            foreach (var diagnostic in diagnostics)
            {
                var lineIndex = sourceText.GetLineIndex(diagnostic.Span.Start);
                var line = sourceText.Lines[lineIndex];
                var columnNumber = diagnostic.Span.Start - line.Start + 1;

                var prefixSpan = TextSpan.FromBounds(line.Start, diagnostic.Span.Start);
                var suffixSpan = TextSpan.FromBounds(diagnostic.Span.End, line.End);

                var prefix = sourceText.ToString(prefixSpan.Start, prefixSpan.Length);
                var error = sourceText.ToString(diagnostic.Span.Start, diagnostic.Span.Length);
                var suffix = sourceText.ToString(suffixSpan.Start, suffixSpan.Length);

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write($"({lineIndex + 1}, {columnNumber}): ");
                Console.WriteLine(diagnostic);

                Console.ResetColor();
                Console.Write("    ");
                Console.Write(prefix);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write(error);
                Console.ResetColor();
                Console.WriteLine(suffix);
            }

            Console.WriteLine();
        }
    }
}
