using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax;

public sealed class CompilationUnitSyntax : SyntaxNode
{
    public CompilationUnitSyntax(StatementSyntax statement, SyntaxToken endOfFileToken)
    {
        Statement = statement;
        EndOfFileToken = endOfFileToken;

        Span = TextSpan.FromBounds(statement.Span.Start, endOfFileToken.Span.End);
    }

    public override SyntaxKind Kind => SyntaxKind.CompilationUnit;
    public override TextSpan Span { get; }

    public StatementSyntax Statement { get; }
    public SyntaxToken EndOfFileToken { get; }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Statement;
        yield return EndOfFileToken;
    }
}
