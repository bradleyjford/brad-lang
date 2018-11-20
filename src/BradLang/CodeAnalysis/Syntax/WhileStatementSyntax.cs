using System.Collections.Generic;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax
{
    public sealed class WhileStatementSyntax : StatementSyntax
    {
        public WhileStatementSyntax(SyntaxToken whileKeywordToken, ExpressionSyntax conditionExpression, StatementSyntax body)
        {
            WhileKeywordToken = whileKeywordToken;
            ConditionExpression = conditionExpression;
            Body = body;
        }

        public override SyntaxKind Kind => SyntaxKind.WhileStatement;
        public override TextSpan Span => throw new System.NotImplementedException();

        public SyntaxToken WhileKeywordToken { get; }
        public ExpressionSyntax ConditionExpression { get; }
        public StatementSyntax Body { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return WhileKeywordToken;
            yield return ConditionExpression;
            yield return Body;
        }
    }
}
