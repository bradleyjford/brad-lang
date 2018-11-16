using System.Collections.Generic;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax
{
    public abstract class StatementSyntax : SyntaxNode
    {
    }

    public sealed class ExpressionStatementSyntax : StatementSyntax
    {
        public ExpressionStatementSyntax(ExpressionSyntax expression, SyntaxToken statementTerminatorToken)
        {
            Expression = expression;
            StatementTerminatorToken = statementTerminatorToken;
            
            Span = TextSpan.FromBounds(Expression.Span.Start, statementTerminatorToken.Span.End);
        }

        public override SyntaxKind Kind => SyntaxKind.StatementExpression;
        public override TextSpan Span { get; }
        
        public ExpressionSyntax Expression { get; }
        public SyntaxToken StatementTerminatorToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Expression;
            yield return StatementTerminatorToken;
        }
    }
}
