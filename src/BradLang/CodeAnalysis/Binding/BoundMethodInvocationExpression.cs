using System;

namespace BradLang.CodeAnalysis.Binding
{
    sealed class BoundMethodInvocationExpression : BoundExpression
    {
        public BoundMethodInvocationExpression(MethodInfo methodInfo, BoundExpression argumentExpression)
        {
            MethodInfo = methodInfo;
            ArgumentExpression = argumentExpression;
        }

        public override Type Type => MethodInfo.ReturnType;

        public override BoundNodeKind Kind => BoundNodeKind.MethodInvocationExpression;

        public MethodInfo MethodInfo { get; }
        public BoundExpression ArgumentExpression { get; }
    }
}
