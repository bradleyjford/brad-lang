using System;
using System.Collections.Generic;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax
{
    public sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        public ParenthesizedExpressionSyntax(SyntaxToken openParenthesisToken, ExpressionSyntax expression, SyntaxToken closeParenthesisToken)
        {
            OpenParenthesisToken = openParenthesisToken;
            Expression = expression;
            CloseParenthesisToken = closeParenthesisToken;

            Span = TextSpan.FromBounds(openParenthesisToken.Span.Start, closeParenthesisToken.Span.End);
        }

        public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;
        public override TextSpan Span { get; }

        public SyntaxToken OpenParenthesisToken { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken CloseParenthesisToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenParenthesisToken;
            yield return Expression;
            yield return CloseParenthesisToken;
        }
    }
}
