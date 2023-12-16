using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax;

public sealed class CallExpressionSyntax : ExpressionSyntax
{
    public CallExpressionSyntax(
        SyntaxToken identifierToken, 
        SyntaxToken openParenthesisToken,
        SeparatedSyntaxList<ExpressionSyntax> arguments,
        SyntaxToken closeParenthesisToken)
    {
        IdentifierToken = identifierToken;
        OpenParenthesisToken = openParenthesisToken;
        Arguments = arguments;
        CloseParenthesisToken = closeParenthesisToken;
    }

    public override SyntaxKind Kind => SyntaxKind.CallExpression;

    public override TextSpan Span => new TextSpan(IdentifierToken.Span.Start, CloseParenthesisToken.Span.End);

    public SyntaxToken IdentifierToken { get; }
    public SyntaxToken OpenParenthesisToken { get; }
    public SeparatedSyntaxList<ExpressionSyntax> Arguments { get; }
    public SyntaxToken CloseParenthesisToken { get; }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return IdentifierToken;
        yield return OpenParenthesisToken;

        foreach (var child in Arguments.GetWithSeparators())
        {
            yield return child;
        }

        yield return CloseParenthesisToken;
    }
}
