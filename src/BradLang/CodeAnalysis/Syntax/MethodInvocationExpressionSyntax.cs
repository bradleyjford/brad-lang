using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax;

public sealed class MethodInvocationExpressionSyntax : ExpressionSyntax
{
    public MethodInvocationExpressionSyntax(
        SyntaxToken methodNameToken, 
        SyntaxToken openParenthesisToken, 
        ExpressionSyntax argumentExpressionSyntax, 
        SyntaxToken closeParenthesisToken)
    {
        MethodNameToken = methodNameToken;
        OpenParenthesisToken = openParenthesisToken;
        ArgumentExpressionSyntax = argumentExpressionSyntax;
        CloseParenthesisToken = closeParenthesisToken;

        Span = new TextSpan(methodNameToken.Span.Start, closeParenthesisToken.Span.End);
    }

    public override SyntaxKind Kind => SyntaxKind.MethodInvocation;
    public override TextSpan Span { get; }

    public SyntaxToken MethodNameToken { get; }
    public SyntaxToken OpenParenthesisToken { get; }
    public ExpressionSyntax ArgumentExpressionSyntax { get; }
    public SyntaxToken CloseParenthesisToken { get; }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return MethodNameToken;
        yield return OpenParenthesisToken;
        yield return ArgumentExpressionSyntax;
        yield return CloseParenthesisToken;
    }
}
