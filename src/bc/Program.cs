using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BradLang.CodeAnalysis.Syntax;

namespace BradLang.CommandLine
{
    static class Program
    {
        static void Main()
        {
            var variables = new Dictionary<VariableSymbol, object>();

            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();

                if (String.IsNullOrEmpty(line))
                {
                    return;
                }

                if (line == "#cls")
                {
                    Console.Clear();
                    continue;
                }

                if (line == "#variables")
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine();

                    foreach (var variable in variables)
                    {
                        Console.WriteLine($"    {variable.Key.Name} = {variable.Value}");
                    }

                    Console.WriteLine();
                    Console.ResetColor();

                    continue;
                }

                var parser = new Parser(line);

                var syntaxTree = parser.Parse();

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                DumpSyntaxTree(syntaxTree.Root);
                Console.ResetColor();

                var compilation = new Compilation(syntaxTree);

                try
                {
                    var result = compilation.Evaluate(variables);

                    if (result.Diagnostics.Any())
                    {
                        Console.WriteLine();

                        foreach (var diagnostic in result.Diagnostics)
                        {
                            var prefix = line.Substring(0, diagnostic.Span.Start);
                            var error = line.Substring(diagnostic.Span.Start, diagnostic.Span.Length);
                            var suffix = line.Substring(diagnostic.Span.End);

                            Console.ForegroundColor = ConsoleColor.DarkRed;
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

                        continue;
                    }

                    Console.WriteLine($"= {result.Value}");
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(e);
                    Console.ResetColor();
                }
            }
        }

        static void DumpSyntaxTree(SyntaxNode node, string indent = "", bool isLast = true)
        {
            var marker = isLast ? "└──" : "├──";

            Console.Write(indent);
            Console.Write(marker);
            Console.Write(node.Kind);

            if (node is SyntaxToken t && t.Value != null)
            {
                Console.Write(" ");
                Console.Write(t.Value);
            }

            Console.WriteLine();

            indent += isLast ? "   " : "│  ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
                DumpSyntaxTree(child, indent, child == lastChild);
        }
    }
}
