using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using BradLang.CodeAnalysis;
using BradLang.CodeAnalysis.Syntax;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CommandLine
{
    static class Program
    {
        static Compilation _compilation = null;

        static void Main()
        {
            var showTree = false;
            var showProgram = false;

            var variables = new Dictionary<VariableSymbol, object>();

            var input = new StringBuilder();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;

                if (input.Length == 0)
                {
                    Console.Write("» ");
                }
                else
                {
                    Console.Write("· ");
                }

                Console.ResetColor();

                var line = Console.ReadLine();

                if (String.IsNullOrEmpty(line))
                {
                    if (input.Length == 0)
                    {
                        return;
                    }

                    Evaluate(input.ToString(), variables, showTree, showProgram);

                    input.Clear();
                }
                else
                {
                    if (line == "#cls")
                    {
                        Console.Clear();
                    }
                    else if (line == "#variables")
                    {
                        WriteVariables(variables);
                    }
                    else if (line == "#showTree")
                    {
                        showTree = !showTree;
                    }
                    else if (line == "#showProgram")
                    {
                        showProgram = !showProgram;
                    }
                    else
                    {
                        input.AppendLine(line);
                    }
                }
            }
        }

        static void Evaluate(string text, IDictionary<VariableSymbol, object> variables, bool showTree, bool showProgram)
        {
            var sourceText = SourceText.From(text);
            var syntaxTree = SyntaxTree.Parse(sourceText);

            if (showTree)
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
                    if (showProgram)
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
