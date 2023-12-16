using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax;

public sealed class ElseClauseSyntax : SyntaxNode
{
    public ElseClauseSyntax(SyntaxToken elseKeyword, StatementSyntax elseStatement)
    {
        ElseKeyword = elseKeyword;
        ElseStatement = elseStatement;

        Span = elseKeyword.Span;
    }
        
    public override SyntaxKind Kind => SyntaxKind.ElseClause;
    public override TextSpan Span { get; }

    public SyntaxToken ElseKeyword { get; }
    public StatementSyntax ElseStatement { get; }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return ElseKeyword;
        yield return ElseStatement;
    }
}
