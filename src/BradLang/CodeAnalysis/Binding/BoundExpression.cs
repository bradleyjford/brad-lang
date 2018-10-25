using System;

namespace BradLang.CodeAnalysis.Binding
{
    abstract class BoundExpression : BoundNode
    {
        public abstract Type Type { get; }
    }
}