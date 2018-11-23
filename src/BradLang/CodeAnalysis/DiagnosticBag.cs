using System;
using System.Collections;
using System.Collections.Generic;
using BradLang.CodeAnalysis.Syntax;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis
{
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

        internal void ReportInvalidNumber(TextSpan span, string text, Type expectedType)
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

        internal void ReportUndefinedUnaryOperator(TextSpan span, string operatorText, Type operandType)
        {
            var message = $"Unary operator '{operatorText}' is not defined for type {operandType}.";
            Report(span, message);
        }

        internal void ReportUndefinedBinaryOperator(TextSpan span, string operatorText, Type leftType, Type rightType)
        {
            var message = $"Binary operator '{operatorText}' is not defined for types {leftType} and {rightType}.";
            Report(span, message);
        }

        internal void ReportUndefinedName(TextSpan span, string name)
        {
            var message = $"Variable '{name}' doesn't exist.";
            Report(span, message);
        }

        internal void ReportUnterminatedStringConstant(TextSpan span)
        {
            var message = $"Unterminated string constant.";
            Report(span, message);
        }

        internal void  ReportTypeMismatch(TextSpan span, Type fromType, Type toType)
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

        internal void ReportMethodNotDefinied(TextSpan span, string methodName)
        {
            var message = $"Method \"{methodName}\" is not defined.";
            Report(span, message);        
        }
    }
}
