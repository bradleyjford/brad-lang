using System.Collections.Generic;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax
{
    public sealed class PostfixUnaryExpressionSyntax : ExpressionSyntax
    {
        public PostfixUnaryExpressionSyntax(ExpressionSyntax operand, SyntaxToken operatorToken)
        {
            Operand = operand;
            OperatorToken = operatorToken;

            Span = new TextSpan(operand.Span.Start, operatorToken.Span.End);
        }

        public ExpressionSyntax Operand { get; }
        public SyntaxToken OperatorToken { get; }

        public override SyntaxKind Kind => SyntaxKind.PostfixUnaryExpression;
        public override TextSpan Span { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Operand;
            yield return OperatorToken;
        }
    }
}
