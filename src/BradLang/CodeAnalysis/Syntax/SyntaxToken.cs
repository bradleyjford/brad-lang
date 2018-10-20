using System;
using System.Collections.Generic;
using System.Linq;

namespace BradLang.CodeAnalysis.Syntax
{
    public sealed class SyntaxToken : SyntaxNode
    {
        public SyntaxToken(SyntaxKind kind, int position, string text, object value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;

            Span = new TextSpan(position, text?.Length ?? 1);
        }

        public override SyntaxKind Kind { get; }
        public int Position { get; }

        public TextSpan Span { get; }
        public string Text { get; }
        public object Value { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }
}