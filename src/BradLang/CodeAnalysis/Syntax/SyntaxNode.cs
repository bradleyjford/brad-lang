using System;
using System.Collections.Generic;
using System.IO;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public abstract IEnumerable<SyntaxNode> GetChildren();

        public abstract TextSpan Span { get; }

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                SyntaxNodeDiagnosticWriter.Write(writer, this);

                return writer.ToString();
            }
        }
    }
}
