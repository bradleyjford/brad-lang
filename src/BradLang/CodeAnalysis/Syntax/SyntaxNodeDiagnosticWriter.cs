using System.IO;
using System.Linq;

namespace BradLang.CodeAnalysis.Syntax;

public sealed class SyntaxNodeDiagnosticWriter
{
    public static void Write(TextWriter writer, SyntaxNode node, string indent = "", bool isLast = true)
    {
        if (node == null)
        {
            return;
        }

        var isConsoleWriter = writer == Console.Out;

        var marker = isLast ? "└──" : "├──";

        writer.Write(indent);
        writer.Write(marker);

        if (isConsoleWriter)
        {
            Console.ForegroundColor = node is SyntaxToken ? ConsoleColor.Blue : ConsoleColor.Cyan;
        }
            
        writer.Write(node.Kind);

        if (node is SyntaxToken t && t.Value != null)
        {
            writer.Write(" ");
            writer.Write(t.Value);
        }

        if (isConsoleWriter)
        {
            Console.ResetColor();
        }

        writer.WriteLine();

        indent += isLast ? "   " : "│  ";

        var lastChild = node.GetChildren().LastOrDefault();

        foreach (var child in node.GetChildren())
        {
            Write(writer, child, indent, child == lastChild);
        }
    }
}
