using System;
using System.Collections.Generic;
using BradLang.CodeAnalysis.Binding;

namespace BradLang
{
    sealed class Evaluator
    {
        readonly BoundExpression _root;
        readonly IDictionary<VariableSymbol, object> _variables;

        public Evaluator(BoundExpression root, IDictionary<VariableSymbol, object> variables)
        {
            _root = root;
            _variables = variables;
        }

        public object Evaluate()
        {
            return EvaluateExpression(_root);
        }

        object EvaluateExpression(BoundExpression expression)
        {
            switch (expression.Kind)
            {
                case BoundNodeKind.LiteralExpression:
                    return EvaluateLiteralExpression((BoundLiteralExpression)expression);

                case BoundNodeKind.VariableExpression:
                    return EvaluateVariableExpression((BoundVariableExpression)expression);

                case BoundNodeKind.AssignmentExpression:
                    return EvaluateAssignmentExpression((BoundAssignmentExpression)expression);

                case BoundNodeKind.UnaryExpression:
                    return EvaluateUnaryExpression((BoundUnaryExpression)expression);

                case BoundNodeKind.BinaryExpression:
                    return EvaluateBinaryExpression((BoundBinaryExpression)expression);

                case BoundNodeKind.TernaryExpression:
                    return EvaluateTernaryExpression((BoundTernaryExpression)expression);
            }

            throw new Exception($"Unexpected node {expression.Kind}.");
        }

        object EvaluateVariableExpression(BoundVariableExpression expression)
        {
            return _variables[expression.Variable];
        }

        static object EvaluateLiteralExpression(BoundLiteralExpression expression)
        {           
            return expression.Value;
        }

        object EvaluateTernaryExpression(BoundTernaryExpression expression)
        {
            var conditionResult = EvaluateExpression(expression.Condition);

            if ((bool)conditionResult)
            {
                return EvaluateExpression(expression.True);
            }
            else
            {
                return EvaluateExpression(expression.False);
            }
        }

        object EvaluateBinaryExpression(BoundBinaryExpression expression)
        {
            var left = EvaluateExpression(expression.Left);
            var right = EvaluateExpression(expression.Right);

            switch (expression.Operator.Kind)
            {
                case BoundBinaryOperatorKind.Addition:
                    if (expression.Type == typeof(int))
                    {
                        return (int)left + (int)right;
                    }

                    if (expression.Type == typeof(string))
                    {
                        return String.Concat(left, right);
                    }

                    throw new Exception($"Unexpected binary operator {expression.Operator.Kind}.");
                case BoundBinaryOperatorKind.Subtraction:
                    return (int)left - (int)right;
                case BoundBinaryOperatorKind.Multiplication:
                    return (int)left * (int)right;
                case BoundBinaryOperatorKind.Division:
                    return (int)left / (int)right;
                case BoundBinaryOperatorKind.LessThan:
                    return (int)left < (int)right;
                case BoundBinaryOperatorKind.LessThanEquals:
                    return (int)left <= (int)right;
                case BoundBinaryOperatorKind.GreaterThan:
                    return (int)left > (int)right;
                case BoundBinaryOperatorKind.GreaterThanEquals:
                    return (int)left >= (int)right;
                case BoundBinaryOperatorKind.Equals:
                    return Equals(left, right);
                case BoundBinaryOperatorKind.NotEquals:
                    return !Equals(left, right);
                case BoundBinaryOperatorKind.LogicalOr:
                    return (bool)left || (bool)right;
                case BoundBinaryOperatorKind.LogicalAnd:
                    return (bool)left && (bool)right;
                case BoundBinaryOperatorKind.Modulus:
                    return (int)left % (int)right;
                default:
                    throw new Exception($"Unexpected binary operator {expression.Operator.Kind}.");
            }
        }

        object EvaluateAssignmentExpression(BoundAssignmentExpression expression)
        {
            var value = EvaluateExpression(expression.Expression);

            _variables[expression.Variable] = value;

            return value;
        }

        object EvaluateUnaryExpression(BoundUnaryExpression expression)
        {
            var operand = EvaluateExpression(expression.Operand);

            switch (expression.Operator.Kind)
            {
                case BoundUnaryOperatorKind.Negation:
                    return -(int)operand;
                case BoundUnaryOperatorKind.Identity:
                    return (int)operand;
                case BoundUnaryOperatorKind.LogicalNegation:
                    return !(bool)operand;
                default:
                    throw new Exception($"Unsupported unary operator {expression.Operator.Kind}.");
            }
        }
    }
}
