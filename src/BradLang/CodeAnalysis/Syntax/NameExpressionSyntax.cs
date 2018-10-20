using System;
using System.Collections.Generic;

namespace BradLang.CodeAnalysis.Syntax
{
    public sealed class NameExpressionSyntax : ExpressionSyntax
    {
        public NameExpressionSyntax(SyntaxToken nameToken)
        {
            NameToken = nameToken;
        }

        public SyntaxToken NameToken { get; }

        public override SyntaxKind Kind => SyntaxKind.NameExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return NameToken;
        }
    }
}
