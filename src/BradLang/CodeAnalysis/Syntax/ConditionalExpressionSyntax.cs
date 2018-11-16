using System.Collections.Generic;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax
{
    public class ConditionalExpressionSyntax : ExpressionSyntax
    {
        public ConditionalExpressionSyntax(
            ExpressionSyntax conditionExpression, 
            SyntaxToken questionMarkToken, 
            ExpressionSyntax trueExpression, 
            SyntaxToken colonToken,
            ExpressionSyntax falseExpression)
        {
            ConditionExpression = conditionExpression;
            QuestionMarkToken = questionMarkToken;
            TrueExpression = trueExpression;
            ColonToken = colonToken;
            FalseExpression = falseExpression;

            Span = TextSpan.FromBounds(conditionExpression.Span.Start, falseExpression.Span.End);
        }

        public override SyntaxKind Kind => SyntaxKind.TernaryExpression;
        public override TextSpan Span { get; }

        public ExpressionSyntax ConditionExpression { get; }
        public SyntaxToken QuestionMarkToken { get; }
        public ExpressionSyntax TrueExpression { get; }
        public SyntaxToken ColonToken { get; }
        public ExpressionSyntax FalseExpression { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return ConditionExpression;
            yield return QuestionMarkToken;
            yield return TrueExpression;
            yield return ColonToken;
            yield return FalseExpression;
        }
    }
}
