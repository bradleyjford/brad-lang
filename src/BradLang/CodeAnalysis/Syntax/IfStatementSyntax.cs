using System.Collections.Generic;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax
{
    public sealed class IfStatementSyntax : StatementSyntax
    {
        public IfStatementSyntax(
            SyntaxToken ifKeywordToken, 
            SyntaxToken openParenthesisToken, 
            ExpressionSyntax conditionExpression, 
            SyntaxToken closeParenthesisToken, 
            StatementSyntax thenStatement,
            ElseClauseSyntax elseClause)
        {
            IfKeywordToken = ifKeywordToken;
            OpenParenthesisToken = openParenthesisToken;
            ConditionExpression = conditionExpression;
            CloseParenthesisToken = closeParenthesisToken;
            ThenStatement = thenStatement;
            ElseClause = elseClause;

            Span = TextSpan.FromBounds(ifKeywordToken.Span.Start, closeParenthesisToken.Span.End);
        }

        public override SyntaxKind Kind => SyntaxKind.IfStatement;
        public override TextSpan Span { get; }

        public SyntaxToken IfKeywordToken { get; }
        public SyntaxToken OpenParenthesisToken { get; }
        public ExpressionSyntax ConditionExpression { get; }
        public SyntaxToken CloseParenthesisToken { get; }
        public StatementSyntax ThenStatement { get; }
        public ElseClauseSyntax ElseClause { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return IfKeywordToken;
            yield return OpenParenthesisToken;
            yield return ConditionExpression;
            yield return CloseParenthesisToken;
            yield return ThenStatement;

            if (ElseClause != null)
            {
                yield return ElseClause;
            }
        }
    }
}
