using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax;

public sealed class NameExpressionSyntax : ExpressionSyntax
{
    public NameExpressionSyntax(SyntaxToken nameToken)
    {
        NameToken = nameToken;

        Span = nameToken.Span;
    }

    public override SyntaxKind Kind => SyntaxKind.NameExpression;
    public override TextSpan Span { get; }

    public SyntaxToken NameToken { get; }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return NameToken;
    }
}
