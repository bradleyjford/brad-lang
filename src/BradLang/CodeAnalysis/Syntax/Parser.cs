using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax
{
    public sealed class Parser
    {
        readonly ImmutableArray<SyntaxToken> _tokens;
        readonly DiagnosticBag _diagnostics = new DiagnosticBag();


        int _position;

        public Parser(SourceText text)
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

            _tokens = tokens.ToImmutableArray();

            _diagnostics.AddRange(lexer.Diagnostics);
        }

        public DiagnosticBag Diagnostics => _diagnostics;

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

            return new SyntaxToken(kind, Current.Span.Start, null, null);
        }

        public CompilationUnitSyntax ParseCompilationUnit()
        {
            var statement = ParseStatement();
            var endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);

            return new CompilationUnitSyntax(statement, endOfFileToken);
        }

        StatementSyntax ParseStatement()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.OpenBraceToken:
                    return ParseBlockStatement();
                case SyntaxKind.LetKeyword:
                case SyntaxKind.VarKeyword:
                    return ParseVariableDeclarationStatement();
                default:
                    return ParseExpressionStatement();
            }
        }

        StatementSyntax ParseBlockStatement()
        {
            var openBraceToken = MatchToken(SyntaxKind.OpenBraceToken);
            var statements = ImmutableArray.CreateBuilder<StatementSyntax>();;

            while (Current.Kind != SyntaxKind.CloseBraceToken)
            {
                var statement = ParseStatement();

                statements.Add(statement);
            }

            var closeBraceToken = MatchToken(SyntaxKind.CloseBraceToken);

            return new BlockStatementSyntax(openBraceToken, statements.ToImmutable(), closeBraceToken);
        }

        StatementSyntax ParseVariableDeclarationStatement()
        {
            var expectedKind = Current.Kind == SyntaxKind.LetKeyword ? SyntaxKind.LetKeyword : SyntaxKind.VarKeyword;

            var keywordToken = MatchToken(expectedKind);
            var identifierToken = MatchToken(SyntaxKind.IdentifierToken);
            var equalsToken = MatchToken(SyntaxKind.EqualsToken);
            var initializer = ParseExpression();

            return new VariableDeclarationStatementSyntax(keywordToken, identifierToken, equalsToken, initializer);
        }

        StatementSyntax ParseExpressionStatement()
        {
            var expression = ParseExpression();

            return new ExpressionStatementSyntax(expression);
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
                var right = ParseExpression();

                return new AssignmentExpressionSyntax(identifierToken, operatorToken, right);
            }

            return ParseTernaryExpression();
        }

        ExpressionSyntax ParseTernaryExpression()
        {
            var left = ParseBinaryExpression();

            var ternayOperatorPrecedence = SyntaxFacts.GetTernaryOperatorPrecedence(Current.Kind);

            if (ternayOperatorPrecedence != 0)
            {
                var questionMarkToken = Next();
                var trueExpression = ParseExpression();
                var colonToken = MatchToken(SyntaxKind.ColonToken);
                var falseExpression = ParseExpression();

                left = new ConditionalExpressionSyntax(left, questionMarkToken, trueExpression, colonToken, falseExpression);
            }

            return left;
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
                    return ParseParenthesizedExpression();

                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                    return ParseLiteralBooleanExpression();

                case SyntaxKind.StringToken:
                    return ParseLiteralStringExpression();

                case SyntaxKind.NumberToken:
                    return ParseLiteralNumberExpression();

                case SyntaxKind.IdentifierToken:
                    return ParseNameExpression();

                default:
                    var unknownToken = MatchToken(SyntaxKind.UnknownToken);
                    return new LiteralExpressionSyntax(unknownToken);
            }
        }

        ExpressionSyntax ParseParenthesizedExpression()
        {
            var left = MatchToken(SyntaxKind.OpenParenthesisToken);
            var expression = ParseExpression();
            var right = MatchToken(SyntaxKind.CloseParenthesisToken);

            return new ParenthesizedExpressionSyntax(left, expression, right);
        }

        ExpressionSyntax ParseLiteralBooleanExpression()
        {
            var isTrue = Current.Kind == SyntaxKind.TrueKeyword;
            var keywordToken = isTrue ? MatchToken(SyntaxKind.TrueKeyword) : MatchToken(SyntaxKind.FalseKeyword);

            return new LiteralExpressionSyntax(keywordToken, isTrue);
        }

        ExpressionSyntax ParseLiteralStringExpression()
        {
            var stringToken = MatchToken(SyntaxKind.StringToken);

            return new LiteralStringExpressionSyntax(stringToken);
        }

        ExpressionSyntax ParseLiteralNumberExpression()
        {
            var numberToken = MatchToken(SyntaxKind.NumberToken);

            return new LiteralExpressionSyntax(numberToken);
        }

        ExpressionSyntax ParseNameExpression()
        {
            var nameToken = MatchToken(SyntaxKind.IdentifierToken);

            return new NameExpressionSyntax(nameToken);
        }
    }
}
