using System;
using System.Collections.Generic;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax
{
    public sealed class LiteralStringExpressionSyntax : ExpressionSyntax
    {
        public LiteralStringExpressionSyntax(SyntaxToken stringToken)
            : this(stringToken, (string)stringToken.Value)
        {
        }

        public LiteralStringExpressionSyntax(SyntaxToken stringToken, string value)
        {
            StringToken = stringToken;
            Value = value;

            Span = stringToken.Span;
        }

        public override SyntaxKind Kind => SyntaxKind.LiteralStringExpression;

        public override TextSpan Span { get; }

        public SyntaxToken StringToken { get; }
        public string Value { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return StringToken;
        }
    }
}
