using System.Collections.Generic;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax
{
    public sealed class ReturnStatementSyntax : StatementSyntax
    {
        public ReturnStatementSyntax(SyntaxToken returnKeywordToken, ExpressionSyntax valueExpression)
        {
            ReturnKeywordToken = returnKeywordToken;
            ValueExpression = valueExpression;

            Span = new TextSpan(returnKeywordToken.Span.Start, valueExpression.Span.End);
        }

        public override SyntaxKind Kind => SyntaxKind.ReturnStatement;
        public override TextSpan Span { get; }

        public SyntaxToken ReturnKeywordToken { get; }
        public ExpressionSyntax ValueExpression { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return ReturnKeywordToken;
            yield return ValueExpression;
        }
    }
}
