using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax;

public sealed class ExpressionStatementSyntax : StatementSyntax
{
    public ExpressionStatementSyntax(ExpressionSyntax expression)
    {
        Expression = expression;
            
        Span = expression.Span;
    }

    public override SyntaxKind Kind => SyntaxKind.StatementExpression;
    public override TextSpan Span { get; }
        
    public ExpressionSyntax Expression { get; }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Expression;
    }
}
