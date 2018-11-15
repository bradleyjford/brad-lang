using System.Collections.Generic;
using System.Collections.Immutable;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax
{
    public sealed class BlockStatementSyntax : StatementSyntax
    {
        public BlockStatementSyntax(SyntaxToken openBraceToken, ImmutableArray<StatementSyntax> statements, SyntaxToken closeBraceToken)
        {
            OpenBraceToken = openBraceToken;
            Statements = statements;
            CloseBraceToken = closeBraceToken;

            Span = new TextSpan(openBraceToken.Span.Start, closeBraceToken.Span.End);
        }

        public override SyntaxKind Kind => SyntaxKind.BlockStatement;

        public override TextSpan Span { get; }

        public SyntaxToken OpenBraceToken { get; }
        public ImmutableArray<StatementSyntax> Statements { get; }
        public SyntaxToken CloseBraceToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenBraceToken;
            
            foreach (var statement in Statements)
            {
                yield return statement;
            }

            yield return CloseBraceToken;
        }
    }

}
