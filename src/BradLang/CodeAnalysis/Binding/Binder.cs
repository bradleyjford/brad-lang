using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BradLang.CodeAnalysis.Syntax;

namespace BradLang.CodeAnalysis.Binding
{
    sealed class Binder
    {
        public static BoundGlobalScope BindGlobalScope(BoundGlobalScope previous, SyntaxTree syntaxTree)
        {
            var parentScope = CreateParentScope(previous);
            var binder = new Binder(parentScope);

            var statement = binder.BindStatement(syntaxTree.Root.Statement);
            
            var variables = binder._scope.GetDeclaredVariables();
            var diagnostics = binder.Diagnostics.ToImmutableArray();

            if (previous != null)
            {
                diagnostics = diagnostics.InsertRange(0, previous.Diagnostics);
            }
            
            return new BoundGlobalScope(previous, variables, diagnostics, statement);
        }

        static BoundScope CreateParentScope(BoundGlobalScope previous)
        {
            var stack = new Stack<BoundGlobalScope>();

            while (previous != null)
            {
                stack.Push(previous);

                previous = previous.Previous;
            }

            BoundScope parent = null;

            while (stack.Count > 0)
            {
                previous = stack.Pop();

                var scope = new BoundScope(parent);

                foreach (var variable in previous.Variables)
                {
                    scope.TryDeclareVariable(variable);
                }

                parent = scope;
            }

            return parent;
        }

        readonly DiagnosticBag _diagnostics = new DiagnosticBag();

        BoundScope _scope;

        Binder(BoundScope parent)
        {
            _scope = new BoundScope(parent);
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        BoundStatement BindStatement(StatementSyntax syntax)
        {
            switch (syntax.Kind)
            {
                case SyntaxKind.BlockStatement:
                    return BindBlockStatement((BlockStatementSyntax)syntax);
                case SyntaxKind.ForStatement:
                    return BindForStatement((ForStatementSyntax)syntax);
                case SyntaxKind.IfStatement:
                    return BindIfStatement((IfStatementSyntax)syntax);
                case SyntaxKind.VariableDeclarationStatement:
                    return BindVariableDeclarationStatement((VariableDeclarationStatementSyntax)syntax);
                case SyntaxKind.WhileStatement:
                    return BoundWhileStatement((WhileStatementSyntax)syntax);
                default: 
                    return BindExpressionStatement((ExpressionStatementSyntax)syntax);
            }
        }

        BoundStatement BindBlockStatement(BlockStatementSyntax syntax)
        {
            _scope = new BoundScope(_scope);

            var statements = ImmutableArray.CreateBuilder<BoundStatement>();

            foreach (var statementSyntax in syntax.Statements)
            {
                var statement = BindStatement(statementSyntax);

                statements.Add(statement);
            }

            _scope = _scope.Parent;

            return new BoundBlockStatement(statements.ToImmutable());
        }

        BoundStatement BindForStatement(ForStatementSyntax syntax)
        {
            var lowerBound = BindExpression(syntax.LowerBoundExpression, typeof(int));
            var upperBound = BindExpression(syntax.UpperBoundExpression, typeof(int));

            _scope = new BoundScope(_scope);

            var name = syntax.IdentifierToken.Text;

            var variable = new VariableSymbol(name, typeof(int), true);
            
            if (!_scope.TryDeclareVariable(variable))
            {
                _diagnostics.ReportVariableAlreadyDeclared(syntax.IdentifierToken.Span, name);
            }

            var body = BindStatement(syntax.Body);

            _scope = _scope.Parent;

            return new BoundForStatement(variable, lowerBound, upperBound, body);
        }

        BoundStatement BindIfStatement(IfStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.ConditionExpression, typeof(bool));

            var thenStatement = BindStatement(syntax.ThenStatement);
            BoundStatement elseStatement = null;
            
            if (syntax.ElseClause != null)
            {
                elseStatement = BindStatement(syntax.ElseClause.ElseStatement);
            }

            return new BoundIfStatement(condition, thenStatement, elseStatement);
        }

        BoundStatement BindVariableDeclarationStatement(VariableDeclarationStatementSyntax syntax)
        {
            var initializerExpression = BindExpression(syntax.Initializer);

            var isConstant = syntax.KeywordToken.Kind == SyntaxKind.LetKeyword;

            var variableSymbol = new VariableSymbol(syntax.IdentifierToken.Text, initializerExpression.Type, isConstant);

            if (!_scope.TryDeclareVariable(variableSymbol))
            {
                _diagnostics.ReportVariableAlreadyDeclared(syntax.IdentifierToken.Span, syntax.IdentifierToken.Text);
            }

            return new BoundVariableDeclaration(variableSymbol, initializerExpression);
        }

        BoundStatement BoundWhileStatement(WhileStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.ConditionExpression, typeof(bool));
            var body = BindStatement(syntax.Body);

            return new BoundWhileStatement(condition, body);
        }

        BoundStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
        {
            var expression = BindExpression(syntax.Expression);

            return new BoundExpressionStatement(expression);
        }

        BoundExpression BindExpression(ExpressionSyntax syntax, Type expectedType)
        {
            var expression = BindExpression(syntax);

            if (expression.Type != expectedType)
            {
                _diagnostics.ReportTypeMismatch(syntax.Span, expression.Type, expectedType);
            }

            return expression;
        }

        BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            switch (syntax.Kind)
            {
                case SyntaxKind.ParenthesizedExpression:
                    return BindParenthesizedExpression((ParenthesizedExpressionSyntax)syntax);
                case SyntaxKind.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionSyntax)syntax);
                case SyntaxKind.LiteralStringExpression:
                    return BindLiteralStringExpression((LiteralStringExpressionSyntax)syntax);
                case SyntaxKind.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionSyntax)syntax);
                case SyntaxKind.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax)syntax);
                case SyntaxKind.TernaryExpression:
                    return BindTernaryExpression((ConditionalExpressionSyntax)syntax);
                case SyntaxKind.AssignmentExpression:
                    return BindAssignmentExpression((AssignmentExpressionSyntax)syntax);
                case SyntaxKind.NameExpression:
                    return BindNameExpression((NameExpressionSyntax)syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}.");
            }
        }

        BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            var boundExpression = BindExpression(syntax.Expression);

            if (!_scope.TryLookupVariable(name, out var variable))
            {
                _diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name);
                return boundExpression;
            }
            
            if (variable.IsReadOnly)
            {
                _diagnostics.ReportCannotAssign(syntax.IdentifierToken.Span, name);
                return boundExpression;
            }

            if (variable.Type != boundExpression.Type)
            {
                _diagnostics.ReportTypeMismatch(syntax.Expression.Span, boundExpression.Type, variable.Type);
                return boundExpression;
            }

            return new BoundAssignmentExpression(variable, boundExpression);
        }

        BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            var name = syntax.NameToken.Text;

            if (String.IsNullOrEmpty(name))
            {
                // Specified syntax was inserted by the Parser as a result of a parse error.
                // Diagnostic has already been reported by the Parser.
                return new BoundLiteralExpression(String.Empty);
            }

            if (!_scope.TryLookupVariable(name, out var variable))
            {
                _diagnostics.ReportUndefinedName(syntax.NameToken.Span, name);

                return new BoundLiteralExpression(null);
            }

            return new BoundVariableExpression(variable);
        }

        BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax)
        {
            return BindExpression(syntax.Expression);
        }

        BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var value = syntax.Value;

            return new BoundLiteralExpression(value);
        }

        BoundExpression BindLiteralStringExpression(LiteralStringExpressionSyntax syntax)
        {
            var value = syntax.Value.Replace("\\", "");

            return new BoundLiteralExpression(value);
        }

        BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperand = BindExpression(syntax.Operand);
            var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);

            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundOperand.Type);

                return boundOperand;
            }

            return new BoundUnaryExpression(boundOperator, boundOperand);
        }

        BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var left = BindExpression(syntax.LeftExpression);
            var right = BindExpression(syntax.RightExpression);
            var boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, left.Type, right.Type);

            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, left.Type, right.Type);

                return left;
            }

            return new BoundBinaryExpression(left, boundOperator, right);
        }

        BoundExpression BindTernaryExpression(ConditionalExpressionSyntax syntax)
        {
            var condition = BindExpression(syntax.ConditionExpression);
            var trueExpression = BindExpression(syntax.TrueExpression);
            var falseExpression = BindExpression(syntax.FalseExpression);

            if (condition.Type != typeof(bool))
            {
                _diagnostics.ReportTypeMismatch(syntax.QuestionMarkToken.Span, condition.Type, typeof(bool));

                return trueExpression;
            }

            var boundOperator = BoundTernaryOperator.Bind(syntax.Kind, condition.Type, trueExpression.Type, falseExpression.Type);

            if (boundOperator == null)
            {
                _diagnostics.ReportTypeMismatch(syntax.ColonToken.Span, trueExpression.Type, falseExpression.Type);

                return trueExpression;
            }

            return new BoundTernaryExpression(boundOperator, condition, trueExpression, falseExpression);
        }
    }
}
