using System;
using System.Collections.Generic;
using System.Linq;
using BradLang.CodeAnalysis.Syntax;
using Xunit;

namespace BradLang.Tests.CodeAnalysis.Syntax
{
    sealed class AssertingEnumerator : IDisposable
    {
        readonly IEnumerator<SyntaxNode> _enumerator;
        bool _hasErrors;

        public AssertingEnumerator(SyntaxNode node)
        {
            _enumerator = Flatten(node).GetEnumerator();
        }

        private bool MarkFailed()
        {
            _hasErrors = true;

            return false;
        }

        public void Dispose()
        {
            if (!_hasErrors)
            {
                Assert.False(_enumerator.MoveNext());
            }

            _enumerator.Dispose();
        }

        static IEnumerable<SyntaxNode> Flatten(SyntaxNode node)
        {
            var stack = new Stack<SyntaxNode>();

            stack.Push(node);

            while (stack.Count > 0)
            {
                var currentNode = stack.Pop();

                yield return currentNode;

                foreach (var child in currentNode.GetChildren().Reverse())
                {
                    stack.Push(child);
                }
            }
        }

        public void AssertNode(SyntaxKind kind)
        {
            try
            {
                Assert.True(_enumerator.MoveNext());
                Assert.Equal(kind, _enumerator.Current.Kind);
                Assert.IsNotType<SyntaxToken>(_enumerator.Current);
            }
            catch when (MarkFailed())
            {
                throw;
            }
        }

        public void AssertToken(SyntaxKind kind, string text)
        {
            try
            {
                Assert.True(_enumerator.MoveNext());
                Assert.Equal(kind, _enumerator.Current.Kind);

                var token = Assert.IsType<SyntaxToken>(_enumerator.Current);
                
                Assert.Equal(text, token.Text);
            }
            catch when (MarkFailed())
            {
                throw;
            }
        }
    }
}
