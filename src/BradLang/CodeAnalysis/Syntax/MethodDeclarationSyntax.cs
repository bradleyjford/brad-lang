using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax;

public sealed class MethodDeclarationSyntax : StatementSyntax
{
    public MethodDeclarationSyntax(
        SyntaxToken returnTypeToken,
        SyntaxToken methodNameToken,
        SyntaxToken openParenthesisToken,
        SyntaxToken parameterTypeToken,
        SyntaxToken parameterNameToken,
        SyntaxToken closeParenthesisToken,
        StatementSyntax bodyStatement)
    {
        ReturnTypeToken = returnTypeToken;
        MethodNameToken = methodNameToken;
        OpenParenthesisToken = openParenthesisToken;
        ParameterTypeToken = parameterTypeToken;
        ParameterNameToken = parameterNameToken;
        CloseParenthesisToken = closeParenthesisToken;
        BodyStatement = bodyStatement;

        Span = new TextSpan(methodNameToken.Span.Start, closeParenthesisToken.Span.End);
    }

    public override SyntaxKind Kind => SyntaxKind.MethodDeclaration;
    public override TextSpan Span { get; }
    public SyntaxToken ReturnTypeToken { get; }
    public SyntaxToken MethodNameToken { get; }
    public SyntaxToken OpenParenthesisToken { get; }
    public SyntaxToken ParameterTypeToken { get; }
    public SyntaxToken ParameterNameToken { get; }
    public SyntaxToken CloseParenthesisToken { get; }
    public StatementSyntax BodyStatement { get; }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return ReturnTypeToken;
        yield return MethodNameToken;
        yield return OpenParenthesisToken;
        yield return ParameterTypeToken;
        yield return ParameterNameToken;
        yield return CloseParenthesisToken;
        yield return BodyStatement;
    }
}
