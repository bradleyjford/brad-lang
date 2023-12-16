using System.Linq;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax;

public sealed class SyntaxToken : SyntaxNode
{
    public SyntaxToken(SyntaxKind kind, int position, string text, object value)
    {
        Kind = kind;
        Text = text;
        Value = value;

        Span = new TextSpan(position, text?.Length ?? 1);
    }

    public override SyntaxKind Kind { get; }
    public override TextSpan Span { get; }
        
    public string Text { get; }
    public object Value { get; }

    public bool IsMissing => Text == null;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Enumerable.Empty<SyntaxNode>();
    }
}
