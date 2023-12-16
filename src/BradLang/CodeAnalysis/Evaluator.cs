using BradLang.CodeAnalysis.Binding;
using BradLang.CodeAnalysis.Symbols;

namespace BradLang.CodeAnalysis;

sealed class Evaluator
{
    readonly BoundBlockStatement _root;
    readonly IDictionary<VariableSymbol, object> _variables;
    readonly IDictionary<BoundLabel, int> _labels = new Dictionary<BoundLabel, int>();

    object _lastValue;

    public Evaluator(BoundBlockStatement root, IDictionary<VariableSymbol, object> variables)
    {
        _root = root;
        _variables = variables;

        IndexLabels();
    }

    void IndexLabels()
    {
        for (var i = 0; i < _root.Statements.Length; i++)
        {
            if (_root.Statements[i] is BoundLabelStatement l)
            {
                _labels.Add(l.Label, i + 1);
            }
        }
    }

    public object Evaluate()
    {
        var ip = 0;

        while (ip < _root.Statements.Length)
        {
            var statement = _root.Statements[ip];

            switch (statement.Kind)
            {
                case BoundNodeKind.GotoStatement:
                    var label = ((BoundGotoStatement)statement).Label;

                    ip = _labels[label];

                    break;

                case BoundNodeKind.ConditionalGotoStatement:
                    var conditionalGoto = (BoundConditionalGotoStatement)statement;

                    var condition = (bool)EvaluateExpression(conditionalGoto.Condition);

                    if (condition == conditionalGoto.JumpIfTrue)
                    {
                        ip = _labels[conditionalGoto.Label];
                    }
                    else
                    {
                        ip++;
                    }

                    break;

                case BoundNodeKind.VariableDeclaration:
                    EvaluateVariableDeclaration((BoundVariableDeclaration)statement);
                    ip++;

                    break;

                case BoundNodeKind.ExpressionStatement:
                    EvaluateExpressionStatement((BoundExpressionStatement)statement);
                    ip++;

                    break;

                case BoundNodeKind.LabelStatement:
                    ip++;

                    break;

                default:
                    throw new InvalidOperationException($"Unexpected node {statement.Kind}.");
            }

        }

        return _lastValue;
    }

    void EvaluateVariableDeclaration(BoundVariableDeclaration statement)
    {
        _variables[statement.VariableSymbol] = EvaluateExpression(statement.Initializer);
    }

    void EvaluateExpressionStatement(BoundExpressionStatement node)
    {
        _lastValue = EvaluateExpression(node.Expression);
    }

    object EvaluateExpression(BoundNode node)
    {
        switch (node.Kind)
        {
            case BoundNodeKind.CallExpression:
                return EvalulateCallExpression((BoundCallExpression)node);

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

    object EvalulateCallExpression(BoundCallExpression node)
    {
        if (node.Function == BuiltinFunctions.Input)
        {
            return Console.ReadLine();
        }
        else if (node.Function == BuiltinFunctions.Print)
        {
            var message = (string)EvaluateExpression(node.Arguments[0]);

            Console.WriteLine(message);

            return null;
        }
        else
        {
            throw new Exception($"Unknown function \"{node.Function.Name}\".");
        }
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
            case BoundUnaryOperatorKind.PrefixIncrement:
                var prefixIncrementValue = (int)operand + 1;

                if (expression.Operand.Kind == BoundNodeKind.VariableExpression)
                {
                    var variableExpression = (BoundVariableExpression)expression.Operand;

                    _variables[variableExpression.Variable] = prefixIncrementValue;
                }

                return prefixIncrementValue;
            case BoundUnaryOperatorKind.PrefixDecrement:
                var prefixDecrementValue = (int)operand - 1;

                if (expression.Operand.Kind == BoundNodeKind.VariableExpression)
                {
                    var variableExpression = (BoundVariableExpression)expression.Operand;

                    _variables[variableExpression.Variable] = prefixDecrementValue;
                }

                return prefixDecrementValue;
            case BoundUnaryOperatorKind.PostfixIncrement:
                var postfixIncrementValue = (int)operand + 1;

                if (expression.Operand.Kind == BoundNodeKind.VariableExpression)
                {
                    var variableExpression = (BoundVariableExpression)expression.Operand;

                    _variables[variableExpression.Variable] = postfixIncrementValue;

                    return operand;
                }

                return postfixIncrementValue;
            case BoundUnaryOperatorKind.PostfixDecrement:
                var postfixDecrementValue = (int)operand - 1;

                if (expression.Operand.Kind == BoundNodeKind.VariableExpression)
                {
                    var variableExpression = (BoundVariableExpression)expression.Operand;

                    _variables[variableExpression.Variable] = postfixDecrementValue;

                    return operand;
                }

                return postfixDecrementValue; 
            case BoundUnaryOperatorKind.OnesCompliment:
                return ~(int)operand;
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
                if (expression.Type == TypeSymbol.Int)
                {
                    return (int)left + (int)right;
                }

                if (expression.Type == TypeSymbol.String)
                {
                    return string.Concat(left, right);
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
            case BoundBinaryOperatorKind.BitwiseAnd:
                return (int)left & (int)right;
            case BoundBinaryOperatorKind.BitwiseOr:
                return (int)left | (int)right;
            case BoundBinaryOperatorKind.BitwiseXor:
                return (int)left ^ (int)right;
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
