using System;
using System.Collections.Generic;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax
{
    public sealed class UnaryExpressionSyntax : ExpressionSyntax
    {
        public SyntaxToken OperatorToken { get; }
        public ExpressionSyntax Operand { get; }

        public UnaryExpressionSyntax(SyntaxToken operatorToken, ExpressionSyntax operand)
        {
            OperatorToken = operatorToken;
            Operand = operand;

            Span = TextSpan.FromBounds(operatorToken.Span.Start, operand.Span.End);
        }

        public override SyntaxKind Kind => SyntaxKind.UnaryExpression;

        public override TextSpan Span { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OperatorToken;
            yield return Operand;
        }
    }
}
