using System;
using System.Collections.Generic;

namespace BradLang.CodeAnalysis.Syntax
{
    public sealed class Parser
    {
        readonly SyntaxToken[] _tokens;
        readonly DiagnosticBag _diagnostics = new DiagnosticBag();

        int _position;

        public Parser(string text)
        {
            var tokens = new List<SyntaxToken>();

            var lexer = new Lexer(text);

            SyntaxToken token;

            do
            {
                token = lexer.Lex();

                if (token.Kind != SyntaxKind.WhiteSpaceToken &&
                    token.Kind != SyntaxKind.UnknownToken)
                {
                    tokens.Add(token);
                }
            } while (token.Kind != SyntaxKind.EndOfFileToken);

            _tokens = tokens.ToArray();

            _diagnostics.AddRange(lexer.Diagnostics);
        }

        public IEnumerable<Diagnostic> Diagnostics => _diagnostics;

        SyntaxToken Peek(int offset)
        {
            var index = _position + offset;

            if (index >= _tokens.Length)
            {
                return _tokens[_tokens.Length - 1];
            }

            return _tokens[index];
        }

        SyntaxToken Current => Peek(0);

        SyntaxToken Next()
        {
            var current = Current;

            _position++;

            return current;
        }

        SyntaxToken MatchToken(SyntaxKind kind)
        {
            if (Current.Kind == kind)
            {
                return Next();
            }
            
            _diagnostics.ReportUnexpectedToken(Current.Span, Current.Kind, kind);
            
            return new SyntaxToken(kind, Current.Position, null, null);
        }

        public SyntaxTree Parse()
        {
            var root = ParseExpression();

            var endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);

            return new SyntaxTree(root, endOfFileToken, _diagnostics);
        }

        ExpressionSyntax ParseExpression()
        {
            return ParseAssignmentExpression();
        }

        ExpressionSyntax ParseAssignmentExpression()
        {
            if (Current.Kind == SyntaxKind.IdentifierToken &&
                Peek(1).Kind == SyntaxKind.EqualsToken)
            {
                var identifierToken = Next();
                var operatorToken = Next();
                var right = ParseAssignmentExpression();

                return new AssignmentExpressionSyntax(identifierToken, operatorToken, right);
            }

            return ParseBinaryExpression();
        }

        ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
        {
            ExpressionSyntax left;

            var unaryOperatorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();

            if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
            {
                var operatorToken = Next();
                var operand = ParseBinaryExpression(unaryOperatorPrecedence);

                left = new UnaryExpressionSyntax(operatorToken, operand);
            }
            else
            {
                left = ParsePrimaryExpression();
            }

            while (true)
            {
                var precedence = Current.Kind.GetBinaryOperatorPrecedence();

                if (precedence == 0 || precedence <= parentPrecedence)
                {
                    break;
                }

                var operatorToken = Next();
                var right = ParseBinaryExpression(precedence);

                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        ExpressionSyntax ParsePrimaryExpression()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.OpenParenthesisToken:
                    var left = Next();
                    var expression = ParseExpression();
                    var right = MatchToken(SyntaxKind.CloseParenthesisToken);

                    return new ParenthesizedExpressionSyntax(left, expression, right);

                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                    var keywordToken = Next();
                    var value = keywordToken.Kind == SyntaxKind.TrueKeyword;

                    return new LiteralExpressionSyntax(keywordToken, value);

                case SyntaxKind.IdentifierToken:
                    var nameToken = Next();
                    return new NameExpressionSyntax(nameToken);

                case SyntaxKind.StringToken:
                    var stringToken = Next();
                    return new LiteralStringExpressionSyntax(stringToken);

                case SyntaxKind.NumberToken:
                    var numberToken = Next();
                    return new LiteralExpressionSyntax(numberToken);

                default:
                    var unknownToken = MatchToken(SyntaxKind.UnknownToken);
                    return new LiteralExpressionSyntax(unknownToken);
            }
        }
    }
}
