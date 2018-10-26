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
                    var literalExpression = (BoundLiteralExpression)expression;
                    return literalExpression.Value;

                case BoundNodeKind.VariableExpression:
                    var variableExpression = (BoundVariableExpression)expression;

                    return _variables[variableExpression.Variable];

                case BoundNodeKind.AssignmentExpression:
                    var assignmentExpression = (BoundAssignmentExpression)expression;
                    var value = EvaluateExpression(assignmentExpression.Expression);

                    _variables[assignmentExpression.Variable] = value;

                    return value;

                case BoundNodeKind.UnaryExpression:
                    var unaryExpression = (BoundUnaryExpression)expression;

                    var operand = EvaluateExpression(unaryExpression.Operand);

                    switch (unaryExpression.Operator.Kind)
                    {
                        case BoundUnaryOperatorKind.Negation:
                            return -(int)operand;
                        case BoundUnaryOperatorKind.Identity:
                            return (int)operand;
                        case BoundUnaryOperatorKind.LogicalNegation:
                            return !(bool)operand;
                        default:
                            throw new Exception($"Unsupported unary operator {unaryExpression.Operator.Kind}.");
                    }

                case BoundNodeKind.BinaryExpression:
                    var binaryExpression = (BoundBinaryExpression)expression;

                    var left = EvaluateExpression(binaryExpression.Left);
                    var right = EvaluateExpression(binaryExpression.Right);

                    switch (binaryExpression.Operator.Kind)
                    {
                        case BoundBinaryOperatorKind.Addition:
                            if (binaryExpression.Type == typeof(int))
                            {
                                return (int)left + (int)right;
                            }

                            if (binaryExpression.Type == typeof(string))
                            {
                                return String.Concat(left, right);
                            }

                            throw new Exception($"Unexpected binary operator {binaryExpression.Operator.Kind}.");
                        case BoundBinaryOperatorKind.Subtraction:
                            return (int)left - (int)right;
                        case BoundBinaryOperatorKind.Multiplication:
                            return (int)left * (int)right;
                        case BoundBinaryOperatorKind.Division:
                            return (int)left / (int)right;
                        case BoundBinaryOperatorKind.Equals:
                            return Equals(left, right);
                        case BoundBinaryOperatorKind.NotEquals:
                            return !Equals(left, right);
                        case BoundBinaryOperatorKind.LogicalOr:
                            return (bool)left || (bool)right;
                        case BoundBinaryOperatorKind.LogicalAnd:
                            return (bool)left && (bool)right;
                        default:
                            throw new Exception($"Unexpected binary operator {binaryExpression.Operator.Kind}.");
                    }

                    case BoundNodeKind.TernaryExpression:
                        var ternaryExpression = (BoundTernaryExpression)expression;

                        var conditionResult = EvaluateExpression(ternaryExpression.Condition);

                        if ((bool)conditionResult)
                        {
                            return EvaluateExpression(ternaryExpression.True);
                        }
                        else
                        {
                            return EvaluateExpression(ternaryExpression.False);
                        }
            }

            throw new Exception($"Unexpected node {expression.Kind}.");
        }
    }
}
