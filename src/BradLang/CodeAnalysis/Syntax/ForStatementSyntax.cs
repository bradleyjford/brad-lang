using System.Collections.Generic;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax
{
    public sealed class ForStatementSyntax : StatementSyntax
    {
        public ForStatementSyntax(
            SyntaxToken forKeywordToken, 
            SyntaxToken identifierToken, 
            SyntaxToken equalsToken, 
            ExpressionSyntax lowerBoundExpression, 
            SyntaxToken toKeywordToken, 
            ExpressionSyntax upperBoundExpression, 
            StatementSyntax body)
        {
            ForKeywordToken = forKeywordToken;
            IdentifierToken = identifierToken;
            EqualsToken = equalsToken;
            LowerBoundExpression = lowerBoundExpression;
            ToKeywordToken = toKeywordToken;
            UpperBoundExpression = upperBoundExpression;
            Body = body;

            Span = new TextSpan(forKeywordToken.Span.Start, upperBoundExpression.Span.End);
        }

        public override SyntaxKind Kind => SyntaxKind.ForStatement;
        public override TextSpan Span { get;}

        public SyntaxToken ForKeywordToken { get; }
        public SyntaxToken IdentifierToken { get; }
        public SyntaxToken EqualsToken { get; }
        public ExpressionSyntax LowerBoundExpression { get; }
        public SyntaxToken ToKeywordToken { get; }
        public ExpressionSyntax UpperBoundExpression { get; }
        public StatementSyntax Body { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return ForKeywordToken;
            yield return IdentifierToken;
            yield return EqualsToken;
            yield return LowerBoundExpression;
            yield return ToKeywordToken;
            yield return UpperBoundExpression;
            yield return Body;
        }
    }
}
