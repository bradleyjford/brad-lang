using System;
using System.Collections.Generic;

namespace BradLang.CodeAnalysis.Syntax
{
    public static class SyntaxFacts
    {
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.BangToken:
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 10;

                default:
                    return 0;
            }
        }

        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.StarToken:
                case SyntaxKind.SlashToken:
                case SyntaxKind.PercentToken:
                    return 7;

                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 6;

                case SyntaxKind.LessThanToken:
                case SyntaxKind.LessThanEqualsToken:
                case SyntaxKind.GreaterThanToken:
                case SyntaxKind.GreaterThanEqualsToken:
                    return 5;

                case SyntaxKind.EqualsEqualsToken:
                case SyntaxKind.BangEqualsToken:
                    return 4;

                case SyntaxKind.AmpersandAmpersandToken:
                    return 3;

                case SyntaxKind.PipePipeToken:
                    return 2;

                default:
                    return 0;
            }
        }

        public static int GetTernaryOperatorPrecedence(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.QuestionMarkToken:
                    return 1;

                default:
                    return 0;
            }
        }

        public static IEnumerable<SyntaxKind> GetUnaryOperatorKinds()
        {
            var kinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));

            foreach (var kind in kinds)
            {
                if (GetUnaryOperatorPrecedence(kind) > 0)
                {
                    yield return kind;
                }
            }
        }

        public static IEnumerable<SyntaxKind> GetBinaryOperatorKinds()
        {
            var kinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));

            foreach (var kind in kinds)
            {
                if (GetBinaryOperatorPrecedence(kind) > 0)
                {
                    yield return kind;
                }
            }
        }

        public static SyntaxKind GetKeywordKind(string keyword)
        {
            switch (keyword)
            {
                case "else":
                    return SyntaxKind.ElseKeyword;
                case "false":
                    return SyntaxKind.FalseKeyword;
                case "for":
                    return SyntaxKind.ForKeyword;
                case "if":
                    return SyntaxKind.IfKeyword;
                case "let":
                    return SyntaxKind.LetKeyword;
                case "return":
                    return SyntaxKind.ReturnKeyword;
                case "to":
                    return SyntaxKind.ToKeyword;
                case "true":
                    return SyntaxKind.TrueKeyword;
                case "var":
                    return SyntaxKind.VarKeyword;
                case "while":
                    return SyntaxKind.WhileKeyword;
                default:
                    return SyntaxKind.IdentifierToken;
            }
        }

        public static bool IsKeyword(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.ElseKeyword:
                case SyntaxKind.FalseKeyword:
                case SyntaxKind.ForKeyword:
                case SyntaxKind.IfKeyword:
                case SyntaxKind.LetKeyword:
                case SyntaxKind.ReturnKeyword:
                case SyntaxKind.ToKeyword:
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.VarKeyword:
                case SyntaxKind.WhileKeyword:
                    return true;
            }

            return false;
        }

        public static string GetText(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.AmpersandToken:
                    return "&";
                case SyntaxKind.BangToken:
                    return "!";
                case SyntaxKind.ColonToken:
                    return ":";
                case SyntaxKind.EqualsToken:
                    return "=";
                case SyntaxKind.PipeToken:
                    return "|";
                case SyntaxKind.QuestionMarkToken:
                    return "?";
                case SyntaxKind.SemicolonToken:
                    return ";";
                    
                case SyntaxKind.OpenParenthesisToken:
                    return "(";
                case SyntaxKind.CloseParenthesisToken:
                    return ")";

                case SyntaxKind.OpenBraceToken:
                    return "{";
                case SyntaxKind.CloseBraceToken:
                    return "}";

                case SyntaxKind.PlusToken:
                    return "+";
                case SyntaxKind.MinusToken:
                    return "-";
                case SyntaxKind.StarToken:
                    return "*";
                case SyntaxKind.SlashToken:
                    return "/";
                case SyntaxKind.PercentToken:
                    return "%";

                case SyntaxKind.AmpersandAmpersandToken:
                    return "&&";
                case SyntaxKind.PipePipeToken:
                    return "||";

                case SyntaxKind.EqualsEqualsToken:
                    return "==";
                case SyntaxKind.BangEqualsToken:
                    return "!=";
                case SyntaxKind.LessThanToken:
                    return "<";
                case SyntaxKind.LessThanEqualsToken:
                    return "<=";
                case SyntaxKind.GreaterThanToken:
                    return ">";
                case SyntaxKind.GreaterThanEqualsToken:
                    return ">=";

                case SyntaxKind.ElseKeyword:
                    return "else";
                case SyntaxKind.FalseKeyword:
                    return "false";
                case SyntaxKind.ForKeyword:
                    return "for";                    
                case SyntaxKind.IfKeyword:
                    return "if";
                case SyntaxKind.ReturnKeyword:
                    return "return";
                case SyntaxKind.LetKeyword:
                    return "let";
                case SyntaxKind.ToKeyword:
                    return "to";
                case SyntaxKind.TrueKeyword:
                    return "true";
                case SyntaxKind.VarKeyword:
                    return "var";
                case SyntaxKind.WhileKeyword:
                    return "while";
            }

            return null;
        }
    }
}
