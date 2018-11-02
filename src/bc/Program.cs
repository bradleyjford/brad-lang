using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BradLang.CodeAnalysis.Syntax;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CommandLine
{
    static class Program
    {
        static void Main()
        {
            var variables = new Dictionary<VariableSymbol, object>();

            var input = new StringBuilder();

            while (true)
            {
                if (input.Length == 0)
                {
                    Console.Write("> ");
                }
                else
                {
                    Console.Write("| ");
                }

                var line = Console.ReadLine();

                if (String.IsNullOrEmpty(line))
                {
                    if (input.Length == 0)
                    {
                        return;
                    }

                    Evaluate(input.ToString(), variables);

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
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine();

                        foreach (var variable in variables)
                        {
                            Console.WriteLine($"    {variable.Key.Name} = {variable.Value}");
                        }

                        Console.WriteLine();
                        Console.ResetColor();
                    }
                    else
                    {
                        input.AppendLine(line);
                    }
                }
            }
        }

        static void Evaluate(string text, IDictionary<VariableSymbol, object> variables)
        {
            var sourceText = SourceText.From(text);
            var syntaxTree = SyntaxTree.Parse(sourceText);

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(syntaxTree.Root.ToString());
            Console.ResetColor();

            var compilation = new Compilation(syntaxTree);

            try
            {
                var result = compilation.Evaluate(variables);

                if (!result.Diagnostics.Any())
                {
                    Console.WriteLine($"= {result.Value}");

                }
                else
                {
                    Console.WriteLine();

                    foreach (var diagnostic in result.Diagnostics)
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
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(e);
                Console.ResetColor();
            }

        }
    }
}
