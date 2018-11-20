using System;
using System.Collections.Generic;
using BradLang.CodeAnalysis.Binding;

namespace BradLang.CodeAnalysis
{
    sealed class Evaluator
    {
        readonly BoundStatement _root;
        readonly IDictionary<VariableSymbol, object> _variables;

        object _lastValue;

        public Evaluator(BoundStatement root, IDictionary<VariableSymbol, object> variables)
        {
            _root = root;
            _variables = variables;
        }

        public object Evaluate()
        {
            EvaluateStatement(_root);

            return _lastValue;
        }

        void EvaluateStatement(BoundStatement statement)
        {
            switch (statement.Kind)
            {
                case BoundNodeKind.BlockStatement:
                    EvaluateBlockStatement((BoundBlockStatement)statement);
                    break;

                case BoundNodeKind.ForStatement:
                    EvaluateForStatement((BoundForStatement)statement);
                    break;

                case BoundNodeKind.IfStatement:
                    EvaluateIfStatement((BoundIfStatement)statement);
                    break;

                case BoundNodeKind.VariableDeclaration:
                    EvaluateVariableDeclaration((BoundVariableDeclaration)statement);
                    break;

                case BoundNodeKind.WhileStatement:
                    EvaluateWhileStatement((BoundWhileStatement)statement);
                    break;

                case BoundNodeKind.ExpressionStatement:
                    EvaluateExpressionStatement((BoundExpressionStatement)statement);
                    break;

                default:
                    throw new InvalidOperationException($"Unexpected node {statement.Kind}.");
            }
        }

        void EvaluateBlockStatement(BoundBlockStatement node)
        {
            foreach (var statement in node.Statements)
            {
                EvaluateStatement(statement);
            }
        }

        void EvaluateForStatement(BoundForStatement statement)
        {
            var lowerBound = (int)EvaluateExpression(statement.LowerBound);
            var upperBound = (int)EvaluateExpression(statement.UpperBound);

            for (var i = lowerBound; i <= upperBound; i++)
            {
                _variables[statement.Variable] = i;

                EvaluateStatement(statement.Body);
            }
        }

        void EvaluateIfStatement(BoundIfStatement statement)
        {
            var conditionStatisfied = (bool)EvaluateExpression(statement.Condition);

            if (conditionStatisfied)
            {
                EvaluateStatement(statement.ThenStatement);
            }
            else if (statement.ElseStatement != null)
            {
                EvaluateStatement(statement.ElseStatement);
            }
        }

        void EvaluateVariableDeclaration(BoundVariableDeclaration statement)
        {
            _variables[statement.VariableSymbol] = EvaluateExpression(statement.Initializer);
        }

        void EvaluateWhileStatement(BoundWhileStatement statement)
        {
            while(true)
            {
                var conditionStatisfied = (bool)EvaluateExpression(statement.Condition);

                if (!conditionStatisfied)
                {
                    break;
                }

                EvaluateStatement(statement.Body);
            }
        }

        void EvaluateExpressionStatement(BoundExpressionStatement node)
        {
            _lastValue = EvaluateExpression(node.Expression);
        }

        object EvaluateExpression(BoundNode node)
        {
            switch (node.Kind)
            {
                case BoundNodeKind.LiteralExpression:
                    return EvaluateLiteralExpression((BoundLiteralExpression)node);

                case BoundNodeKind.VariableExpression:
                    return EvaluateVariableExpression((BoundVariableExpression)node);

                case BoundNodeKind.AssignmentExpression:
                    return EvaluateAssignmentExpression((BoundAssignmentExpression)node);

                case BoundNodeKind.UnaryExpression:
                    return EvaluateUnaryExpression((BoundUnaryExpression)node);

                case BoundNodeKind.BinaryExpression:
                    return EvaluateBinaryExpression((BoundBinaryExpression)node);

                case BoundNodeKind.TernaryExpression:
                    return EvaluateTernaryExpression((BoundTernaryExpression)node);
            }

            throw new Exception($"Unexpected node {node.Kind}.");
        }

        object EvaluateLiteralExpression(BoundLiteralExpression expression)
        {
            return expression.Value;
        }

        object EvaluateVariableExpression(BoundVariableExpression expression)
        {
            return _variables[expression.Variable];
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

        object EvaluateTernaryExpression(BoundTernaryExpression expression)
        {
            var conditionResult = (bool)EvaluateExpression(expression.Condition);

            if (conditionResult)
            {
                return EvaluateExpression(expression.True);
            }
            else
            {
                return EvaluateExpression(expression.False);
            }
        }
    }
}
