using System.Collections.Generic;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax
{
    public class ConditionalExpressionSyntax : ExpressionSyntax
    {
        public ConditionalExpressionSyntax(
            ExpressionSyntax condition, 
            SyntaxToken questionMarkToken, 
            ExpressionSyntax @true, 
            SyntaxToken colonToken,
            ExpressionSyntax @false)
        {
            Condition = condition;
            QuestionMarkToken = questionMarkToken;
            True = @true;
            ColonToken = colonToken;
            False = @false;

            Span = new TextSpan(condition.Span.Start, @false.Span.End);
        }

        public ExpressionSyntax Condition { get; }
        public SyntaxToken QuestionMarkToken { get; }
        public ExpressionSyntax True { get; }
        public SyntaxToken ColonToken { get; }
        public ExpressionSyntax False { get; }

        public override SyntaxKind Kind => SyntaxKind.TernaryExpression;

        public override TextSpan Span { get; }
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Condition;
            yield return QuestionMarkToken;
            yield return True;
            yield return ColonToken;
            yield return False;
        }
    }
}
