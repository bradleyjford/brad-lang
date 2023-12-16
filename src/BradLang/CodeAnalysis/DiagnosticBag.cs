using System.Collections;
using BradLang.CodeAnalysis.Symbols;
using BradLang.CodeAnalysis.Syntax;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis;

public sealed class DiagnosticBag : IEnumerable<Diagnostic>
{
    readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

    public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void AddRange(IEnumerable<Diagnostic> diagnostics)
    {
        _diagnostics.AddRange(diagnostics);
    }

    void Report(TextSpan span, string message)
    {
        _diagnostics.Add(new Diagnostic(span, message));
    }

    internal void ReportInvalidNumber(TextSpan span, string text, TypeSymbol expectedType)
    {
        var message = $"ERROR: Unable to convert \"{text}\" to type '{expectedType}'.";
        Report(span, message);
    }

    internal void ReportBadCharacter(int position, char character)
    {
        var span = new TextSpan(position, 1);
        var message = $"ERROR: Unexpected character \"{character}\".";
        Report(span, message);
    }

    internal void ReportUnexpectedToken(TextSpan span, SyntaxKind actualKind, SyntaxKind expectedKind)
    {
        var message = $"Unexpected token <{actualKind}>, expected <{expectedKind}>.";
        Report(span, message);
    }

    internal void ReportUndefinedUnaryOperator(TextSpan span, string operatorText, TypeSymbol operandType)
    {
        var message = $"Unary operator '{operatorText}' is not defined for type {operandType}.";
        Report(span, message);
    }

    internal void ReportUndefinedPostfixUnaryOperator(TextSpan span, string operatorText, TypeSymbol operandType)
    {
        var message = $"Postfix unary operator '{operatorText}' is not defined for type {operandType}.";
        Report(span, message);
    }

    internal void ReportUndefinedBinaryOperator(TextSpan span, string operatorText, TypeSymbol leftType, TypeSymbol rightType)
    {
        var message = $"Binary operator '{operatorText}' is not defined for types {leftType} and {rightType}.";
        Report(span, message);
    }

    internal void ReportUndefinedName(TextSpan span, string name)
    {
        var message = $"Variable '{name}' doesn't exist.";
        Report(span, message);
    }

    internal void ReportUnterminatedStringLiteral(TextSpan span)
    {
        var message = $"Unterminated string constant.";
        Report(span, message);
    }

    internal void  ReportTypeMismatch(TextSpan span, TypeSymbol fromType, TypeSymbol toType)
    {
        var message = $"Cannot convert from type '{fromType}' to '{toType}'.";
        Report(span, message);
    }

    internal void ReportCannotAssign(TextSpan span, string name)
    {
        var message = $"Variable \"{name}\" is read-only and cannot be assigned to.";
        Report(span, message);
    }
        
    internal void ReportVariableAlreadyDeclared(TextSpan span, string name)
    {
        var message = $"Variable \"{name}\" is already declared.";
        Report(span, message);
    }

    internal void ReportUndefinedFunction(TextSpan span, string name)
    {
        var message = $"Function \"{name}\" is not defined.";
        Report(span, message);
    }

    internal void ReportIncorrectArgumentCount(TextSpan span, string name, int expectedCount, int actualCount)
    {
        var message = $"Function '{name}' requires {expectedCount} arguments but was given {actualCount}.";
        Report(span, message);
    }
}
