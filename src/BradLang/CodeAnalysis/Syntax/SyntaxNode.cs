using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public abstract IEnumerable<SyntaxNode> GetChildren();

        public abstract TextSpan Span { get; }

        public SyntaxToken GetLastToken()
        {
            if (this is SyntaxToken token)
            {
                return token;
            }

            // A syntax node should always contain at least 1 token.
            return GetChildren().Last().GetLastToken();
        }

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
