using System;
using System.Collections.Generic;

namespace Compression.LZString.CSharp
{
    internal class SequenceEqualityComparer<TElement>
        : EqualityComparer<ReadOnlyMemory<TElement>>
        where TElement : IEquatable<TElement>
    {
        private IEqualityComparer<TElement> ElementComparer;

        private SequenceEqualityComparer(IEqualityComparer<TElement> elementComparer)
        {
            ElementComparer = elementComparer ?? throw new ArgumentNullException(nameof(elementComparer));
        }

        public override bool Equals(ReadOnlyMemory<TElement> x, ReadOnlyMemory<TElement> y)
        {

            for(var i = 0; i < x.Length; ++i)
            {
                if(!ElementComparer.Equals(x.Span[i], y.Span[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode(ReadOnlyMemory<TElement> collection)
        {
            var ret = 0;
            for (int i = (collection.Length >= 8 ? collection.Length - 8 : 0); i < collection.Length; i++)
            {
                ret = CombineHashCodes(ret, ElementComparer.GetHashCode(collection.Span[i]));
            }
            return ret;
        }

        private static int CombineHashCodes(int h1, int h2)
        {
            return (((h1 << 5) + h1) ^ h2);
        }

        public static new SequenceEqualityComparer<TElement> Default { get; } = new SequenceEqualityComparer<TElement>(EqualityComparer<TElement>.Default);
    }

}
