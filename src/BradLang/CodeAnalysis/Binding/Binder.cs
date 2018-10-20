using System;
using System.Collections.Generic;
using System.Linq;
using BradLang.CodeAnalysis.Syntax;

namespace BradLang.CodeAnalysis.Binding
{
    sealed class Binder
    {
        readonly SyntaxTree _syntaxTree;
        readonly IDictionary<VariableSymbol, object> _variables;
        readonly DiagnosticBag _diagnostics = new DiagnosticBag();

        public IEnumerable<Diagnostic> Diagnostics => _diagnostics;

        public Binder(SyntaxTree syntaxTree, IDictionary<VariableSymbol, object> variables)
        {
            _syntaxTree = syntaxTree;
            _variables = variables;
        }

        public BoundExpression Bind()
        {
            return BindExpression(_syntaxTree.Root);
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

            var variable = _variables.Keys.SingleOrDefault(v => v.Name == name);

            if (variable == null)
            {
                variable = new VariableSymbol(name, boundExpression.Type);

                _variables.Add(variable, null);
            }

            return new BoundAssignmentExpression(variable, boundExpression);
        }

        BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            var name = syntax.NameToken.Text;

            var variable = _variables.Keys.SingleOrDefault(v => v.Name == name);

            if (variable == null)
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
    }
}
