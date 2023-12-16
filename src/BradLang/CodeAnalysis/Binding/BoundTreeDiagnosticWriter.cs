using System.IO;
using System.Linq;

namespace BradLang.CodeAnalysis.Binding;

internal sealed class BoundTreeDiagnosticWriter
{
    public static void Write(TextWriter writer, BoundNode node)
    {
        Write(writer, node, "", true);

        writer.WriteLine();
    }

    private static void Write(TextWriter writer, BoundNode node, string indent, bool isLast)
    {
        var isToConsole = writer == Console.Out;

        var marker = isLast ? "└──" : "├──";

        if (isToConsole)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }

        writer.Write(indent);
        writer.Write(marker);

        if (isToConsole)
        {
            Console.ForegroundColor = GetColor(node);
        }

        var text = GetText(node);
        writer.Write(text);

        var isFirstProperty = true;

        foreach (var p in node.GetProperties())
        {
            if (isFirstProperty)
            {
                isFirstProperty = false;
            }
            else
            {
                if (isToConsole)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }

                writer.Write(",");
            }

            writer.Write(" ");

            if (isToConsole)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }

            writer.Write(p.Name);

            if (isToConsole)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
            }

            writer.Write(" = ");

            if (isToConsole)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            }

            writer.Write(p.Value);
        }

        if (isToConsole)
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

    private static string GetText(BoundNode node)
    {
        if (node is BoundBinaryExpression b)
        {
            return b.Operator.Kind.ToString() + "Expression";
        }

        if (node is BoundUnaryExpression u)
        {
            return u.Operator.Kind.ToString() + "Expression";
        }

        return node.Kind.ToString();
    }

    private static ConsoleColor GetColor(BoundNode node)
    {
        if (node is BoundExpression)
            return ConsoleColor.Blue;

        if (node is BoundStatement)
            return ConsoleColor.Cyan;

        return ConsoleColor.Yellow;
    }
}
