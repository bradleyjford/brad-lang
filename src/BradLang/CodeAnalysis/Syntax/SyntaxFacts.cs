using System;

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

        public static SyntaxKind GetKeywordKind(string keyword)
        {
            switch (keyword)
            {
                case "true":
                    return SyntaxKind.TrueKeyword;
                case "false":
                    return SyntaxKind.FalseKeyword;
                default:
                    return SyntaxKind.IdentifierToken;
            }
        }

        public static bool IsKeyword(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
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
                
                case SyntaxKind.OpenParenthesisToken:
                    return "(";
                case SyntaxKind.CloseParenthesisToken:
                    return ")";

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

                case SyntaxKind.TrueKeyword:
                    return "true";
                case SyntaxKind.FalseKeyword:
                    return "false";
            }

            return null;
        }
    }
}
