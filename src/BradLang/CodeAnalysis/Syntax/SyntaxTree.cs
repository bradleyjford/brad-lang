using System;
using System.Collections.Generic;
using System.Linq;

namespace BradLang.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }
        public IReadOnlyCollection<Diagnostic> Diagnostics { get; }

        public SyntaxTree(ExpressionSyntax root, SyntaxToken endOfFileToken, IEnumerable<Diagnostic> diagnostics)
        {
            Root = root;
            EndOfFileToken = endOfFileToken;
            Diagnostics = diagnostics.ToArray();
        }
    }
}
