using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        internal SyntaxTree(ExpressionSyntax root, SyntaxToken endOfFileToken, ImmutableArray<Diagnostic> diagnostics)
        {
            Root = root;
            EndOfFileToken = endOfFileToken;
            Diagnostics = diagnostics;
        }

        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
    }
}
