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

        SyntaxToken PeekToken(int offset)
        {
            var index = _position + offset;

            if (index >= _tokens.Length)
            {
                return _tokens[_tokens.Length - 1];
            }

            return _tokens[index];
        }

        SyntaxToken CurrentToken => PeekToken(0);

        SyntaxToken NextToken()
        {
            var current = CurrentToken;

            _position++;

            return current;
        }

        SyntaxToken MatchToken(SyntaxKind kind)
        {
            if (CurrentToken.Kind == kind)
            {
                return NextToken();
            }

            _diagnostics.ReportUnexpectedToken(CurrentToken.Span, CurrentToken.Kind, kind);

            return new SyntaxToken(kind, CurrentToken.Span.Start, null, null);
        }

        public CompilationUnitSyntax ParseCompilationUnit()
        {
            var statement = ParseStatement();
            var endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);

            return new CompilationUnitSyntax(statement, endOfFileToken);
        }

        StatementSyntax ParseStatement()
        {
            switch (CurrentToken.Kind)
            {
                case SyntaxKind.OpenBraceToken:
                    return ParseBlockStatement();
                case SyntaxKind.IfKeyword:
                    return ParseIfStatement();
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

            while (CurrentToken.Kind != SyntaxKind.CloseBraceToken &&
                CurrentToken.Kind != SyntaxKind.EndOfFileToken)
            {
                var startToken = CurrentToken;

                var statement = ParseStatement();

                statements.Add(statement);

                if (CurrentToken == startToken)
                {
                    NextToken();
                }
            }

            var closeBraceToken = MatchToken(SyntaxKind.CloseBraceToken);

            return new BlockStatementSyntax(openBraceToken, statements.ToImmutable(), closeBraceToken);
        }

        StatementSyntax ParseIfStatement()
        {
            var ifKeyword = MatchToken(SyntaxKind.IfKeyword);
            var openParenthesisToken = MatchToken(SyntaxKind.OpenParenthesisToken);
            var condition = ParseExpression();
            var closeParenthesisToken = MatchToken(SyntaxKind.CloseParenthesisToken);
            var thenStatement = ParseStatement();

            var elseClause = ParseElseClause();

            return new IfStatementSyntax(ifKeyword, openParenthesisToken, condition, closeParenthesisToken, thenStatement, elseClause);
        }

        ElseClauseSyntax ParseElseClause()
        {
            if (CurrentToken.Kind != SyntaxKind.ElseKeyword)
            {
                return null;
            }

            var elseKeyword = NextToken();
            var elseStatement = ParseStatement();

            return new ElseClauseSyntax(elseKeyword, elseStatement);
        }

        StatementSyntax ParseVariableDeclarationStatement()
        {
            var expectedKind = CurrentToken.Kind == SyntaxKind.LetKeyword ? SyntaxKind.LetKeyword : SyntaxKind.VarKeyword;

            var keywordToken = MatchToken(expectedKind);
            var identifierToken = MatchToken(SyntaxKind.IdentifierToken);
            var equalsToken = MatchToken(SyntaxKind.EqualsToken);
            var initializer = ParseExpression();
            var statementTerminatorToken = MatchToken(SyntaxKind.SemicolonToken);

            return new VariableDeclarationStatementSyntax(keywordToken, identifierToken, equalsToken, initializer, statementTerminatorToken);
        }

        StatementSyntax ParseExpressionStatement()
        {
            var expression = ParseExpression();
            var statementTerminatorToken = MatchToken(SyntaxKind.SemicolonToken);

            return new ExpressionStatementSyntax(expression, statementTerminatorToken);
        }

        ExpressionSyntax ParseExpression()
        {
            return ParseAssignmentExpression();
        }

        ExpressionSyntax ParseAssignmentExpression()
        {
            if (CurrentToken.Kind == SyntaxKind.IdentifierToken &&
                PeekToken(1).Kind == SyntaxKind.EqualsToken)
            {
                var identifierToken = NextToken();
                var operatorToken = NextToken();
                var right = ParseExpression();

                return new AssignmentExpressionSyntax(identifierToken, operatorToken, right);
            }

            return ParseTernaryExpression();
        }

        ExpressionSyntax ParseTernaryExpression()
        {
            var left = ParseBinaryExpression();

            var ternayOperatorPrecedence = SyntaxFacts.GetTernaryOperatorPrecedence(CurrentToken.Kind);

            if (ternayOperatorPrecedence != 0)
            {
                var questionMarkToken = NextToken();
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

            var unaryOperatorPrecedence = CurrentToken.Kind.GetUnaryOperatorPrecedence();

            if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
            {
                var operatorToken = NextToken();
                var operand = ParseBinaryExpression(unaryOperatorPrecedence);

                left = new UnaryExpressionSyntax(operatorToken, operand);
            }
            else
            {
                left = ParsePrimaryExpression();
            }

            while (true)
            {
                var precedence = CurrentToken.Kind.GetBinaryOperatorPrecedence();

                if (precedence == 0 || precedence <= parentPrecedence)
                {
                    break;
                }

                var operatorToken = NextToken();

                var right = ParseBinaryExpression(precedence);

                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        ExpressionSyntax ParsePrimaryExpression()
        {
            switch (CurrentToken.Kind)
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
            var isTrue = CurrentToken.Kind == SyntaxKind.TrueKeyword;
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
