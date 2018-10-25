using System;
using System.Collections.Generic;
using System.Linq;

namespace BradLang.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        public static SyntaxTree Parse(string text)
        {
            var parser = new Parser(text);

            return parser.Parse();
        }

        internal SyntaxTree(ExpressionSyntax root, SyntaxToken endOfFileToken, IEnumerable<Diagnostic> diagnostics)
        {
            Root = root;
            EndOfFileToken = endOfFileToken;
            Diagnostics = diagnostics.ToArray();
        }

        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }
        public IReadOnlyCollection<Diagnostic> Diagnostics { get; }
    }
}
