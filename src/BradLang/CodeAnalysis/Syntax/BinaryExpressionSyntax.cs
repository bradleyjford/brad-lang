using System;
using System.Collections.Generic;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax
{
    public sealed class BinaryExpressionSyntax : ExpressionSyntax
    {
        public BinaryExpressionSyntax(ExpressionSyntax leftExpression, SyntaxToken operatorToken, ExpressionSyntax rightExpression)
        {
            LeftExpression = leftExpression;
            OperatorToken = operatorToken;
            RightExpression = rightExpression;

            Span = TextSpan.FromBounds(leftExpression.Span.Start, rightExpression.Span.End);
        }

        public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

        public override TextSpan Span { get; }

        public ExpressionSyntax LeftExpression { get; }
        public SyntaxToken OperatorToken { get; }
        public ExpressionSyntax RightExpression { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LeftExpression;
            yield return OperatorToken;
            yield return RightExpression;
        }
    }
}
