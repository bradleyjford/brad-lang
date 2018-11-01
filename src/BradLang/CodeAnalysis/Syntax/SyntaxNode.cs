using System;
using System.Collections.Generic;
using System.IO;

namespace BradLang.CodeAnalysis.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public abstract IEnumerable<SyntaxNode> GetChildren();

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                SyntaxNodeDiagnosticWriter.DumpSyntaxTree(writer, this);

                return writer.ToString();
            }
        }
    }
}
