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

                var syntaxTree = SyntaxTree.Parse(line);

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(syntaxTree.Root.ToString());
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
    }
}
