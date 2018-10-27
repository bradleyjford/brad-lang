﻿using System.Collections.Generic;

namespace BradLang.CodeAnalysis.Syntax
{
    class TernaryExpressionSyntax : ExpressionSyntax
    {
        public TernaryExpressionSyntax(
            ExpressionSyntax condition, 
            SyntaxToken questionMarkToken, 
            ExpressionSyntax @true, 
            SyntaxToken colonToken,
            ExpressionSyntax @false)
        {
            Condition = condition;
            QuestionMarkToken = questionMarkToken;
            True = @true;
            ColonToken = colonToken;
            False = @false;
        }

        public ExpressionSyntax Condition { get; }
        public SyntaxToken QuestionMarkToken { get; }
        public ExpressionSyntax True { get; }
        public SyntaxToken ColonToken { get; }
        public ExpressionSyntax False { get; }

        public override SyntaxKind Kind => SyntaxKind.TernaryExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Condition;
            yield return QuestionMarkToken;
            yield return True;
            yield return ColonToken;
            yield return False;
        }
    }
}