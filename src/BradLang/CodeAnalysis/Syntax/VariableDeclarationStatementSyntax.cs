using System.Collections.Generic;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax
{
    public sealed class VariableDeclarationStatementSyntax : StatementSyntax
    {
        public VariableDeclarationStatementSyntax(SyntaxToken keywordToken, SyntaxToken identifierToken, SyntaxToken equalsToken, ExpressionSyntax initializer)
        {
            KeywordToken = keywordToken;
            IdentifierToken = identifierToken;
            EqualsToken = equalsToken;
            Initializer = initializer;

            Span = new TextSpan(keywordToken.Span.Start, initializer.Span.End);
        }

        public override SyntaxKind Kind => SyntaxKind.VariableDeclarationStatement;

        public override TextSpan Span { get; }

        public SyntaxToken KeywordToken { get; }
        public SyntaxToken IdentifierToken { get; }
        public SyntaxToken EqualsToken { get; }
        public ExpressionSyntax Initializer { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return KeywordToken;
            yield return IdentifierToken;
            yield return EqualsToken;
            yield return Initializer;
        }
    }
}
