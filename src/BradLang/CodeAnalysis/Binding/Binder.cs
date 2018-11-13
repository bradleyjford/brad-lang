using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BradLang.CodeAnalysis.Syntax;

namespace BradLang.CodeAnalysis.Binding
{

    sealed class Binder
    {
        readonly BoundScope _scope;
        readonly DiagnosticBag _diagnostics = new DiagnosticBag();

        public DiagnosticBag Diagnostics => _diagnostics;

        public Binder(BoundScope parent)
        {
            _scope = new BoundScope(parent);
        }

        public static BoundGlobalScope BindGlobalScope(BoundGlobalScope previous, SyntaxTree syntaxTree)
        {
            var parentScope = CreateParentScope(previous);
            var binder = new Binder(parentScope);

            var expression = binder.BindExpression(syntaxTree.Root.Expression);
            
            var variables = binder._scope.GetDeclaredVariables();
            var diagnostics = binder.Diagnostics.ToImmutableArray();

            if (previous != null)
            {
                diagnostics = diagnostics.InsertRange(0, previous.Diagnostics);
            }
            
            return new BoundGlobalScope(previous, variables, diagnostics, expression);
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
                case SyntaxKind.NameExpression:
                    return BindNameExpression((NameExpressionSyntax)syntax);
                case SyntaxKind.AssignmentExpression:
                    return BindAssignmentExpression((AssignmentExpressionSyntax)syntax);
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
                variable = new VariableSymbol(name, boundExpression.Type);

                _scope.TryDeclareVariable(variable);
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
            var left = BindExpression(syntax.Left);
            var right = BindExpression(syntax.Right);
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
            var condition = BindExpression(syntax.Condition);
            var trueExpression = BindExpression(syntax.True);
            var falseExpression = BindExpression(syntax.False);

            if (condition.Type != typeof(bool))
            {
                _diagnostics.ReportTypeMismatch(syntax.QuestionMarkToken.Span, condition.Type, typeof(bool));

                return trueExpression;
            }

            var boundOperator = BoundTernaryOperator.Bind(condition.Type, trueExpression.Type, falseExpression.Type);

            if (boundOperator == null)
            {
                _diagnostics.ReportTypeMismatch(syntax.ColonToken.Span, trueExpression.Type, falseExpression.Type);

                return trueExpression;
            }

            return new BoundTernaryExpression(boundOperator, condition, trueExpression, falseExpression);
        }
    }
}
