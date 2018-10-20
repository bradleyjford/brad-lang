using System;

namespace BradLang.CodeAnalysis.Binding
{
    abstract class BoundNode
    {
        public abstract BoundNodeKind Kind { get; }

    }
}