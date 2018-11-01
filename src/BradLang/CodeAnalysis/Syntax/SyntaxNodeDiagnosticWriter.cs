using System.IO;
using System.Linq;

namespace BradLang.CodeAnalysis.Syntax
{
    sealed class SyntaxNodeDiagnosticWriter
    {
        public static void DumpSyntaxTree(TextWriter writer, SyntaxNode node, string indent = "", bool isLast = true)
        {
            var marker = isLast ? "└──" : "├──";

            writer.Write(indent);
            writer.Write(marker);
            writer.Write(node.Kind);

            if (node is SyntaxToken t && t.Value != null)
            {
                writer.Write(" ");
                writer.Write(t.Value);
            }

            writer.WriteLine();

            indent += isLast ? "   " : "│  ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
            {
                DumpSyntaxTree(writer, child, indent, child == lastChild);
            }
        }
    }
}
